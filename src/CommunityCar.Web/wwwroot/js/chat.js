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
            this.showConnectionStatus(window.chatLocalizer?.Reconnecting || 'Reconnecting...', 'warning');
        });

        this.connection.onreconnected(() => {
            console.log('Chat connection restored');
            this.showConnectionStatus(window.chatLocalizer?.Connected || 'Connected', 'success');
            this.reconnectAttempts = 0;

            // Rejoin current conversation if any
            if (this.currentConversationId) {
                this.joinConversation(this.currentConversationId);
            }
        });

        this.connection.onclose(() => {
            console.log('Chat connection closed');
            this.showConnectionStatus(window.chatLocalizer?.Disconnected || 'Disconnected', 'error');
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
                if (typeof lucide !== 'undefined') {
                    lucide.createIcons();
                }
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
                onlineIndicator.classList.remove('hidden');
            }

            const statusText = element.querySelector('.chat-header-status');
            if (statusText) {
                statusText.textContent = window.chatLocalizer?.Online || 'Online';
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
                onlineIndicator.classList.add('hidden');
            }

            const statusText = element.querySelector('.chat-header-status');
            if (statusText) {
                statusText.textContent = window.chatLocalizer?.Offline || 'Offline';
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
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }
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
        typingIndicator.className = 'typing-indicator flex items-center gap-3 p-4 animate-in fade-in duration-300';
        typingIndicator.setAttribute('data-typing-user', userId);

        typingIndicator.innerHTML = `
            <div class="w-8 h-8 rounded-full bg-muted border border-border flex items-center justify-center text-xs font-bold text-muted-foreground shrink-0">
                <i data-lucide="user" class="w-4 h-4"></i>
            </div>
            <div class="flex flex-col gap-1">
                <span class="text-xs font-medium text-muted-foreground">${window.chatLocalizer?.Typing || 'Typing...'}</span>
                <div class="flex gap-1">
                    <div class="w-2 h-2 bg-primary rounded-full animate-bounce delay-0"></div>
                    <div class="w-2 h-2 bg-primary rounded-full animate-bounce [animation-delay:150ms]"></div>
                    <div class="w-2 h-2 bg-primary rounded-full animate-bounce [animation-delay:300ms]"></div>
                </div>
            </div>
        `;

        messagesContainer.appendChild(typingIndicator);
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }
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
                    unreadBadge.classList.remove('hidden');
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
        let statusElement = document.querySelector('.chat-connection-status');
        if (!statusElement) {
            // Create status element if it doesn't exist
            statusElement = document.createElement('div');
            statusElement.className = 'chat-connection-status fixed top-4 right-4 px-4 py-2 rounded-lg text-sm font-medium z-50 transition-all';
            document.body.appendChild(statusElement);
        }

        statusElement.textContent = message;
        statusElement.className = `chat-connection-status fixed top-4 right-4 px-4 py-2 rounded-lg text-sm font-medium z-50 transition-all ${type === 'success' ? 'bg-green-500 text-white' : type === 'warning' ? 'bg-yellow-500 text-white' : 'bg-red-500 text-white'}`;
        statusElement.classList.remove('hidden');

        if (type === 'success') {
            setTimeout(() => {
                statusElement.classList.add('hidden');
            }, 3000);
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
            return window.chatLocalizer?.JustNow || 'Just now';
        } else if (diffInMinutes < 60) {
            const template = window.chatLocalizer?.MinutesAgo || '{0}m ago';
            return template.replace('{0}', diffInMinutes);
        } else if (diffInMinutes < 1440) {
            const template = window.chatLocalizer?.HoursAgo || '{0}h ago';
            return template.replace('{0}', Math.floor(diffInMinutes / 60));
        } else {
            const template = window.chatLocalizer?.DaysAgo || '{0}d ago';
            const days = Math.floor(diffInMinutes / 1440);
            if (days < 7) {
                return template.replace('{0}', days);
            } else {
                return date.toLocaleDateString();
            }
        }
    }
}

// Initialize chat client when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    if (typeof signalR !== 'undefined') {
        window.chatClient = new ChatClient();

        // Initialize chat UI interactions
        initializeChatUI();
    } else {
        console.warn('SignalR library not loaded');
    }
});

// Chat UI initialization and SignalR integration
function initializeChatUI() {
    const conversationItems = document.querySelectorAll('[data-conversation-id]');
    const chatEmptyState = document.getElementById('chat-empty-state');
    const chatHeader = document.getElementById('chat-header');
    const chatMessages = document.getElementById('chat-messages');
    const chatInputArea = document.getElementById('chat-input-area');
    const messageInput = document.getElementById('message-input');
    const sendBtn = document.getElementById('send-message-btn');
    const newChatBtn = document.getElementById('new-chat-btn');
    const searchInput = document.getElementById('chat-search');

    let currentConversationId = null;

    // Load conversations from SignalR
    loadConversations();

    // Conversation selection
    conversationItems.forEach(item => {
        item.addEventListener('click', function () {
            const conversationId = this.getAttribute('data-conversation-id');
            selectConversation(conversationId);
        });
    });

    // Message input handling
    messageInput.addEventListener('input', function () {
        const hasContent = this.value.trim().length > 0;
        sendBtn.disabled = !hasContent;
        this.style.height = 'auto';
        this.style.height = Math.min(this.scrollHeight, 160) + 'px';

        // Send typing indicator
        if (currentConversationId && hasContent) {
            window.chatClient.startTyping(currentConversationId);
        }
    });

    messageInput.addEventListener('keydown', function (e) {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            sendMessage();
        }
    });

    // Send message
    sendBtn.addEventListener('click', sendMessage);

    // New conversation
    if (newChatBtn) {
        newChatBtn.addEventListener('click', () => {
            // TODO: Implement new conversation modal
            console.log('New conversation clicked');
        });
    }

    // Search functionality
    if (searchInput) {
        searchInput.addEventListener('input', function () {
            const query = this.value.toLowerCase();
            filterConversations(query);
        });
    }

    async function loadConversations() {
        try {
            const response = await fetch('/chats/conversations');
            if (response.ok) {
                const conversations = await response.json();
                renderConversations(conversations);
            }
        } catch (error) {
            console.error('Failed to load conversations:', error);
        }
    }

    function renderConversations(conversations) {
        const conversationsList = document.getElementById('conversations-list');
        if (!conversationsList || conversations.length === 0) return;

        conversationsList.innerHTML = conversations.map(conv => `
            <div class="flex items-center gap-4 p-4 cursor-pointer hover:bg-primary/5 transition-all border-l-4 border-l-transparent relative group/conv" data-conversation-id="${conv.Id}">
                <div class="relative shrink-0">
                    <div class="w-12 h-12 rounded-full bg-gradient-to-br from-primary to-primary/80 flex items-center justify-center text-primary-foreground font-black text-xs shadow-lg group-hover/conv:scale-105 transition-transform">
                        ${getConversationAvatar(conv)}
                    </div>
                    ${conv.Participants?.some(p => p.IsOnline) ? '<div class="absolute bottom-0 right-0 w-3.5 h-3.5 bg-green-500 border-2 border-background rounded-full ring-1 ring-border shadow-sm"></div>' : ''}
                </div>
                <div class="flex-1 min-w-0">
                    <div class="flex justify-between items-center mb-0.5">
                        <span class="font-black text-sm text-foreground tracking-tight">${conv.Title}</span>
                        <span class="text-[9px] font-black uppercase text-muted-foreground opacity-60">${formatTimeAgo(conv.LastActivity)}</span>
                    </div>
                    <div class="text-[11px] font-medium text-muted-foreground truncate opacity-80 group-hover/conv:opacity-100 transition-opacity">
                        ${conv.LastMessage?.Content || 'No messages yet'}
                    </div>
                </div>
                ${conv.UnreadCount > 0 ? `<div class="absolute right-4 top-1/2 -translate-y-1/2 w-5 h-5 bg-primary text-primary-foreground text-[10px] font-black rounded-lg flex items-center justify-center shadow-lg shadow-primary/20">${conv.UnreadCount}</div>` : ''}
            </div>
        `).join('');

        // Re-attach event listeners
        conversationsList.querySelectorAll('[data-conversation-id]').forEach(item => {
            item.addEventListener('click', function () {
                const conversationId = this.getAttribute('data-conversation-id');
                selectConversation(conversationId);
            });
        });
    }

    async function selectConversation(conversationId) {
        try {
            // Update UI state
            document.querySelectorAll('[data-conversation-id]').forEach(i =>
                i.classList.remove('bg-primary/10', 'border-primary', 'active'));
            document.querySelector(`[data-conversation-id="${conversationId}"]`)
                ?.classList.add('bg-primary/10', 'border-primary', 'active');

            // Show chat interface
            chatEmptyState.classList.add('hidden');
            chatHeader.classList.remove('hidden');
            chatHeader.classList.add('flex');
            chatMessages.classList.remove('hidden');
            chatMessages.classList.add('flex');
            chatInputArea.classList.remove('hidden');
            chatInputArea.classList.add('block');

            currentConversationId = conversationId;

            // Join conversation via SignalR
            await window.chatClient.joinConversation(conversationId);

            // Load conversation details and messages
            const [conversation, messages] = await Promise.all([
                fetch(`/chats/conversations/${conversationId}`).then(r => r.json()),
                fetch(`/chats/conversations/${conversationId}/messages`).then(r => r.json())
            ]);

            // Update header
            updateChatHeader(conversation);

            // Load messages
            renderMessages(messages);

            // Mark conversation as read
            await fetch(`/chats/conversations/${conversationId}/read`, { method: 'POST' });

        } catch (error) {
            console.error('Failed to select conversation:', error);
        }
    }

    function updateChatHeader(conversation) {
        const chatTitle = document.getElementById('chat-title');
        const chatAvatar = document.getElementById('chat-avatar');
        const chatStatus = document.getElementById('chat-status');
        const chatOnlineIndicator = document.getElementById('chat-online-indicator');

        if (chatTitle) chatTitle.textContent = conversation.Title;
        if (chatAvatar) chatAvatar.textContent = getConversationAvatar(conversation);

        const isOnline = conversation.Participants?.some(p => p.IsOnline);
        if (chatStatus) {
            chatStatus.textContent = isOnline ?
                (window.chatLocalizer?.Online || 'Online') :
                (window.chatLocalizer?.Offline || 'Offline');
        }
        if (chatOnlineIndicator) {
            if (isOnline) {
                chatOnlineIndicator.classList.remove('hidden');
                chatOnlineIndicator.classList.add('block');
            } else {
                chatOnlineIndicator.classList.add('hidden');
                chatOnlineIndicator.classList.remove('block');
            }
        }
    }

    function renderMessages(messages) {
        chatMessages.innerHTML = '';

        if (messages.length === 0) {
            chatMessages.innerHTML = `
                <div class="flex-1 flex items-center justify-center">
                    <div class="text-center">
                        <div class="w-16 h-16 bg-muted rounded-full flex items-center justify-center mb-4 mx-auto">
                            <i data-lucide="message-circle" class="w-8 h-8 text-muted-foreground opacity-30"></i>
                        </div>
                        <p class="text-sm text-muted-foreground">No messages yet. Start the conversation!</p>
                    </div>
                </div>
            `;
            if (typeof lucide !== 'undefined') {
                lucide.createIcons();
            }
            return;
        }

        // Group messages by date
        const groupedMessages = groupMessagesByDate(messages);

        Object.entries(groupedMessages).forEach(([date, msgs]) => {
            // Add date separator
            const dateSeparator = document.createElement('div');
            dateSeparator.className = 'flex justify-center my-6';
            dateSeparator.innerHTML = `<span class="px-4 py-1.5 bg-muted/40 text-[10px] font-black uppercase tracking-widest text-muted-foreground rounded-full border border-border">${date}</span>`;
            chatMessages.appendChild(dateSeparator);

            // Add messages
            msgs.forEach(message => {
                const messageElement = window.chatClient.createMessageElement(message, message.IsOwnMessage);
                chatMessages.appendChild(messageElement);
            });
        });

        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }
        chatMessages.scrollTop = chatMessages.scrollHeight;
    }

    async function sendMessage() {
        const content = messageInput.value.trim();
        if (!content || !currentConversationId) return;

        try {
            // Clear input immediately
            messageInput.value = '';
            sendBtn.disabled = true;
            messageInput.style.height = 'auto';

            // Stop typing indicator
            await window.chatClient.stopTyping(currentConversationId);

            // Send via SignalR (which will broadcast to all participants)
            const success = await window.chatClient.sendMessage(currentConversationId, content);

            if (!success) {
                // Fallback to SignalR if SignalR fails
                await fetch('/chats/messages', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        ConversationId: currentConversationId,
                        Content: content
                    })
                });
            }

        } catch (error) {
            console.error('Failed to send message:', error);
            // Restore message input on error
            messageInput.value = content;
            sendBtn.disabled = false;
        }
    }

    function filterConversations(query) {
        const conversations = document.querySelectorAll('[data-conversation-id]');
        conversations.forEach(conv => {
            const title = conv.querySelector('.font-black.text-sm')?.textContent.toLowerCase() || '';
            const preview = conv.querySelector('.text-\\[11px\\]')?.textContent.toLowerCase() || '';
            const matches = title.includes(query) || preview.includes(query);
            if (matches) {
                conv.classList.remove('hidden');
                conv.classList.add('flex');
            } else {
                conv.classList.add('hidden');
                conv.classList.remove('flex');
            }
        });
    }

    function getConversationAvatar(conversation) {
        if (conversation.IsGroupChat) {
            return conversation.Title.substring(0, 2).toUpperCase();
        } else {
            const otherParticipant = conversation.Participants?.find(p => !p.IsCurrentUser);
            return otherParticipant?.Name.substring(0, 2).toUpperCase() || 'U';
        }
    }

    function groupMessagesByDate(messages) {
        const groups = {};
        const today = new Date();
        const yesterday = new Date(today);
        yesterday.setDate(yesterday.getDate() - 1);

        messages.forEach(message => {
            const messageDate = new Date(message.CreatedAt);
            let dateKey;

            if (messageDate.toDateString() === today.toDateString()) {
                dateKey = 'Today';
            } else if (messageDate.toDateString() === yesterday.toDateString()) {
                dateKey = 'Yesterday';
            } else {
                dateKey = messageDate.toLocaleDateString();
            }

            if (!groups[dateKey]) {
                groups[dateKey] = [];
            }
            groups[dateKey].push(message);
        });

        return groups;
    }

    function formatTimeAgo(dateString) {
        return window.chatClient?.formatTime(dateString) || 'Unknown';
    }
}