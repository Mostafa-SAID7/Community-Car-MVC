/**
 * AI Dashboard Management
 * Handles general dashboard interactions and analytics.
 */
class AIDashboardManager {
    constructor() {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => this.init());
        } else {
            this.init();
        }
    }

    init() {
        // Initialize icons
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }

        this.setupEventHandlers();
    }

    setupEventHandlers() {
        document.body.addEventListener('click', (event) => {
            const target = event.target.closest('[data-action]');
            if (!target) return;

            const action = target.dataset.action;

            switch (action) {
                case 'reload-page':
                    this.reloadPage(target);
                    break;
            }
        });
    }

    reloadPage(button) {
        if (button) {
            button.classList.add('opacity-75', 'cursor-not-allowed');
            const icon = button.querySelector('[data-lucide]');
            if (icon) icon.classList.add('animate-spin');
        }

        window.location.reload();
    }
}

// Global instance
window.aiDashboard = new AIDashboardManager();
