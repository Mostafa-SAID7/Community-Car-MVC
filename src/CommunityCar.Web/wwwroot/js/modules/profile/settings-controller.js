/**
 * Profile Settings Controller
 * Handles profile configuration, tabs, and account actions.
 */
(function (CC) {
    class SettingsController extends CC.Utils.BaseComponent {
        constructor() {
            super('ProfileSettings');
        }

        init() {
            if (this.initialized) return;
            super.init();

            if (typeof lucide !== 'undefined') lucide.createIcons();

            this.setupEventHandlers();
            this.showTab('profile');
            this.restorePreview();
        }

        setupEventHandlers() {
            document.body.addEventListener('click', (event) => {
                const target = event.target.closest('[data-action]');
                if (!target) return;

                const action = target.dataset.action;
                const tab = target.dataset.tab;

                switch (action) {
                    case 'show-tab':
                        this.showTab(tab);
                        break;
                    case 'show-sessions':
                        if (CC.Services.Toaster) CC.Services.Toaster.info('Coming soon!');
                        break;
                    case 'show-delete-modal':
                        this.confirmDelete();
                        break;
                }
            });

            // Image Preview
            const pfInput = document.querySelector('input[name="ProfilePicture"]');
            if (pfInput) pfInput.addEventListener('change', (e) => this.previewImage(e.target));

            // Ajax Forms
            document.querySelectorAll('form[data-ajax="true"]').forEach(form => {
                form.addEventListener('submit', (e) => this.handleAjaxSubmit(e));
            });
        }

        async handleAjaxSubmit(event) {
            event.preventDefault();
            const form = event.target;
            const btn = form.querySelector('button[type="submit"]');
            const originalText = btn.innerHTML;

            try {
                btn.disabled = true;
                btn.innerHTML = 'Updating...';

                const response = await fetch(form.action, {
                    method: 'POST',
                    body: new FormData(form),
                    headers: { 'X-Requested-With': 'XMLHttpRequest' }
                });
                const data = await response.json();

                if (data.success) {
                    if (CC.Services.Toaster) CC.Services.Toaster.success(data.message || 'Updated successfully');
                    if (form.action.toLowerCase().includes('security')) form.reset();
                } else {
                    if (CC.Services.Toaster) CC.Services.Toaster.error(data.message || 'Failed to update');
                }
            } catch (e) {
                console.error(e);
                if (CC.Services.Toaster) CC.Services.Toaster.error('An error occurred');
            } finally {
                btn.disabled = false;
                btn.innerHTML = originalText;
            }
        }

        showTab(tabName) {
            document.querySelectorAll('.tab-content').forEach(c => c.classList.add('hidden'));
            document.querySelectorAll('[id$="Tab"]').forEach(b => {
                b.classList.remove('bg-background', 'text-foreground', 'shadow-lg', 'border', 'border-border');
                b.classList.add('text-muted-foreground/60');
            });

            const content = document.getElementById(`${tabName}Settings`);
            if (content) content.classList.remove('hidden');

            const btn = document.getElementById(`${tabName}Tab`);
            if (btn) {
                btn.classList.add('bg-background', 'text-foreground', 'shadow-lg', 'border', 'border-border');
                btn.classList.remove('text-muted-foreground/60');
            }
        }

        previewImage(input) {
            if (input.files && input.files[0]) {
                const reader = new FileReader();
                reader.onload = (e) => {
                    const preview = document.getElementById('profile-preview');
                    if (preview) {
                        preview.src = e.target.result;
                        sessionStorage.setItem('profilePicturePreview', e.target.result);
                    }
                };
                reader.readAsDataURL(input.files[0]);
            }
        }

        restorePreview() {
            const preview = sessionStorage.getItem('profilePicturePreview');
            if (preview) {
                const img = document.getElementById('profile-preview');
                if (img) img.src = preview;
                sessionStorage.removeItem('profilePicturePreview');
            }
        }

        confirmDelete() {
            if (confirm('Delete account? This cannot be undone.')) {
                this.deleteAccount();
            }
        }

        async deleteAccount() {
            try {
                const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
                const res = await fetch('/Profile/DeleteAccount', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token
                    }
                });
                const data = await res.json();
                if (data.success) {
                    window.location.href = '/';
                } else {
                    if (CC.Services.Toaster) CC.Services.Toaster.error(data.message);
                }
            } catch (e) { console.error(e); }
        }
    }

    CC.Modules = CC.Modules || {};
    CC.Modules.Profile = CC.Modules.Profile || {};
    CC.Modules.Profile.Settings = new SettingsController();

})(window.CommunityCar);
