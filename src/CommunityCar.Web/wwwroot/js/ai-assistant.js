// AI Assistant Chat Logic
document.addEventListener('DOMContentLoaded', () => {
    const toggleChat = document.getElementById('toggle-chat');
    const closeChat = document.getElementById('close-chat');
    const chatWidget = document.getElementById('ai-chat-widget');
    const chatInput = document.getElementById('chat-input');
    const sendChat = document.getElementById('send-chat');
    const chatMessages = document.getElementById('chat-messages');

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

    function scrollToBottom() {
        if (chatMessages) {
            requestAnimationFrame(() => {
                chatMessages.scrollTop = chatMessages.scrollHeight;
            });
        }
    }

    function addChatMessage(text, isAi = false) {
        if (!chatMessages) return;

        const msgDiv = document.createElement('div');
        msgDiv.className = 'flex gap-3 mb-4'; // Tailwind flex container

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
            const response = await fetch('/api/ai-chat/send', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ message: text })
            });

            const data = await response.json();
            const indicator = document.getElementById('chat-typing');
            if (indicator) indicator.remove();

            if (response.ok) {
                addChatMessage(data.response, true);
            } else {
                addChatMessage(window.aiLocalizer?.ServiceError || "Sorry, I encountered an error. Please try again.", true);
            }
        } catch (error) {
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
