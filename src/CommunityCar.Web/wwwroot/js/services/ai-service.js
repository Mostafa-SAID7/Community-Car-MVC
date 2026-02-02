/**
 * AI Service
 * Handles interactions with AI Assistant, training models, and settings.
 */
class AIService extends CC.Services.BaseService {
    constructor() {
        super('AIService');
        this.history = [];
    }

    /**
     * Send message to AI Assistant
     * @param {string} message 
     */
    async sendMessage(message) {
        try {
            const response = await this.post('/AiAgent/Assistant/Chat', { message });
            if (response.success) {
                this.history.push({ role: 'user', content: message });
                this.history.push({ role: 'assistant', content: response.data.content });
                this.emit('message:received', response.data);
            }
            return response;
        } catch (error) {
            this.handleError(error, 'message:error');
            throw error;
        }
    }

    /**
     * Get chat history
     */
    async getHistory() {
        try {
            const response = await this.get('/AiAgent/Assistant/History');
            if (response.success) {
                this.history = response.data;
                this.emit('history:loaded', this.history);
            }
            return response;
        } catch (error) {
            this.handleError(error, 'history:error');
            throw error;
        }
    }

    /**
     * Start training session
     */
    async startTraining() {
        return await this.post('/AiAgent/AIManagement/StartTraining');
    }

    /**
     * Retrain a specific model
     * @param {string} modelName 
     */
    async retrainModel(modelName) {
        return await this.post('/AiAgent/AIManagement/RetrainModel', { modelName });
    }

    /**
     * Save AI Settings
     * @param {object} settings 
     */
    async saveSettings(settings) {
        return await this.post('/AiAgent/AIManagement/Settings/Save', settings);
    }

    /**
     * Toggle specific AI setting
     * @param {string} settingName 
     * @param {boolean} enabled 
     */
    async toggleSetting(settingName, enabled) {
        return await this.post('/AiAgent/AIManagement/Settings/Toggle', { settingName, enabled });
    }

    /**
     * Reset settings to default
     */
    async resetSettings() {
        return await this.post('/AiAgent/AIManagement/Settings/Reset');
    }
}

CC.Services.AI = new AIService();
