/**
 * CC.Utils.ErrorBoundary
 * Enhanced Error Boundary System for CommunityCar.
 * Provides comprehensive client-side error handling, logging, and recovery.
 */
(function (CC) {
    class ErrorBoundary extends CC.Utils.BaseComponent {
        constructor() {
            super('ErrorBoundary');
            this.boundaries = new Map();
            this.errorStats = {
                totalErrors: 0,
                boundaryErrors: 0,
                networkErrors: 0,
                jsErrors: 0,
                recoveredErrors: 0
            };
            this.retryAttempts = new Map();
            this.maxRetries = 3;
            this.retryDelay = 1000;
            this.service = CC.Services.Error;
        }

        init() {
            if (this.initialized) return;
            super.init();

            // Global error handlers
            window.addEventListener('error', (event) => this.handleGlobalError(event));
            window.addEventListener('unhandledrejection', (event) => this.handleUnhandledRejection(event));

            // Network error monitoring
            this.setupNetworkMonitoring();

            // Performance monitoring
            this.setupPerformanceMonitoring();

            console.log('🛡️ Error Boundary System initialized');
        }

        // Register a new error boundary
        registerBoundary(name, element, options = {}) {
            const boundary = {
                name,
                element,
                options: {
                    fallbackContent: options.fallbackContent || this.getDefaultFallback(name),
                    onError: options.onError || (() => { }),
                    onRecover: options.onRecover || (() => { }),
                    retryable: options.retryable !== false,
                    autoRecover: options.autoRecover || false,
                    recoveryDelay: options.recoveryDelay || 5000,
                    ...options
                },
                errors: [],
                isActive: true,
                lastError: null,
                recoveryAttempts: 0,
                originalContent: element.innerHTML
            };

            this.boundaries.set(name, boundary);

            element.setAttribute('data-error-boundary', boundary.name);
            element.setAttribute('data-boundary-status', 'active');

            // Wrap element events if needed
            this.wrapElementEvents(element, boundary);
            this.setupMutationObserver(element, boundary);

            console.log(`🛡️ Registered error boundary: ${name}`);
            return boundary;
        }

        // Handle errors within a boundary
        async handleBoundaryError(boundary, error, context = {}) {
            console.error(`🚨 Error in boundary "${boundary.name}":`, error);

            this.errorStats.boundaryErrors++;
            this.errorStats.totalErrors++;

            const errorInfo = {
                id: this.generateErrorId(),
                boundary: boundary.name,
                error: {
                    message: error.message,
                    stack: error.stack,
                    type: error.constructor.name
                },
                context: {
                    timestamp: new Date().toISOString(),
                    userAgent: navigator.userAgent,
                    url: window.location.href,
                    ...context
                },
                recovered: false
            };

            boundary.errors.push(errorInfo);
            boundary.lastError = errorInfo;
            boundary.isActive = false;

            boundary.element.setAttribute('data-boundary-status', 'error');
            this.showFallback(boundary, errorInfo);

            // Log error to server via ErrorService
            this.service.logError(error, { boundary: boundary.name, ...context });

            if (boundary.options.onError) boundary.options.onError(errorInfo, boundary);

            if (boundary.options.retryable) {
                this.scheduleRecovery(boundary, errorInfo);
            }

            this.showErrorNotification(boundary, errorInfo);
            return errorInfo;
        }

        showFallback(boundary, errorInfo) {
            const fallbackHtml = typeof boundary.options.fallbackContent === 'function'
                ? boundary.options.fallbackContent(errorInfo, boundary)
                : boundary.options.fallbackContent;

            boundary.element.innerHTML = `
                <div class="error-boundary-fallback bg-red-50 border border-red-200 rounded-lg p-6 text-center">
                    <div class="flex items-center justify-center mb-4">
                        <i data-lucide="alert-triangle" class="w-8 h-8 text-red-500"></i>
                    </div>
                    <h3 class="text-lg font-semibold text-red-800 mb-2">Something went wrong</h3>
                    <p class="text-red-600 mb-4">${fallbackHtml}</p>
                    <div class="flex gap-2 justify-center">
                        ${boundary.options.retryable ? `
                            <button onclick="CC.Utils.ErrorBoundary.retryBoundary('${boundary.name}')" 
                                    class="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 transition-colors">
                                <i data-lucide="refresh-cw" class="w-4 h-4 inline mr-2"></i>
                                Try Again
                            </button>
                        ` : ''}
                        <button onclick="CC.Utils.ErrorBoundary.reportError('${errorInfo.id}')" 
                                class="px-4 py-2 bg-gray-600 text-white rounded hover:bg-gray-700 transition-colors">
                            <i data-lucide="flag" class="w-4 h-4 inline mr-2"></i>
                            Report Issue
                        </button>
                    </div>
                    <div class="mt-4 text-xs text-gray-500">
                        Error ID: ${errorInfo.id}
                    </div>
                </div>
            `;

            if (window.lucide) window.lucide.createIcons({ parent: boundary.element });
        }

        scheduleRecovery(boundary, errorInfo) {
            if (boundary.recoveryAttempts >= this.maxRetries) return;
            const delay = boundary.options.recoveryDelay * Math.pow(2, boundary.recoveryAttempts);
            setTimeout(() => this.attemptRecovery(boundary, errorInfo), delay);
        }

        async attemptRecovery(boundary, errorInfo) {
            boundary.recoveryAttempts++;
            try {
                boundary.element.innerHTML = boundary.originalContent;
                boundary.element.setAttribute('data-boundary-status', 'recovering');
                this.wrapElementEvents(boundary.element, boundary);

                await new Promise(resolve => setTimeout(resolve, 100));

                boundary.isActive = true;
                boundary.element.setAttribute('data-boundary-status', 'active');
                errorInfo.recovered = true;
                this.errorStats.recoveredErrors++;

                if (boundary.options.onRecover) boundary.options.onRecover(errorInfo, boundary);
                if (window.ToasterSystem) window.ToasterSystem.show('Component recovered', 'success');
            } catch (recoveryError) {
                if (boundary.recoveryAttempts < this.maxRetries) this.scheduleRecovery(boundary, errorInfo);
                else this.showPermanentError(boundary, errorInfo);
            }
        }

        retryBoundary(name) {
            const b = this.boundaries.get(name);
            if (b) {
                b.recoveryAttempts = 0;
                this.attemptRecovery(b, b.lastError);
            }
        }

        handleGlobalError(event) {
            this.errorStats.jsErrors++;
            this.service.logError(event.error || event.message, { type: 'global' });
        }

        handleUnhandledRejection(event) {
            this.errorStats.jsErrors++;
            this.service.logError(event.reason, { type: 'promise' });
        }

        setupNetworkMonitoring() {
            const originalFetch = window.fetch;
            window.fetch = async (...args) => {
                try {
                    const response = await originalFetch(...args);
                    if (!response.ok) this.handleNetworkError(response, args[0]);
                    return response;
                } catch (error) {
                    this.handleNetworkError(error, args[0]);
                    throw error;
                }
            };
        }

        handleNetworkError(error, url) {
            this.errorStats.networkErrors++;
            this.service.logError(error, { type: 'network', url });
        }

        setupPerformanceMonitoring() {
            if ('PerformanceObserver' in window) {
                const observer = new PerformanceObserver((list) => {
                    for (const entry of list.getEntries()) {
                        if (entry.duration > 100) {
                            this.service.logError('Long task detected', { duration: entry.duration, type: 'performance' });
                        }
                    }
                });
                observer.observe({ entryTypes: ['longtask'] });
            }
        }

        generateErrorId() {
            return 'err_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
        }

        getDefaultFallback(name) {
            return `The ${name} component encountered an error.`;
        }

        showErrorNotification(boundary, info) {
            if (window.ToasterSystem) window.ToasterSystem.show(`Error in ${boundary.name}: ${info.error.message}`, 'error');
        }

        showPermanentError(boundary, info) {
            boundary.element.innerHTML = `
                <div class="error-boundary-permanent bg-red-100 border border-red-300 rounded-lg p-6 text-center">
                    <i data-lucide="x-circle" class="w-12 h-12 text-red-500 mx-auto mb-4"></i>
                    <h3 class="text-lg font-semibold text-red-800 mb-2">Component Unavailable</h3>
                    <p class="text-red-600 mb-4">Multi-recovery failure.</p>
                    <button onclick="window.location.reload()" class="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700">Reload</button>
                </div>
            `;
            if (window.lucide) window.lucide.createIcons({ parent: boundary.element });
        }

        reportError(id) {
            console.log('Reporting error:', id);
            if (window.ToasterSystem) window.ToasterSystem.show('Report submitted', 'success');
        }

        wrapElementEvents() { }
        setupMutationObserver() { }
    }

    CC.Utils.ErrorBoundary = new ErrorBoundary();

    // Legacy support
    window.ErrorBoundary = CC.Utils.ErrorBoundary;
})(window.CommunityCar);
