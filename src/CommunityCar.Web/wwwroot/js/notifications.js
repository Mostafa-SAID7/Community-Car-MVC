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
            console.log('Notification SignalR connection started');
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
        // Set up notification bell click handler
        const notificationBell = document.querySelector('.notification-bell');
        if (notificationBell) {
            notificationBell.addEventListener('click', (e) => {
                e.stopPropagation();
                this.toggleNotificationDropdown();
            });
        }

        // Set up mark all as read button
        const markAllBtn = document.querySelector('.mark-all-read-btn');
        if (markAllBtn) {
            markAllBtn.addEventListener('click', () => {
                this.markAllAsRead();
            });
        }

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
            // TODO: Load notifications from API when implemented
            // const response = await fetch('/api/notifications');
            // const data = await response.json();
            // this.notifications = data.notifications || [];
            // this.unreadCount = data.unreadCount || 0;
            // this.updateNotificationUI();
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
        if (!notification.IsRead) {
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
        const notification = this.notifications.find(n => n.Id === notificationId);
        if (notification && !notification.IsRead) {
            notification.IsRead = true;
            this.unreadCount = Math.max(0, this.unreadCount - 1);
            this.updateNotificationUI();
        }
    }

    handleAllNotificationsMarkedAsRead() {
        this.notifications.forEach(notification => {
            notification.IsRead = true;
        });
        this.unreadCount = 0;
        this.updateNotificationUI();
    }

    // UI Methods
    toggleNotificationDropdown() {
        const dropdown = document.querySelector('.notification-dropdown');
        if (!dropdown) return;

        if (this.isDropdownOpen) {
            this.closeNotificationDropdown();
        } else {
            this.openNotificationDropdown();
        }
    }

    openNotificationDropdown() {
        const dropdown = document.querySelector('.notification-dropdown');
        if (!dropdown) return;

        dropdown.classList.add('visible');
        this.isDropdownOpen = true;
        
        // Load notifications if empty
        if (this.notifications.length === 0) {
            this.loadNotifications();
        }
        
        this.renderNotifications();
    }

    closeNotificationDropdown() {
        const dropdown = document.querySelector('.notification-dropdown');
        if (!dropdown) return;

        dropdown.classList.remove('visible');
        this.isDropdownOpen = false;
    }

    updateNotificationUI() {
        // Update notification badge
        const badge = document.querySelector('.notification-badge');
        if (badge) {
            if (this.unreadCount > 0) {
                badge.style.display = 'block';
                if (this.unreadCount > 9) {
                    badge.classList.add('has-count');
                    badge.textContent = '9+';
                } else if (this.unreadCount > 1) {
                    badge.classList.add('has-count');
                    badge.textContent = this.unreadCount.toString();
                } else {
                    badge.classList.remove('has-count');
                }
            } else {
                badge.style.display = 'none';
            }
        }

        // Update dropdown if open
        if (this.isDropdownOpen) {
            this.renderNotifications();
        }
    }

    renderNotifications() {
        const notificationList = document.querySelector('.notification-list');
        if (!notificationList) return;

        if (this.notifications.length === 0) {
            notificationList.innerHTML = `
                <div class="notification-empty">
                    <i data-lucide="bell"></i>
                    <h3>No notifications</h3>
                    <p>You're all caught up!</p>
                </div>
            `;
        } else {
            notificationList.innerHTML = this.notifications
                .slice(0, 20) // Show only latest 20 notifications
                .map(notification => this.createNotificationHTML(notification))
                .join('');
        }

        // Initialize Lucide icons
        lucide.createIcons();

        // Add click handlers
        this.addNotificationClickHandlers();
    }

    createNotificationHTML(notification) {
        const timeAgo = this.formatTimeAgo(notification.CreatedAt);
        const unreadClass = notification.IsRead ? '' : 'unread';
        
        return `
            <div class="notification-item ${unreadClass}" data-notification-id="${notification.Id}" data-action-url="${notification.ActionUrl || ''}">
                <div class="notification-icon ${notification.Type}">
                    <i data-lucide="${notification.IconClass}"></i>
                </div>
                <div class="notification-content">
                    <div class="notification-title">${this.escapeHtml(notification.Title)}</div>
                    <div class="notification-message">${this.escapeHtml(notification.Message)}</div>
                    <div class="notification-time">${timeAgo}</div>
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
        // Create toast element
        const toast = document.createElement('div');
        toast.className = 'notification-toast';
        toast.innerHTML = `
            <div class="notification-toast-content">
                <div class="notification-toast-icon ${notification.Type}">
                    <i data-lucide="${notification.IconClass}"></i>
                </div>
                <div class="notification-toast-body">
                    <div class="notification-toast-title">${this.escapeHtml(notification.Title)}</div>
                    <div class="notification-toast-message">${this.escapeHtml(notification.Message)}</div>
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

    // API Methods
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