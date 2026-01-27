// Shared Interactions JavaScript - MVC Version
// Handles likes, comments, shares, bookmarks, views for all content types using MVC controllers

class SharedInteractions {
    constructor() {
        this.isLoading = false;
        this.init();
    }

    init() {
        this.bindEvents();
        this.initializeCounters();
    }

    bindEvents() {
        // Bind click events for action buttons
        document.addEventListener('click', (e) => {
            const button = e.target.closest('[data-action]');
            if (!button) return;

            const action = button.dataset.action;
            const entityId = button.dataset.entityId;
            const entityType = button.dataset.entityType;

            switch (action) {
                case 'toggle-like':
                    this.toggleLike(entityId, entityType, button);
                    break;
                case 'toggle-comments':
                    this.toggleComments(entityId);
                    break;
                case 'share-content':
                    this.shareContent(entityId, entityType);
                    break;
                case 'bookmark-content':
                    this.bookmarkContent(entityId, entityType, button);
                    break;
            }
        });

        // Bind comment submission
        document.addEventListener('keypress', (e) => {
            if (e.key === 'Enter' && e.target.matches('[data-comment-input]')) {
                const entityId = e.target.dataset.commentInput;
                const entityType = e.target.closest('[data-entity-type]')?.dataset.entityType;
                this.submitComment(entityId, entityType, e.target);
            }
        });
    }

    initializeCounters() {
        // Track views for visible content
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const element = entry.target;
                    const entityId = element.dataset.contentId;
                    const entityType = element.dataset.contentType;

                    if (entityId && entityType && !element.dataset.viewed) {
                        this.trackView(entityId, entityType);
                        element.dataset.viewed = 'true';
                    }
                }
            });
        }, { threshold: 0.5 });

        // Observe all content items
        document.querySelectorAll('[data-content-id]').forEach(item => {
            observer.observe(item);
        });
    }

    async toggleLike(entityId, entityType, button) {
        if (this.isLoading) return;

        try {
            this.isLoading = true;
            const isLiked = button.classList.contains('text-primary');

            // Optimistic UI update
            this.updateLikeButton(button, !isLiked);
            this.updateLikeCount(entityId, isLiked ? -1 : 1);

            const response = await fetch('/Reactions/Toggle', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: new URLSearchParams({
                    entityId: entityId,
                    entityType: entityType,
                    reactionType: 'Like'
                })
            });

            if (response.status === 401) {
                // User not authenticated - revert UI and show login prompt
                this.updateLikeButton(button, isLiked);
                this.updateLikeCount(entityId, isLiked ? 1 : -1);
                this.showLoginPrompt();
                return;
            }

            const result = await response.json();

            if (!result.success) {
                // Revert optimistic update on failure
                this.updateLikeButton(button, isLiked);
                this.updateLikeCount(entityId, isLiked ? 1 : -1);
                this.showError(result.message || 'Failed to update like');
            } else {
                // Update with actual counts from server
                this.updateLikeCount(entityId, result.data.likeCount, true);
            }
        } catch (error) {
            console.error('Error toggling like:', error);
            // Revert optimistic update on error
            const isLiked = !button.classList.contains('text-primary');
            this.updateLikeButton(button, isLiked);
            this.updateLikeCount(entityId, isLiked ? 1 : -1);
            this.showError('Failed to update like');
        } finally {
            this.isLoading = false;
        }
    }

    updateLikeButton(button, isLiked) {
        const icon = button.querySelector('[data-lucide="heart"]');

        if (isLiked) {
            button.classList.add('text-primary', 'bg-primary/5');
            button.classList.remove('text-muted-foreground');
            icon?.classList.add('fill-current');
        } else {
            button.classList.remove('text-primary', 'bg-primary/5');
            button.classList.add('text-muted-foreground');
            icon?.classList.remove('fill-current');
        }
    }

    updateLikeCount(entityId, change, absolute = false) {
        const statsBox = document.querySelector(`[data-stats-type="likes"][data-entity-id="${entityId}"]`);
        if (!statsBox) return;

        const countSpan = statsBox.querySelector('span');
        if (!countSpan) return;

        let currentCount = parseInt(countSpan.textContent.replace(/,/g, '')) || 0;
        let newCount = absolute ? change : currentCount + change;

        if (newCount < 0) newCount = 0;

        countSpan.textContent = newCount.toLocaleString();

        // Show/hide stats box based on count
        if (newCount === 0) {
            statsBox.style.display = 'none';
        } else {
            statsBox.style.display = 'inline-flex';
        }
    }

    toggleComments(entityId) {
        const commentsSection = document.getElementById(`comments-${entityId}`);
        if (!commentsSection) return;

        const isVisible = !commentsSection.classList.contains('hidden');

        if (isVisible) {
            commentsSection.classList.add('hidden');
        } else {
            commentsSection.classList.remove('hidden');
            // Load comments if not already loaded
            if (!commentsSection.dataset.loaded) {
                this.loadComments(entityId, commentsSection.dataset.entityType);
                commentsSection.dataset.loaded = 'true';
            }
        }
    }

    async loadComments(entityId, entityType) {
        try {
            const response = await fetch(`/Comments/get/${entityId}?entityType=${entityType}&page=1&pageSize=10`);
            const result = await response.json();

            if (result.success) {
                this.renderComments(entityId, result.data.comments);
            }
        } catch (error) {
            console.error('Error loading comments:', error);
        }
    }

    renderComments(entityId, comments) {
        const commentsList = document.getElementById(`comments-list-${entityId}`);
        if (!commentsList) return;

        if (comments.length === 0) {
            commentsList.innerHTML = `
                <div class="text-center py-8 text-muted-foreground">
                    <i data-lucide="message-circle" class="w-12 h-12 mx-auto mb-4 opacity-30"></i>
                    <p class="text-sm">No comments yet. Be the first to comment!</p>
                </div>
            `;
            return;
        }

        const commentsHtml = comments.map(comment => `
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

        commentsList.innerHTML = commentsHtml;

        // Reinitialize Lucide icons
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }
    }

    async submitComment(entityId, entityType, input) {
        const content = input.value.trim();
        if (!content) return;

        try {
            input.disabled = true;

            const response = await fetch('/Comments/Add', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: new URLSearchParams({
                    entityId: entityId,
                    entityType: entityType,
                    content: content
                })
            });

            if (response.status === 401) {
                this.showLoginPrompt();
                return;
            }

            const result = await response.json();

            if (result.success) {
                input.value = '';
                this.updateCommentCount(entityId, 1);
                // Reload comments to show the new one
                this.loadComments(entityId, entityType);
                this.showSuccess('Comment posted successfully!');
            } else {
                this.showError(result.message || 'Failed to post comment');
            }
        } catch (error) {
            console.error('Error posting comment:', error);
            this.showError('Failed to post comment');
        } finally {
            input.disabled = false;
        }
    }

    updateCommentCount(entityId, change) {
        const statsBox = document.querySelector(`[data-stats-type="comments"][data-entity-id="${entityId}"]`);
        if (!statsBox) return;

        const countSpan = statsBox.querySelector('span');
        if (!countSpan) return;

        let currentCount = parseInt(countSpan.textContent.replace(/,/g, '')) || 0;
        let newCount = currentCount + change;

        if (newCount < 0) newCount = 0;

        countSpan.textContent = newCount.toLocaleString();
    }

    async shareContent(entityId, entityType) {
        if (navigator.share) {
            try {
                await navigator.share({
                    title: 'Check out this content',
                    url: `${window.location.origin}/content/${entityId}`
                });
                this.trackShare(entityId, entityType, 'native');
            } catch (error) {
                if (error.name !== 'AbortError') {
                    this.showShareModal(entityId, entityType);
                }
            }
        } else {
            this.showShareModal(entityId, entityType);
        }
    }

    showShareModal(entityId, entityType) {
        const url = `${window.location.origin}/content/${entityId}`;

        if (window.AlertModal) {
            const shareHtml = `
                <div class="space-y-4">
                    <div class="flex items-center gap-3 p-3 bg-muted/30 rounded-lg">
                        <input type="text" value="${url}" readonly class="flex-1 bg-transparent text-sm" id="shareUrl">
                        <button onclick="navigator.clipboard.writeText('${url}'); window.AlertModal.success('Link copied!')" class="btn btn-sm btn-primary">Copy</button>
                    </div>
                    <div class="grid grid-cols-2 gap-3">
                        <button onclick="window.open('https://www.facebook.com/sharer/sharer.php?u=${encodeURIComponent(url)}', '_blank')" class="btn btn-outline">Facebook</button>
                        <button onclick="window.open('https://twitter.com/intent/tweet?url=${encodeURIComponent(url)}', '_blank')" class="btn btn-outline">Twitter</button>
                        <button onclick="window.open('https://wa.me/?text=${encodeURIComponent(url)}', '_blank')" class="btn btn-outline">WhatsApp</button>
                        <button onclick="window.open('https://t.me/share/url?url=${encodeURIComponent(url)}', '_blank')" class="btn btn-outline">Telegram</button>
                    </div>
                </div>
            `;

            window.AlertModal.show({
                type: 'info',
                title: 'Share Content',
                message: shareHtml,
                confirmText: 'Close'
            });
        }
    }

    async trackShare(entityId, entityType, platform) {
        try {
            await fetch('/Shares/Track', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: new URLSearchParams({
                    entityId: entityId,
                    entityType: entityType,
                    platform: platform
                })
            });

            this.updateShareCount(entityId, 1);
        } catch (error) {
            console.error('Error tracking share:', error);
        }
    }

    updateShareCount(entityId, change) {
        const statsBox = document.querySelector(`[data-stats-type="shares"][data-entity-id="${entityId}"]`);
        if (!statsBox) return;

        const countSpan = statsBox.querySelector('span');
        if (!countSpan) return;

        let currentCount = parseInt(countSpan.textContent.replace(/,/g, '')) || 0;
        let newCount = currentCount + change;

        if (newCount < 0) newCount = 0;

        countSpan.textContent = newCount.toLocaleString();

        // Show stats box if it wasn't visible
        if (newCount > 0) {
            statsBox.style.display = 'inline-flex';
        }
    }

    async bookmarkContent(entityId, entityType, button) {
        if (this.isLoading) return;

        try {
            this.isLoading = true;
            const isBookmarked = button.classList.contains('text-yellow-500');

            // Optimistic UI update
            this.updateBookmarkButton(button, !isBookmarked);

            const response = await fetch('/Bookmarks/Toggle', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: new URLSearchParams({
                    entityId: entityId,
                    entityType: entityType
                })
            });

            if (response.status === 401) {
                // User not authenticated - revert UI and show login prompt
                this.updateBookmarkButton(button, isBookmarked);
                this.showLoginPrompt();
                return;
            }

            const result = await response.json();

            if (!result.success) {
                // Revert optimistic update on failure
                this.updateBookmarkButton(button, isBookmarked);
                this.showError(result.message || 'Failed to update bookmark');
            } else {
                this.showSuccess(result.data.isBookmarked ? 'Content bookmarked!' : 'Bookmark removed');
            }
        } catch (error) {
            console.error('Error toggling bookmark:', error);
            // Revert optimistic update on error
            this.updateBookmarkButton(button, isBookmarked);
            this.showError('Failed to update bookmark');
        } finally {
            this.isLoading = false;
        }
    }

    updateBookmarkButton(button, isBookmarked) {
        const icon = button.querySelector('[data-lucide="bookmark"]');

        if (isBookmarked) {
            button.classList.add('text-yellow-500', 'bg-yellow-500/5');
            button.classList.remove('text-muted-foreground');
            icon?.classList.add('fill-current');
        } else {
            button.classList.remove('text-yellow-500', 'bg-yellow-500/5');
            button.classList.add('text-muted-foreground');
            icon?.classList.remove('fill-current');
        }
    }

    async trackView(entityId, entityType) {
        try {
            await fetch('/Views/Track', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify({
                    entityId: entityId,
                    entityType: entityType
                })
            });
        } catch (error) {
            console.error('Error tracking view:', error);
        }
    }

    getAntiForgeryToken() {
        return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
    }

    showSuccess(message) {
        if (window.AlertModal) {
            window.AlertModal.success(message);
        } else {
            console.log('Success:', message);
        }
    }

    showError(message) {
        if (window.AlertModal) {
            window.AlertModal.error(message);
        } else {
            console.error('Error:', message);
        }
    }

    showLoginPrompt() {
        if (window.AlertModal) {
            window.AlertModal.show({
                type: 'info',
                title: 'Login Required',
                message: 'You need to be logged in to interact with content. Would you like to login now?',
                confirmText: 'Login',
                showCancel: true,
                cancelText: 'Cancel',
                callback: (confirmed) => {
                    if (confirmed) {
                        window.location.href = '/Login?returnUrl=' + encodeURIComponent(window.location.pathname);
                    }
                }
            });
        } else if (confirm('You need to be logged in to interact with content. Would you like to login now?')) {
            window.location.href = '/Login?returnUrl=' + encodeURIComponent(window.location.pathname);
        }
    }
}

// Global functions for backward compatibility
window.toggleLike = function (entityId, entityType) {
    // Try to find button by data attributes first
    let button = document.querySelector(`[data-action="toggle-like"][data-entity-id="${entityId}"]`);

    // If not found, try alternative selectors
    if (!button) {
        button = document.querySelector(`[data-button-type="like"][data-entity-id="${entityId}"]`);
    }

    if (button && window.sharedInteractions) {
        window.sharedInteractions.toggleLike(entityId, entityType, button);
    } else {
        console.error('Like button not found for entity:', entityId);
    }
};

window.toggleComments = function (entityId) {
    if (window.sharedInteractions) {
        window.sharedInteractions.toggleComments(entityId);
    }
};

window.shareContent = function (entityId, entityType) {
    if (window.sharedInteractions) {
        window.sharedInteractions.shareContent(entityId, entityType);
    }
};

window.bookmarkContent = function (entityId, entityType) {
    const button = document.querySelector(`[onclick*="bookmarkContent('${entityId}'"]`);
    if (button && window.sharedInteractions) {
        window.sharedInteractions.bookmarkContent(entityId, entityType, button);
    }
};

window.handleCommentSubmit = function (event, entityId, entityType) {
    if (event.key === 'Enter' && window.sharedInteractions) {
        window.sharedInteractions.submitComment(entityId, entityType, event.target);
    }
};

window.submitComment = function (entityId, entityType) {
    const input = document.querySelector(`[data-comment-input="${entityId}"]`);
    if (input && window.sharedInteractions) {
        window.sharedInteractions.submitComment(entityId, entityType, input);
    }
};

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function () {
    window.sharedInteractions = new SharedInteractions();
});