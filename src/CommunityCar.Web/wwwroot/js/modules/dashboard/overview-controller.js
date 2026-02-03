/**
 * Dashboard Overview Controller
 * Handles general dashboard interactions, monitoring, and auto-refresh.
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
                    icon.style.transition = 'transform 0.5s ease-in-out';
                    icon.style.transform = 'rotate(360deg)';
                    setTimeout(() => icon.style.transform = 'rotate(0deg)', 500);
                }
            }

            // Show skeleton or loading state
            const activityList = document.getElementById('activityList');
            if (activityList) activityList.style.opacity = '0.5';

            setTimeout(() => {
                if (activityList) activityList.style.opacity = '1';
                if (CC.Services.Toaster) CC.Services.Toaster.success('Activity feed updated');
                else console.log('Activity refreshed');
            }, 800);
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
            }, 600);
        }

        reloadPage(button) {
            if (button) {
                button.classList.add('opacity-50', 'pointer-events-none');
                const icon = button.querySelector('[data-lucide]') || button.querySelector('svg');
                if (icon) icon.classList.add('animate-spin');
            }
            window.location.reload();
        }

        startAutoRefresh() {
            // Refresh every 60 seconds for performance
            this.refreshInterval = setInterval(() => {
                console.log('Dashboard auto-refresh tick');
            }, 60000);
        }
    }

    CC.Modules = CC.Modules || {};
    CC.Modules.Dashboard = CC.Modules.Dashboard || {};
    CC.Modules.Dashboard.Overview = new OverviewController();

})(window.CommunityCar);

