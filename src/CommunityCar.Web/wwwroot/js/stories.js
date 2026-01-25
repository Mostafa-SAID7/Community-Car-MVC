// Stories JavaScript Functionality

let currentStoryIndex = 0;
let currentStorySet = [];
let storyViewerModal = null;
let storyProgressInterval = null;
let storyAutoAdvanceTimeout = null;

// Initialize stories functionality
function initializeStories() {
    loadStoriesData();
    setupStoryViewer();

    // Auto-refresh stories every 5 minutes
    setInterval(refreshStories, 300000);
}

// Load stories data from API
function loadStoriesData() {
    fetch('/api/stories/feed?count=20')
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                updateStoriesDisplay(data.data);
            } else {
                console.error('Error loading stories:', data.message);
            }
        })
        .catch(error => {
            console.error('Error loading stories:', error);
        });
}

// Update stories display in the UI
function updateStoriesDisplay(stories) {
    const container = document.getElementById('storiesContainer');
    if (!container) return;

    // Keep the "Add Story" button
    const addStoryBtn = container.querySelector('.group\\/add-story');
    container.innerHTML = '';

    if (addStoryBtn) {
        container.appendChild(addStoryBtn);
    }

    stories.forEach(story => {
        const storyElement = createStoryElement(story);
        container.appendChild(storyElement);
    });
}

// Create story element HTML
function createStoryElement(story) {
    const storyDiv = document.createElement('div');
    storyDiv.className = 'flex-shrink-0 flex flex-col items-center gap-2 cursor-pointer group/story snap-start';
    storyDiv.setAttribute('data-story-id', story.id);
    storyDiv.onclick = () => openStoryViewer(story.id);

    const timeRemaining = calculateTimeRemaining(story.expiresAt);
    const isViewed = story.viewCount > 0; // Simple check, in real app you'd track per user

    storyDiv.innerHTML = `
        <div class="w-16 h-16 rounded-full p-0.5 relative transition-all group-hover/story:scale-105 active:scale-95 shadow-md">
            ${!isViewed ? `
                <div class="absolute inset-0 rounded-full bg-gradient-to-tr from-primary via-primary/50 to-primary/80 p-0.5 animate-spin-slow">
                    <div class="w-full h-full rounded-full bg-background shadow-inner"></div>
                </div>
            ` : ''}
            <div class="w-full h-full rounded-full overflow-hidden border border-border relative z-10 p-[1px] bg-background">
                <img src="${story.thumbnailUrl || story.mediaUrl}" alt="${story.authorName}" class="w-full h-full object-cover rounded-full ${isViewed ? 'opacity-60 grayscale-[0.2]' : ''}" />
                ${story.isHighlighted ? `
                    <div class="absolute top-0 right-0 w-4 h-4 bg-yellow-500 rounded-full border-2 border-background flex items-center justify-center">
                        <i class="fas fa-star text-white text-[8px]"></i>
                    </div>
                ` : ''}
                ${story.type === 'Video' ? `
                    <div class="absolute bottom-1 right-1 w-3 h-3 bg-black/50 rounded-full flex items-center justify-center">
                        <i class="fas fa-play text-white text-[6px]"></i>
                    </div>
                ` : ''}
            </div>
        </div>
        <span class="text-[10px] font-bold text-foreground text-center truncate w-16 opacity-80 group-hover/story:opacity-100 transition-opacity">
            ${story.authorName.split(' ')[0]}
        </span>
        <div class="text-[8px] text-muted-foreground text-center opacity-60">
            ${timeRemaining}
        </div>
    `;

    return storyDiv;
}

// Calculate time remaining for story
function calculateTimeRemaining(expiresAt) {
    const now = new Date();
    const expires = new Date(expiresAt);
    const diff = expires - now;

    if (diff <= 0) return 'Expired';

    const hours = Math.floor(diff / (1000 * 60 * 60));
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));

    if (hours > 0) {
        return `${hours}h`;
    } else if (minutes > 0) {
        return `${minutes}m`;
    } else {
        return '<1m';
    }
}

// Setup story viewer modal
function setupStoryViewer() {
    // Create story viewer modal if it doesn't exist
    if (!document.getElementById('storyViewerModal')) {
        const modalHTML = `
            <div id="storyViewerModal" class="fixed inset-0 z-[9999] hidden bg-black/95 backdrop-blur-sm">
                <div class="relative w-full h-full flex items-center justify-center">
                    <!-- Close button -->
                    <button onclick="closeStoryViewer()" class="absolute top-4 right-4 z-50 w-10 h-10 bg-black/50 hover:bg-black/70 rounded-full flex items-center justify-center text-white transition-all">
                        <i class="fas fa-times text-lg"></i>
                    </button>
                    
                    <!-- Story navigation -->
                    <button onclick="previousStory()" class="absolute left-4 top-1/2 -translate-y-1/2 z-40 w-12 h-12 bg-black/50 hover:bg-black/70 rounded-full flex items-center justify-center text-white transition-all">
                        <i class="fas fa-chevron-left text-lg"></i>
                    </button>
                    
                    <button onclick="nextStory()" class="absolute right-4 top-1/2 -translate-y-1/2 z-40 w-12 h-12 bg-black/50 hover:bg-black/70 rounded-full flex items-center justify-center text-white transition-all">
                        <i class="fas fa-chevron-right text-lg"></i>
                    </button>
                    
                    <!-- Story content container -->
                    <div class="relative max-w-md w-full h-full max-h-[90vh] mx-4">
                        <!-- Progress bars -->
                        <div id="storyProgressBars" class="absolute top-4 left-4 right-4 z-30 flex gap-1">
                            <!-- Dynamic progress bars will be inserted here -->
                        </div>
                        
                        <!-- Story header -->
                        <div id="storyHeader" class="absolute top-12 left-4 right-4 z-30 flex items-center gap-3 text-white">
                            <div class="w-8 h-8 rounded-full overflow-hidden border border-white/20">
                                <img id="storyAuthorAvatar" src="" alt="" class="w-full h-full object-cover" />
                            </div>
                            <div class="flex-1">
                                <div id="storyAuthorName" class="text-sm font-bold"></div>
                                <div id="storyTimeAgo" class="text-xs opacity-70"></div>
                            </div>
                            <div class="flex items-center gap-2">
                                <button onclick="toggleStoryLike()" id="storyLikeBtn" class="w-8 h-8 rounded-full bg-black/30 hover:bg-black/50 flex items-center justify-center transition-all">
                                    <i class="fas fa-heart text-sm"></i>
                                </button>
                                <button onclick="shareStory()" class="w-8 h-8 rounded-full bg-black/30 hover:bg-black/50 flex items-center justify-center transition-all">
                                    <i class="fas fa-share text-sm"></i>
                                </button>
                            </div>
                        </div>
                        
                        <!-- Story media -->
                        <div id="storyMediaContainer" class="w-full h-full rounded-2xl overflow-hidden bg-black flex items-center justify-center">
                            <img id="storyImage" src="" alt="" class="max-w-full max-h-full object-contain hidden" />
                            <video id="storyVideo" src="" class="max-w-full max-h-full object-contain hidden" controls></video>
                        </div>
                        
                        <!-- Story caption -->
                        <div id="storyCaption" class="absolute bottom-4 left-4 right-4 z-30 text-white text-sm bg-black/30 backdrop-blur-sm rounded-lg p-3 hidden">
                        </div>
                        
                        <!-- Story stats -->
                        <div id="storyStats" class="absolute bottom-16 left-4 right-4 z-30 flex items-center justify-between text-white text-xs opacity-70">
                            <div class="flex items-center gap-4">
                                <span id="storyViews"><i class="fas fa-eye mr-1"></i>0</span>
                                <span id="storyLikes"><i class="fas fa-heart mr-1"></i>0</span>
                            </div>
                            <div id="storyLocation" class="hidden">
                                <i class="fas fa-map-marker-alt mr-1"></i><span></span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;

        document.body.insertAdjacentHTML('beforeend', modalHTML);
        storyViewerModal = document.getElementById('storyViewerModal');

        // Add keyboard navigation
        document.addEventListener('keydown', handleStoryKeyboard);

        // Add touch/click navigation
        const mediaContainer = document.getElementById('storyMediaContainer');
        mediaContainer.addEventListener('click', handleStoryClick);
    }
}

// Open story viewer
function openStoryViewer(storyId) {
    // Load all active stories first
    fetch('/api/stories/active')
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                currentStorySet = data.data;
                currentStoryIndex = currentStorySet.findIndex(s => s.id === storyId);
                if (currentStoryIndex === -1) currentStoryIndex = 0;

                showStoryViewer();
                displayCurrentStory();
            }
        })
        .catch(error => {
            console.error('Error loading stories:', error);
        });
}

// Show story viewer modal
function showStoryViewer() {
    if (storyViewerModal) {
        storyViewerModal.classList.remove('hidden');
        document.body.style.overflow = 'hidden';
    }
}

// Close story viewer
function closeStoryViewer() {
    if (storyViewerModal) {
        storyViewerModal.classList.add('hidden');
        document.body.style.overflow = '';

        // Clear intervals and timeouts
        if (storyProgressInterval) {
            clearInterval(storyProgressInterval);
            storyProgressInterval = null;
        }
        if (storyAutoAdvanceTimeout) {
            clearTimeout(storyAutoAdvanceTimeout);
            storyAutoAdvanceTimeout = null;
        }
    }
}

// Display current story
function displayCurrentStory() {
    if (!currentStorySet.length || currentStoryIndex < 0 || currentStoryIndex >= currentStorySet.length) {
        closeStoryViewer();
        return;
    }

    const story = currentStorySet[currentStoryIndex];

    // Update progress bars
    updateProgressBars();

    // Update header
    document.getElementById('storyAuthorAvatar').src = story.authorAvatar || '/images/default-avatar.png';
    document.getElementById('storyAuthorName').textContent = story.authorName;
    document.getElementById('storyTimeAgo').textContent = story.timeAgo;

    // Update media
    const imageEl = document.getElementById('storyImage');
    const videoEl = document.getElementById('storyVideo');

    if (story.type === 'Video') {
        imageEl.classList.add('hidden');
        videoEl.classList.remove('hidden');
        videoEl.src = story.mediaUrl;
        videoEl.currentTime = 0;
        videoEl.play();
    } else {
        videoEl.classList.add('hidden');
        imageEl.classList.remove('hidden');
        imageEl.src = story.mediaUrl;
    }

    // Update caption
    const captionEl = document.getElementById('storyCaption');
    const isArabic = window.feedLocalizer && window.feedLocalizer.isArabic;
    const displayCaption = isArabic && story.captionAr ? story.captionAr : story.caption;

    if (displayCaption) {
        captionEl.textContent = displayCaption;
        captionEl.dir = isArabic && story.captionAr ? 'rtl' : 'ltr';
        captionEl.classList.remove('hidden');
    } else {
        captionEl.classList.add('hidden');
    }

    // Update stats
    document.getElementById('storyViews').innerHTML = `<i class="fas fa-eye mr-1"></i>${story.viewCount}`;
    document.getElementById('storyLikes').innerHTML = `<i class="fas fa-heart mr-1"></i>${story.likeCount}`;

    // Update location
    const locationEl = document.getElementById('storyLocation');
    if (story.locationName) {
        locationEl.querySelector('span').textContent = story.locationName;
        locationEl.classList.remove('hidden');
    } else {
        locationEl.classList.add('hidden');
    }

    // Update like button
    const likeBtn = document.getElementById('storyLikeBtn');
    likeBtn.classList.toggle('text-red-500', story.isLikedByUser);

    // Mark as viewed
    markStoryAsViewed(story.id);

    // Start progress and auto-advance
    startStoryProgress();
}

// Update progress bars
function updateProgressBars() {
    const progressContainer = document.getElementById('storyProgressBars');
    progressContainer.innerHTML = '';

    currentStorySet.forEach((_, index) => {
        const bar = document.createElement('div');
        bar.className = 'flex-1 h-0.5 bg-white/30 rounded-full overflow-hidden';

        const progress = document.createElement('div');
        progress.className = 'h-full bg-white transition-all duration-100';
        progress.style.width = index < currentStoryIndex ? '100%' : index === currentStoryIndex ? '0%' : '0%';

        bar.appendChild(progress);
        progressContainer.appendChild(bar);
    });
}

// Start story progress
function startStoryProgress() {
    // Clear existing intervals
    if (storyProgressInterval) clearInterval(storyProgressInterval);
    if (storyAutoAdvanceTimeout) clearTimeout(storyAutoAdvanceTimeout);

    const progressBar = document.getElementById('storyProgressBars').children[currentStoryIndex]?.querySelector('div');
    if (!progressBar) return;

    const duration = currentStorySet[currentStoryIndex].type === 'Video' ? 15000 : 5000; // 15s for video, 5s for image
    const startTime = Date.now();

    storyProgressInterval = setInterval(() => {
        const elapsed = Date.now() - startTime;
        const progress = Math.min((elapsed / duration) * 100, 100);
        progressBar.style.width = `${progress}%`;

        if (progress >= 100) {
            clearInterval(storyProgressInterval);
            nextStory();
        }
    }, 50);
}

// Navigate to previous story
function previousStory() {
    if (currentStoryIndex > 0) {
        currentStoryIndex--;
        displayCurrentStory();
    } else {
        closeStoryViewer();
    }
}

// Navigate to next story
function nextStory() {
    if (currentStoryIndex < currentStorySet.length - 1) {
        currentStoryIndex++;
        displayCurrentStory();
    } else {
        closeStoryViewer();
    }
}

// Handle keyboard navigation
function handleStoryKeyboard(event) {
    if (!storyViewerModal || storyViewerModal.classList.contains('hidden')) return;

    switch (event.key) {
        case 'Escape':
            closeStoryViewer();
            break;
        case 'ArrowLeft':
            previousStory();
            break;
        case 'ArrowRight':
        case ' ':
            event.preventDefault();
            nextStory();
            break;
    }
}

// Handle story click navigation
function handleStoryClick(event) {
    const rect = event.currentTarget.getBoundingClientRect();
    const clickX = event.clientX - rect.left;
    const width = rect.width;

    if (clickX < width / 2) {
        previousStory();
    } else {
        nextStory();
    }
}

// Toggle story like
function toggleStoryLike() {
    const story = currentStorySet[currentStoryIndex];
    if (!story) return;

    const isLiked = story.isLikedByUser;
    const endpoint = isLiked ? 'DELETE' : 'POST';

    fetch(`/api/stories/${story.id}/like`, {
        method: endpoint,
        headers: {
            'Content-Type': 'application/json',
        }
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                story.isLikedByUser = !isLiked;
                story.likeCount += isLiked ? -1 : 1;

                // Update UI
                const likeBtn = document.getElementById('storyLikeBtn');
                likeBtn.classList.toggle('text-red-500', story.isLikedByUser);
                document.getElementById('storyLikes').innerHTML = `<i class="fas fa-heart mr-1"></i>${story.likeCount}`;

                showNotification(
                    isLiked ? (window.storiesLocalizer?.RemovedLike || 'Removed like') : (window.storiesLocalizer?.Liked || 'Liked!'),
                    'success'
                );
            }
        })
        .catch(error => {
            console.error('Error toggling story like:', error);
            showNotification(window.storiesLocalizer?.ErrorUpdatingLike || 'Error updating like', 'error');
        });
}

// Share story
function shareStory() {
    const story = currentStorySet[currentStoryIndex];
    if (!story) return;

    fetch(`/api/stories/${story.id}/share`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        }
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                if (navigator.share) {
                    navigator.share({
                        title: `${story.authorName}'s Story`,
                        text: story.caption || 'Check out this story!',
                        url: data.shareUrl
                    });
                } else {
                    navigator.clipboard.writeText(data.shareUrl)
                        .then(() => {
                            showNotification(window.storiesLocalizer?.LinkCopiedToClipboard || 'Link copied to clipboard!', 'success');
                        })
                        .catch(() => {
                            showNotification(window.storiesLocalizer?.UnableToCopyLink || 'Unable to copy link', 'error');
                        });
                }
            }
        })
        .catch(error => {
            console.error('Error sharing story:', error);
            showNotification(window.storiesLocalizer?.ErrorSharingStory || 'Error sharing story', 'error');
        });
}

// Mark story as viewed
function markStoryAsViewed(storyId) {
    fetch(`/api/stories/${storyId}/view`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        }
    })
        .catch(error => {
            console.error('Error marking story as viewed:', error);
        });
}

// Open create story modal
function openCreateStoryModal() {
    // Redirect to create story page for now
    window.location.href = '/stories/create';
}

// Refresh stories
function refreshStories() {
    loadStoriesData();
}

// Show notification
function showNotification(message, type = 'info') {
    if (window.notify && typeof window.notify[type] === 'function') {
        window.notify[type](message);
    } else {
        console.log(`${type.toUpperCase()}: ${message}`);
    }
}

// Export functions for global access
window.storiesFunctions = {
    openStoryViewer,
    openCreateStoryModal,
    refreshStories,
    toggleStoryLike,
    shareStory,
    closeStoryViewer,
    previousStory,
    nextStory
};

// Make functions globally accessible
Object.assign(window, window.storiesFunctions);

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function () {
    initializeStories();
});