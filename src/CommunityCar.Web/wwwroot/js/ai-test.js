/**
 * AI Test Console Manager
 * Handles testing interactions.
 */
class AITestManager {
    constructor() {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => this.init());
        } else {
            this.init();
        }
    }

    init() {
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }

        this.setupCharacterCount();
        this.setupEventHandlers();
    }

    setupCharacterCount() {
        const testMessage = document.getElementById('testMessage');
        const charCount = document.getElementById('charCount');

        if (testMessage && charCount) {
            testMessage.addEventListener('input', function () {
                charCount.textContent = `${this.value.length} characters`;
            });
        }
    }

    setupEventHandlers() {
        document.body.addEventListener('click', (event) => {
            const target = event.target.closest('[data-action]');
            if (!target) return;

            const action = target.dataset.action;

            switch (action) {
                case 'send-test':
                    this.sendTestMessage();
                    break;
            }
        });
    }

    async sendTestMessage() {
        const testMessageInput = document.getElementById('testMessage');
        const message = testMessageInput.value.trim();

        if (window.AlertModal && !message) {
            window.AlertModal.error('Please enter a test message');
            return;
        } else if (!message) {
            alert('Please enter a test message');
            return;
        }

        const sendButton = document.querySelector('[data-action="send-test"]');
        const testResults = document.getElementById('testResults');
        const aiResponse = document.getElementById('aiResponse');

        // Update UI
        const originalContent = sendButton.innerHTML;
        sendButton.disabled = true;
        sendButton.innerHTML = '<i data-lucide="loader-2" class="w-4 h-4 mr-2 animate-spin"></i>Testing...';
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }

        try {
            const response = await fetch('/Dashboard/AIManagement/TestMessage', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                },
                body: JSON.stringify({
                    message: message
                })
            });

            const data = await response.json();

            if (data.success) {
                aiResponse.textContent = data.data.response;
                testResults.classList.remove('hidden');
            } else {
                aiResponse.textContent = data.message || 'Test failed';
                testResults.classList.remove('hidden');
            }
        } catch (error) {
            console.error('Test error:', error);
            aiResponse.textContent = 'Connection error. Please check your network.';
            testResults.classList.remove('hidden');
        } finally {
            // Reset button
            sendButton.disabled = false;
            sendButton.innerHTML = originalContent;
            if (typeof lucide !== 'undefined') {
                lucide.createIcons();
            }
        }
    }
}

// Global instance
window.aiTest = new AITestManager();
