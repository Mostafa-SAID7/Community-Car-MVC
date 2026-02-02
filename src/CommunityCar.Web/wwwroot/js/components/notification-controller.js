/**
 * Notification Controller
 * Handles UI for notifications: Badges, Dropdowns, Toasts.
 * Listens to InteractionService events.
 */
(function (CC) {
    class NotificationController extends CC.Utils.BaseComponent {
        constructor() {
            super('NotificationController');
            this.notifications = [];
            this.unreadCount = 0;
            this.isDropdownOpen = false;
        }

        init() {
            if (this.initialized) return;
            super.init();

            this.bindUI();
            this.subscribeToEvents();
            this.loadInitialData();
        }

        bindUI() {
            // Bells
            document.querySelectorAll('.notification-bell, .mobile-notification-bell').forEach(btn => {
                btn.addEventListener('click', (e) => {
                    e.stopPropagation();
                    this.toggleDropdown();
                });
            });

            // Mark All Read
            document.querySelectorAll('.mark-all-read-btn, .mobile-mark-all-read-btn').forEach(btn => {
                btn.addEventListener('click', (e) => {
                    e.stopPropagation();
                    CC.Services.Notification.markAllAsRead();
                });
            });

            // Close Dropdown on outside click
            document.addEventListener('click', (e) => {
                if (this.isDropdownOpen && !e.target.closest('.notification-wrapper')) {
                    this.closeDropdown();
                }
            });

            // Notification Item Clicks (Delegated)
            document.addEventListener('click', (e) => {
                const item = e.target.closest('.notification-item');
                if (item && !e.target.closest('.delete-btn')) { // exclude internal actions if any
                    this.handleNotificationClick(item);
                }
            });
        }

        subscribeToEvents() {
            CC.Events.subscribe('notification:received', (notification) => this.onNotificationReceived(notification));
            CC.Events.subscribe('notification:read', (id) => this.onNotificationRead(id));
            CC.Events.subscribe('notification:all_read', () => this.onAllRead());
        }

        async loadInitialData() {
            const result = await CC.Services.Notification.loadNotifications();
            if (result.success && result.data) {
                this.notifications = result.data;
                this.updateUnreadCount();
                this.updateBadges();
            }
        }

        // --- Event Handlers ---

        onNotificationReceived(notification) {
            this.notifications.unshift(notification);
            this.updateUnreadCount();
            this.updateBadges();
            this.renderList(); // Refresh list if open
            this.showToast(notification);
            this.playAudio();
        }

        onNotificationRead(id) {
            const notif = this.notifications.find(n => (n.id || n.Id) == id);
            if (notif) {
                notif.isRead = true;
                notif.IsRead = true;
                this.updateUnreadCount();
                this.updateBadges();
                this.renderList();
            }
        }

        onAllRead() {
            this.notifications.forEach(n => { n.isRead = true; n.IsRead = true; });
            this.updateUnreadCount();
            this.updateBadges();
            this.renderList();
        }

        toggleDropdown() {
            const dropdowns = document.querySelectorAll('.notification-dropdown, .mobile-notification-dropdown');
            if (!dropdowns.length) return;

            this.isDropdownOpen = !this.isDropdownOpen;
            dropdowns.forEach(d => {
                d.classList.toggle('hidden', !this.isDropdownOpen);
            });

            if (this.isDropdownOpen) {
                this.renderList();
            }
        }

        closeDropdown() {
            this.isDropdownOpen = false;
            document.querySelectorAll('.notification-dropdown, .mobile-notification-dropdown').forEach(d => d.classList.add('hidden'));
        }

        handleNotificationClick(item) {
            const id = item.dataset.notificationId;
            const url = item.dataset.actionUrl;

            if (item.classList.contains('unread')) {
                CC.Services.Notification.markAsRead(id);
            }

            this.closeDropdown();

            if (url && url !== 'null') {
                window.location.href = url;
            }
        }

        // --- Renderers ---

        updateUnreadCount() {
            this.unreadCount = this.notifications.filter(n => !(n.isRead || n.IsRead)).length;
        }

        updateBadges() {
            document.querySelectorAll('.notification-badge, .mobile-notification-badge').forEach(badge => {
                if (this.unreadCount > 0) {
                    badge.classList.remove('hidden');
                    badge.textContent = this.unreadCount > 9 ? '9+' : this.unreadCount;
                    badge.classList.toggle('has-count', this.unreadCount > 9);
                } else {
                    badge.classList.add('hidden');
                }
            });
        }

        renderList() {
            if (!this.isDropdownOpen) return;

            const containers = document.querySelectorAll('.notification-list, .mobile-notification-list');
            const html = this.notifications.length === 0 ? this.getEmptyStateHtml() :
                this.notifications.slice(0, 20).map(n => this.getItemHtml(n)).join('');

            containers.forEach(c => {
                c.innerHTML = html;
            });

            if (typeof lucide !== 'undefined') lucide.createIcons();
        }

        getItemHtml(n) {
            const isRead = n.isRead || n.IsRead;
            const unreadClass = isRead ? '' : 'unread';
            // Adapting properties from various casings seen in logs
            const id = n.id || n.Id;
            const url = n.actionUrl || n.ActionUrl;
            const type = n.type || n.Type;
            const icon = n.iconClass || n.IconClass || 'bell';
            const title = n.title || n.Title;
            const body = n.message || n.Message;
            const timeAgo = this.formatTimeAgo(n.createdAt || n.CreatedAt);

            return `
            <div class="notification-item ${unreadClass} p-3 hover:bg-muted/50 transition-colors cursor-pointer border-b border-border last:border-0" data-notification-id="${id}" data-action-url="${url}">
                <div class="flex gap-3">
                    <div class="notification-icon w-8 h-8 rounded-full flex items-center justify-center shrink-0 ${type}">
                        <i data-lucide="${icon}" class="w-4 h-4"></i>
                    </div>
                    <div class="notification-content flex-1 min-w-0">
                        <div class="notification-title text-sm font-semibold truncate">${this.escapeHtml(title)}</div>
                        <div class="notification-message text-xs text-muted-foreground line-clamp-2">${this.escapeHtml(body)}</div>
                        <div class="notification-time text-[10px] text-muted-foreground mt-1">${timeAgo}</div>
                    </div>
                </div>
            </div>`;
        }

        getEmptyStateHtml() {
            return `
                    <div class="flex flex-col items-center justify-center py-12 px-4 text-center">
                        <div class="w-12 h-12 rounded-2xl bg-muted flex items-center justify-center mb-4">
                            <i data-lucide="bell-off" class="w-6 h-6 text-muted-foreground"></i>
                        </div>
                        <h3 class="text-sm font-semibold mb-1">No notifications</h3>
                        <p class="text-xs text-muted-foreground">You're all caught up!</p>
                    </div>`;
        }

        showToast(n) {
            // Simplified Toast Logic - can delegate to CC.Services.Toaster later if merged
            // For now reproducing local toast logic for notifications
            const toast = document.createElement('div');
            toast.className = 'notification-toast';
            // ... (HTML construction similar to original)
        }

        escapeHtml(text) {
            if (!text) return '';
            return text.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#039;");
        }

        formatTimeAgo(dateString) {
            // Basic implementation or use existing Utils
            const date = new Date(dateString);
            const now = new Date();
            const diff = Math.floor((now - date) / 60000); // minutes
            if (diff < 1) return 'Just now';
            if (diff < 60) return `${diff}m ago`;
            if (diff < 1440) return `${Math.floor(diff / 60)}h ago`;
            return date.toLocaleDateString();
        }

        playAudio() {
            const audio = new Audio('/sounds/notification.mp3');
            audio.volume = 0.2;
            audio.play().catch(() => { });
        }
    }

    CC.Components.Notification = new NotificationController();

})(window.CommunityCar);
