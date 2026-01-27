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
    }

    showTab(tabName) {
        // Hide all tab contents
        document.querySelectorAll('.tab-content').forEach(content => {
            content.classList.add('hidden');
        });

        // Remove active class from all tab buttons
        document.querySelectorAll('[id$="Tab"]').forEach(btn => {
            btn.classList.remove('bg-primary', 'text-primary-foreground');
            btn.classList.add('bg-background', 'border', 'border-border', 'text-muted-foreground', 'hover:text-foreground');
        });

        // Show selected tab content
        const selectedContent = document.getElementById(`${tabName}Settings`);
        if (selectedContent) {
            selectedContent.classList.remove('hidden');
        }

        // Add active class to selected tab button
        const selectedTab = document.getElementById(`${tabName}Tab`);
        if (selectedTab) {
            selectedTab.classList.add('bg-primary', 'text-primary-foreground');
            selectedTab.classList.remove('bg-background', 'border', 'border-border', 'text-muted-foreground', 'hover:text-foreground');
        }
    }

    previewImage(input) {
        if (input.files && input.files[0]) {
            const reader = new FileReader();
            reader.onload = function (e) {
                const preview = document.getElementById('profile-preview');
                if (preview) {
                    preview.src = e.target.result;
                }
            };
            reader.readAsDataURL(input.files[0]);
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
