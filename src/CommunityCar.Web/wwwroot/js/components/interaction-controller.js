/**
 * Interaction Controller Component
 * Handles UI events for interactions and updates the DOM.
 * Uses InteractionService for API calls.
 */
(function (CC) {
    class InteractionController extends CC.Utils.BaseComponent {
        constructor() {
            super('InteractionController');
            this.isLoading = false;
        }

        init() {
            if (this.initialized) return;
            super.init();

            this.setupEventDelegation();
            this.initializeCounters();
        }

        setupEventDelegation() {
            // Click interactions
            document.addEventListener('click', (e) => this.handleClick(e));

            // Comment input
            document.addEventListener('keypress', (e) => {
                if (e.key === 'Enter' && e.target.matches('[data-comment-input]')) {
                    const entityId = e.target.dataset.commentInput;
                    const entityType = e.target.closest('[data-entity-type]')?.dataset.entityType;
                    this.handleCommentSubmit(entityId, entityType, e.target);
                }
            });
        }

        handleClick(e) {
            const button = e.target.closest('[data-action]');
            if (!button) return;

            const action = button.dataset.action;
            const entityId = button.dataset.entityId;
            const entityType = button.dataset.entityType;

            switch (action) {
                case 'toggle-like':
                    this.handleLikeToggle(entityId, entityType, button);
                    break;
                case 'toggle-comments':
                    this.handleCommentsToggle(entityId, entityType);
                    break;
                case 'share-content':
                    this.handleShare(entityId, entityType);
                    break;
                case 'bookmark-content':
                    this.handleBookmark(entityId, entityType, button);
                    break;
            }
        }

        initializeCounters() {
            // Intersection Observer for view tracking
            const observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        const element = entry.target;
                        const entityId = element.dataset.contentId;
                        const entityType = element.dataset.contentType;

                        if (entityId && entityType && !element.dataset.viewed) {
                            // Assuming View Tracking might be part of InteractionService later
                            // specific view tracking logic was simplified in previous shared-interactions
                            // For now just marking as viewed
                            element.dataset.viewed = 'true';
                        }
                    }
                });
            }, { threshold: 0.5 });

            document.querySelectorAll('[data-content-id]').forEach(item => observer.observe(item));
        }

        // --- Handlers ---

        async handleLikeToggle(entityId, entityType, button) {
            if (this.isLoading) return;

            const isLiked = button.classList.contains('text-primary');

            // UI Optimistic Update
            this.updateLikeButton(button, !isLiked);
            this.updateCounter(entityId, 'likes', isLiked ? -1 : 1);

            try {
                this.isLoading = true;
                const result = await CC.Services.Interaction.toggleLike(entityId, entityType);

                if (result.isUnauthorized) {
                    // Revert and show login
                    this.revertLikeState(button, isLiked, entityId);
                    // Use Toaster for login prompt or redirect
                    if (CC.Services.Toaster) CC.Services.Toaster.info('Please login to react.', 'Authentication Required');
                    return;
                }

                if (!result.success) {
                    this.revertLikeState(button, isLiked, entityId);
                    if (CC.Services.Toaster) CC.Services.Toaster.error(result.message || 'Failed to update like');
                } else if (result.data) {
                    // Sync with server count if available
                    this.updateCounter(entityId, 'likes', result.data.likeCount, true);
                }

            } catch (error) {
                this.revertLikeState(button, isLiked, entityId);
            } finally {
                this.isLoading = false;
            }
        }

        async handleCommentSubmit(entityId, entityType, input) {
            const content = input.value.trim();
            if (!content) return;

            input.disabled = true;

            try {
                const result = await CC.Services.Interaction.submitComment(entityId, entityType, content);

                if (result.isUnauthorized) {
                    if (CC.Services.Toaster) CC.Services.Toaster.info('Please login to comment.');
                    return;
                }

                if (result.success) {
                    input.value = '';
                    this.updateCounter(entityId, 'comments', 1);
                    this.refreshComments(entityId, entityType);
                    if (CC.Services.Toaster) CC.Services.Toaster.success('Comment posted successfully!');
                } else {
                    if (CC.Services.Toaster) CC.Services.Toaster.error(result.message || 'Failed to post comment');
                }
            } catch (error) {
                if (CC.Services.Toaster) CC.Services.Toaster.error('An error occurred while posting.');
            } finally {
                input.disabled = false;
                input.focus();
            }
        }

        handleCommentsToggle(entityId, entityType) {
            const section = document.getElementById(`comments-${entityId}`);
            if (!section) return;

            if (section.classList.contains('hidden')) {
                section.classList.remove('hidden');
                if (!section.dataset.loaded) {
                    this.refreshComments(entityId, entityType);
                    section.dataset.loaded = 'true';
                }
            } else {
                section.classList.add('hidden');
            }
        }

        async refreshComments(entityId, entityType) {
            const container = document.getElementById(`comments-list-${entityId}`);
            if (!container) return;

            try {
                const result = await CC.Services.Interaction.loadComments(entityId, entityType);
                if (result.success && result.data && result.data.comments) {
                    this.renderComments(container, result.data.comments);
                }
            } catch (error) {
                container.innerHTML = '<p class="text-sm text-center text-red-500">Failed to load comments.</p>';
            }
        }

        handleShare(entityId, entityType) {
            if (navigator.share) {
                navigator.share({
                    title: 'Community Car',
                    url: `${window.location.origin}/content/${entityId}`
                }).catch(console.error);
            } else {
                // Fallback copy to clipboard
                navigator.clipboard.writeText(`${window.location.origin}/content/${entityId}`);
                if (CC.Services.Toaster) CC.Services.Toaster.success('Link copied to clipboard!');
            }
        }

        handleBookmark(entityId, entityType, button) {
            // Toggle logic similar to like
            // Allowing generic implementation for now
            button.classList.toggle('fill-current');
            button.classList.toggle('text-primary');
            CC.Services.Interaction.bookmarkContent(entityId, entityType); // Fire and forget for now
        }


        // --- UI Helpers ---

        updateLikeButton(button, isLiked) {
            const icon = button.querySelector('[data-lucide="heart"]');
            if (isLiked) {
                button.classList.add('text-primary', 'bg-primary/5');
                button.classList.remove('text-muted-foreground');
                if (icon) icon.classList.add('fill-current');
            } else {
                button.classList.remove('text-primary', 'bg-primary/5');
                button.classList.add('text-muted-foreground');
                if (icon) icon.classList.remove('fill-current');
            }
        }

        revertLikeState(button, wasLiked, entityId) {
            this.updateLikeButton(button, wasLiked);
            this.updateCounter(entityId, 'likes', wasLiked ? 1 : -1);
        }

        updateCounter(entityId, type, change, isAbsolute = false) {
            const statsBox = document.querySelector(`[data-stats-type="${type}"][data-entity-id="${entityId}"]`);
            if (!statsBox) return;

            const countSpan = statsBox.querySelector('span');
            if (!countSpan) return;

            let currentCount = parseInt(countSpan.textContent.replace(/,/g, '')) || 0;
            let newCount = isAbsolute ? change : currentCount + change;
            if (newCount < 0) newCount = 0;

            countSpan.textContent = newCount.toLocaleString();

            if (newCount === 0) {
                statsBox.classList.add('hidden');
                statsBox.classList.remove('inline-flex');
            } else {
                statsBox.classList.remove('hidden');
                statsBox.classList.add('inline-flex');
            }
        }

        renderComments(container, comments) {
            if (!comments || comments.length === 0) {
                container.innerHTML = `
                    <div class="text-center py-8 text-muted-foreground">
                        <i data-lucide="message-circle" class="w-12 h-12 mx-auto mb-4 opacity-30"></i>
                         <p class="text-sm">No comments yet. Be the first to comment!</p>
                    </div>`;
                return;
            }

            container.innerHTML = comments.map(comment => `
                <div class="flex gap-3 p-3 rounded-lg bg-background/50 border border-border/30" data-comment-id="${comment.id}">
                    <div class="w-8 h-8 rounded-full overflow-hidden shrink-0">
                        <img src="${comment.authorAvatar || '/images/default-avatar.png'}" alt="${comment.authorName}" class="w-full h-full object-cover" />
                    </div>
                    <div class="flex-1">
                        <div class="flex items-center gap-2 mb-1">
                            <span class="text-sm font-bold text-foreground">${comment.authorName}</span>
                            <span class="text-xs text-muted-foreground">${comment.timeAgo}</span>
                        </div>
                        <p class="text-sm text-foreground/90 leading-relaxed">${comment.content}</p>
                    </div>
                </div>
            `).join('');

            if (typeof lucide !== 'undefined') lucide.createIcons();
        }
    }

    // Initialize Singleton
    CC.Components.Interaction = new InteractionController();

})(window.CommunityCar);
