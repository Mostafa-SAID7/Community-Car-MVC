/**
 * AI Controller
 * Manages UI for AI Assistant, Training Dashboard, and Settings.
 */
class AIController extends CC.Utils.BaseComponent {
    constructor() {
        super('AIController');
        this.service = CC.Services.AI;
    }

    init() {
        this.bindEvents();
        this.setupSubscriptions();
        this.initializeAssistant();
    }

    bindEvents() {
        // Assistant Events
        this.delegate('submit', '#ai-chat-form', async (e, target) => {
            e.preventDefault();
            const input = target.querySelector('input[name="message"]');
            const message = input.value.trim();
            if (message) {
                input.value = '';
                await this.handleSendMessage(message);
            }
        });

        this.delegate('click', '[data-action="toggle-ai-chat"]', () => {
            const widget = document.getElementById('ai-assistant-widget');
            if (widget) widget.classList.toggle('hidden');
        });

        // Training Events
        this.delegate('click', '[data-action="start-training"]', async () => {
            await this.handleStartTraining();
        });

        this.delegate('click', '[data-action="retrain-model"]', async (e, target) => {
            const model = target.dataset.model;
            await this.handleRetrainModel(model, target);
        });

        // Settings Events
        this.delegate('submit', '#aiSettingsForm', async (e, target) => {
            e.preventDefault();
            await this.handleSaveSettings(target);
        });

        this.delegate('change', '.ai-setting-toggle', async (e, target) => {
            const setting = target.name || target.id;
            await this.handleToggleSetting(setting, target.checked, target);
        });

        this.delegate('click', '#resetAiSettingsBtn', async () => {
            await this.handleResetSettings();
        });
    }

    setupSubscriptions() {
        this.service.on('message:received', (data) => {
            this.appendMessage('assistant', data.content);
        });

        this.service.on('message:error', () => {
            this.appendMessage('system', 'Sorry, I encountered an error. Please try again.');
        });
    }

    async initializeAssistant() {
        const chatContainer = document.getElementById('ai-chat-history');
        if (chatContainer) {
            const history = await this.service.getHistory();
            if (history.success) {
                history.data.forEach(msg => this.appendMessage(msg.role, msg.content));
            }
        }
    }

    async handleSendMessage(message) {
        this.appendMessage('user', message);
        this.showTypingIndicator(true);
        try {
            await this.service.sendMessage(message);
        } finally {
            this.showTypingIndicator(false);
        }
    }

    appendMessage(role, content) {
        const historyContainer = document.getElementById('ai-chat-history');
        if (!historyContainer) return;

        const msgDiv = document.createElement('div');
        msgDiv.className = `flex ${role === 'user' ? 'justify-end' : 'justify-start'} mb-4`;

        const bubble = document.createElement('div');
        bubble.className = `max-w-[80%] p-3 rounded-2xl ${role === 'user'
                ? 'bg-primary text-primary-foreground'
                : role === 'system'
                    ? 'bg-destructive/10 text-destructive text-xs italic'
                    : 'bg-muted text-foreground'
            }`;
        bubble.textContent = content;

        msgDiv.appendChild(bubble);
        historyContainer.appendChild(msgDiv);
        historyContainer.scrollTop = historyContainer.scrollHeight;
    }

    showTypingIndicator(show) {
        const indicator = document.getElementById('ai-typing-indicator');
        if (indicator) indicator.classList.toggle('hidden', !show);
    }

    async handleStartTraining() {
        if (await this.confirm('Start a new training session for all models?')) {
            const result = await this.service.startTraining();
            if (result.success) {
                CC.Utils.Notifier.success('Training started successfully');
                setTimeout(() => location.reload(), 1500);
            }
        }
    }

    async handleRetrainModel(model, button) {
        if (await this.confirm(`Retrain the ${model} model?`)) {
            this.setLoading(button, true);
            const result = await this.service.retrainModel(model);
            if (result.success) {
                CC.Utils.Notifier.success(`${model} queued for retraining`);
                setTimeout(() => location.reload(), 1500);
            }
            this.setLoading(button, false);
        }
    }

    async handleSaveSettings(form) {
        const formData = new FormData(form);
        const settings = Object.fromEntries(formData.entries());
        // Handle checkboxes separately
        settings.sentimentAnalysis = form.querySelector('#enableSentiment')?.checked;
        settings.moderationEnabled = form.querySelector('#enableModeration')?.checked;

        const result = await this.service.saveSettings(settings);
        if (result.success) {
            CC.Utils.Notifier.success('AI Settings saved');
        }
    }

    async handleToggleSetting(name, enabled, toggle) {
        const result = await this.service.toggleSetting(name, enabled);
        if (!result.success) {
            toggle.checked = !enabled; // Revert
            CC.Utils.Notifier.error(result.message || 'Failed to update setting');
        } else {
            CC.Utils.Notifier.success(result.message);
        }
    }

    async handleResetSettings() {
        if (await this.confirm('Reset all AI settings to defaults?')) {
            const result = await this.service.resetSettings();
            if (result.success) {
                CC.Utils.Notifier.success('Settings reset');
                location.reload();
            }
        }
    }

    async confirm(message) {
        if (window.AlertModal) {
            return await new Promise(resolve => window.AlertModal.confirm(message, 'Confirm Action', resolve));
        }
        return confirm(message);
    }

    setLoading(button, isLoading) {
        if (isLoading) {
            button.disabled = true;
            button.innerHTML = '<i data-lucide="loader-2" class="w-4 h-4 animate-spin"></i>';
            if (window.lucide) window.lucide.createIcons({ parent: button });
        } else {
            button.disabled = false;
        }
    }
}

CC.Modules.AI = new AIController();
