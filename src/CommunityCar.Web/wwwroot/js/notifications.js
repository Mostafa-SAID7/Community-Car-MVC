// SignalR Notifications Client
class NotificationClient {
    constructor() {
        this.connection = null;
        this.notifications = [];
        this.unreadCount = 0;
        this.isDropdownOpen = false;
        this.reconnectAttempts = 0;
        this.maxReconnectAttempts = 5;

        this.init();
    }

    async init() {
        try {
            // Initialize SignalR connection
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl("/hubs/notifications")
                .withAutomaticReconnect([0, 2000, 10000, 30000])
                .build();

            // Set up event handlers
            this.setupEventHandlers();

            // Start connection
            await this.start();

            // Initialize UI
            this.initializeUI();

            console.log('Notification client initialized successfully');
        } catch (error) {
            console.error('Failed to initialize notification client:', error);
        }
    }

    setupEventHandlers() {
        // Connection events
        this.connection.onreconnecting(() => {
            console.log('Notification connection lost. Reconnecting...');
        });

        this.connection.onreconnected(() => {
            console.log('Notification connection restored');
            this.reconnectAttempts = 0;
        });

        this.connection.onclose(() => {
            console.log('Notification connection closed');
        });

        // Notification events
        this.connection.on('ReceiveNotification', (notification) => {
            this.handleReceiveNotification(notification);
        });

        this.connection.on('NotificationMarkedAsRead', (notificationId) => {
            this.handleNotificationMarkedAsRead(notificationId);
        });

        this.connection.on('AllNotificationsMarkedAsRead', () => {
            this.handleAllNotificationsMarkedAsRead();
        });
    }

    async start() {
        try {
            await this.connection.start();
            window.notificationConnection = this.connection; // Expose globally
            console.log('✅ Notification SignalR connection started');
        } catch (error) {
            console.error('Error starting notification connection:', error);

            // Retry connection
            if (this.reconnectAttempts < this.maxReconnectAttempts) {
                this.reconnectAttempts++;
                setTimeout(() => this.start(), 5000);
            }
        }
    }

    initializeUI() {
        // Set up notification bell click handlers (Desktop & Mobile)
        const notificationBells = document.querySelectorAll('.notification-bell, .mobile-notification-bell');
        notificationBells.forEach(bell => {
            bell.addEventListener('click', (e) => {
                e.stopPropagation();
                this.toggleNotificationDropdown();
            });
        });

        // Set up mark all as read buttons (Desktop & Mobile)
        const markAllBtns = document.querySelectorAll('.mark-all-read-btn, .mobile-mark-all-read-btn');
        markAllBtns.forEach(btn => {
            btn.addEventListener('click', (e) => {
                e.stopPropagation();
                this.markAllAsRead();
            });
        });

        // Close dropdown when clicking outside
        document.addEventListener('click', (e) => {
            if (this.isDropdownOpen && !e.target.closest('.notification-wrapper')) {
                this.closeNotificationDropdown();
            }
        });

        // Load existing notifications
        this.loadNotifications();
    }

    async loadNotifications() {
        try {
            const response = await fetch('/shared/Notifications?page=1&pageSize=20', {
                headers: {
                    'Accept': 'application/json'
                }
            });
            const result = await response.json();

            if (result.success && result.data) {
                this.notifications = result.data;
                this.unreadCount = this.notifications.filter(n => !n.isRead).length;
                this.updateNotificationUI();
            }
        } catch (error) {
            console.error('Error loading notifications:', error);
        }
    }

    // Event handlers
    handleReceiveNotification(notification) {
        console.log('Received notification:', notification);

        // Add to notifications array
        this.notifications.unshift(notification);

        // Update unread count
        const isRead = notification.isRead ?? notification.IsRead ?? false;
        if (!isRead) {
            this.unreadCount++;
        }

        // Update UI
        this.updateNotificationUI();

        // Show toast notification
        this.showToastNotification(notification);

        // Play notification sound
        this.playNotificationSound();
    }

    handleNotificationMarkedAsRead(notificationId) {
        const notification = this.notifications.find(n => (n.id || n.Id) === notificationId);
        if (notification) {
            const isRead = notification.isRead ?? notification.IsRead ?? false;
            if (!isRead) {
                if (notification.isRead !== undefined) notification.isRead = true;
                if (notification.IsRead !== undefined) notification.IsRead = true;
                this.unreadCount = Math.max(0, this.unreadCount - 1);
                this.updateNotificationUI();
            }
        }
    }

    handleAllNotificationsMarkedAsRead() {
        this.notifications.forEach(notification => {
            if (notification.isRead !== undefined) notification.isRead = true;
            if (notification.IsRead !== undefined) notification.IsRead = true;
        });
        this.unreadCount = 0;
        this.updateNotificationUI();
    }

    // UI Methods
    toggleNotificationDropdown() {
        const dropdowns = document.querySelectorAll('.notification-dropdown, .mobile-notification-dropdown');
        if (dropdowns.length === 0) return;

        if (this.isDropdownOpen) {
            this.closeNotificationDropdown();
        } else {
            this.openNotificationDropdown();
        }
    }

    openNotificationDropdown() {
        const dropdowns = document.querySelectorAll('.notification-dropdown, .mobile-notification-dropdown');
        if (dropdowns.length === 0) return;

        dropdowns.forEach(dropdown => {
            dropdown.classList.remove('hidden');
        });
        this.isDropdownOpen = true;

        // Load notifications if empty
        if (this.notifications.length === 0) {
            this.loadNotifications();
        }

        this.renderNotifications();
    }

    closeNotificationDropdown() {
        const dropdowns = document.querySelectorAll('.notification-dropdown, .mobile-notification-dropdown');
        if (dropdowns.length === 0) return;

        dropdowns.forEach(dropdown => {
            dropdown.classList.add('hidden');
        });
        this.isDropdownOpen = false;
    }

    updateNotificationUI() {
        // Update notification badge
        const badges = document.querySelectorAll('.notification-badge, .mobile-notification-badge');
        badges.forEach(badge => {
            if (this.unreadCount > 0) {
                badge.classList.remove('hidden');
                badge.textContent = this.unreadCount > 9 ? '9+' : this.unreadCount.toString();
                if (this.unreadCount > 9) {
                    badge.classList.add('has-count');
                } else {
                    badge.classList.remove('has-count');
                }
            } else {
                badge.classList.add('hidden');
            }
        });

        // Update dropdown if open
        if (this.isDropdownOpen) {
            this.renderNotifications();
        }
    }

    renderNotifications() {
        const notificationLists = document.querySelectorAll('.notification-list, .mobile-notification-list');
        if (notificationLists.length === 0) return;

        notificationLists.forEach(notificationList => {
            if (this.notifications.length === 0) {
                notificationList.innerHTML = `
                    <div class="flex flex-col items-center justify-center py-12 px-4 text-center">
                        <div class="w-12 h-12 rounded-2xl bg-muted flex items-center justify-center mb-4">
                            <i data-lucide="bell-off" class="w-6 h-6 text-muted-foreground"></i>
                        </div>
                        <h3 class="text-sm font-semibold mb-1">No notifications</h3>
                        <p class="text-xs text-muted-foreground">You're all caught up!</p>
                    </div>
                `;
            } else {
                notificationList.innerHTML = this.notifications
                    .slice(0, 20) // Show only latest 20 notifications
                    .map(notification => this.createNotificationHTML(notification))
                    .join('');
            }
        });

        // Initialize Lucide icons
        lucide.createIcons();

        // Add click handlers
        this.addNotificationClickHandlers();
    }

    createNotificationHTML(notification) {
        const createdAt = notification.createdAt || notification.CreatedAt;
        const timeAgo = this.formatTimeAgo(createdAt);
        const isRead = notification.isRead ?? notification.IsRead ?? false;
        const unreadClass = isRead ? '' : 'unread';
        const id = notification.id || notification.Id;
        const actionUrl = notification.actionUrl || notification.ActionUrl;
        const type = notification.type || notification.Type;
        const iconClass = notification.iconClass || notification.IconClass;
        const title = notification.title || notification.Title;
        const message = notification.message || notification.Message;

        return `
            <div class="notification-item ${unreadClass} p-3 hover:bg-muted/50 transition-colors cursor-pointer border-b border-border last:border-0" data-notification-id="${id}" data-action-url="${actionUrl || ''}">
                <div class="flex gap-3">
                    <div class="notification-icon w-8 h-8 rounded-full flex items-center justify-center shrink-0 ${type}">
                        <i data-lucide="${iconClass || 'bell'}" class="w-4 h-4"></i>
                    </div>
                    <div class="notification-content flex-1 min-w-0">
                        <div class="notification-title text-sm font-semibold truncate">${this.escapeHtml(title)}</div>
                        <div class="notification-message text-xs text-muted-foreground line-clamp-2">${this.escapeHtml(message)}</div>
                        <div class="notification-time text-[10px] text-muted-foreground mt-1">${timeAgo}</div>
                    </div>
                </div>
            </div>
        `;
    }

    addNotificationClickHandlers() {
        const notificationItems = document.querySelectorAll('.notification-item');
        notificationItems.forEach(item => {
            item.addEventListener('click', () => {
                const notificationId = item.getAttribute('data-notification-id');
                const actionUrl = item.getAttribute('data-action-url');

                // Mark as read
                if (item.classList.contains('unread')) {
                    this.markAsRead(notificationId);
                }

                // Navigate to action URL
                if (actionUrl && actionUrl !== 'null') {
                    window.location.href = actionUrl;
                }

                this.closeNotificationDropdown();
            });
        });
    }

    showToastNotification(notification) {
        const type = notification.type || notification.Type;
        const iconClass = notification.iconClass || notification.IconClass;
        const title = notification.title || notification.Title;
        const message = notification.message || notification.Message;
        const actionUrl = notification.actionUrl || notification.ActionUrl;

        // Create toast element
        const toast = document.createElement('div');
        toast.className = 'notification-toast';
        toast.innerHTML = `
            <div class="notification-toast-content">
                <div class="notification-toast-icon ${type}">
                    <i data-lucide="${iconClass || 'bell'}"></i>
                </div>
                <div class="notification-toast-body">
                    <div class="notification-toast-title">${this.escapeHtml(title)}</div>
                    <div class="notification-toast-message">${this.escapeHtml(message)}</div>
                </div>
                <button class="notification-toast-close" type="button">
                    <i data-lucide="x"></i>
                </button>
            </div>
            <div class="notification-toast-progress"></div>
        `;

        // Add to DOM
        document.body.appendChild(toast);

        // Initialize Lucide icons
        lucide.createIcons();

        // Show toast
        setTimeout(() => {
            toast.classList.add('visible');
        }, 100);

        // Set up close button
        const closeBtn = toast.querySelector('.notification-toast-close');
        closeBtn.addEventListener('click', () => {
            this.removeToast(toast);
        });

        // Auto-remove after 5 seconds
        const progressBar = toast.querySelector('.notification-toast-progress');
        progressBar.style.width = '100%';
        progressBar.style.transitionDuration = '5s';

        setTimeout(() => {
            progressBar.style.width = '0%';
        }, 100);

        setTimeout(() => {
            this.removeToast(toast);
        }, 5000);

        // Click to navigate
        if (notification.ActionUrl) {
            toast.addEventListener('click', (e) => {
                if (!e.target.closest('.notification-toast-close')) {
                    window.location.href = notification.ActionUrl;
                }
            });
        }
    }

    removeToast(toast) {
        toast.classList.add('removing');
        setTimeout(() => {
            if (toast.parentElement) {
                toast.parentElement.removeChild(toast);
            }
        }, 300);
    }

    //  Methods
    async markAsRead(notificationId) {
        if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
            return;
        }

        try {
            await this.connection.invoke('MarkNotificationAsRead', notificationId);
        } catch (error) {
            console.error('Error marking notification as read:', error);
        }
    }

    async markAllAsRead() {
        if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
            return;
        }

        try {
            await this.connection.invoke('MarkAllNotificationsAsRead');
        } catch (error) {
            console.error('Error marking all notifications as read:', error);
        }
    }

    // Utility Methods
    playNotificationSound() {
        try {
            const audio = new Audio('/sounds/notification.mp3');
            audio.volume = 0.2;
            audio.play().catch(() => {
                // Ignore audio play errors (user interaction required)
            });
        } catch (error) {
            // Ignore audio errors
        }
    }

    formatTimeAgo(dateString) {
        const date = new Date(dateString);
        const now = new Date();
        const diffInMinutes = Math.floor((now - date) / (1000 * 60));

        if (diffInMinutes < 1) {
            return 'Just now';
        } else if (diffInMinutes < 60) {
            return `${diffInMinutes}m ago`;
        } else if (diffInMinutes < 1440) {
            return `${Math.floor(diffInMinutes / 60)}h ago`;
        } else if (diffInMinutes < 10080) {
            return `${Math.floor(diffInMinutes / 1440)}d ago`;
        } else {
            return date.toLocaleDateString();
        }
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
}

// Initialize notification client when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    if (typeof signalR !== 'undefined') {
        window.notificationClient = new NotificationClient();
    } else {
        console.warn('SignalR library not loaded');
    }
});