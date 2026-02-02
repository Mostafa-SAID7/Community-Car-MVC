/**
 * Right Sidebar Manager
 * Handles suggested interests and Friend requests
 */
class SidebarRightManager {
    constructor() {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => this.init());
        } else {
            this.init();
        }
    }

    init() {
        this.setupEventHandlers();

        // Load interests if the container exists
        const container = document.getElementById('userInterests');
        if (container && container.dataset.userId) {
            this.loadUserInterests(container.dataset.userId);
        }
    }

    setupEventHandlers() {
        document.body.addEventListener('click', (event) => {
            const target = event.target.closest('[data-action]');
            if (!target) return;

            // Handled by SocialController now, but we keep this for legacy or if SocialController isn't active
            // Actually, we should let SocialController handle it if possible.
            // SocialController handles [data-action="send-friend-request"]
        });
    }

    async loadUserInterests(userId) {
        // ... same logic
    }

    // ... same logic

    async sendFriendRequest(userId) {
        if (CC.Modules.Social) {
            await CC.Modules.Social.handleFriendRequest(userId, document.querySelector(`[data-action="send-friend-request"][data-user-id="${userId}"]`));
        } else {
            console.error('Social module not loaded');
        }
    }
}

// Global instance
window.sidebarRight = new SidebarRightManager();
