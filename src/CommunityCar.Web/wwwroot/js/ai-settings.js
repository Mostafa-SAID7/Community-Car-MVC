/**
 * AI Settings Management
 * Handles configuration saving, resetting, and toggling.
 */
class AISettingsManager {
    constructor() {
        this.init();
    }

    init() {
        document.addEventListener('DOMContentLoaded', () => {
            if (typeof lucide !== 'undefined') lucide.createIcons();
            this.setupForm();
            this.setupToggles();
            this.setupReset();
        });
    }

    setupForm() {
        const form = document.getElementById('aiSettingsForm');
        if (!form) return;

        form.addEventListener('submit', async (e) => {
            e.preventDefault();
            const saveBtn = document.getElementById('saveBtn');
            const formData = new FormData(form);

            const settings = {
                defaultProvider: formData.get('defaultProvider'),
                maxResponseLength: parseInt(formData.get('maxResponseLength')),
                responseTimeout: parseInt(formData.get('responseTimeout')),
                sentimentAnalysis: document.getElementById('enableSentiment').checked,
                moderationEnabled: document.getElementById('enableModeration').checked
            };

            try {
                this.showLoading(true);
                saveBtn.disabled = true;

                const response = await fetch('/Dashboard/AIManagement/Settings/Save', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                    },
                    body: JSON.stringify(settings)
                });

                const result = await response.json();
                if (result.success) {
                    this.notify('Settings saved', 'success');
                } else {
                    this.notify(result.message || 'Save failed', 'error');
                }
            } catch (error) {
                console.error('Error saving settings:', error);
                this.notify('Operation failed', 'error');
            } finally {
                this.showLoading(false);
                saveBtn.disabled = false;
            }
        });
    }

    setupToggles() {
        const toggles = [
            document.getElementById('enableSentiment'),
            document.getElementById('enableModeration')
        ].filter(t => t !== null);

        toggles.forEach(toggle => {
            toggle.addEventListener('change', async function () {
                const settingName = this.name === 'sentimentAnalysis' ? 'SentimentAnalysis' : 'ModerationEnabled';

                try {
                    const response = await fetch('/Dashboard/AIManagement/Settings/Toggle', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                        },
                        body: JSON.stringify({
                            settingName: settingName,
                            enabled: this.checked
                        })
                    });

                    const result = await response.json();
                    if (!result.success) {
                        this.checked = !this.checked;
                        window.aiSettings.notify(result.message || 'Toggle failed', 'error');
                    } else {
                        window.aiSettings.notify(result.message, 'success');
                    }
                } catch (error) {
                    console.error('Error toggling setting:', error);
                    this.checked = !this.checked;
                    window.aiSettings.notify('Toggle failed', 'error');
                }
            });
        });
    }

    setupReset() {
        const resetBtn = document.getElementById('resetBtn');
        if (!resetBtn) return;

        resetBtn.addEventListener('click', async () => {
            if (window.AlertModal) {
                window.AlertModal.confirm(
                    'Are you sure you want to reset all settings to defaults? This action cannot be undone.',
                    'Reset Configuration',
                    async (confirmed) => {
                        if (!confirmed) return;
                        await this.performReset();
                    }
                );
            } else if (confirm('Reset to defaults?')) {
                await this.performReset();
            }
        });
    }

    async performReset() {
        const resetBtn = document.getElementById('resetBtn');
        try {
            this.showLoading(true);
            resetBtn.disabled = true;

            const response = await fetch('/Dashboard/AIManagement/Settings/Reset', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                }
            });

            const result = await response.json();
            if (result.success && result.settings) {
                document.getElementById('defaultProvider').value = result.settings.defaultProvider;
                document.getElementById('maxResponseLength').value = result.settings.maxResponseLength;
                document.getElementById('responseTimeout').value = result.settings.responseTimeout;
                document.getElementById('enableSentiment').checked = result.settings.sentimentAnalysis;
                document.getElementById('enableModeration').checked = result.settings.moderationEnabled;
                this.notify('Reset complete', 'success');
            } else {
                this.notify('Reset failed', 'error');
            }
        } catch (error) {
            console.error('Error resetting settings:', error);
            this.notify('Reset failed', 'error');
        } finally {
            this.showLoading(false);
            resetBtn.disabled = false;
        }
    }

    showLoading(show) {
        const overlay = document.getElementById('loadingOverlay');
        if (overlay) overlay.style.display = show ? 'flex' : 'none';
    }

    notify(message, type) {
        if (window.AlertModal) {
            if (type === 'success') window.AlertModal.success(message);
            else if (type === 'error') window.AlertModal.error(message);
            else window.AlertModal.show({ type: 'info', title: 'Settings', message: message });
        } else {
            console.log(`${type.toUpperCase()}: ${message}`);
        }
    }
}

// Global instance
window.aiSettings = new AISettingsManager();
