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

            if (target.dataset.action === 'send-friend-request') {
                this.sendFriendRequest(target.dataset.userId);
            }
        });
    }

    async loadUserInterests(userId) {
        try {
            const response = await fetch(`/api/analytics/interests/${userId}?limit=5`);
            const result = await response.json();

            if (result.success && result.data.length > 0) {
                this.updateUserInterestsDisplay(result.data);
            } else {
                this.showEmptyInterests();
            }
        } catch (error) {
            console.error('Error loading user interests:', error);
            this.showEmptyInterests();
        }
    }

    updateUserInterestsDisplay(interests) {
        const container = document.getElementById('userInterests');
        if (!container) return;

        container.innerHTML = '';

        interests.forEach(interest => {
            const interestEl = document.createElement('div');
            interestEl.className = 'flex items-center justify-between p-2 rounded-md bg-accent/30 hover:bg-accent/50 transition-colors';
            interestEl.innerHTML = `
                <div class="flex items-center gap-2">
                    <div class="w-2 h-2 rounded-full bg-primary"></div>
                    <span class="text-xs font-medium text-foreground">${interest.displayName}</span>
                </div>
                <div class="text-xs text-muted-foreground">${Math.round(interest.score)}</div>
            `;
            container.appendChild(interestEl);
        });

        // Add view all link
        const viewAllEl = document.createElement('a');
        viewAllEl.href = '/profile/interests';
        viewAllEl.className = 'block text-sm text-primary hover:text-primary/80 transition-colors mt-3 font-medium';
        // Note: Localization should ideally be handled by passing translated strings or using a localization library
        viewAllEl.textContent = 'View All Interests';
        container.appendChild(viewAllEl);
    }

    showEmptyInterests() {
        const container = document.getElementById('userInterests');
        if (!container) return;

        container.innerHTML = `
            <div class="flex flex-col items-center justify-center py-4 text-center">
                <i data-lucide="heart" class="w-6 h-6 text-muted-foreground mb-2"></i>
                <p class="text-xs text-muted-foreground">Start exploring to build your interests</p>
            </div>
        `;

        if (typeof lucide !== 'undefined') lucide.createIcons();
    }

    sendFriendRequest(userId) {
        // Mock implementation
        console.log('Sending friend request to:', userId);
        if (window.AlertModal) {
            window.AlertModal.success('Friend request sent!');
        } else {
            alert('Friend request sent!');
        }
    }
}

// Global instance
window.sidebarRight = new SidebarRightManager();
