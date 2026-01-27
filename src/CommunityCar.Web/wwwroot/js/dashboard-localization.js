class DashboardLocalization {
    constructor() {
        this.init();
    }

    init() {
        this.setupEventHandlers();
        this.initIcons();
    }

    setupEventHandlers() {
        document.body.addEventListener('click', (event) => {
            const target = event.target.closest('[data-action]');
            if (!target) return;

            const action = target.dataset.action;

            if (action === 'confirm-delete-culture') {
                this.confirmDeleteCulture(target);
            }
        });
    }

    confirmDeleteCulture(button) {
        const form = button.closest('form');
        const message = 'Archive this culture protocol?';

        if (window.AlertModal) {
            window.AlertModal.confirm(message, 'Archive Culture', function (confirmed) {
                if (confirmed) form.submit();
            });
        } else if (confirm(message)) {
            form.submit();
        }
    }

    initIcons() {
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }
    }
}

// Initialize on DOMContentLoaded
document.addEventListener('DOMContentLoaded', () => {
    window.dashboardLocalization = new DashboardLocalization();
});
