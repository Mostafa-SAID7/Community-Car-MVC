// Interactions JavaScript - Like, Comment, Share functionality
class InteractionManager {
    constructor() {
        this.initializeEventListeners();
    }

    initializeEventListeners() {
        // Reaction buttons
        document.addEventListener('click', (e) => {
            if (e.target.matches('.reaction-btn')) {
                this.handleReaction(e);
            }
            if (e.target.matches('.comment-btn')) {
                this.toggleCommentSection(e);
            }
            if (e.target.matches('.share-btn')) {
                this.handleShare(e);
            }
            if (e.target.matches('.submit-comment-btn')) {
                this.submitComment(e);
            }
            if (e.target.matches('.reply-btn')) {
                this.toggleReplyForm(e);
            }
            if (e.target.matches('.submit-reply-btn')) {
                this.submitReply(e);
            }
            if (e.target.matches('.edit-comment-btn')) {
                this.editComment(e);
            }
            if (e.target.matches('.delete-comment-btn')) {
                this.deleteComment(e);
            }
        });

        // Comment form submissions
        document.addEventListener('keypress', (e) => {
            if (e.target.matches('.comment-input') && e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                this.submitComment(e);
            }
        });
    }

    async handleReaction(event) {
        event.preventDefault();
        const btn = event.target.closest('.reaction-btn');
        const entityId = btn.dataset.entityId;
        const entityType = btn.dataset.entityType;
        const reactionType = btn.dataset.reactionType;
        const isActive = btn.classList.contains('active');

        try {
            let response;
            if (isActive) {
                // Remove reaction
                response = await fetch('/Reactions/Toggle', {
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
            } else {
                // Add reaction
                response = await fetch('/Reactions/Toggle', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',
                        'RequestVerificationToken': this.getAntiForgeryToken()
                    },
                    body: new URLSearchParams({
                        entityId: entityId,
                        entityType: entityType,
                        reactionType: reactionType
                    })
                });
            }

            const result = await response.json();
            if (result.success) {
                this.updateReactionUI(entityId, result.summary);
                this.showToast(result.message, 'success');
            } else {
                this.showToast(result.message, 'error');
            }
        } catch (error) {
            console.error('Error handling reaction:', error);
            this.showToast('Failed to update reaction', 'error');
        }
    }

    toggleCommentSection(event) {
        event.preventDefault();
        const btn = event.target.closest('.comment-btn');
        const entityId = btn.dataset.entityId;
        const entityType = btn.dataset.entityType;
        const commentsSection = document.querySelector(`#comments-${entityId}`);

        if (commentsSection.style.display === 'none' || !commentsSection.style.display) {
            commentsSection.style.display = 'block';
            this.loadComments(entityId, entityType);
        } else {
            commentsSection.style.display = 'none';
        }
    }

    async loadComments(entityId, entityType) {
        try {
            const response = await fetch(`/Comments/get/${entityId}?entityType=${entityType}`);
            const comments = await response.json();

            const commentsContainer = document.querySelector(`#comments-list-${entityId}`);
            commentsContainer.innerHTML = this.renderComments(comments);
        } catch (error) {
            console.error('Error loading comments:', error);
            this.showToast('Failed to load comments', 'error');
        }
    }

    async submitComment(event) {
        event.preventDefault();
        const btn = event.target.closest('.submit-comment-btn') || event.target;
        const form = btn.closest('.comment-form');
        const textarea = form.querySelector('.comment-input');
        const entityId = form.dataset.entityId;
        const entityType = form.dataset.entityType;
        const content = textarea.value.trim();

        if (!content) {
            this.showToast('Please enter a comment', 'warning');
            return;
        }

        try {
            const response = await fetch('/Comments/Add', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: new URLSearchParams({
                    content: content,
                    entityId: entityId,
                    entityType: entityType
                })
            });

            const result = await response.json();
            if (result.success) {
                textarea.value = '';
                this.addCommentToUI(entityId, result.comment);
                this.showToast('Comment added successfully', 'success');
            } else {
                this.showToast('Failed to add comment', 'error');
            }
        } catch (error) {
            console.error('Error submitting comment:', error);
            this.showToast('Failed to add comment', 'error');
        }
    }

    async submitReply(event) {
        event.preventDefault();
        const btn = event.target.closest('.submit-reply-btn');
        const form = btn.closest('.reply-form');
        const textarea = form.querySelector('.reply-input');
        const parentCommentId = form.dataset.parentCommentId;
        const content = textarea.value.trim();

        if (!content) {
            this.showToast('Please enter a reply', 'warning');
            return;
        }

        try {
            const response = await fetch('/Comments/Add', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: new URLSearchParams({
                    content: content,
                    parentCommentId: parentCommentId,
                    // We need entityId and entityType here too for the Add action
                    entityId: btn.closest('.comment-form')?.dataset.entityId || form.dataset.entityId || "",
                    entityType: btn.closest('.comment-form')?.dataset.entityType || form.dataset.entityType || ""
                })
            });

            const result = await response.json();
            if (result.success) {
                textarea.value = '';
                form.style.display = 'none';
                this.addReplyToUI(parentCommentId, result.reply);
                this.showToast('Reply added successfully', 'success');
            } else {
                this.showToast('Failed to add reply', 'error');
            }
        } catch (error) {
            console.error('Error submitting reply:', error);
            this.showToast('Failed to add reply', 'error');
        }
    }

    async handleShare(event) {
        event.preventDefault();
        const btn = event.target.closest('.share-btn');
        const entityId = btn.dataset.entityId;
        const entityType = btn.dataset.entityType;

        // Show share modal
        this.showShareModal(entityId, entityType);
    }

    async showShareModal(entityId, entityType) {
        try {
            const response = await fetch(`/Shared/Interactions/GetShareMetadata?entityId=${entityId}&entityType=${entityType}`);
            const metadata = await response.json();

            const modal = this.createShareModal(metadata);
            document.body.appendChild(modal);
            modal.style.display = 'block';
        } catch (error) {
            console.error('Error loading share metadata:', error);
            this.showToast('Failed to load share options', 'error');
        }
    }

    createShareModal(metadata) {
        const modal = document.createElement('div');
        modal.className = 'share-modal';
        modal.innerHTML = `
            <div class="share-modal-content">
                <div class="share-modal-header">
                    <h3>Share this content</h3>
                    <button class="close-modal">&times;</button>
                </div>
                <div class="share-modal-body">
                    <div class="share-preview">
                        <h4>${metadata.title}</h4>
                        <p>${metadata.description}</p>
                    </div>
                    <div class="share-options">
                        <button class="share-option" data-platform="facebook" data-url="${metadata.socialMediaUrls.facebook}">
                            <i class="fab fa-facebook"></i> Facebook
                        </button>
                        <button class="share-option" data-platform="twitter" data-url="${metadata.socialMediaUrls.twitter}">
                            <i class="fab fa-twitter"></i> Twitter
                        </button>
                        <button class="share-option" data-platform="linkedin" data-url="${metadata.socialMediaUrls.linkedin}">
                            <i class="fab fa-linkedin"></i> LinkedIn
                        </button>
                        <button class="share-option" data-platform="whatsapp" data-url="${metadata.socialMediaUrls.whatsapp}">
                            <i class="fab fa-whatsapp"></i> WhatsApp
                        </button>
                        <button class="copy-link-btn" data-url="${metadata.url}">
                            <i class="fas fa-link"></i> Copy Link
                        </button>
                    </div>
                </div>
            </div>
        `;

        // Add event listeners
        modal.querySelector('.close-modal').addEventListener('click', () => {
            modal.remove();
        });

        modal.querySelectorAll('.share-option').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const url = e.target.closest('.share-option').dataset.url;
                window.open(url, '_blank', 'width=600,height=400');
                this.recordShare(metadata.entityId, metadata.entityType, e.target.closest('.share-option').dataset.platform);
            });
        });

        modal.querySelector('.copy-link-btn').addEventListener('click', (e) => {
            const url = e.target.closest('.copy-link-btn').dataset.url;
            navigator.clipboard.writeText(url).then(() => {
                this.showToast('Link copied to clipboard', 'success');
                this.recordShare(metadata.entityId, metadata.entityType, 'link');
            });
        });

        return modal;
    }

    async recordShare(entityId, entityType, platform) {
        try {
            await fetch('/Shared/Interactions/ShareEntity', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify({
                    entityId: entityId,
                    entityType: parseInt(entityType),
                    shareType: platform === 'link' ? 2 : 4, // DirectLink or SocialMedia
                    platform: platform
                })
            });
        } catch (error) {
            console.error('Error recording share:', error);
        }
    }

    toggleReplyForm(event) {
        event.preventDefault();
        const btn = event.target.closest('.reply-btn');
        const commentId = btn.dataset.commentId;
        const replyForm = document.querySelector(`#reply-form-${commentId}`);

        if (replyForm.style.display === 'none' || !replyForm.style.display) {
            replyForm.style.display = 'block';
            replyForm.querySelector('.reply-input').focus();
        } else {
            replyForm.style.display = 'none';
        }
    }

    async editComment(event) {
        event.preventDefault();
        const btn = event.target.closest('.edit-comment-btn');
        const commentId = btn.dataset.commentId;
        const commentElement = document.querySelector(`#comment-${commentId}`);
        const contentElement = commentElement.querySelector('.comment-content');
        const currentContent = contentElement.textContent;

        // Replace content with textarea
        contentElement.innerHTML = `
            <textarea class="edit-comment-input">${currentContent}</textarea>
            <div class="edit-comment-actions">
                <button class="save-comment-btn" data-comment-id="${commentId}">Save</button>
                <button class="cancel-edit-btn" data-original-content="${currentContent}">Cancel</button>
            </div>
        `;

        // Add event listeners for save/cancel
        contentElement.querySelector('.save-comment-btn').addEventListener('click', async (e) => {
            const newContent = contentElement.querySelector('.edit-comment-input').value.trim();
            if (newContent) {
                try {
                    const response = await fetch('/Shared/Interactions/UpdateComment', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'RequestVerificationToken': this.getAntiForgeryToken()
                        },
                        body: JSON.stringify({
                            commentId: commentId,
                            content: newContent
                        })
                    });

                    const result = await response.json();
                    if (result.success) {
                        contentElement.innerHTML = newContent;
                        this.showToast('Comment updated successfully', 'success');
                    } else {
                        this.showToast(result.message, 'error');
                    }
                } catch (error) {
                    console.error('Error updating comment:', error);
                    this.showToast('Failed to update comment', 'error');
                }
            }
        });

        contentElement.querySelector('.cancel-edit-btn').addEventListener('click', (e) => {
            const originalContent = e.target.dataset.originalContent;
            contentElement.innerHTML = originalContent;
        });
    }

    async deleteComment(event) {
        event.preventDefault();
        const btn = event.target.closest('.delete-comment-btn');
        const commentId = btn.dataset.commentId;

        if (window.AlertModal) {
            window.AlertModal.confirm('Are you sure you want to delete this comment?', 'Delete Comment', (confirmed) => {
                if (confirmed) {
                    this.executeDeleteComment(commentId);
                }
            });
        } else if (confirm('Are you sure you want to delete this comment?')) {
            this.executeDeleteComment(commentId);
        }
    }

    async executeDeleteComment(commentId) {
        try {
            const response = await fetch('/Shared/Interactions/DeleteComment', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: new URLSearchParams({
                    commentId: commentId
                })
            });

            const result = await response.json();
            if (result.success) {
                document.querySelector(`#comment-${commentId}`).remove();
                this.showToast('Comment deleted successfully', 'success');
            } else {
                this.showToast(result.message, 'error');
            }
        } catch (error) {
            console.error('Error deleting comment:', error);
            this.showToast('Failed to delete comment', 'error');
        }
    }

    updateReactionUI(entityId, summary) {
        const reactionContainer = document.querySelector(`#reactions-${entityId}`);
        if (!reactionContainer) return;

        // Update reaction buttons
        summary.availableReactions.forEach(reaction => {
            const btn = reactionContainer.querySelector(`[data-reaction-type="${reaction.type}"]`);
            if (btn) {
                btn.classList.toggle('active', summary.userReaction === reaction.type);
                const countSpan = btn.querySelector('.reaction-count');
                if (countSpan) {
                    countSpan.textContent = reaction.count;
                }
            }
        });

        // Update total count
        const totalElement = reactionContainer.querySelector('.total-reactions');
        if (totalElement) {
            totalElement.textContent = `${summary.totalReactions} reactions`;
        }
    }

    renderComments(comments) {
        return comments.map(comment => `
            <div class="comment" id="comment-${comment.id}">
                <div class="comment-header">
                    <img src="${comment.authorAvatar || '/images/default-avatar.png'}" alt="${comment.authorName}" class="comment-avatar">
                    <div class="comment-meta">
                        <span class="comment-author">${comment.authorName}</span>
                        <span class="comment-time">${comment.timeAgo}</span>
                    </div>
                </div>
                <div class="comment-content">${comment.content}</div>
                <div class="comment-actions">
                    <button class="reply-btn" data-comment-id="${comment.id}">Reply</button>
                    ${comment.canEdit ? `<button class="edit-comment-btn" data-comment-id="${comment.id}">Edit</button>` : ''}
                    ${comment.canDelete ? `<button class="delete-comment-btn" data-comment-id="${comment.id}">Delete</button>` : ''}
                </div>
                <div class="reply-form" id="reply-form-${comment.id}" style="display: none;" data-parent-comment-id="${comment.id}">
                    <textarea class="reply-input" placeholder="Write a reply..."></textarea>
                    <button class="submit-reply-btn">Reply</button>
                </div>
                <div class="replies">
                    ${comment.replies ? comment.replies.map(reply => this.renderReply(reply)).join('') : ''}
                </div>
            </div>
        `).join('');
    }

    renderReply(reply) {
        return `
            <div class="reply" id="comment-${reply.id}">
                <div class="comment-header">
                    <img src="${reply.authorAvatar || '/images/default-avatar.png'}" alt="${reply.authorName}" class="comment-avatar">
                    <div class="comment-meta">
                        <span class="comment-author">${reply.authorName}</span>
                        <span class="comment-time">${reply.timeAgo}</span>
                    </div>
                </div>
                <div class="comment-content">${reply.content}</div>
                <div class="comment-actions">
                    ${reply.canEdit ? `<button class="edit-comment-btn" data-comment-id="${reply.id}">Edit</button>` : ''}
                    ${reply.canDelete ? `<button class="delete-comment-btn" data-comment-id="${reply.id}">Delete</button>` : ''}
                </div>
            </div>
        `;
    }

    addCommentToUI(entityId, comment) {
        const commentsList = document.querySelector(`#comments-list-${entityId}`);
        const commentHtml = this.renderComments([comment]);
        commentsList.insertAdjacentHTML('afterbegin', commentHtml);
    }

    addReplyToUI(parentCommentId, reply) {
        const repliesContainer = document.querySelector(`#comment-${parentCommentId} .replies`);
        const replyHtml = this.renderReply(reply);
        repliesContainer.insertAdjacentHTML('beforeend', replyHtml);
    }

    getAntiForgeryToken() {
        return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
    }

    showToast(message, type = 'info') {
        // Create toast notification
        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;
        toast.textContent = message;

        document.body.appendChild(toast);

        // Show toast
        setTimeout(() => toast.classList.add('show'), 100);

        // Hide toast after 3 seconds
        setTimeout(() => {
            toast.classList.remove('show');
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new InteractionManager();
});