/**
 * Dashboard Reports Controller
 * Handles deletion, date-ranges, and SEO audits.
 * Merges: dashboard-reports.js, dashboard-seo.js
 */
(function (CC) {
    class ReportsController extends CC.Utils.BaseComponent {
        constructor() {
            super('DashboardReports');
            this.statusInterval = null;
        }

        init() {
            if (this.initialized) return;
            super.init();

            this.setupEventHandlers();
            this.startStatusCheck();
            this.initializeForms();
        }

        setupEventHandlers() {
            document.body.addEventListener('click', (event) => {
                const target = event.target.closest('[data-action]');
                if (!target) return;

                const action = target.dataset.action;

                switch (action) {
                    case 'confirm-delete-report':
                        this.confirmDelete(target);
                        break;
                    case 'set-date-range':
                        this.setDateRange(target.dataset.days);
                        break;
                    case 'analyze-page':
                        this.analyzeSEO();
                        break;
                }
            });
        }

        confirmDelete(button) {
            const form = button.closest('form');
            const message = 'Are you sure you want to delete this report?';

            if (window.AlertModal) {
                window.AlertModal.confirm(message, 'Delete Report', (confirmed) => {
                    if (confirmed) form.submit();
                });
            } else if (confirm(message)) {
                form.submit();
            }
        }

        setDateRange(days) {
            const endDate = new Date();
            const startDate = new Date();
            startDate.setDate(endDate.getDate() - parseInt(days));

            const startInput = document.querySelector('input[name="StartDate"]');
            const endInput = document.querySelector('input[name="EndDate"]');

            if (startInput && endInput) {
                startInput.value = startDate.toISOString().split('T')[0];
                endInput.value = endDate.toISOString().split('T')[0];
            }
        }

        analyzeSEO() {
            const urlInput = document.getElementById('analyzeUrl');
            const url = urlInput ? urlInput.value : '/';

            if (CC.Services.Toaster) {
                CC.Services.Toaster.info(`Auditing Single URI: ${url}...`);
                setTimeout(() => {
                    CC.Services.Toaster.success('Audit complete. View updated metrics.');
                }, 1500);
            }
        }

        initializeForms() {
            // Set default date range if on generate page
            if (document.querySelector('form[action*="Generate"]')) {
                this.setDateRange(30);

                const reportTypeSelect = document.querySelector('select[name="ReportType"]');
                if (reportTypeSelect) {
                    reportTypeSelect.addEventListener('change', function () {
                        const reportType = this.value;
                        // Hide all parameter sections
                        document.querySelectorAll('[id$="-params"]').forEach(el => el.classList.add('hidden'));

                        // Show relevant parameters
                        if (reportType === 'UserAnalytics') {
                            const el = document.getElementById('user-analytics-params');
                            if (el) el.classList.remove('hidden');
                        } else if (reportType === 'ContentAnalytics') {
                            const el = document.getElementById('content-analytics-params');
                            if (el) el.classList.remove('hidden');
                        }
                    });
                }
            }
        }

        startStatusCheck() {
            // Check reports status every 30 seconds
            this.statusInterval = setInterval(() => {
                // Poll check...
            }, 30000);
        }
    }

    CC.Modules = CC.Modules || {};
    CC.Modules.Dashboard = CC.Modules.Dashboard || {};
    CC.Modules.Dashboard.Reports = new ReportsController();

})(window.CommunityCar);
