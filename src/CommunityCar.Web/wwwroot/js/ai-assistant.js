// AI Assistant Chat Logic with History
document.addEventListener('DOMContentLoaded', () => {
    const toggleChat = document.getElementById('toggle-chat');
    const closeChat = document.getElementById('close-chat');
    const chatWidget = document.getElementById('ai-chat-widget');
    const chatInput = document.getElementById('chat-input');
    const sendChat = document.getElementById('send-chat');
    const chatMessages = document.getElementById('chat-messages');
    const clearChatBtn = document.getElementById('clear-chat-btn');
    const chatHistoryBtn = document.getElementById('chat-history-btn');
    const historyModal = document.getElementById('chat-history-modal');
    const closeHistoryModal = document.getElementById('close-history-modal');
    const exportHistoryBtn = document.getElementById('export-history-btn');
    const clearAllHistoryBtn = document.getElementById('clear-all-history-btn');

    // Chat history storage
    let currentConversationId = null;
    let chatHistory = JSON.parse(localStorage.getItem('aiChatHistory') || '[]');
    let currentMessages = [];

    // Initialize
    loadCurrentConversation();

    if (toggleChat && chatWidget) {
        toggleChat.addEventListener('click', () => {
            chatWidget.classList.toggle('active');
            if (chatWidget.classList.contains('active')) {
                chatInput.focus();
                scrollToBottom();
                if (typeof lucide !== 'undefined') lucide.createIcons();
            }
        });
    }

    if (closeChat) {
        closeChat.addEventListener('click', () => {
            chatWidget.classList.remove('active');
        });
    }

    if (clearChatBtn) {
        clearChatBtn.addEventListener('click', () => {
            if (confirm('Are you sure you want to clear the current chat?')) {
                clearCurrentChat();
            }
        });
    }

    if (chatHistoryBtn) {
        chatHistoryBtn.addEventListener('click', () => {
            showHistoryModal();
        });
    }

    if (closeHistoryModal) {
        closeHistoryModal.addEventListener('click', () => {
            hideHistoryModal();
        });
    }

    if (exportHistoryBtn) {
        exportHistoryBtn.addEventListener('click', () => {
            exportChatHistory();
        });
    }

    if (clearAllHistoryBtn) {
        clearAllHistoryBtn.addEventListener('click', () => {
            if (confirm('Are you sure you want to clear all chat history? This action cannot be undone.')) {
                clearAllHistory();
            }
        });
    }

    // Close modal when clicking outside
    if (historyModal) {
        historyModal.addEventListener('click', (e) => {
            if (e.target === historyModal) {
                hideHistoryModal();
            }
        });
    }

    function scrollToBottom() {
        if (chatMessages) {
            requestAnimationFrame(() => {
                chatMessages.scrollTop = chatMessages.scrollHeight;
            });
        }
    }

    function generateConversationId() {
        return 'conv_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
    }

    function loadCurrentConversation() {
        // Load the most recent conversation or start a new one
        if (chatHistory.length > 0) {
            const lastConversation = chatHistory[0];
            currentConversationId = lastConversation.id;
            currentMessages = [...lastConversation.messages];

            // Display messages in chat
            if (chatMessages) {
                chatMessages.innerHTML = '';
                // Add welcome message
                addWelcomeMessage();
                // Add conversation messages
                currentMessages.forEach(msg => {
                    addChatMessage(msg.text, msg.isAi, false);
                });
            }
        } else {
            startNewConversation();
        }
    }

    function startNewConversation() {
        currentConversationId = generateConversationId();
        currentMessages = [];
        if (chatMessages) {
            chatMessages.innerHTML = '';
            addWelcomeMessage();
        }
    }

    function addWelcomeMessage() {
        if (chatMessages && chatMessages.children.length === 0) {
            const welcomeDiv = document.createElement('div');
            welcomeDiv.className = 'flex gap-3';
            welcomeDiv.innerHTML = `
                <div class="flex items-center justify-center w-8 h-8 rounded-full bg-primary text-primary-foreground shrink-0 mt-1">
                    <i data-lucide="sparkles" class="w-4 h-4"></i>
                </div>
                <div class="flex-1 bg-card border border-border rounded-lg p-3 shadow-sm">
                    <p class="text-sm text-foreground">Hello! I'm your AI assistant. How can I help you with your automotive needs today?</p>
                </div>
            `;
            chatMessages.appendChild(welcomeDiv);
            if (typeof lucide !== 'undefined') lucide.createIcons();
        }
    }

    function addChatMessage(text, isAi = false, saveToHistory = true) {
        if (!chatMessages) return;

        const msgDiv = document.createElement('div');
        msgDiv.className = 'flex gap-3 mb-4';

        let innerHTML = '';
        if (isAi) {
            innerHTML = `
                <div class="flex items-center justify-center w-8 h-8 rounded-full bg-primary text-primary-foreground shrink-0 mt-1">
                    <i data-lucide="sparkles" class="w-4 h-4"></i>
                </div>
                <div class="flex-1 bg-card border border-border rounded-lg p-3 shadow-sm">
                    <p class="text-sm text-foreground">${text}</p>
                </div>
            `;
        } else {
            const isRtl = document.documentElement.dir === 'rtl' || document.body.dir === 'rtl' || msgDiv.closest('[dir="rtl"]') !== null;
            const marginClass = isRtl ? 'mr-12' : 'ml-12';
            innerHTML = `
                <div class="flex-1 bg-primary text-primary-foreground rounded-lg p-3 shadow-sm ${marginClass}">
                     <p class="text-sm">${text}</p>
                </div>
                <div class="flex items-center justify-center w-8 h-8 rounded-full bg-muted text-muted-foreground shrink-0 mt-1">
                    <i data-lucide="user" class="w-4 h-4"></i>
                </div>
            `;
        }

        msgDiv.innerHTML = innerHTML;
        chatMessages.appendChild(msgDiv);
        scrollToBottom();
        if (typeof lucide !== 'undefined') lucide.createIcons();

        // Save to current messages and history
        if (saveToHistory) {
            const message = {
                id: Date.now(),
                text: text,
                isAi: isAi,
                timestamp: new Date().toISOString()
            };
            currentMessages.push(message);
            saveCurrentConversation();
        }
    }

    function saveCurrentConversation() {
        if (!currentConversationId || currentMessages.length === 0) return;

        // Find existing conversation or create new one
        let conversationIndex = chatHistory.findIndex(conv => conv.id === currentConversationId);

        const conversation = {
            id: currentConversationId,
            title: generateConversationTitle(),
            messages: [...currentMessages],
            createdAt: conversationIndex === -1 ? new Date().toISOString() : chatHistory[conversationIndex].createdAt,
            updatedAt: new Date().toISOString()
        };

        if (conversationIndex === -1) {
            // Add new conversation at the beginning
            chatHistory.unshift(conversation);
        } else {
            // Update existing conversation
            chatHistory[conversationIndex] = conversation;
            // Move to front if it's not already there
            if (conversationIndex !== 0) {
                chatHistory.splice(conversationIndex, 1);
                chatHistory.unshift(conversation);
            }
        }

        // Keep only last 50 conversations
        if (chatHistory.length > 50) {
            chatHistory = chatHistory.slice(0, 50);
        }

        localStorage.setItem('aiChatHistory', JSON.stringify(chatHistory));
    }

    function generateConversationTitle() {
        if (currentMessages.length === 0) return 'New Conversation';

        // Use the first user message as title (truncated)
        const firstUserMessage = currentMessages.find(msg => !msg.isAi);
        if (firstUserMessage) {
            return firstUserMessage.text.length > 50
                ? firstUserMessage.text.substring(0, 50) + '...'
                : firstUserMessage.text;
        }

        return 'New Conversation';
    }

    function clearCurrentChat() {
        if (chatMessages) {
            chatMessages.innerHTML = '';
            addWelcomeMessage();
        }
        startNewConversation();
    }

    function showHistoryModal() {
        if (historyModal) {
            updateHistoryDisplay();
            historyModal.classList.remove('opacity-0', 'pointer-events-none');
            historyModal.classList.add('opacity-100');
        }
    }

    function hideHistoryModal() {
        if (historyModal) {
            historyModal.classList.add('opacity-0', 'pointer-events-none');
            historyModal.classList.remove('opacity-100');
        }
    }

    function updateHistoryDisplay() {
        const historyContainer = document.getElementById('history-conversations');
        if (!historyContainer) return;

        if (chatHistory.length === 0) {
            historyContainer.innerHTML = `
                <div class="text-center py-8 text-muted-foreground">
                    <i data-lucide="message-circle" class="w-12 h-12 mx-auto mb-4 opacity-50"></i>
                    <p>No chat history yet</p>
                    <p class="text-sm">Start a conversation to see your history here</p>
                </div>
            `;
            if (typeof lucide !== 'undefined') lucide.createIcons();
            return;
        }

        historyContainer.innerHTML = '';
        chatHistory.forEach((conversation, index) => {
            const conversationDiv = document.createElement('div');
            conversationDiv.className = 'p-4 rounded-lg border border-border hover:bg-muted/50 transition-colors cursor-pointer conversation-item';
            conversationDiv.setAttribute('data-conversation-title', conversation.title.toLowerCase());

            const messageCount = conversation.messages.length;
            const lastMessage = conversation.messages[conversation.messages.length - 1];
            const updatedAt = new Date(conversation.updatedAt);

            conversationDiv.innerHTML = `
                <div class="flex items-start justify-between">
                    <div class="flex-1">
                        <h4 class="font-medium text-foreground mb-1">${conversation.title}</h4>
                        <p class="text-sm text-muted-foreground mb-2 line-clamp-2">
                            ${lastMessage ? (lastMessage.isAi ? 'AI: ' : 'You: ') + lastMessage.text : 'No messages'}
                        </p>
                        <div class="flex items-center gap-4 text-xs text-muted-foreground">
                            <span>${messageCount} messages</span>
                            <span>${formatRelativeTime(updatedAt)}</span>
                        </div>
                    </div>
                    <div class="flex items-center gap-2 ml-4">
                        <button class="btn btn-outline btn-sm load-conversation-btn" data-conversation-id="${conversation.id}">
                            <i data-lucide="message-circle" class="w-4 h-4 mr-1"></i>
                            Load
                        </button>
                        <button class="btn btn-outline btn-sm delete-conversation-btn" data-conversation-id="${conversation.id}">
                            <i data-lucide="trash-2" class="w-4 h-4"></i>
                        </button>
                    </div>
                </div>
            `;

            historyContainer.appendChild(conversationDiv);
        });

        // Add event listeners for conversation actions
        document.querySelectorAll('.load-conversation-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                e.stopPropagation();
                const conversationId = btn.getAttribute('data-conversation-id');
                loadConversation(conversationId);
                hideHistoryModal();
            });
        });

        document.querySelectorAll('.delete-conversation-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                e.stopPropagation();
                const conversationId = btn.getAttribute('data-conversation-id');
                if (confirm('Are you sure you want to delete this conversation?')) {
                    deleteConversation(conversationId);
                    updateHistoryDisplay();
                }
            });
        });

        // Add search functionality
        const searchInput = document.getElementById('history-search');
        if (searchInput) {
            searchInput.addEventListener('input', function () {
                const searchTerm = this.value.toLowerCase();
                const conversationItems = document.querySelectorAll('.conversation-item');

                conversationItems.forEach(item => {
                    const title = item.getAttribute('data-conversation-title');
                    if (title.includes(searchTerm)) {
                        item.style.display = '';
                    } else {
                        item.style.display = 'none';
                    }
                });
            });
        }

        if (typeof lucide !== 'undefined') lucide.createIcons();
    }

    function loadConversation(conversationId) {
        const conversation = chatHistory.find(conv => conv.id === conversationId);
        if (!conversation) return;

        currentConversationId = conversationId;
        currentMessages = [...conversation.messages];

        // Clear and reload chat
        if (chatMessages) {
            chatMessages.innerHTML = '';
            addWelcomeMessage();
            currentMessages.forEach(msg => {
                addChatMessage(msg.text, msg.isAi, false);
            });
        }
    }

    function deleteConversation(conversationId) {
        chatHistory = chatHistory.filter(conv => conv.id !== conversationId);
        localStorage.setItem('aiChatHistory', JSON.stringify(chatHistory));

        // If we deleted the current conversation, start a new one
        if (currentConversationId === conversationId) {
            startNewConversation();
        }
    }

    function exportChatHistory() {
        if (chatHistory.length === 0) {
            alert('No chat history to export');
            return;
        }

        const exportData = {
            exportDate: new Date().toISOString(),
            totalConversations: chatHistory.length,
            conversations: chatHistory
        };

        const blob = new Blob([JSON.stringify(exportData, null, 2)], { type: 'application/json' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `ai-chat-history-${new Date().toISOString().split('T')[0]}.json`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    }

    function clearAllHistory() {
        chatHistory = [];
        localStorage.removeItem('aiChatHistory');
        startNewConversation();
        hideHistoryModal();
    }

    function formatRelativeTime(date) {
        const now = new Date();
        const diffMs = now - date;
        const diffMins = Math.floor(diffMs / 60000);
        const diffHours = Math.floor(diffMs / 3600000);
        const diffDays = Math.floor(diffMs / 86400000);

        if (diffMins < 1) return 'Just now';
        if (diffMins < 60) return `${diffMins}m ago`;
        if (diffHours < 24) return `${diffHours}h ago`;
        if (diffDays < 7) return `${diffDays}d ago`;
        return date.toLocaleDateString();
    }

    async function handleChatSend() {
        if (!chatInput) return;

        const text = chatInput.value.trim();
        if (!text) return;

        chatInput.value = '';
        addChatMessage(text, false);

        // Show typing indicator
        const typingDiv = document.createElement('div');
        typingDiv.className = 'flex gap-3 mb-4 typing-indicator';
        typingDiv.id = 'chat-typing';
        typingDiv.innerHTML = `
            <div class="flex items-center justify-center w-8 h-8 rounded-full bg-primary text-primary-foreground shrink-0 mt-1">
                <i data-lucide="sparkles" class="w-4 h-4"></i>
            </div>
            <div class="flex-1 bg-card border border-border rounded-lg p-3 shadow-sm">
                <div class="flex space-x-1">
                    <div class="w-2 h-2 bg-foreground/50 rounded-full animate-bounce"></div>
                    <div class="w-2 h-2 bg-foreground/50 rounded-full animate-bounce" style="animation-delay: 0.2s"></div>
                    <div class="w-2 h-2 bg-foreground/50 rounded-full animate-bounce" style="animation-delay: 0.4s"></div>
                </div>
            </div>
        `;
        chatMessages.appendChild(typingDiv);
        scrollToBottom();
        if (typeof lucide !== 'undefined') lucide.createIcons();

        try {
            console.log('Sending message to AI:', text);
            const response = await fetch('/ai-chat/send', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    message: text,
                    conversationId: currentConversationId,
                    context: ''
                })
            });

            console.log('Response status:', response.status);
            const data = await response.json();
            console.log('Response data:', data);

            const indicator = document.getElementById('chat-typing');
            if (indicator) indicator.remove();

            if (response.ok && data.success) {
                addChatMessage(data.data.response, true);
            } else {
                console.error('AI service error:', data);
                addChatMessage(data.message || data.error || window.aiLocalizer?.ServiceError || "Sorry, I encountered an error. Please try again.", true);
            }
        } catch (error) {
            console.error('Network error:', error);
            const indicator = document.getElementById('chat-typing');
            if (indicator) indicator.remove();
            addChatMessage(window.aiLocalizer?.ConnectionError || "Connection error. Please check your network.", true);
        }
    }

    if (sendChat) {
        sendChat.addEventListener('click', handleChatSend);
    }

    if (chatInput) {
        chatInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') handleChatSend();
        });
    }
});
