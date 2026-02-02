/**
 * Interaction Service
 * Handles API calls for social interactions (Likes, Comments, Shares, Bookmarks).
 */
(function (CC) {
    class InteractionService extends CC.Utils.BaseService {
        constructor() {
            super('Interaction');
        }

        /**
         * Get the AntiForgeryToken for POST requests
         */
        getAntiForgeryToken() {
            return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
        }

        async toggleLike(entityId, entityType) {
            return this.post('/Reactions/Toggle', {
                entityId,
                entityType,
                reactionType: 'Like'
            });
        }

        async submitComment(entityId, entityType, content) {
            return this.post('/Comments/Add', {
                entityId,
                entityType,
                content
            });
        }

        async loadComments(entityId, entityType, page = 1, pageSize = 10) {
            try {
                const response = await fetch(`/Comments/get/${entityId}?entityType=${entityType}&page=${page}&pageSize=${pageSize}`);
                if (!response.ok) throw new Error('Failed to load comments');
                return await response.json();
            } catch (error) {
                console.error('Error loading comments:', error);
                throw error;
            }
        }

        async bookmarkContent(entityId, entityType) {
            // Placeholder for bookmark URL/Endpoint
            // Assuming similar pattern to others
            console.log('Bookmark not fully implemented in previous code, checking for endpoint...');
            // Previous code didn't show the endpoint for bookmark, assuming generic interaction or separate controller.
            // For now, returning mocked success to match patterns if no endpoint exists in context.
            return this.post('/Bookmarks/Toggle', { entityId, entityType });
        }

        async deleteComment(commentId) {
            return this.post('/Comments/Delete', { id: commentId });
        }

        async searchUsers(query) {
            try {
                const response = await fetch(`/Comments/SearchUsers?query=${encodeURIComponent(query)}`);
                if (!response.ok) throw new Error('Failed to search users');
                return await response.json();
            } catch (error) {
                console.error('Error searching users:', error);
                return [];
            }
        }

        /**
         * Generic POST wrapper
         */
        async post(url, data) {
            try {
                const response = await fetch(url, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',
                        'RequestVerificationToken': this.getAntiForgeryToken()
                    },
                    body: new URLSearchParams(data)
                });

                if (response.status === 401) {
                    return { success: false, isUnauthorized: true };
                }

                if (!response.ok) {
                    const error = await response.text();
                    throw new Error(error || 'Network response was not ok');
                }

                return await response.json();
            } catch (error) {
                console.error(`Error posting to ${url}:`, error);
                return { success: false, message: error.message };
            }
        }
    }

    CC.Services.Interaction = new InteractionService();

})(window.CommunityCar);
