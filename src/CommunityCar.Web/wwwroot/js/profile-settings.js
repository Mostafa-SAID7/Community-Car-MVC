/**
 * Profile Settings Manager
 * Handles all interactions on the Profile Settings page
 */
class ProfileSettingsManager {
    constructor() {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => this.init());
        } else {
            this.init();
        }
    }

    init() {
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }

        this.setupEventHandlers();
        this.showTab('profile'); // Show profile tab by default
        this.restorePreview(); // Restore preview if available
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
                    this.showActiveSessions();
                    break;
                case 'show-delete-modal':
                    this.showDeleteAccountModal();
                    break;
            }
        });

        // Image preview handler
        const profilePictureInput = document.querySelector('input[name="ProfilePicture"]');
        if (profilePictureInput) {
            profilePictureInput.addEventListener('change', (e) => this.previewImage(e.target));
        }

        // AJAX Form submission handler
        document.querySelectorAll('form[data-ajax="true"]').forEach(form => {
            form.addEventListener('submit', (e) => this.handleAjaxSubmit(e));
        });
    }

    async handleAjaxSubmit(event) {
        event.preventDefault();
        const form = event.target;
        const submitBtn = form.querySelector('button[type="submit"]');
        const originalBtnText = submitBtn.innerHTML;

        try {
            // Set loading state
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<span class="flex items-center justify-center"><svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path></svg> Updating...</span>';

            const formData = new FormData(form);
            const response = await fetch(form.action, {
                method: 'POST',
                body: formData,
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            const data = await response.json();

            if (data.success) {
                if (window.AlertModal) {
                    window.AlertModal.success(data.message || 'Settings updated successfully.');
                } else {
                    alert(data.message || 'Settings updated successfully.');
                }

                // If password form, clear inputs
                if (form.action.toLowerCase().includes('security')) {
                    form.reset();
                }
            } else {
                if (window.AlertModal) {
                    window.AlertModal.error(data.message || 'Failed to update settings.');
                } else {
                    alert(data.message || 'Failed to update settings.');
                }
            }
        } catch (error) {
            console.error('AJAX Submit Error:', error);
            if (window.AlertModal) {
                window.AlertModal.error('An unexpected error occurred. Please try again.');
            } else {
                alert('An unexpected error occurred. Please try again.');
            }
        } finally {
            submitBtn.disabled = false;
            submitBtn.innerHTML = originalBtnText;
        }
    }

    showTab(tabName) {
        // Hide all tab contents
        document.querySelectorAll('.tab-content').forEach(content => {
            content.classList.add('hidden');
        });

        // Remove active class from all tab buttons (feed-style classes)
        document.querySelectorAll('[id$="Tab"]').forEach(btn => {
            btn.classList.remove('bg-background', 'text-foreground', 'shadow-lg', 'border', 'border-border');
            btn.classList.add('text-muted-foreground/60', 'hover:text-foreground', 'hover:bg-muted/50');
        });

        // Show selected tab content
        const selectedContent = document.getElementById(`${tabName}Settings`);
        if (selectedContent) {
            selectedContent.classList.remove('hidden');
        }

        // Add active class to selected tab button (feed-style classes)
        const selectedTab = document.getElementById(`${tabName}Tab`);
        if (selectedTab) {
            selectedTab.classList.add('bg-background', 'text-foreground', 'shadow-lg', 'border', 'border-border');
            selectedTab.classList.remove('text-muted-foreground/60', 'hover:text-foreground', 'hover:bg-muted/50');
        }
    }

    previewImage(input) {
        if (input.files && input.files[0]) {
            const reader = new FileReader();
            reader.onload = function (e) {
                const preview = document.getElementById('profile-preview');
                if (preview) {
                    preview.src = e.target.result;
                    // Store preview in session for persistence after form submit
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
            if (img) {
                img.src = preview;
            }
            // Clear after restoring so it doesn't persist forever
            sessionStorage.removeItem('profilePicturePreview');
        }
    }

    showActiveSessions() {
        // TODO: Implement active sessions modal
        if (window.AlertModal) {
            window.AlertModal.info('This feature is coming soon!');
        } else {
            alert('Active Sessions feature is coming soon!');
        }
    }

    showDeleteAccountModal() {
        if (window.AlertModal) {
            window.AlertModal.confirm(
                'Are you sure you want to delete your account? This action cannot be undone and all your data will be permanently deleted.',
                () => {
                    // Handle account deletion
                    this.deleteAccount();
                }
            );
        } else {
            if (confirm('Are you sure you want to delete your account? This action cannot be undone.')) {
                this.deleteAccount();
            }
        }
    }

    async deleteAccount() {
        try {
            const response = await fetch('/Profile/DeleteAccount', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerification Token': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                }
            });

            const data = await response.json();

            if (data.success) {
                if (window.AlertModal) {
                    window.AlertModal.success('Your account has been deleted. You will be redirected to the home page.');
                }
                setTimeout(() => {
                    window.location.href = '/';
                }, 2000);
            } else {
                if (window.AlertModal) {
                    window.AlertModal.error(data.message || 'Failed to delete account. Please try again.');
                } else {
                    alert(data.message || 'Failed to delete account. Please try again.');
                }
            }
        } catch (error) {
            console.error('Delete account error:', error);
            if (window.AlertModal) {
                window.AlertModal.error('An error occurred. Please try again.');
            } else {
                alert('An error occurred. Please try again.');
            }
        }
    }
}

// Global instance
window.profileSettings = new ProfileSettingsManager();
