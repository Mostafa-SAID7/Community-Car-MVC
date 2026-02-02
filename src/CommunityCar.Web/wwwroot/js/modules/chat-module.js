/**
 * Chat Module Controller
 * Handles UI interactions for the chat feature.
 */
class ChatController extends CC.Utils.BaseComponent {
    constructor() {
        super('ChatModule');
        this.service = CC.Services.Chat;
        this.typingTimer = null;
        this.isTyping = false;

        // UI Elements Cache
        this.elements = {};
    }

    init() {
        if (super.init()) return;

        // Verify we are on a page with chat elements
        if (!document.getElementById('chat-messages') && !document.querySelectorAll('[data-conversation-id]').length) {
            return;
        }

        this.initializeService();
        this.cacheElements();
        this.attachEventListeners();
        this.loadConversations();
    }

    initializeService() {
        this.service.init();

        // Subscribe to service events
        this.service.on('receiveMessage', (msg) => this.handleReceiveMessage(msg));
        this.service.on('messageMarkedAsRead', (id) => this.handleMessageMarkedAsRead(id));
        this.service.on('userTyping', (data) => this.handleUserTyping(data));
        this.service.on('userStoppedTyping', (data) => this.handleUserStoppedTyping(data));
        this.service.on('userOnline', (uid) => this.handleUserStatus(uid, true));
        this.service.on('userOffline', (uid) => this.handleUserStatus(uid, false));
        this.service.on('reconnecting', () => this.showConnectionStatus('Reconnecting...', 'warning'));
        this.service.on('reconnected', () => this.showConnectionStatus('Connected', 'success'));
        this.service.on('close', () => this.showConnectionStatus('Disconnected', 'error'));
    }

    cacheElements() {
        this.elements = {
            chatEmptyState: document.getElementById('chat-empty-state'),
            chatHeader: document.getElementById('chat-header'),
            chatMessages: document.getElementById('chat-messages'),
            chatInputArea: document.getElementById('chat-input-area'),
            messageInput: document.getElementById('message-input'),
            sendBtn: document.getElementById('send-message-btn'),
            newChatBtn: document.getElementById('new-chat-btn'),
            searchInput: document.getElementById('chat-search'),
            conversationsList: document.getElementById('conversations-list'),
            chatTitle: document.getElementById('chat-title'),
            chatAvatar: document.getElementById('chat-avatar'),
            chatStatus: document.getElementById('chat-status'),
            chatOnlineIndicator: document.getElementById('chat-online-indicator')
        };
    }

    attachEventListeners() {
        const { messageInput, sendBtn, newChatBtn, searchInput } = this.elements;

        if (messageInput) {
            messageInput.addEventListener('input', () => this.handleInput());
            messageInput.addEventListener('keydown', (e) => {
                if (e.key === 'Enter' && !e.shiftKey) {
                    e.preventDefault();
                    this.sendMessage();
                }
            });
        }

        if (sendBtn) {
            sendBtn.addEventListener('click', () => this.sendMessage());
        }

        if (newChatBtn) {
            newChatBtn.addEventListener('click', () => console.log('New conversation clicked')); // TODO: Implement Modal
        }

        if (searchInput) {
            searchInput.addEventListener('input', (e) => this.filterConversations(e.target.value));
        }

        // Global delegate for conversation clicks
        if (this.elements.conversationsList) {
            this.elements.conversationsList.addEventListener('click', (e) => {
                const item = e.target.closest('[data-conversation-id]');
                if (item) {
                    this.selectConversation(item.dataset.conversationId);
                }
            });
        }
    }

    async loadConversations() {
        try {
            const conversations = await this.service.getConversations();
            this.renderConversations(conversations);
        } catch (error) {
            console.error(error);
        }
    }

    renderConversations(conversations) {
        if (!this.elements.conversationsList) return;

        const html = conversations.map(conv => this.createConversationHTML(conv)).join('');
        this.elements.conversationsList.innerHTML = html;

        // Re-init lucide if available
        if (typeof lucide !== 'undefined') lucide.createIcons();
    }

    createConversationHTML(conv) {
        const isOnline = conv.Participants?.some(p => p.IsOnline);
        const avatar = this.getConversationAvatar(conv);
        const timeAgo = this.formatTime(conv.LastActivity);
        const lastMsg = conv.LastMessage?.Content || 'No messages yet';
        const unreadDisplay = conv.UnreadCount > 0
            ? `<div class="absolute right-4 top-1/2 -translate-y-1/2 w-5 h-5 bg-primary text-primary-foreground text-[10px] font-black rounded-lg flex items-center justify-center shadow-lg shadow-primary/20">${conv.UnreadCount}</div>`
            : '';

        return `
            <div class="flex items-center gap-4 p-4 cursor-pointer hover:bg-primary/5 transition-all border-l-4 border-l-transparent relative group/conv" data-conversation-id="${conv.Id}">
                <div class="relative shrink-0">
                    <div class="w-12 h-12 rounded-full bg-gradient-to-br from-primary to-primary/80 flex items-center justify-center text-primary-foreground font-black text-xs shadow-lg group-hover/conv:scale-105 transition-transform">
                        ${avatar}
                    </div>
                    ${isOnline ? '<div class="absolute bottom-0 right-0 w-3.5 h-3.5 bg-green-500 border-2 border-background rounded-full ring-1 ring-border shadow-sm"></div>' : ''}
                </div>
                <div class="flex-1 min-w-0">
                    <div class="flex justify-between items-center mb-0.5">
                        <span class="font-black text-sm text-foreground tracking-tight">${conv.Title}</span>
                        <span class="text-[9px] font-black uppercase text-muted-foreground opacity-60">${timeAgo}</span>
                    </div>
                    <div class="text-[11px] font-medium text-muted-foreground truncate opacity-80 group-hover/conv:opacity-100 transition-opacity conversation-preview">
                        ${lastMsg}
                    </div>
                </div>
                ${unreadDisplay}
            </div>
        `;
    }

    async selectConversation(conversationId) {
        try {
            // UI Update
            document.querySelectorAll('[data-conversation-id]').forEach(i => i.classList.remove('bg-primary/10', 'border-primary', 'active'));
            const activeItem = document.querySelector(`[data-conversation-id="${conversationId}"]`);
            if (activeItem) activeItem.classList.add('bg-primary/10', 'border-primary', 'active');

            // Show Chat Interface
            this.elements.chatEmptyState.classList.add('hidden');
            this.elements.chatHeader.classList.remove('hidden', 'hidden');
            this.elements.chatHeader.classList.add('flex');
            this.elements.chatMessages.classList.remove('hidden');
            this.elements.chatMessages.classList.add('flex');
            this.elements.chatInputArea.classList.remove('hidden');
            this.elements.chatInputArea.classList.add('block');

            // Join SignalR Group
            await this.service.joinConversation(conversationId);

            // Load Data
            const [conversation, messages] = await Promise.all([
                this.service.getConversationDetails(conversationId),
                this.service.getMessages(conversationId)
            ]);

            this.updateHeader(conversation);
            this.renderMessages(messages);

            // Mark Read
            await this.service.markConversationRead(conversationId);

            // Update unread badge in list locally
            if (activeItem) {
                const badge = activeItem.querySelector('.absolute.right-4'); // simplistic selector
                if (badge) badge.remove();
            }

        } catch (error) {
            console.error('Failed to select conversation', error);
        }
    }

    updateHeader(conversation) {
        if (this.elements.chatTitle) this.elements.chatTitle.textContent = conversation.Title;
        if (this.elements.chatAvatar) this.elements.chatAvatar.textContent = this.getConversationAvatar(conversation);

        const isOnline = conversation.Participants?.some(p => p.IsOnline);
        if (this.elements.chatStatus) {
            this.elements.chatStatus.textContent = isOnline ? (window.chatLocalizer?.Online || 'Online') : (window.chatLocalizer?.Offline || 'Offline');
        }
        if (this.elements.chatOnlineIndicator) {
            if (isOnline) {
                this.elements.chatOnlineIndicator.classList.remove('hidden');
                this.elements.chatOnlineIndicator.classList.add('block');
            } else {
                this.elements.chatOnlineIndicator.classList.add('hidden');
            }
        }
    }

    renderMessages(messages) {
        const container = this.elements.chatMessages;
        if (!container) return;

        container.innerHTML = '';

        if (messages.length === 0) {
            container.innerHTML = this.getEmptyMessagesHTML();
            if (typeof lucide !== 'undefined') lucide.createIcons();
            return;
        }

        const grouped = this.groupMessagesByDate(messages);
        Object.entries(grouped).forEach(([date, msgs]) => {
            container.insertAdjacentHTML('beforeend', `
                <div class="flex justify-center my-6">
                    <span class="px-4 py-1.5 bg-muted/40 text-[10px] font-black uppercase tracking-widest text-muted-foreground rounded-full border border-border">${date}</span>
                </div>
            `);
            msgs.forEach(msg => {
                container.appendChild(this.createMessageElement(msg, msg.IsOwnMessage));
            });
        });

        if (typeof lucide !== 'undefined') lucide.createIcons();
        container.scrollTop = container.scrollHeight;
    }

    createMessageElement(message, isOwn) {
        const group = document.createElement('div');
        group.className = `message-group ${isOwn ? 'own' : 'other'}`;
        group.setAttribute('data-message-id', message.Id);

        const time = this.formatTime(message.CreatedAt);
        const statusHtml = isOwn ? `<span class="message-status sent"><i data-lucide="check"></i></span>` : '';

        group.innerHTML = `
            <div class="message-item">
                <div class="message-bubble">${message.Content}</div>
                <div class="message-meta">
                    <span class="message-time">${time}</span>
                    ${statusHtml}
                </div>
            </div>
        `;
        return group;
    }

    async sendMessage() {
        const input = this.elements.messageInput;
        const btn = this.elements.sendBtn;
        const content = input.value;
        const conversationId = this.service.currentConversationId;

        if (!content.trim() || !conversationId) return;

        input.value = '';
        btn.disabled = true;
        input.style.height = 'auto';

        await this.service.stopTyping(conversationId);

        const success = await this.service.sendMessage(conversationId, content);
        if (!success) {
            input.value = content; // restore on failure
            btn.disabled = false;
        }
    }

    handleReceiveMessage(message) {
        if (!this.elements.chatMessages) return;

        // If current conversation, append message
        if (message.ConversationId === this.service.currentConversationId) {
            const isOwn = message.SenderId === this.getCurrentUserId();
            const elem = this.createMessageElement(message, isOwn);
            this.elements.chatMessages.appendChild(elem);
            this.elements.chatMessages.scrollTop = this.elements.chatMessages.scrollHeight;
            if (typeof lucide !== 'undefined') lucide.createIcons();

            // Mark as read immediately if valid
            if (!isOwn) {
                this.service.markMessageAsRead(message.Id);
            }
        }

        // Update conversation list preview/unread count
        this.updateConversationList(message);

        // Sound
        if (message.SenderId !== this.getCurrentUserId()) {
            new Audio('/sounds/notification.mp3').play().catch(() => { });
        }
    }

    handleInput() {
        const input = this.elements.messageInput;
        const btn = this.elements.sendBtn;
        const hasContent = input.value.trim().length > 0;

        btn.disabled = !hasContent;
        input.style.height = 'auto';
        input.style.height = Math.min(input.scrollHeight, 160) + 'px';

        if (hasContent && this.service.currentConversationId) {
            this.triggerTyping();
        }
    }

    triggerTyping() {
        // Debounce typing start
        if (!this.isTyping) {
            this.isTyping = true;
            this.service.startTyping(this.service.currentConversationId);
        }

        if (this.typingTimer) clearTimeout(this.typingTimer);
        this.typingTimer = setTimeout(() => {
            this.isTyping = false;
            this.service.stopTyping(this.service.currentConversationId);
        }, 3000);
    }

    handleUserTyping({ userId, conversationId }) {
        if (conversationId !== this.service.currentConversationId) return;
        // Show typing indicator logic (simplified)
        const container = this.elements.chatMessages;
        if (!container.querySelector(`[data-typing-user="${userId}"]`)) {
            const indicator = document.createElement('div');
            indicator.className = 'typing-indicator flex items-center gap-3 p-4 animate-in fade-in duration-300';
            indicator.setAttribute('data-typing-user', userId);
            indicator.innerHTML = `
                <div class="w-8 h-8 rounded-full bg-muted border border-border flex items-center justify-center text-xs font-bold text-muted-foreground shrink-0"><i data-lucide="user" class="w-4 h-4"></i></div>
                <div class="flex flex-col gap-1">
                    <span class="text-xs font-medium text-muted-foreground">${window.chatLocalizer?.Typing || 'Typing...'}</span>
                    <div class="flex gap-1"><div class="w-2 h-2 bg-primary rounded-full animate-bounce delay-0"></div><div class="w-2 h-2 bg-primary rounded-full animate-bounce [animation-delay:150ms]"></div><div class="w-2 h-2 bg-primary rounded-full animate-bounce [animation-delay:300ms]"></div></div>
                </div>`;
            container.appendChild(indicator);
            container.scrollTop = container.scrollHeight;
            if (typeof lucide !== 'undefined') lucide.createIcons();
        }
    }

    handleUserStoppedTyping({ userId, conversationId }) {
        if (conversationId !== this.service.currentConversationId) return;
        const indicator = this.elements.chatMessages.querySelector(`[data-typing-user="${userId}"]`);
        if (indicator) indicator.remove();
    }

    // Helpers
    getCurrentUserId() {
        return document.querySelector('meta[name="user-id"]')?.getAttribute('content');
    }

    getConversationAvatar(conversation) {
        if (conversation.IsGroupChat) {
            return conversation.Title.substring(0, 2).toUpperCase();
        }
        const otherParticipant = conversation.Participants?.find(p => !p.IsCurrentUser);
        return otherParticipant?.Name.substring(0, 2).toUpperCase() || 'U';
    }

    groupMessagesByDate(messages) {
        const groups = {};
        const today = new Date();
        const yesterday = new Date(today);
        yesterday.setDate(yesterday.getDate() - 1);

        messages.forEach(message => {
            const date = new Date(message.CreatedAt);
            let key;
            if (date.toDateString() === today.toDateString()) key = 'Today';
            else if (date.toDateString() === yesterday.toDateString()) key = 'Yesterday';
            else key = date.toLocaleDateString();

            if (!groups[key]) groups[key] = [];
            groups[key].push(message);
        });
        return groups;
    }

    // ... Additional helpers from original chat.js (formatTime, updateConversationList etc)
    formatTime(dateString) { return CC.Services.Chat.formatTime ? CC.Services.Chat.formatTime(dateString) : (new Date(dateString).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })); }

    updateConversationList(message) {
        // Logic to move conversation to top and update preview
        const convId = message.ConversationId;
        const item = document.querySelector(`[data-conversation-id="${convId}"]`);
        if (item) {
            const preview = item.querySelector('.conversation-preview');
            if (preview) preview.textContent = message.Content;
            // re-insert at top
            item.parentElement.prepend(item);

            // increment unread if not active
            if (convId !== this.service.currentConversationId && message.SenderId !== this.getCurrentUserId()) {
                // Add unread badge logic
                // ...
            }
        }
    }

    getEmptyMessagesHTML() {
        return `
            <div class="flex-1 flex items-center justify-center">
                <div class="text-center">
                    <div class="w-16 h-16 bg-muted rounded-full flex items-center justify-center mb-4 mx-auto">
                        <i data-lucide="message-circle" class="w-8 h-8 text-muted-foreground opacity-30"></i>
                    </div>
                    <p class="text-sm text-muted-foreground">No messages yet. Start the conversation!</p>
                </div>
            </div>`;
    }

    showConnectionStatus(msg, type) {
        // Implementation of notification toaster or custom element
        if (window.showToast) {
            window.showToast(msg, type);
        } else {
            console.log(`[Chat Status] ${type}: ${msg}`);
        }
    }
}

// Initialize
CommunityCar.Modules.Chat = new ChatController();
document.addEventListener('DOMContentLoaded', () => {
    CommunityCar.Modules.Chat.init();
});
