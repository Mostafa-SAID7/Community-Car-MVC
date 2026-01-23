/**
 * Friends functionality for CommunityCar
 * Handles friend requests, notifications, and interactions
 */

class FriendsManager {
    constructor() {
        this.init();
    }

    init() {
        this.bindEvents();
        this.initializeTooltips();
    }

    bindEvents() {
        // Bind friend action buttons
        document.addEventListener('click', (e) => {
            if (e.target.matches('[data-action="send-friend-request"]')) {
                this.handleSendFriendRequest(e);
            } else if (e.target.matches('[data-action="accept-request"]')) {
                this.handleAcceptRequest(e);
            } else if (e.target.matches('[data-action="decline-request"]')) {
                this.handleDeclineRequest(e);
            } else if (e.target.matches('[data-action="remove-friend"]')) {
                this.handleRemoveFriend(e);
            } else if (e.target.matches('[data-action="block-user"]')) {
                this.handleBlockUser(e);
            }
        });

        // Close dropdown menus when clicking outside
        document.addEventListener('click', (e) => {
            if (!e.target.closest('.dropdown-menu') && !e.target.closest('[data-toggle="dropdown"]')) {
                this.closeAllDropdowns();
            }
        });
    }

    async handleSendFriendRequest(e) {
        e.preventDefault();
        const button = e.target.closest('button');
        const receiverId = button.dataset.receiverId;
        const receiverName = button.dataset.receiverName;

        if (!receiverId) return;

        try {
            button.disabled = true;
            button.innerHTML = '<i data-lucide="loader-2" class="w-4 h-4 animate-spin"></i> Sending...';

            const response = await this.makeRequest('/friends/send-request', {
                receiverId: receiverId
            });

            if (response.success) {
                button.innerHTML = '<i data-lucide="check" class="w-4 h-4"></i> Request Sent';
                button.classList.remove('btn-primary');
                button.classList.add('btn-secondary');
                this.showNotification('success', response.message);
            } else {
                throw new Error(response.message);
            }
        } catch (error) {
            button.disabled = false;
            button.innerHTML = '<i data-lucide="user-plus" class="w-4 h-4"></i> Add Friend';
            this.showNotification('error', error.message || this.getLocalizedMessage('ErrorSendingRequest', 'Failed to send friend request'));
        }
    }

    async handleAcceptRequest(e) {
        e.preventDefault();
        const button = e.target.closest('button');
        const friendshipId = button.dataset.friendshipId;

        if (!friendshipId) return;

        try {
            const response = await this.makeRequest('/friends/accept-request', {
                friendshipId: friendshipId
            });

            if (response.success) {
                this.showNotification('success', response.message);
                // Remove the request card or reload the page
                const requestCard = button.closest('.request-card');
                if (requestCard) {
                    requestCard.style.display = 'none';
                } else {
                    location.reload();
                }
            } else {
                throw new Error(response.message);
            }
        } catch (error) {
            this.showNotification('error', error.message || this.getLocalizedMessage('ErrorAcceptingRequest', 'Failed to accept friend request'));
        }
    }

    async handleDeclineRequest(e) {
        e.preventDefault();
        const button = e.target.closest('button');
        const friendshipId = button.dataset.friendshipId;

        if (!friendshipId) return;

        try {
            const response = await this.makeRequest('/friends/decline-request', {
                friendshipId: friendshipId
            });

            if (response.success) {
                this.showNotification('success', response.message);
                // Remove the request card or reload the page
                const requestCard = button.closest('.request-card');
                if (requestCard) {
                    requestCard.style.display = 'none';
                } else {
                    location.reload();
                }
            } else {
                throw new Error(response.message);
            }
        } catch (error) {
            this.showNotification('error', error.message || this.getLocalizedMessage('ErrorDecliningRequest', 'Failed to decline friend request'));
        }
    }

    async handleRemoveFriend(e) {
        e.preventDefault();
        const button = e.target.closest('button');
        const friendId = button.dataset.friendId;
        const friendName = button.dataset.friendName;

        if (!friendId || !confirm(this.getLocalizedMessage('ConfirmRemoveFriend', `Are you sure you want to remove ${friendName} from your friends?`).replace('{0}', friendName))) {
            return;
        }

        try {
            const response = await this.makeRequest('/friends/remove-friend', {
                friendId: friendId
            });

            if (response.success) {
                this.showNotification('success', response.message);
                location.reload();
            } else {
                throw new Error(response.message);
            }
        } catch (error) {
            this.showNotification('error', error.message || this.getLocalizedMessage('ErrorRemovingFriend', 'Failed to remove friend'));
        }
    }

    async handleBlockUser(e) {
        e.preventDefault();
        const button = e.target.closest('button');
        const userToBlockId = button.dataset.userToBlockId;
        const userName = button.dataset.userName;

        if (!userToBlockId || !confirm(this.getLocalizedMessage('ConfirmBlockUser', `Are you sure you want to block ${userName}? This will prevent them from contacting you.`).replace('{0}', userName))) {
            return;
        }

        try {
            const response = await this.makeRequest('/friends/block-user', {
                userToBlockId: userToBlockId
            });

            if (response.success) {
                this.showNotification('success', response.message);
                location.reload();
            } else {
                throw new Error(response.message);
            }
        } catch (error) {
            this.showNotification('error', error.message || this.getLocalizedMessage('ErrorBlockingUser', 'Failed to block user'));
        }
    }

    async makeRequest(url, data) {
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify(data)
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        return await response.json();
    }

    showNotification(type, message) {
        // Use the global notification system if available
        if (window.notify) {
            window.notify[type](message);
        } else {
            // Fallback to alert
            alert(message);
        }
    }

    getLocalizedMessage(key, fallback) {
        // Try to get localized message from window.friendsLocalizer if available
        if (window.friendsLocalizer && window.friendsLocalizer[key]) {
            return window.friendsLocalizer[key];
        }
        return fallback;
    }

    toggleDropdown(dropdownId) {
        const dropdown = document.getElementById(dropdownId);
        if (!dropdown) return;

        // Close all other dropdowns
        this.closeAllDropdowns();
        
        // Toggle current dropdown
        dropdown.classList.toggle('hidden');
    }

    closeAllDropdowns() {
        document.querySelectorAll('[id^="friend-menu-"], [id^="suggestion-menu-"]').forEach(menu => {
            menu.classList.add('hidden');
        });
    }

    initializeTooltips() {
        // Initialize tooltips for friend status indicators
        const tooltipElements = document.querySelectorAll('[data-tooltip]');
        tooltipElements.forEach(element => {
            element.addEventListener('mouseenter', this.showTooltip);
            element.addEventListener('mouseleave', this.hideTooltip);
        });
    }

    showTooltip(e) {
        const element = e.target;
        const tooltipText = element.dataset.tooltip;
        
        if (!tooltipText) return;

        const tooltip = document.createElement('div');
        tooltip.className = 'absolute z-50 px-2 py-1 text-xs bg-gray-900 text-white rounded shadow-lg pointer-events-none';
        tooltip.textContent = tooltipText;
        tooltip.id = 'tooltip';

        document.body.appendChild(tooltip);

        const rect = element.getBoundingClientRect();
        tooltip.style.left = rect.left + (rect.width / 2) - (tooltip.offsetWidth / 2) + 'px';
        tooltip.style.top = rect.top - tooltip.offsetHeight - 5 + 'px';
    }

    hideTooltip() {
        const tooltip = document.getElementById('tooltip');
        if (tooltip) {
            tooltip.remove();
        }
    }

    // Utility functions for external use
    viewProfile(userId) {
        window.location.href = `/profile/${userId}`;
    }

    viewMutualFriends(userId) {
        window.location.href = `/friends/mutual/${userId}`;
    }

    openChat(userId) {
        window.location.href = `/chats?user=${userId}`;
    }
}

// Initialize Friends Manager when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.friendsManager = new FriendsManager();
});

// Global functions for backward compatibility
function sendFriendRequest(receiverId, receiverName) {
    const button = event.target.closest('button');
    button.dataset.receiverId = receiverId;
    button.dataset.receiverName = receiverName;
    button.dataset.action = 'send-friend-request';
    window.friendsManager.handleSendFriendRequest(event);
}

function acceptRequest(friendshipId) {
    const button = event.target.closest('button');
    button.dataset.friendshipId = friendshipId;
    button.dataset.action = 'accept-request';
    window.friendsManager.handleAcceptRequest(event);
}

function declineRequest(friendshipId) {
    const button = event.target.closest('button');
    button.dataset.friendshipId = friendshipId;
    button.dataset.action = 'decline-request';
    window.friendsManager.handleDeclineRequest(event);
}

function cancelRequest(friendshipId) {
    declineRequest(friendshipId);
}

function removeFriend(friendId, friendName) {
    const button = event.target.closest('button');
    button.dataset.friendId = friendId;
    button.dataset.friendName = friendName;
    button.dataset.action = 'remove-friend';
    window.friendsManager.handleRemoveFriend(event);
}

function blockUser(userToBlockId, userName) {
    const button = event.target.closest('button');
    button.dataset.userToBlockId = userToBlockId;
    button.dataset.userName = userName;
    button.dataset.action = 'block-user';
    window.friendsManager.handleBlockUser(event);
}

function toggleFriendMenu(userId) {
    window.friendsManager.toggleDropdown(`friend-menu-${userId}`);
}

function toggleSuggestionMenu(userId) {
    window.friendsManager.toggleDropdown(`suggestion-menu-${userId}`);
}

function viewProfile(userId) {
    window.friendsManager.viewProfile(userId);
}

function viewMutualFriends(userId) {
    window.friendsManager.viewMutualFriends(userId);
}

function hideSuggestion(userId) {
    const card = event.target.closest('.bg-muted\\/20');
    if (card) {
        card.style.display = 'none';
        window.friendsManager.showNotification('success', window.friendsManager.getLocalizedMessage('SuggestionHidden', 'Suggestion hidden'));
    }
}