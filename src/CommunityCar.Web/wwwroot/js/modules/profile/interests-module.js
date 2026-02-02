/**
 * Interests Module
 * Handles loading and display of user interests in the sidebar.
 */
(function (CC) {
    class InterestsController extends CC.Utils.BaseComponent {
        constructor() {
            super('InterestsController');
            this.service = CC.Services.Social; // Reusing Social service or create Profile service
        }

        init() {
            const container = document.getElementById('userInterests');
            if (container && container.dataset.userId) {
                this.loadInterests(container.dataset.userId, container);
            }
        }

        async loadInterests(userId, container) {
            try {
                // Mock or real API call
                const response = await fetch(`/api/profile/${userId}/interests`);
                if (response.ok) {
                    const interests = await response.json();
                    this.renderInterests(interests, container);
                } else {
                    // Fallback to mock data if API fails during migration
                    this.renderMockInterests(container);
                }
            } catch (error) {
                console.warn('Failed to load interests, using mock data');
                this.renderMockInterests(container);
            }
        }

        renderInterests(interests, container) {
            if (!interests || interests.length === 0) {
                container.innerHTML = '<p class="text-xs text-muted-foreground">No interests added yet.</p>';
                return;
            }

            container.innerHTML = interests.map(interest => `
                <div class="flex items-center gap-2 p-2 rounded-md hover:bg-accent transition-colors cursor-pointer">
                    <i data-lucide="tag" class="w-3.5 h-3.5 text-primary"></i>
                    <span class="text-xs font-medium">${interest.name}</span>
                </div>
            `).join('');

            if (window.lucide) window.lucide.createIcons({ parent: container });
        }

        renderMockInterests(container) {
            const mock = [
                { name: 'Classic Cars' },
                { name: 'Drifting' },
                { name: 'Electric Vehicles' }
            ];
            this.renderInterests(mock, container);
        }
    }

    CC.Modules.Interests = new InterestsController();
})(window.CommunityCar);
