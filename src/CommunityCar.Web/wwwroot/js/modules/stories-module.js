/**
 * Stories Module
 * Handles Story Viewer Modal, Auto-advance, and Interaction.
 */
(function (CC) {
    class StoriesModule extends CC.Utils.BaseComponent {
        constructor() {
            super('StoriesModule');
            this.currentStoryIndex = 0;
            this.currentStorySet = [];
            this.modal = null;
            this.timer = null;
        }

        init() {
            if (this.initialized) return;
            super.init();

            this.loadStories();
            // Refresh interval
            setInterval(() => this.loadStories(), 300000);

            // Setup global hook for opening viewer (legacy support + external triggers)
            window.openStoryViewer = (id) => this.openViewer(id);
        }

        async loadStories() {
            const res = await CC.Services.Stories.getFeed();
            if (res.success) {
                this.renderFeed(res.data);
            }
        }

        renderFeed(stories) {
            const container = document.getElementById('storiesContainer');
            if (!container) return;

            // Preserve Add Button
            const addBtn = container.querySelector('.group\\/add-story'); // Escaped selector
            container.innerHTML = '';
            if (addBtn) container.appendChild(addBtn);

            stories.forEach(story => {
                const el = this.createStoryElement(story);
                container.appendChild(el);
            });
        }

        createStoryElement(story) {
            const div = document.createElement('div');
            div.className = 'flex-shrink-0 flex flex-col items-center gap-2 cursor-pointer group/story snap-start';
            div.dataset.storyId = story.id;
            div.onclick = () => this.openViewer(story.id);

            // ... HTML construction simplified for brevity, assume similar to original ...
            const isViewed = story.viewCount > 0;
            div.innerHTML = `
                <div class="w-16 h-16 rounded-full p-0.5 relative transition-all group-hover/story:scale-105 active:scale-95 shadow-md">
                     ${!isViewed ? '<div class="absolute inset-0 rounded-full bg-gradient-to-tr from-primary via-primary/50 to-primary/80 p-0.5 animate-spin-slow"></div>' : ''}
                     <div class="w-full h-full rounded-full overflow-hidden border border-border relative z-10 p-[1px] bg-background">
                         <img src="${story.thumbnailUrl || story.mediaUrl}" class="w-full h-full object-cover rounded-full ${isViewed ? 'opacity-60' : ''}" />
                     </div>
                </div>
                <span class="text-[10px] font-bold text-foreground text-center truncate w-16 opacity-80">${story.authorName.split(' ')[0]}</span>
             `;
            return div;
        }

        async openViewer(storyId) {
            const res = await CC.Services.Stories.getActive();
            if (res.success) {
                this.currentStorySet = res.data;
                this.currentStoryIndex = this.currentStorySet.findIndex(s => s.id === storyId);
                if (this.currentStoryIndex === -1) this.currentStoryIndex = 0;

                this.ensureModalExists();
                this.modal.classList.remove('hidden');
                this.displayStory();
            }
        }

        ensureModalExists() {
            if (this.modal) return;
            // Inject Modal HTML
            const modalHTML = `
            <div id="storyViewerModal" class="fixed inset-0 z-[9999] hidden bg-black/95 backdrop-blur-sm">
                 <button class="absolute top-4 right-4 z-50 text-white p-2" onclick="CC.Modules.Stories.closeViewer()">✕</button>
                 <div class="relative w-full h-full flex items-center justify-center">
                      <div id="storyContent" class="relative max-w-md w-full h-full max-h-[90vh]">
                          <!-- Progress -->
                          <div id="storyProgressBars" class="absolute top-4 left-4 right-4 z-30 flex gap-1"></div>
                          <!-- Media -->
                          <div id="storyMediaContainer" class="w-full h-full flex items-center justify-center">
                              <img id="storyImage" class="max-w-full max-h-full object-contain hidden">
                              <video id="storyVideo" class="max-w-full max-h-full object-contain hidden" autoplay></video>
                          </div>
                          <!-- Click Zones -->
                          <div class="absolute inset-y-0 left-0 w-1/3 z-20" onclick="CC.Modules.Stories.prev()"></div>
                          <div class="absolute inset-y-0 right-0 w-1/3 z-20" onclick="CC.Modules.Stories.next()"></div>
                      </div>
                 </div>
            </div>`;
            document.body.insertAdjacentHTML('beforeend', modalHTML);
            this.modal = document.getElementById('storyViewerModal');
        }

        closeViewer() {
            if (this.modal) this.modal.classList.add('hidden');
            if (this.timer) clearInterval(this.timer);
            const video = document.getElementById('storyVideo');
            if (video) video.pause();
        }

        displayStory() {
            if (!this.currentStorySet.length) return;
            const story = this.currentStorySet[this.currentStoryIndex];

            // Update Media
            const img = document.getElementById('storyImage');
            const vid = document.getElementById('storyVideo');

            if (story.type === 'Video') {
                img.classList.add('hidden');
                vid.classList.remove('hidden');
                vid.src = story.mediaUrl;
                vid.play();
            } else {
                vid.classList.add('hidden');
                img.classList.remove('hidden');
                img.src = story.mediaUrl;
            }

            this.startProgress();
            CC.Services.Stories.markViewed(story.id);
        }

        startProgress() {
            if (this.timer) clearInterval(this.timer);
            const duration = this.currentStorySet[this.currentStoryIndex].type === 'Video' ? 15000 : 5000;
            const start = Date.now();

            // Reset bars
            const barsContainer = document.getElementById('storyProgressBars');
            barsContainer.innerHTML = this.currentStorySet.map((_, i) => `
                <div class="flex-1 h-1 bg-white/30 rounded overflow-hidden">
                    <div class="h-full bg-white transition-all" style="width: ${i < this.currentStoryIndex ? '100%' : '0%'}"></div>
                </div>
             `).join('');

            const currentBar = barsContainer.children[this.currentStoryIndex].firstElementChild;

            this.timer = setInterval(() => {
                const elapsed = Date.now() - start;
                const pct = Math.min(100, (elapsed / duration) * 100);
                currentBar.style.width = `${pct}%`;

                if (pct >= 100) {
                    this.next();
                }
            }, 50);
        }

        next() {
            if (this.currentStoryIndex < this.currentStorySet.length - 1) {
                this.currentStoryIndex++;
                this.displayStory();
            } else {
                this.closeViewer();
            }
        }

        prev() {
            if (this.currentStoryIndex > 0) {
                this.currentStoryIndex--;
                this.displayStory();
            }
        }
    }

    CC.Modules.Stories = new StoriesModule();
})(window.CommunityCar);
