/**
 * Error Handler Module
 * Handles error page interactions, reporting, and modal management
 */
class ErrorHandler {
    constructor() {
        this.errorData = null;

        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => this.init());
        } else {
            this.init();
        }
    }

    init() {
        this.setupEventHandlers();
        this.initializeLucide();

        // Check for stored error data
        const storedError = localStorage.getItem('lastError');
        if (storedError) {
            try {
                this.errorData = JSON.parse(storedError);
                this.displayErrorData(this.errorData);
                // Clear after using
                localStorage.removeItem('lastError');
            } catch (e) {
                console.error('Failed to parse stored error data', e);
            }
        } else {
            // Load mock data if no stored error (for demonstration/dev)
            this.loadMockErrorData();
        }
    }

    setupEventHandlers() {
        // Global click delegation
        document.body.addEventListener('click', (event) => {
            const target = event.target.closest('[data-action]');
            if (!target) return;

            const action = target.dataset.action;

            switch (action) {
                case 'toggle-dark-mode':
                    this.toggleDarkMode();
                    break;
                case 'go-back':
                    history.back();
                    break;
                case 'toggle-details':
                    this.toggleDetails();
                    break;
                case 'report-error':
                    this.showReportModal();
                    break;
                case 'close-report-modal':
                    this.hideReportModal();
                    break;
                case 'close-success-modal':
                    this.hideSuccessModal();
                    break;
                case 'reload-page':
                    window.location.reload();
                    break;
                case 'go-home':
                    window.location.href = '/';
                    break;
            }
        });

        // Modal outside click handlers
        document.addEventListener('click', (e) => {
            if (e.target.id === 'reportModal') this.hideReportModal();
            if (e.target.id === 'successModal') this.hideSuccessModal();
        });

        // Escape key handler
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') {
                this.hideReportModal();
                this.hideSuccessModal();
            }
        });

        // Form submission
        const reportForm = document.getElementById('errorReportForm');
        if (reportForm) {
            reportForm.addEventListener('submit', (e) => this.handleReportSubmit(e));
        }
    }

    initializeLucide() {
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }
    }

    toggleDarkMode() {
        document.documentElement.classList.toggle('dark');
        // You might want to save this preference
        const isDark = document.documentElement.classList.contains('dark');
        localStorage.setItem('theme', isDark ? 'dark' : 'light');
    }

    toggleDetails() {
        const details = document.getElementById('technicalDetails');
        const toggleText = document.getElementById('toggleText');

        if (!details || !toggleText) return;

        if (details.classList.contains('hidden')) {
            details.classList.remove('hidden');
            toggleText.textContent = 'Hide Details';
        } else {
            details.classList.add('hidden');
            toggleText.textContent = 'Show Details';
        }
    }

    showReportModal() {
        const modal = document.getElementById('reportModal');
        if (!modal) return;

        modal.classList.remove('hidden');
        document.body.style.overflow = 'hidden';

        // Pre-fill error ID if available
        const form = document.getElementById('errorReportForm');
        if (form && this.errorData && this.errorData.errorId) {
            form.dataset.errorId = this.errorData.errorId;
        }
    }

    hideReportModal() {
        const modal = document.getElementById('reportModal');
        if (modal) {
            modal.classList.add('hidden');
            document.body.style.overflow = 'auto';
        }
    }

    showSuccessModal(ticketId) {
        const modal = document.getElementById('successModal');
        const ticketDisplay = document.getElementById('ticketId');

        if (modal && ticketDisplay) {
            ticketDisplay.textContent = ticketId;
            modal.classList.remove('hidden');
        }
    }

    hideSuccessModal() {
        const modal = document.getElementById('successModal');
        if (modal) {
            modal.classList.add('hidden');
            document.body.style.overflow = 'auto';
        }
    }

    async handleReportSubmit(e) {
        e.preventDefault();

        const form = e.target;
        const formData = new FormData(form);
        const reportData = {
            errorId: form.dataset.errorId || (this.errorData ? this.errorData.errorId : ''),
            userName: formData.get('userName'),
            userEmail: formData.get('userEmail'),
            priority: formData.get('priority'),
            description: formData.get('description'),
            stepsToReproduce: formData.get('stepsToReproduce'),
            expectedBehavior: formData.get('expectedBehavior'),
            actualBehavior: formData.get('actualBehavior')
        };

        const submitBtn = form.querySelector('button[type="submit"]');
        const originalBtnText = submitBtn ? submitBtn.innerHTML : 'Submit';

        if (submitBtn) {
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<span class="animate-spin mr-2">⏳</span> Sending...';
        }

        try {
            const response = await fetch('/api/error-reporting/submit', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(reportData)
            });

            const result = await response.json();

            if (result.success) {
                this.hideReportModal();
                this.showSuccessModal(result.ticketId);
                form.reset();
            } else {
                if (window.AlertModal) {
                    window.AlertModal.error('Failed to submit error report: ' + result.message);
                } else {
                    alert('Failed to submit error report: ' + result.message);
                }
            }
        } catch (error) {
            console.error('Error submitting report:', error);
            if (window.AlertModal) {
                window.AlertModal.error('Failed to submit error report. Please try again later.');
            } else {
                alert('Failed to submit error report. Please try again later.');
            }
        } finally {
            if (submitBtn) {
                submitBtn.disabled = false;
                submitBtn.innerHTML = originalBtnText;
            }
        }
    }

    displayErrorData(data) {
        if (!data) return;

        const setText = (id, text) => {
            const el = document.getElementById(id);
            if (el) el.textContent = text;
        };

        if (data.message) setText('errorMessage', data.message);
        if (data.timestamp) setText('errorTimestamp', new Date(data.timestamp).toLocaleString());
        if (data.path) setText('errorPath', data.path);
        if (data.details) setText('errorDetails', data.details);
        if (data.stackTrace) setText('stackTrace', data.stackTrace);
    }

    loadMockErrorData() {
        // Only load mock data if elements are empty or waiting for data
        const errorMsg = document.getElementById('errorMessage');
        if (errorMsg && errorMsg.textContent.includes('Loading')) {
            const mockData = {
                errorId: 'ERR-' + Math.random().toString(36).substr(2, 9).toUpperCase(),
                message: 'System connectivity interruption detected in sub-module sequence.',
                timestamp: new Date().toISOString(),
                path: '/system/core/navigation',
                details: 'The requested view could not be located in the expected directories.',
                stackTrace: 'Mock stack trace for demonstration purposes...'
            };
            this.displayErrorData(mockData);
        }
    }
}

// Global instance
window.errorHandler = new ErrorHandler();

// Legacy compatibility
window.setErrorData = function (data) {
    localStorage.setItem('lastError', JSON.stringify(data));
};
