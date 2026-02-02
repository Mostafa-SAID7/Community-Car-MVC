/**
 * CC.Modules.Error
 * Unified error page management.
 */
(function () {
    class ErrorController extends CC.Utils.BaseComponent {
        constructor() {
            super('ErrorController');
            this.service = CC.Services.Error;
            this.errorData = null;
        }

        init() {
            this.setupEventHandlers();
            this.initializeLucide();

            const storedError = this.service.getStoredError();
            if (storedError) {
                this.errorData = storedError;
                this.displayErrorData(this.errorData);
                this.service.clearStoredError();
            } else {
                this.loadMockErrorData();
            }
        }

        setupEventHandlers() {
            this.delegate('click', '[data-action]', (e, target) => {
                const action = target.dataset.action;
                switch (action) {
                    case 'toggle-dark-mode': this.toggleDarkMode(); break;
                    case 'go-back': history.back(); break;
                    case 'toggle-details': this.toggleDetails(); break;
                    case 'report-error': this.showReportModal(); break;
                    case 'close-report-modal': this.hideReportModal(); break;
                    case 'close-success-modal': this.hideSuccessModal(); break;
                    case 'reload-page': window.location.reload(); break;
                    case 'go-home': window.location.href = '/'; break;
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

            const reportForm = document.getElementById('errorReportForm');
            if (reportForm) {
                reportForm.addEventListener('submit', (e) => this.handleReportSubmit(e));
            }
        }

        toggleDarkMode() {
            document.documentElement.classList.toggle('dark');
            const isDark = document.documentElement.classList.contains('dark');
            localStorage.setItem('theme', isDark ? 'dark' : 'light');
        }

        toggleDetails() {
            const details = document.getElementById('technicalDetails');
            const toggleText = document.getElementById('toggleText');
            if (!details || !toggleText) return;

            const isHidden = details.classList.toggle('hidden');
            toggleText.textContent = isHidden ?
                (window.errorHandlerLocalization?.showDetails || 'Show Details') :
                (window.errorHandlerLocalization?.hideDetails || 'Hide Details');
        }

        showReportModal() {
            const modal = document.getElementById('reportModal');
            if (!modal) return;
            modal.classList.remove('hidden');
            document.body.style.overflow = 'hidden';

            const form = document.getElementById('errorReportForm');
            if (form && this.errorData?.errorId) {
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
            const reportData = Object.fromEntries(formData.entries());
            reportData.errorId = form.dataset.errorId || this.errorData?.errorId || '';

            const submitBtn = form.querySelector('button[type="submit"]');
            const originalText = submitBtn?.innerHTML;

            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML = `<span class="animate-spin mr-2">⏳</span> ${window.errorHandlerLocalization?.sending || 'Sending...'}`;
            }

            try {
                const result = await this.service.submitReport(reportData);
                if (result.success) {
                    this.hideReportModal();
                    this.showSuccessModal(result.ticketId);
                    form.reset();
                } else {
                    CC.Utils.Notifier.error((window.errorHandlerLocalization?.submitFailed || 'Failed: ') + result.message);
                }
            } catch (error) {
                CC.Utils.Notifier.error(window.errorHandlerLocalization?.submitError || 'Reporting failed');
            } finally {
                if (submitBtn) {
                    submitBtn.disabled = false;
                    submitBtn.innerHTML = originalText;
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
            const errorMsg = document.getElementById('errorMessage');
            if (errorMsg && errorMsg.textContent.includes('Loading')) {
                this.displayErrorData({
                    errorId: 'ERR-' + Math.random().toString(36).substr(2, 9).toUpperCase(),
                    message: 'System connectivity interruption detected.',
                    timestamp: new Date().toISOString(),
                    path: '/system/core/navigation',
                    details: 'Resource not found in target directory.',
                    stackTrace: 'Mock stack trace...'
                });
            }
        }

        initializeLucide() {
            if (window.lucide) window.lucide.createIcons();
        }
    }

    CC.Modules.Error = new ErrorController();

    // Legacy support
    window.setErrorData = (data) => CC.Services.Error.storeErrorForPage(data);
})();
