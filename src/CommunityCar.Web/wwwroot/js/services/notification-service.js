/**
 * Notification Service
 * Manages SignalR connection and real-time event dispatching.
 */
(function (CC) {
    class NotificationService extends CC.Utils.BaseService {
        constructor() {
            super('Notification');
            this.connection = null;
            this.reconnectAttempts = 0;
            this.maxReconnectAttempts = 5;
            this.isConnected = false;
        }

        async init() {
            if (this.initialized) return;
            // Dependencies check: ensure SignalR is loaded
            if (typeof signalR === 'undefined') {
                console.warn('SignalR not loaded. NotificationService disabled.');
                return;
            }

            this.initialized = true;

            try {
                this.connection = new signalR.HubConnectionBuilder()
                    .withUrl("/hubs/notifications")
                    .withAutomaticReconnect([0, 2000, 10000, 30000])
                    .build();

                this.setupEventHandlers();
                await this.start();

            } catch (error) {
                console.error('Failed to initialize Notification Service:', error);
            }
        }

        setupEventHandlers() {
            this.connection.onreconnecting(() => console.log('Reconnecting notifications...'));
            this.connection.onreconnected(() => {
                console.log('Notifications reconnected');
                this.reconnectAttempts = 0;
                this.isConnected = true;
            });
            this.connection.onclose(() => {
                console.log('Notifications disconnected');
                this.isConnected = false;
            });

            // Dispatch Hub Events to EventBus
            this.connection.on('ReceiveNotification', (notification) => {
                CC.Events.publish('notification:received', notification);
            });

            this.connection.on('NotificationMarkedAsRead', (id) => {
                CC.Events.publish('notification:read', id);
            });

            this.connection.on('AllNotificationsMarkedAsRead', () => {
                CC.Events.publish('notification:all_read');
            });
        }

        async start() {
            try {
                await this.connection.start();
                this.isConnected = true;
                console.log('✅ Notification Service Connected');
            } catch (error) {
                console.error('Notification connection error:', error);
                this.isConnected = false;
                if (this.reconnectAttempts < this.maxReconnectAttempts) {
                    this.reconnectAttempts++;
                    setTimeout(() => this.start(), 5000);
                }
            }
        }

        // API Methods
        async loadNotifications(page = 1, pageSize = 20) {
            try {
                const response = await fetch(`/shared/Notifications?page=${page}&pageSize=${pageSize}`, {
                    headers: { 'Accept': 'application/json' }
                });
                return await response.json();
            } catch (error) {
                console.error('Error loading notifications:', error);
                return { success: false, data: [] };
            }
        }

        async markAsRead(id) {
            if (!this.isConnected) return;
            try {
                await this.connection.invoke('MarkNotificationAsRead', id);
            } catch (error) {
                console.error('Error marking as read:', error);
            }
        }

        async markAllAsRead() {
            if (!this.isConnected) return;
            try {
                await this.connection.invoke('MarkAllNotificationsAsRead');
            } catch (error) {
                console.error('Error marking all read:', error);
            }
        }
    }

    CC.Services.Notification = new NotificationService();
    // Auto-init handled by script defer or manual call?
    // BaseService doesn't auto-init.
    // Let's hook into DOMContentLoaded via core or global init.
    // Or just call it immediately if scripts are deferred.
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => CC.Services.Notification.init());
    } else {
        CC.Services.Notification.init();
    }

})(window.CommunityCar);
