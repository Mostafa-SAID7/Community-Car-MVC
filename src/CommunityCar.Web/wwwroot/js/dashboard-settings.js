class DashboardSettings {
    constructor() {
        this.init();
    }

    init() {
        this.setupEventHandlers();
        this.setupAutoSave();
    }

    setupEventHandlers() {
        document.body.addEventListener('click', (event) => {
            const target = event.target.closest('[data-action]');
            if (!target) return;

            const action = target.dataset.action;

            if (action === 'confirm-reset') {
                this.confirmReset(target);
            } else if (action === 'confirm-reset-all') {
                this.confirmResetAll(target);
            }
        });
    }

    confirmReset(button) {
        const form = button.closest('form');
        const message = 'Are you sure you want to reset this setting to default?';

        if (window.AlertModal) {
            window.AlertModal.confirm(message, 'Reset Setting', function (confirmed) {
                if (confirmed) form.submit();
            });
        } else if (confirm(message)) {
            form.submit();
        }
    }

    confirmResetAll(button) {
        const form = button.closest('form');
        const message = 'Are you sure you want to reset ALL settings to default? This action cannot be undone.';

        if (window.AlertModal) {
            window.AlertModal.confirm(message, 'Reset All Settings', function (confirmed) {
                if (confirmed) form.submit();
            });
        } else if (confirm(message)) {
            form.submit();
        }
    }

    setupAutoSave() {
        const settingInputs = document.querySelectorAll('input[name="Value"]');
        settingInputs.forEach(input => {
            let timeout;
            input.addEventListener('input', function () {
                clearTimeout(timeout);
                timeout = setTimeout(() => {
                    // Optional: Implement auto-save logic here if needed backend supports it
                    console.log('Setting changed:', this.value);
                }, 1000);
            });
        });
    }
}

// Initialize on DOMContentLoaded
document.addEventListener('DOMContentLoaded', () => {
    window.dashboardSettings = new DashboardSettings();
});
