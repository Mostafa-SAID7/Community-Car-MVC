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
            chatWidget.classList.toggle('d-none');
            if (!chatWidget.classList.contains('d-none')) {
                chatInput.focus();
                scrollToBottom();
                if (typeof lucide !== 'undefined') lucide.createIcons();
            }
        });
    }

    if (closeChat) {
        closeChat.addEventListener('click', () => {
            chatWidget.classList.add('d-none');
        });
    }

    function scrollToBottom() {
        if (chatMessages) {
            chatMessages.scrollTop = chatMessages.scrollHeight;
        }
    }

    function addChatMessage(text, isAi = false) {
        if (!chatMessages) return;

        const msgDiv = document.createElement('div');
        msgDiv.className = `message ${isAi ? 'ai-message' : 'user-message'}`;
        msgDiv.innerHTML = `<div class="message-content">${text}</div>`;
        chatMessages.appendChild(msgDiv);
        scrollToBottom();
    }

    async function handleChatSend() {
        if (!chatInput) return;

        const text = chatInput.value.trim();
        if (!text) return;

        chatInput.value = '';
        addChatMessage(text, false);

        // Show typing indicator
        const typingDiv = document.createElement('div');
        typingDiv.className = 'message ai-message typing-indicator';
        typingDiv.id = 'chat-typing';
        typingDiv.innerHTML = '<div class="message-content">Thinking...</div>';
        chatMessages.appendChild(typingDiv);
        scrollToBottom();

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
                addChatMessage("Sorry, I encountered an error. Please try again.", true);
            }
        } catch (error) {
            const indicator = document.getElementById('chat-typing');
            if (indicator) indicator.remove();
            addChatMessage("Connection error. Please check your network.", true);
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
