/**
 * Dashboard Overview Controller
 * Handles general dashboard interactions, monitoring, and auto-refresh.
 * Merges functionality from: dashboard-overview.js, dashboard-monitoring.js, ai-dashboard.js
 */
(function (CC) {
    class OverviewController extends CC.Utils.BaseComponent {
        constructor() {
            super('DashboardOverview');
            this.refreshInterval = null;
        }

        init() {
            if (this.initialized) return;
            super.init();

            this.setupEventHandlers();
            this.startAutoRefresh();

            // Re-init icons if needed (though layout controller handles some)
            if (typeof lucide !== 'undefined') {
                lucide.createIcons();
            }
        }

        setupEventHandlers() {
            document.body.addEventListener('click', (event) => {
                const target = event.target.closest('[data-action]');
                if (!target) return;

                const action = target.dataset.action;

                switch (action) {
                    case 'refresh-activity':
                        this.refreshActivity(target);
                        break;
                    case 'refresh-data':
                        this.refreshData(target);
                        break;
                    case 'reload-page':
                        this.reloadPage(target);
                        break;
                }
            });
        }

        refreshActivity(button) {
            if (button) {
                const icon = button.querySelector('i') || button.querySelector('svg');
                if (icon) {
                    icon.classList.add('fa-spin', 'animate-spin');
                    setTimeout(() => icon.classList.remove('fa-spin', 'animate-spin'), 1000);
                }
            }

            if (CC.Services.Toaster) CC.Services.Toaster.success('Activity feed updated');
            else console.log('Activity refreshed');

            // TODO: Fetch logic
        }

        refreshData(button) {
            if (button) {
                const icon = button.querySelector('svg') || button.querySelector('i');
                if (icon) icon.classList.add('animate-spin');
            }

            if (CC.Services.Toaster) CC.Services.Toaster.info('Refreshing monitoring data...');

            // Mock Refresh
            setTimeout(() => {
                location.reload();
            }, 500);
        }

        reloadPage(button) {
            if (button) {
                button.classList.add('opacity-75', 'cursor-not-allowed');
                const icon = button.querySelector('[data-lucide]') || button.querySelector('svg');
                if (icon) icon.classList.add('animate-spin');
            }
            window.location.reload();
        }

        startAutoRefresh() {
            // Refresh every 30 seconds
            this.refreshInterval = setInterval(() => {
                // this.refreshActivity(null); // Optional auto-fetch
                console.log('Dashboard auto-refresh tick');
            }, 30000);
        }
    }

    CC.Modules = CC.Modules || {};
    CC.Modules.Dashboard = CC.Modules.Dashboard || {};
    CC.Modules.Dashboard.Overview = new OverviewController();

})(window.CommunityCar);
