class DashboardOverview {
    constructor() {
        this.init();
    }

    init() {
        this.setupEventHandlers();
        this.startAutoRefresh();
    }

    setupEventHandlers() {
        document.body.addEventListener('click', (event) => {
            const target = event.target.closest('[data-action]');
            if (!target) return;

            const action = target.dataset.action;

            if (action === 'refresh-activity') {
                this.refreshActivity(target);
            }
        });
    }

    refreshActivity(button) {
        // Rotate icon if button passed
        if (button) {
            const icon = button.querySelector('i');
            if (icon) icon.classList.add('fa-spin');

            // Remove spin after delay
            setTimeout(() => {
                if (icon) icon.classList.remove('fa-spin');
            }, 1000);
        }

        console.log('Activity refreshed');

        // Use existing toast if available
        if (window.showToast) window.showToast('Activity feed updated', 'success');

        // Here you would typically fetch partial view or JSON
        // fetch('/Dashboard/RecentActivity')...
    }

    startAutoRefresh() {
        // Refresh every 30 seconds
        this.refreshInterval = setInterval(() => {
            this.refreshActivity();
        }, 30000);
    }
}

// Initialize on DOMContentLoaded
document.addEventListener('DOMContentLoaded', () => {
    window.dashboardOverview = new DashboardOverview();
});
