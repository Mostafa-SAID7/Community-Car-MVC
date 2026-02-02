/**
 * Social Controller
 * Manages UI for follow buttons, friend management, and blocking.
 */
class SocialController extends CC.Utils.BaseComponent {
    constructor() {
        super('SocialController');
        this.service = CC.Services.Social;
    }

    init() {
        this.bindEvents();
        this.setupSubscriptions();
    }

    bindEvents() {
        // Universal follow button handler
        this.delegate('click', '[data-action="follow"]', async (e, target) => {
            const userId = target.dataset.userId;
            await this.handleFollowToggle(userId, target);
        });

        // Friend request handler
        this.delegate('click', '[data-action="friend-request"], [data-action="send-friend-request"]', async (e, target) => {
            const userId = target.dataset.userId;
            await this.handleFriendRequest(userId, target);
        });

        // Friend response (accept/reject)
        this.delegate('click', '[data-action="friend-respond"]', async (e, target) => {
            const requestId = target.dataset.requestId;
            const accept = target.dataset.accept === 'true';
            await this.handleFriendResponse(requestId, accept, target);
        });

        // Block handler
        this.delegate('click', '[data-action="block"]', async (e, target) => {
            const userId = target.dataset.userId;
            await this.handleBlockToggle(userId, target);
        });

        // Remove friend
        this.delegate('click', '[data-action="remove-friend"]', async (e, target) => {
            const userId = target.dataset.userId;
            await this.handleRemoveFriend(userId, target);
        });
    }

    setupSubscriptions() {
        this.service.on('follow:toggled', (data) => {
            this.updateFollowUI(data.userId, data.isFollowing);
        });

        this.service.on('friendRequest:sent', (data) => {
            this.updateFriendRequestUI(data.userId, 'sent');
        });

        this.service.on('block:toggled', (data) => {
            this.updateBlockUI(data.userId, data.isBlocked);
            if (data.isBlocked) {
                // If blocked, also update follow UI as they are likely unfollowed
                this.updateFollowUI(data.userId, false);
            }
        });
    }

    async handleFollowToggle(userId, button) {
        try {
            this.setLoading(button, true);
            const result = await this.service.toggleFollow(userId);
            if (result.success) {
                CC.Utils.Notifier.success(result.message || 'Updated follow status');
            } else {
                CC.Utils.Notifier.error(result.message || 'Failed to update follow status');
            }
        } catch (error) {
            console.error('Follow toggle error:', error);
        } finally {
            this.setLoading(button, false);
        }
    }

    async handleFriendRequest(userId, button) {
        try {
            this.setLoading(button, true);
            const result = await this.service.sendFriendRequest(userId);
            if (result.success) {
                CC.Utils.Notifier.success(result.message || 'Friend request sent');
            } else {
                CC.Utils.Notifier.error(result.message || 'Failed to send friend request');
            }
        } catch (error) {
            console.error('Friend request error:', error);
        } finally {
            this.setLoading(button, false);
        }
    }

    async handleFriendResponse(requestId, accept, button) {
        try {
            this.setLoading(button, true);
            const result = await this.service.respondToFriendRequest(requestId, accept);
            if (result.success) {
                CC.Utils.Notifier.success(result.message || (accept ? 'Request accepted' : 'Request rejected'));
                // Remove the row or update UI
                const container = button.closest('[data-friend-request-row]');
                if (container) {
                    container.classList.add('fade-out');
                    setTimeout(() => container.remove(), 300);
                }
            } else {
                CC.Utils.Notifier.error(result.message || 'Failed to process request');
            }
        } catch (error) {
            console.error('Friend response error:', error);
        } finally {
            this.setLoading(button, false);
        }
    }

    async handleBlockToggle(userId, button) {
        const isCurrentlyBlocked = button.classList.contains('blocked');
        const action = isCurrentlyBlocked ? 'Unblock' : 'Block';

        if (window.AlertModal) {
            window.AlertModal.confirm(
                `Are you sure you want to ${action.toLowerCase()} this user?`,
                `${action} User`,
                async (confirmed) => {
                    if (confirmed) {
                        this.executeBlockToggle(userId, button);
                    }
                }
            );
        } else if (confirm(`Are you sure you want to ${action.toLowerCase()} this user?`)) {
            this.executeBlockToggle(userId, button);
        }
    }

    async executeBlockToggle(userId, button) {
        try {
            this.setLoading(button, true);
            const result = await this.service.toggleBlock(userId);
            if (result.success) {
                CC.Utils.Notifier.success(result.message || 'Updated block status');
            } else {
                CC.Utils.Notifier.error(result.message || 'Failed to update block status');
            }
        } catch (error) {
            console.error('Block toggle error:', error);
        } finally {
            this.setLoading(button, false);
        }
    }

    async handleRemoveFriend(userId, button) {
        if (window.AlertModal) {
            window.AlertModal.confirm(
                'Are you sure you want to remove this friend?',
                'Remove Friend',
                async (confirmed) => {
                    if (confirmed) {
                        this.executeRemoveFriend(userId, button);
                    }
                }
            );
        } else if (confirm('Are you sure you want to remove this friend?')) {
            this.executeRemoveFriend(userId, button);
        }
    }

    async executeRemoveFriend(userId, button) {
        try {
            this.setLoading(button, true);
            const result = await this.service.removeFriend(userId);
            if (result.success) {
                CC.Utils.Notifier.success(result.message || 'Friend removed');
                const container = button.closest('[data-friend-row]');
                if (container) {
                    container.classList.add('fade-out');
                    setTimeout(() => container.remove(), 300);
                }
            } else {
                CC.Utils.Notifier.error(result.message || 'Failed to remove friend');
            }
        } catch (error) {
            console.error('Remove friend error:', error);
        } finally {
            this.setLoading(button, false);
        }
    }

    updateFollowUI(userId, isFollowing) {
        const buttons = document.querySelectorAll(`[data-action="follow"][data-user-id="${userId}"]`);
        buttons.forEach(btn => {
            const text = btn.querySelector('.follow-text') || btn;
            const icon = btn.querySelector('[data-lucide="user-plus"], [data-lucide="user-check"], [data-lucide="user-minus"]');

            if (isFollowing) {
                btn.classList.add('following', 'btn-secondary');
                btn.classList.remove('btn-primary');
                if (text && text !== btn) text.textContent = window.friendsLocalizer?.unfollow || 'Unfollow';
                if (icon) {
                    icon.setAttribute('data-lucide', 'user-minus');
                    if (window.lucide) window.lucide.createIcons({ parent: btn });
                }
            } else {
                btn.classList.remove('following', 'btn-secondary');
                btn.classList.add('btn-primary');
                if (text && text !== btn) text.textContent = window.friendsLocalizer?.follow || 'Follow';
                if (icon) {
                    icon.setAttribute('data-lucide', 'user-plus');
                    if (window.lucide) window.lucide.createIcons({ parent: btn });
                }
            }
        });
    }

    updateFriendRequestUI(userId, status) {
        const buttons = document.querySelectorAll(`[data-action="friend-request"][data-user-id="${userId}"]`);
        buttons.forEach(btn => {
            if (status === 'sent') {
                btn.disabled = true;
                btn.classList.add('opacity-50');
                const text = btn.querySelector('.request-text') || btn;
                if (text && text !== btn) text.textContent = window.friendsLocalizer?.requestSent || 'Request Sent';
            }
        });
    }

    updateBlockUI(userId, isBlocked) {
        const buttons = document.querySelectorAll(`[data-action="block"][data-user-id="${userId}"]`);
        buttons.forEach(btn => {
            const text = btn.querySelector('.block-text') || btn;
            if (isBlocked) {
                btn.classList.add('blocked', 'text-red-500');
                if (text && text !== btn) text.textContent = window.friendsLocalizer?.unblock || 'Unblock';
            } else {
                btn.classList.remove('blocked', 'text-red-500');
                if (text && text !== btn) text.textContent = window.friendsLocalizer?.block || 'Block';
            }
        });
    }

    setLoading(button, isLoading) {
        if (isLoading) {
            button.disabled = true;
            button.dataset.originalHtml = button.innerHTML;
            button.innerHTML = '<span class="animate-spin mr-2">⏳</span>';
        } else {
            button.disabled = false;
            if (button.dataset.originalHtml) {
                button.innerHTML = button.dataset.originalHtml;
            }
        }
    }
}

CC.Modules.Social = new SocialController();
