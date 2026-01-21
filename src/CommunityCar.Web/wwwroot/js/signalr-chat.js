// SignalR Chat Client
class ChatClient {
    constructor() {
        this.connection = null;
        this.currentConversationId = null;
        this.typingTimer = null;
        this.isTyping = false;
        this.reconnectAttempts = 0;
        this.maxReconnectAttempts = 5;
        
        this.init();
    }

    async init() {
        try {
            // Initialize SignalR connection
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl("/hubs/chat")
                .withAutomaticReconnect([0, 2000, 10000, 30000])
                .build();

            // Set up event handlers
            this.setupEventHandlers();
            
            // Start connection
            await this.start();
            
            console.log('Chat client initialized successfully');
        } catch (error) {
            console.error('Failed to initialize chat client:', error);
        }
    }

    setupEventHandlers() {
        // Connection events
        this.connection.onreconnecting(() => {
            console.log('Chat connection lost. Reconnecting...');
            this.showConnectionStatus('Reconnecting...', 'warning');
        });

        this.connection.onreconnected(() => {
            console.log('Chat connection restored');
            this.showConnectionStatus('Connected', 'success');
            this.reconnectAttempts = 0;
            
            // Rejoin current conversation if any
            if (this.currentConversationId) {
                this.joinConversation(this.currentConversationId);
            }
        });

        this.connection.onclose(() => {
            console.log('Chat connection closed');
            this.showConnectionStatus('Disconnected', 'error');
        });

        // Message events
        this.connection.on('ReceiveMessage', (message) => {
            this.handleReceiveMessage(message);
        });

        this.connection.on('MessageMarkedAsRead', (messageId) => {
            this.handleMessageMarkedAsRead(messageId);
        });

        // Typing events
        this.connection.on('UserTyping', (userId, conversationId) => {
            this.handleUserTyping(userId, conversationId);
        });

        this.connection.on('UserStoppedTyping', (userId, conversationId) => {
            this.handleUserStoppedTyping(userId, conversationId);
        });

        // Online status events
        this.connection.on('UserOnline', (userId) => {
            this.handleUserOnline(userId);
        });

        this.connection.on('UserOffline', (userId) => {
            this.handleUserOffline(userId);
        });
    }

    async start() {
        try {
            await this.connection.start();
            console.log('Chat SignalR connection started');
        } catch (error) {
            console.error('Error starting chat connection:', error);
            
            // Retry connection
            if (this.reconnectAttempts < this.maxReconnectAttempts) {
                this.reconnectAttempts++;
                setTimeout(() => this.start(), 5000);
            }
        }
    }

    async joinConversation(conversationId) {
        if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
            console.warn('Cannot join conversation: connection not ready');
            return;
        }

        try {
            await this.connection.invoke('JoinConversation', conversationId);
            this.currentConversationId = conversationId;
            console.log(`Joined conversation: ${conversationId}`);
        } catch (error) {
            console.error('Error joining conversation:', error);
        }
    }

    async leaveConversation(conversationId) {
        if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
            return;
        }

        try {
            await this.connection.invoke('LeaveConversation', conversationId);
            if (this.currentConversationId === conversationId) {
                this.currentConversationId = null;
            }
            console.log(`Left conversation: ${conversationId}`);
        } catch (error) {
            console.error('Error leaving conversation:', error);
        }
    }

    async sendMessage(conversationId, content) {
        if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
            console.warn('Cannot send message: connection not ready');
            return false;
        }

        if (!content.trim()) {
            return false;
        }

        try {
            await this.connection.invoke('SendMessage', conversationId, content.trim());
            return true;
        } catch (error) {
            console.error('Error sending message:', error);
            return false;
        }
    }

    async markMessageAsRead(messageId) {
        if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
            return;
        }

        try {
            await this.connection.invoke('MarkMessageAsRead', messageId);
        } catch (error) {
            console.error('Error marking message as read:', error);
        }
    }

    async startTyping(conversationId) {
        if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
            return;
        }

        if (!this.isTyping) {
            this.isTyping = true;
            try {
                await this.connection.invoke('StartTyping', conversationId);
            } catch (error) {
                console.error('Error sending typing indicator:', error);
            }
        }

        // Clear existing timer
        if (this.typingTimer) {
            clearTimeout(this.typingTimer);
        }

        // Set timer to stop typing after 3 seconds of inactivity
        this.typingTimer = setTimeout(() => {
            this.stopTyping(conversationId);
        }, 3000);
    }

    async stopTyping(conversationId) {
        if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
            return;
        }

        if (this.isTyping) {
            this.isTyping = false;
            try {
                await this.connection.invoke('StopTyping', conversationId);
            } catch (error) {
                console.error('Error stopping typing indicator:', error);
            }
        }

        if (this.typingTimer) {
            clearTimeout(this.typingTimer);
            this.typingTimer = null;
        }
    }

    // Event handlers
    handleReceiveMessage(message) {
        console.log('Received message:', message);
        
        // Add message to UI
        this.addMessageToUI(message);
        
        // Play notification sound if not from current user
        if (message.SenderId !== this.getCurrentUserId()) {
            this.playNotificationSound();
        }
        
        // Update conversation list
        this.updateConversationList(message);
    }

    handleMessageMarkedAsRead(messageId) {
        // Update message status in UI
        const messageElement = document.querySelector(`[data-message-id="${messageId}"]`);
        if (messageElement) {
            const statusElement = messageElement.querySelector('.message-status');
            if (statusElement) {
                statusElement.classList.remove('sent');
                statusElement.classList.add('read');
                statusElement.innerHTML = '<i data-lucide="check-check"></i>';
                lucide.createIcons();
            }
        }
    }

    handleUserTyping(userId, conversationId) {
        if (conversationId === this.currentConversationId) {
            this.showTypingIndicator(userId);
        }
    }

    handleUserStoppedTyping(userId, conversationId) {
        if (conversationId === this.currentConversationId) {
            this.hideTypingIndicator(userId);
        }
    }

    handleUserOnline(userId) {
        // Update online status in UI
        const userElements = document.querySelectorAll(`[data-user-id="${userId}"]`);
        userElements.forEach(element => {
            const onlineIndicator = element.querySelector('.online-indicator');
            if (onlineIndicator) {
                onlineIndicator.style.display = 'block';
            }
            
            const statusText = element.querySelector('.chat-header-status');
            if (statusText) {
                statusText.textContent = 'Online';
                statusText.classList.add('online');
            }
        });
    }

    handleUserOffline(userId) {
        // Update offline status in UI
        const userElements = document.querySelectorAll(`[data-user-id="${userId}"]`);
        userElements.forEach(element => {
            const onlineIndicator = element.querySelector('.online-indicator');
            if (onlineIndicator) {
                onlineIndicator.style.display = 'none';
            }
            
            const statusText = element.querySelector('.chat-header-status');
            if (statusText) {
                statusText.textContent = 'Offline';
                statusText.classList.remove('online');
            }
        });
    }

    // UI Helper methods
    addMessageToUI(message) {
        const messagesContainer = document.querySelector('.chat-messages');
        if (!messagesContainer) return;

        const isOwnMessage = message.SenderId === this.getCurrentUserId();
        const messageElement = this.createMessageElement(message, isOwnMessage);
        
        messagesContainer.appendChild(messageElement);
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
        
        // Initialize Lucide icons
        lucide.createIcons();
    }

    createMessageElement(message, isOwnMessage) {
        const messageGroup = document.createElement('div');
        messageGroup.className = `message-group ${isOwnMessage ? 'own' : 'other'}`;
        
        const messageItem = document.createElement('div');
        messageItem.className = 'message-item';
        messageItem.setAttribute('data-message-id', message.Id);
        
        const messageBubble = document.createElement('div');
        messageBubble.className = 'message-bubble';
        messageBubble.textContent = message.Content;
        
        const messageMeta = document.createElement('div');
        messageMeta.className = 'message-meta';
        
        const messageTime = document.createElement('span');
        messageTime.className = 'message-time';
        messageTime.textContent = this.formatTime(message.CreatedAt);
        
        messageMeta.appendChild(messageTime);
        
        if (isOwnMessage) {
            const messageStatus = document.createElement('span');
            messageStatus.className = 'message-status sent';
            messageStatus.innerHTML = '<i data-lucide="check"></i>';
            messageMeta.appendChild(messageStatus);
        }
        
        messageItem.appendChild(messageBubble);
        messageItem.appendChild(messageMeta);
        messageGroup.appendChild(messageItem);
        
        return messageGroup;
    }

    showTypingIndicator(userId) {
        // Remove existing typing indicator for this user
        this.hideTypingIndicator(userId);
        
        const messagesContainer = document.querySelector('.chat-messages');
        if (!messagesContainer) return;
        
        const typingIndicator = document.createElement('div');
        typingIndicator.className = 'typing-indicator';
        typingIndicator.setAttribute('data-typing-user', userId);
        
        typingIndicator.innerHTML = `
            <span>Someone is typing</span>
            <div class="typing-dots">
                <div class="typing-dot"></div>
                <div class="typing-dot"></div>
                <div class="typing-dot"></div>
            </div>
        `;
        
        messagesContainer.appendChild(typingIndicator);
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }

    hideTypingIndicator(userId) {
        const typingIndicator = document.querySelector(`[data-typing-user="${userId}"]`);
        if (typingIndicator) {
            typingIndicator.remove();
        }
    }

    updateConversationList(message) {
        const conversationItem = document.querySelector(`[data-conversation-id="${message.ConversationId}"]`);
        if (conversationItem) {
            const preview = conversationItem.querySelector('.conversation-preview');
            const time = conversationItem.querySelector('.conversation-time');
            const unreadBadge = conversationItem.querySelector('.unread-badge');
            
            if (preview) {
                preview.textContent = message.Content;
            }
            
            if (time) {
                time.textContent = this.formatTime(message.CreatedAt);
            }
            
            // Update unread count if not current conversation
            if (message.ConversationId !== this.currentConversationId && message.SenderId !== this.getCurrentUserId()) {
                if (unreadBadge) {
                    const currentCount = parseInt(unreadBadge.textContent) || 0;
                    unreadBadge.textContent = currentCount + 1;
                    unreadBadge.style.display = 'flex';
                } else {
                    const newBadge = document.createElement('span');
                    newBadge.className = 'unread-badge';
                    newBadge.textContent = '1';
                    conversationItem.appendChild(newBadge);
                }
            }
            
            // Move conversation to top
            const conversationsList = conversationItem.parentElement;
            conversationsList.insertBefore(conversationItem, conversationsList.firstChild);
        }
    }

    showConnectionStatus(message, type) {
        // Show connection status in UI
        const statusElement = document.querySelector('.chat-connection-status');
        if (statusElement) {
            statusElement.textContent = message;
            statusElement.className = `chat-connection-status ${type}`;
            statusElement.style.display = 'block';
            
            if (type === 'success') {
                setTimeout(() => {
                    statusElement.style.display = 'none';
                }, 3000);
            }
        }
    }

    playNotificationSound() {
        // Play a subtle notification sound
        try {
            const audio = new Audio('/sounds/notification.mp3');
            audio.volume = 0.3;
            audio.play().catch(() => {
                // Ignore audio play errors (user interaction required)
            });
        } catch (error) {
            // Ignore audio errors
        }
    }

    // Utility methods
    getCurrentUserId() {
        // Get current user ID from meta tag or global variable
        const userIdMeta = document.querySelector('meta[name="user-id"]');
        return userIdMeta ? userIdMeta.getAttribute('content') : null;
    }

    formatTime(dateString) {
        const date = new Date(dateString);
        const now = new Date();
        const diffInMinutes = Math.floor((now - date) / (1000 * 60));
        
        if (diffInMinutes < 1) {
            return 'Just now';
        } else if (diffInMinutes < 60) {
            return `${diffInMinutes}m ago`;
        } else if (diffInMinutes < 1440) {
            return `${Math.floor(diffInMinutes / 60)}h ago`;
        } else {
            return date.toLocaleDateString();
        }
    }
}

// Initialize chat client when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    if (typeof signalR !== 'undefined') {
        window.chatClient = new ChatClient();
    } else {
        console.warn('SignalR library not loaded');
    }
});