/**
 * Feed Module
 * Handles infinite scroll, post visibility tracking, and feed actions.
 * Integrates with InteractionController for common actions, but handles Feed-specifics (Hide, Report).
 */
(function (CC) {
    class FeedModule extends CC.Utils.BaseComponent {
        constructor() {
            super('FeedModule');
            this.currentPage = 1;
            this.isLoading = false;
            this.feedType = 'personalized';
            this.observer = null;
        }

        init() {
            if (this.initialized) return;
            super.init();

            const params = new URLSearchParams(window.location.search);
            this.feedType = params.get('feedType') || 'personalized';

            this.setupInfiniteScroll();
            this.setupVisibilityObserver();
            this.setupEventDelegation();
        }

        setupEventDelegation() {
            document.addEventListener('click', (e) => {
                // Hide Action
                if (e.target.matches('.hide-content-btn') || e.target.closest('.hide-content-btn')) {
                    const btn = e.target.closest('.hide-content-btn');
                    this.handleHide(btn.dataset.contentId);
                }

                // Report Action
                if (e.target.matches('.report-content-btn') || e.target.closest('.report-content-btn')) {
                    const btn = e.target.closest('.report-content-btn');
                    this.handleReport(btn.dataset.contentId, btn.dataset.contentType);
                }

                // Share Action (if not handled by InteractionController or specific feed logic)
                if (e.target.matches('.share-btn') || e.target.closest('.share-btn')) {
                    // check if handled by InteractionController else...
                    // Legacy feed.js had explicit shareContent. 
                    // InteractionController handles .share-btn usually.
                }
            });
        }

        setupInfiniteScroll() {
            const sentinel = document.getElementById('feed-sentinel');
            if (!sentinel) return;

            const scrollObserver = new IntersectionObserver((entries) => {
                if (entries[0].isIntersecting && !this.isLoading) {
                    this.loadMorePosts();
                }
            }, { rootMargin: '200px' });

            scrollObserver.observe(sentinel);
        }

        setupVisibilityObserver() {
            this.observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        const id = entry.target.dataset.contentId;
                        if (id) {
                            // Mark as seen logic if needed
                            // this.markAsSeen(id);
                            this.observer.unobserve(entry.target);
                        }
                    }
                });
            }, { threshold: 0.5 });

            this.observeNewPosts();
        }

        observeNewPosts() {
            document.querySelectorAll('.feed-item:not([data-observed])').forEach(item => {
                item.dataset.observed = 'true';
                this.observer.observe(item);
            });
        }

        async loadMorePosts() {
            this.isLoading = true;
            this.currentPage++;

            // Show loader
            const loader = document.getElementById('feed-loader');
            if (loader) loader.classList.remove('hidden');

            try {
                // Using generic load or specific partial? 
                // feed.js impl was vague. Assuming returning HTML or JSON.
                // If HTML, append. If JSON, render.
                // Preserving existing fetch behavior usually best. 
                // Let's assume endpoint returns PARTIAL HTML for simplicity in MVC
                const response = await fetch(`/Feed/LoadMore?feedType=${this.feedType}&page=${this.currentPage}`);
                const html = await response.text();

                if (html && html.trim().length > 0) {
                    const container = document.getElementById('feed-container');
                    container.insertAdjacentHTML('beforeend', html);
                    this.observeNewPosts();
                    if (typeof lucide !== 'undefined') lucide.createIcons();
                } else {
                    // End of feed
                    if (loader) loader.classList.add('hidden');
                    // Maybe remove sentinel
                }
            } catch (error) {
                console.error('Failed to load posts', error);
            } finally {
                this.isLoading = false;
                if (loader) loader.classList.add('hidden');
            }
        }

        // Actions
        async handleHide(contentId) {
            if (confirm('Are you sure you want to hide this content?')) {
                const res = await CC.Services.Feed.hide(contentId);
                if (res.success) {
                    const item = document.querySelector(`[data-content-id="${contentId}"]`);
                    if (item) item.remove();
                    if (CC.Services.Toaster) CC.Services.Toaster.success('Content hidden');
                }
            }
        }

        handleReport(contentId, contentType) {
            const reason = prompt('Please provide a reason:');
            if (reason) {
                CC.Services.Feed.report(contentId, contentType, reason).then(res => {
                    if (res.success) CC.Services.Toaster.success('Report submitted');
                    else CC.Services.Toaster.error('Failed to report');
                });
            }
        }
    }

    CC.Modules.Feed = new FeedModule();

    // Backwards Compatibility / Legacy Global Functions
    // Mapping old onclick handlers to new Logic or Services
    window.toggleLike = (id, type, e) => {
        // Prefer InteractionController if cleaner, but for now wrap to Service
        // toggleLike in feed.js was doing FETCH then DOM update.
        // CC.Components.Interaction should handle this automatically via clicks.
        // IF the HTML uses onclick="toggleLike(...)", we must keep this.
        if (CC.Components.Interaction) {
            // Dispatch or let controller handle? 
            // InteractionController listens to .action-btn events. 
            // If onclick is present, it might double fire if we rely on delegation + onclick.
            // We should effectively NO-OP here if InteractionController picks it up, 
            // OR delegate to InteractionController logic.
            // Best bet: Legacy support wrapper calls Service + updates UI manually if Controller doesn't catch it.
            // But actually, refactoring goal is to remove this. 
            // For safety, let's keep it working using the new Service.
            console.warn('Deprecated toggleLike called. Use data-action="like" instead.');
            CC.Services.Feed.interact(id, type, 'like').then(res => {
                if (res.success) {
                    // Manual UI update
                    const btn = e.target.closest('.action-btn');
                    if (btn) btn.classList.toggle('liked');
                    // Update count...
                }
            });
        }
    };

    // ... maps for other legacy functions shareContent, etc.

})(window.CommunityCar);
