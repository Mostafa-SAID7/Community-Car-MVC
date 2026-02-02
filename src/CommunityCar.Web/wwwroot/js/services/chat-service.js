/**
 * Chat Service
 * Handles SignalR connection and API interactions for chat functionality.
 */
class ChatService {
    constructor() {
        this.connection = null;
        this.reconnectAttempts = 0;
        this.maxReconnectAttempts = 5;
        this.currentConversationId = null;

        // Event listeners storage
        this.listeners = {
            'reconnecting': [],
            'reconnected': [],
            'close': [],
            'receiveMessage': [],
            'messageMarkedAsRead': [],
            'userTyping': [],
            'userStoppedTyping': [],
            'userOnline': [],
            'userOffline': []
        };
    }

    /**
     * Initialize the service
     */
    async init() {
        if (typeof signalR === 'undefined') {
            console.warn('SignalR library not loaded');
            return;
        }

        try {
            // Initialize SignalR connection
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl("/hubs/chat")
                .withAutomaticReconnect([0, 2000, 10000, 30000])
                .build();

            // Set up internal event handlers
            this._setupHubHandlers();

            // Start connection
            await this.start();
            console.log('Chat Service initialized');
        } catch (error) {
            console.error('Failed to initialize Chat Service:', error);
        }
    }

    /**
     * Start the SignalR connection
     */
    async start() {
        if (!this.connection) return;

        try {
            await this.connection.start();
            console.log('Chat SignalR connection started');
        } catch (error) {
            console.error('Error starting chat connection:', error);
            if (this.reconnectAttempts < this.maxReconnectAttempts) {
                this.reconnectAttempts++;
                setTimeout(() => this.start(), 5000);
            }
        }
    }

    /**
     * Setup internal SignalR hub handlers and dispatch to registered listeners
     */
    _setupHubHandlers() {
        if (!this.connection) return;

        this.connection.onreconnecting(() => this._emit('reconnecting'));
        this.connection.onreconnected(() => {
            this.reconnectAttempts = 0;
            if (this.currentConversationId) {
                this.joinConversation(this.currentConversationId);
            }
            this._emit('reconnected');
        });
        this.connection.onclose(() => this._emit('close'));

        this.connection.on('ReceiveMessage', (message) => this._emit('receiveMessage', message));
        this.connection.on('MessageMarkedAsRead', (messageId) => this._emit('messageMarkedAsRead', messageId));
        this.connection.on('UserTyping', (userId, conversationId) => this._emit('userTyping', { userId, conversationId }));
        this.connection.on('UserStoppedTyping', (userId, conversationId) => this._emit('userStoppedTyping', { userId, conversationId }));
        this.connection.on('UserOnline', (userId) => this._emit('userOnline', userId));
        this.connection.on('UserOffline', (userId) => this._emit('userOffline', userId));
    }

    /**
     * Register an event listener
     * @param {string} event - Event name
     * @param {Function} callback - Callback function
     */
    on(event, callback) {
        if (this.listeners[event]) {
            this.listeners[event].push(callback);
        }
    }

    /**
     * Emit an event to listeners
     * @param {string} event 
     * @param {any} data 
     */
    _emit(event, data) {
        if (this.listeners[event]) {
            this.listeners[event].forEach(cb => cb(data));
        }
    }

    /**
     * Join a conversation
     * @param {string} conversationId 
     */
    async joinConversation(conversationId) {
        if (!this._isConnected()) return;

        try {
            await this.connection.invoke('JoinConversation', conversationId);
            this.currentConversationId = conversationId;
            console.log(`Joined conversation: ${conversationId}`);
        } catch (error) {
            console.error('Error joining conversation:', error);
        }
    }

    /**
     * Leave a conversation
     * @param {string} conversationId 
     */
    async leaveConversation(conversationId) {
        if (!this._isConnected()) return;

        try {
            await this.connection.invoke('LeaveConversation', conversationId);
            if (this.currentConversationId === conversationId) {
                this.currentConversationId = null;
            }
        } catch (error) {
            console.error('Error leaving conversation:', error);
        }
    }

    /**
     * Send a message
     * @param {string} conversationId 
     * @param {string} content 
     */
    async sendMessage(conversationId, content) {
        if (!content.trim()) return false;

        // Try SignalR first
        if (this._isConnected()) {
            try {
                await this.connection.invoke('SendMessage', conversationId, content.trim());
                return true;
            } catch (error) {
                console.error('SignalR send failed, falling back to API', error);
            }
        }

        // Fallback to API
        try {
            const response = await fetch('/chats/messages', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ ConversationId: conversationId, Content: content.trim() })
            });
            return response.ok;
        } catch (error) {
            console.error('API send failed:', error);
            return false;
        }
    }

    /**
     * Mark message as read
     * @param {string} messageId 
     */
    async markMessageAsRead(messageId) {
        if (!this._isConnected()) return;
        try {
            await this.connection.invoke('MarkMessageAsRead', messageId);
        } catch (error) {
            console.error('Error marking message as read:', error);
        }
    }

    /**
     * Send typing indicator
     * @param {string} conversationId 
     */
    async startTyping(conversationId) {
        if (!this._isConnected()) return;
        try {
            await this.connection.invoke('StartTyping', conversationId);
        } catch (error) {
            console.error('Error sending typing indicator:', error);
        }
    }

    /**
     * Stop typing indicator
     * @param {string} conversationId 
     */
    async stopTyping(conversationId) {
        if (!this._isConnected()) return;
        try {
            await this.connection.invoke('StopTyping', conversationId);
        } catch (error) {
            console.error('Error stopping typing indicator:', error);
        }
    }

    /**
     * Mark conversation as read via API
     * @param {string} conversationId 
     */
    async markConversationRead(conversationId) {
        try {
            await fetch(`/chats/conversations/${conversationId}/read`, { method: 'POST' });
        } catch (error) {
            console.error('Error marking conversation read:', error);
        }
    }

    /**
     * Load all conversations via API
     */
    async getConversations() {
        const response = await fetch('/chats/conversations');
        if (!response.ok) throw new Error('Failed to load conversations');
        return await response.json();
    }

    /**
     * Load conversation details
     * @param {string} conversationId 
     */
    async getConversationDetails(conversationId) {
        const response = await fetch(`/chats/conversations/${conversationId}`);
        if (!response.ok) throw new Error('Failed to load conversation details');
        return await response.json();
    }

    /**
     * Load messages for a conversation
     * @param {string} conversationId 
     */
    async getMessages(conversationId) {
        const response = await fetch(`/chats/conversations/${conversationId}/messages`);
        if (!response.ok) throw new Error('Failed to load messages');
        return await response.json();
    }

    _isConnected() {
        return this.connection && this.connection.state === signalR.HubConnectionState.Connected;
    }
}

// Register to Global Namespace
window.CommunityCar = window.CommunityCar || {};
window.CommunityCar.Services = window.CommunityCar.Services || {};
window.CommunityCar.Services.Chat = new ChatService();
