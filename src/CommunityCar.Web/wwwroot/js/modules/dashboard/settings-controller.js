/**
 * Dashboard Settings Controller
 * Handles Configuration, Resets, and Localization toggles.
 * Merges: dashboard-settings.js, dashboard-localization.js
 */
(function (CC) {
    class SettingsController extends CC.Utils.BaseComponent {
        constructor() {
            super('DashboardSettings');
        }

        init() {
            if (this.initialized) return;
            super.init();

            this.setupEventHandlers();
            this.setupAutoSave();
        }

        setupEventHandlers() {
            document.body.addEventListener('click', (event) => {
                const target = event.target.closest('[data-action]');
                if (!target) return;

                const action = target.dataset.action;

                switch (action) {
                    case 'confirm-reset':
                        this.confirmAction(target, 'Are you sure you want to reset this setting to default?', 'Reset Setting');
                        break;
                    case 'confirm-reset-all':
                        this.confirmAction(target, 'Are you sure you want to reset ALL settings? This cannot be undone.', 'Reset All');
                        break;
                    case 'confirm-delete-culture':
                        this.confirmAction(target, 'Archive this culture protocol?', 'Archive Culture');
                        break;
                }
            });
        }

        confirmAction(button, message, title) {
            const form = button.closest('form');
            if (window.AlertModal) {
                window.AlertModal.confirm(message, title, (confirmed) => {
                    if (confirmed) form.submit();
                });
            } else if (confirm(message)) {
                form.submit();
            }
        }

        setupAutoSave() {
            const settingInputs = document.querySelectorAll('.autosave-input'); // Changed selector to be more specific or leave generic
            // Original used 'input[name="Value"]', which might be too broad.
            const inputs = document.querySelectorAll('input[name="Value"]');

            inputs.forEach(input => {
                let timeout;
                input.addEventListener('input', function () {
                    clearTimeout(timeout);
                    timeout = setTimeout(() => {
                        console.log('Setting changed (AutoSave Pending):', this.value);
                    }, 1000);
                });
            });
        }
    }

    CC.Modules = CC.Modules || {};
    CC.Modules.Dashboard = CC.Modules.Dashboard || {};
    CC.Modules.Dashboard.Settings = new SettingsController();

})(window.CommunityCar);
