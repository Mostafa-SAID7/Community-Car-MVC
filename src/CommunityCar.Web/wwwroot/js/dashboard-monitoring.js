class DashboardMonitoring {
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

            if (action === 'refresh-data') {
                this.refreshData(target);
            }
        });
    }

    refreshData(button) {
        // Rotate icon if button passed
        if (button) {
            const icon = button.querySelector('svg');
            if (icon) icon.classList.add('animate-spin');
        }

        if (window.showToast) window.showToast('Refreshing monitoring data...', 'info');

        // Reload page to refresh data (simple implementation)
        // Ideally this would be an AJAX call
        setTimeout(() => {
            location.reload();
        }, 500);
    }

    startAutoRefresh() {
        // Auto-refresh every 30 seconds
        this.interval = setInterval(() => {
            console.log('Auto-refreshing monitoring data...');
            // Could implement AJAX refresh here
        }, 30000);
    }
}

// Initialize on DOMContentLoaded
document.addEventListener('DOMContentLoaded', () => {
    window.dashboardMonitoring = new DashboardMonitoring();
});
