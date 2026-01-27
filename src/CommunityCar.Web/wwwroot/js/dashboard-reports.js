class DashboardReports {
    constructor() {
        this.init();
    }

    init() {
        this.setupEventHandlers();
        this.startStatusCheck();
    }

    setupEventHandlers() {
        document.body.addEventListener('click', (event) => {
            const target = event.target.closest('[data-action]');
            if (!target) return;

            const action = target.dataset.action;

            if (action === 'confirm-delete-report') {
                this.confirmDeleteReport(target);
            } else if (action === 'set-date-range') {
                this.setDateRange(target.dataset.days);
            }
        });
    }

    confirmDeleteReport(button) {
        const form = button.closest('form');
        const message = 'Are you sure you want to delete this report?';

        if (window.AlertModal) {
            window.AlertModal.confirm(message, 'Delete Report', function (confirmed) {
                if (confirmed) form.submit();
            });
        } else if (confirm(message)) {
            form.submit();
        }
    }

    startStatusCheck() {
        // Auto-refresh reports status every 30 seconds
        this.interval = setInterval(() => {
            console.log('Checking report status...');
            // In a real app, you would possibly poll an endpoint here
            // fetch('/Dashboard/Reports/Status')...
        }, 30000);
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
}

// Initialize on DOMContentLoaded
document.addEventListener('DOMContentLoaded', () => {
    window.dashboardReports = new DashboardReports();

    // Set default date range if on generate page
    if (document.querySelector('form[action*="Generate"]')) {
        window.dashboardReports.setDateRange(30);

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
});
