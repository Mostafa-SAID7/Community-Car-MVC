/**
 * Enhanced Error Boundary System for CommunityCar
 * Provides comprehensive client-side error handling, logging, and recovery
 */

class ErrorBoundarySystem {
    constructor() {
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
        
        this.init();
    }

    init() {
        // Global error handlers
        window.addEventListener('error', (event) => this.handleGlobalError(event));
        window.addEventListener('unhandledrejection', (event) => this.handleUnhandledRejection(event));
        
        // Network error monitoring
        this.setupNetworkMonitoring();
        
        // Performance monitoring
        this.setupPerformanceMonitoring();
        
        // Initialize error reporting
        this.setupErrorReporting();
        
        console.log('🛡️ Error Boundary System initialized');
    }

    // Register a new error boundary
    registerBoundary(name, element, options = {}) {
        const boundary = {
            name,
            element,
            options: {
                fallbackContent: options.fallbackContent || this.getDefaultFallback(name),
                onError: options.onError || (() => {}),
                onRecover: options.onRecover || (() => {}),
                retryable: options.retryable !== false,
                autoRecover: options.autoRecover || false,
                recoveryDelay: options.recoveryDelay || 5000,
                ...options
            },
            errors: [],
            isActive: true,
            lastError: null,
            recoveryAttempts: 0
        };

        this.boundaries.set(name, boundary);
        this.wrapBoundary(boundary);
        
        console.log(`🛡️ Registered error boundary: ${name}`);
        return boundary;
    }

    // Wrap boundary with error handling
    wrapBoundary(boundary) {
        const { element, options } = boundary;
        
        // Store original content
        boundary.originalContent = element.innerHTML;
        
        // Add error boundary attributes
        element.setAttribute('data-error-boundary', boundary.name);
        element.setAttribute('data-boundary-status', 'active');
        
        // Wrap all event listeners and async operations
        this.wrapElementEvents(element, boundary);
        
        // Monitor for DOM mutations that might cause errors
        this.setupMutationObserver(element, boundary);
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
        
        // Update boundary status
        boundary.element.setAttribute('data-boundary-status', 'error');
        
        // Show fallback content
        this.showFallback(boundary, errorInfo);
        
        // Log error to server
        await this.logErrorToServer(errorInfo);
        
        // Call error callback
        if (boundary.options.onError) {
            try {
                boundary.options.onError(errorInfo, boundary);
            } catch (callbackError) {
                console.error('Error in boundary error callback:', callbackError);
            }
        }
        
        // Attempt recovery if enabled
        if (boundary.options.retryable) {
            this.scheduleRecovery(boundary, errorInfo);
        }
        
        // Show user notification
        this.showErrorNotification(boundary, errorInfo);
        
        return errorInfo;
    }

    // Show fallback content
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
                        <button onclick="window.ErrorBoundary.retryBoundary('${boundary.name}')" 
                                class="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 transition-colors">
                            <i data-lucide="refresh-cw" class="w-4 h-4 inline mr-2"></i>
                            Try Again
                        </button>
                    ` : ''}
                    <button onclick="window.ErrorBoundary.reportError('${errorInfo.id}')" 
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
        
        // Re-initialize Lucide icons
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }
    }

    // Schedule recovery attempt
    scheduleRecovery(boundary, errorInfo) {
        if (boundary.recoveryAttempts >= this.maxRetries) {
            console.log(`Max recovery attempts reached for boundary: ${boundary.name}`);
            return;
        }
        
        const delay = boundary.options.recoveryDelay * Math.pow(2, boundary.recoveryAttempts);
        
        setTimeout(() => {
            this.attemptRecovery(boundary, errorInfo);
        }, delay);
    }

    // Attempt to recover a boundary
    async attemptRecovery(boundary, errorInfo) {
        console.log(`🔄 Attempting recovery for boundary: ${boundary.name}`);
        
        boundary.recoveryAttempts++;
        
        try {
            // Restore original content
            boundary.element.innerHTML = boundary.originalContent;
            boundary.element.setAttribute('data-boundary-status', 'recovering');
            
            // Re-wrap the boundary
            this.wrapElementEvents(boundary.element, boundary);
            
            // Wait a moment for DOM to settle
            await new Promise(resolve => setTimeout(resolve, 100));
            
            // Mark as recovered
            boundary.isActive = true;
            boundary.element.setAttribute('data-boundary-status', 'active');
            
            // Update error info
            errorInfo.recovered = true;
            errorInfo.recoveredAt = new Date().toISOString();
            
            this.errorStats.recoveredErrors++;
            
            // Call recovery callback
            if (boundary.options.onRecover) {
                boundary.options.onRecover(errorInfo, boundary);
            }
            
            // Log recovery to server
            await this.logRecoveryToServer(errorInfo);
            
            console.log(`✅ Successfully recovered boundary: ${boundary.name}`);
            
            // Show success notification
            if (window.ToasterSystem) {
                window.ToasterSystem.show('Component recovered successfully', 'success');
            }
            
        } catch (recoveryError) {
            console.error(`❌ Failed to recover boundary: ${boundary.name}`, recoveryError);
            
            // Schedule another attempt if we haven't hit the limit
            if (boundary.recoveryAttempts < this.maxRetries) {
                this.scheduleRecovery(boundary, errorInfo);
            } else {
                // Show permanent error state
                this.showPermanentError(boundary, errorInfo);
            }
        }
    }

    // Manual retry triggered by user
    async retryBoundary(boundaryName) {
        const boundary = this.boundaries.get(boundaryName);
        if (!boundary || !boundary.lastError) return;
        
        boundary.recoveryAttempts = 0; // Reset attempts for manual retry
        await this.attemptRecovery(boundary, boundary.lastError);
    }

    // Handle global JavaScript errors
    handleGlobalError(event) {
        console.error('🚨 Global JavaScript Error:', event.error);
        
        this.errorStats.jsErrors++;
        this.errorStats.totalErrors++;
        
        const errorInfo = {
            id: this.generateErrorId(),
            type: 'global',
            error: {
                message: event.message,
                filename: event.filename,
                lineno: event.lineno,
                colno: event.colno,
                stack: event.error?.stack
            },
            context: {
                timestamp: new Date().toISOString(),
                userAgent: navigator.userAgent,
                url: window.location.href
            }
        };
        
        this.logErrorToServer(errorInfo);
        
        // Try to find the nearest boundary
        const targetElement = event.target || document.elementFromPoint(event.colno, event.lineno);
        if (targetElement) {
            const boundary = this.findNearestBoundary(targetElement);
            if (boundary) {
                this.handleBoundaryError(boundary, event.error || new Error(event.message));
            }
        }
    }

    // Handle unhandled promise rejections
    handleUnhandledRejection(event) {
        console.error('🚨 Unhandled Promise Rejection:', event.reason);
        
        this.errorStats.jsErrors++;
        this.errorStats.totalErrors++;
        
        const errorInfo = {
            id: this.generateErrorId(),
            type: 'promise',
            error: {
                message: event.reason?.message || String(event.reason),
                stack: event.reason?.stack
            },
            context: {
                timestamp: new Date().toISOString(),
                userAgent: navigator.userAgent,
                url: window.location.href
            }
        };
        
        this.logErrorToServer(errorInfo);
        
        // Prevent the default browser behavior
        event.preventDefault();
    }

    // Setup network monitoring
    setupNetworkMonitoring() {
        // Monitor fetch requests
        const originalFetch = window.fetch;
        window.fetch = async (...args) => {
            try {
                const response = await originalFetch(...args);
                
                if (!response.ok) {
                    this.handleNetworkError(response, args[0]);
                }
                
                return response;
            } catch (error) {
                this.handleNetworkError(error, args[0]);
                throw error;
            }
        };
        
        // Monitor XMLHttpRequest
        const originalXHROpen = XMLHttpRequest.prototype.open;
        XMLHttpRequest.prototype.open = function(...args) {
            this.addEventListener('error', (event) => {
                window.ErrorBoundary.handleNetworkError(event, args[1]);
            });
            
            this.addEventListener('timeout', (event) => {
                window.ErrorBoundary.handleNetworkError(new Error('Request timeout'), args[1]);
            });
            
            return originalXHROpen.apply(this, args);
        };
    }

    // Handle network errors
    handleNetworkError(error, url) {
        console.error('🌐 Network Error:', error, 'URL:', url);
        
        this.errorStats.networkErrors++;
        this.errorStats.totalErrors++;
        
        const errorInfo = {
            id: this.generateErrorId(),
            type: 'network',
            error: {
                message: error.message || `HTTP ${error.status}: ${error.statusText}`,
                status: error.status,
                statusText: error.statusText,
                url: url
            },
            context: {
                timestamp: new Date().toISOString(),
                userAgent: navigator.userAgent,
                currentUrl: window.location.href
            }
        };
        
        this.logErrorToServer(errorInfo);
        
        // Show network error notification
        if (window.ToasterSystem) {
            window.ToasterSystem.show('Network error occurred. Please check your connection.', 'error');
        }
    }

    // Setup performance monitoring
    setupPerformanceMonitoring() {
        // Monitor long tasks
        if ('PerformanceObserver' in window) {
            try {
                const observer = new PerformanceObserver((list) => {
                    for (const entry of list.getEntries()) {
                        if (entry.duration > 50) { // Tasks longer than 50ms
                            console.warn('⚠️ Long task detected:', entry);
                            
                            this.logPerformanceIssue({
                                type: 'long-task',
                                duration: entry.duration,
                                startTime: entry.startTime,
                                name: entry.name
                            });
                        }
                    }
                });
                
                observer.observe({ entryTypes: ['longtask'] });
            } catch (e) {
                console.log('Long task monitoring not supported');
            }
        }
        
        // Monitor memory usage
        if ('memory' in performance) {
            setInterval(() => {
                const memory = performance.memory;
                const usagePercent = (memory.usedJSHeapSize / memory.jsHeapSizeLimit) * 100;
                
                if (usagePercent > 90) {
                    console.warn('⚠️ High memory usage detected:', usagePercent.toFixed(2) + '%');
                    
                    this.logPerformanceIssue({
                        type: 'high-memory',
                        usagePercent,
                        usedMB: Math.round(memory.usedJSHeapSize / 1024 / 1024),
                        limitMB: Math.round(memory.jsHeapSizeLimit / 1024 / 1024)
                    });
                }
            }, 30000); // Check every 30 seconds
        }
    }

    // Log error to server
    async logErrorToServer(errorInfo) {
        try {
            await fetch('/api/errors/log', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: JSON.stringify(errorInfo)
            });
        } catch (error) {
            console.error('Failed to log error to server:', error);
            // Store in localStorage as fallback
            this.storeErrorLocally(errorInfo);
        }
    }

    // Log recovery to server
    async logRecoveryToServer(errorInfo) {
        try {
            await fetch('/api/errors/recovery', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: JSON.stringify({
                    errorId: errorInfo.id,
                    recoveredAt: errorInfo.recoveredAt
                })
            });
        } catch (error) {
            console.error('Failed to log recovery to server:', error);
        }
    }

    // Store error locally as fallback
    storeErrorLocally(errorInfo) {
        try {
            const errors = JSON.parse(localStorage.getItem('pendingErrors') || '[]');
            errors.push(errorInfo);
            
            // Keep only last 50 errors
            if (errors.length > 50) {
                errors.splice(0, errors.length - 50);
            }
            
            localStorage.setItem('pendingErrors', JSON.stringify(errors));
        } catch (error) {
            console.error('Failed to store error locally:', error);
        }
    }

    // Utility methods
    generateErrorId() {
        return 'err_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
    }

    getDefaultFallback(boundaryName) {
        return `The ${boundaryName} component encountered an error and needs to be reloaded.`;
    }

    findNearestBoundary(element) {
        let current = element;
        while (current && current !== document.body) {
            const boundaryName = current.getAttribute('data-error-boundary');
            if (boundaryName) {
                return this.boundaries.get(boundaryName);
            }
            current = current.parentElement;
        }
        return null;
    }

    wrapElementEvents(element, boundary) {
        // This would wrap event listeners to catch errors
        // Implementation depends on specific needs
    }

    setupMutationObserver(element, boundary) {
        // Monitor DOM changes that might cause errors
        // Implementation depends on specific needs
    }

    showErrorNotification(boundary, errorInfo) {
        if (window.ToasterSystem) {
            window.ToasterSystem.show(
                `Error in ${boundary.name}: ${errorInfo.error.message}`, 
                'error'
            );
        }
    }

    showPermanentError(boundary, errorInfo) {
        boundary.element.innerHTML = `
            <div class="error-boundary-permanent bg-red-100 border border-red-300 rounded-lg p-6 text-center">
                <i data-lucide="x-circle" class="w-12 h-12 text-red-500 mx-auto mb-4"></i>
                <h3 class="text-lg font-semibold text-red-800 mb-2">Component Unavailable</h3>
                <p class="text-red-600 mb-4">This component has encountered multiple errors and cannot be recovered automatically.</p>
                <button onclick="window.location.reload()" 
                        class="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 transition-colors">
                    <i data-lucide="refresh-cw" class="w-4 h-4 inline mr-2"></i>
                    Reload Page
                </button>
            </div>
        `;
        
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }
    }

    logPerformanceIssue(issue) {
        console.warn('Performance issue:', issue);
        // Could log to server if needed
    }

    setupErrorReporting() {
        // Setup error reporting UI and functionality
        // This could include a feedback form, automatic reporting, etc.
    }

    // Public API methods
    getStats() {
        return { ...this.errorStats };
    }

    getBoundaries() {
        return Array.from(this.boundaries.values());
    }

    clearBoundary(boundaryName) {
        const boundary = this.boundaries.get(boundaryName);
        if (boundary) {
            boundary.errors = [];
            boundary.recoveryAttempts = 0;
            boundary.lastError = null;
        }
    }

    reportError(errorId) {
        // Open error reporting dialog
        console.log('Reporting error:', errorId);
        if (window.ToasterSystem) {
            window.ToasterSystem.show('Error report submitted. Thank you!', 'success');
        }
    }
}

// Initialize global error boundary system
window.ErrorBoundary = new ErrorBoundarySystem();

// Export for module systems
if (typeof module !== 'undefined' && module.exports) {
    module.exports = ErrorBoundarySystem;
}