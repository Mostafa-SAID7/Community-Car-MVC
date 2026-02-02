/**
 * CC.Services.Error
 * Service for error reporting and centralized error handling logic.
 */
(function () {
    class ErrorService extends CC.Services.BaseService {
        constructor() {
            super('/api/error-reporting');
        }

        /**
         * Submits an error report to the server.
         * @param {object} reportData 
         * @returns {Promise<object>}
         */
        async submitReport(reportData) {
            try {
                return await this.post('/submit', reportData);
            } catch (error) {
                console.error('Error reporting failed:', error);
                throw error;
            }
        }

        /**
         * Logs an error to the server silently.
         * @param {Error|string} error 
         * @param {object} context 
         */
        async logError(error, context = {}) {
            const data = {
                message: error.message || error,
                stack: error.stack,
                url: window.location.href,
                timestamp: new Date().toISOString(),
                ...context
            };

            try {
                // Background log - don't await/block
                navigator.sendBeacon(`${this.baseUrl}/log`, JSON.stringify(data));
            } catch (e) {
                // Fail silently for background logging
            }
        }

        /**
         * Stores error data in local storage for the error page.
         */
        storeErrorForPage(data) {
            localStorage.setItem('lastError', JSON.stringify(data));
        }

        /**
         * Retrieves stored error data.
         */
        getStoredError() {
            const stored = localStorage.getItem('lastError');
            if (stored) {
                try {
                    return JSON.parse(stored);
                } catch (e) {
                    return null;
                }
            }
            return null;
        }

        /**
         * Clears stored error data.
         */
        clearStoredError() {
            localStorage.removeItem('lastError');
        }
    }

    CC.Services.Error = new ErrorService();
})();
