// Global Notification System
(function() {
    'use strict';
    
    // Global notification function
    window.showNotification = function(message, type = 'info', options = {}) {
        // Default options
        const defaultOptions = {
            duration: 5000,
            closable: true,
            position: 'top-right',
            maxToasts: 5
        };
        
        const config = { ...defaultOptions, ...options };
        
        // Use existing notification systems if available
        if (window.toastr && !options.forceCustom) {
            const toastrOptions = {
                timeOut: config.duration,
                closeButton: config.closable,
                positionClass: `toast-${config.position}`,
                preventDuplicates: true
            };
            
            toastr.options = toastrOptions;
            toastr[type](message);
            return;
        }
        
        // Create custom toast notification
        createCustomToast(message, type, config);
    };
    
    function createCustomToast(message, type, config) {
        // Create toast container if it doesn't exist
        let toastContainer = document.getElementById('toast-container');
        if (!toastContainer) {
            toastContainer = document.createElement('div');
            toastContainer.id = 'toast-container';
            document.body.appendChild(toastContainer);
        }
        
        // Create toast notification
        const toast = document.createElement('div');
        toast.className = `toast-notification toast-${type}`;
        toast.setAttribute('role', 'alert');
        toast.setAttribute('aria-live', 'polite');
        
        // Create toast content
        const content = document.createElement('div');
        content.className = 'toast-content';
        
        // Add icon based on type
        const icon = document.createElement('i');
        icon.className = getIconClass(type);
        
        // Add message
        const messageSpan = document.createElement('span');
        messageSpan.className = 'toast-message';
        messageSpan.textContent = message;
        
        content.appendChild(icon);
        content.appendChild(messageSpan);
        toast.appendChild(content);
        
        // Add close button if closable
        if (config.closable) {
            const closeBtn = document.createElement('button');
            closeBtn.className = 'toast-close';
            closeBtn.innerHTML = '×';
            closeBtn.setAttribute('aria-label', 'Close notification');
            closeBtn.setAttribute('type', 'button');
            toast.appendChild(closeBtn);
            
            closeBtn.addEventListener('click', (e) => {
                e.stopPropagation();
                removeToast(toast, toastContainer);
            });
        }
        
        // Add to container
        toastContainer.appendChild(toast);
        
        // Auto remove if duration is set
        let autoRemoveTimer;
        if (config.duration > 0) {
            autoRemoveTimer = setTimeout(() => {
                removeToast(toast, toastContainer);
            }, config.duration);
        }
        
        // Pause auto-remove on hover
        toast.addEventListener('mouseenter', () => {
            if (autoRemoveTimer) {
                clearTimeout(autoRemoveTimer);
            }
        });
        
        // Resume auto-remove on mouse leave
        toast.addEventListener('mouseleave', () => {
            if (config.duration > 0) {
                autoRemoveTimer = setTimeout(() => {
                    removeToast(toast, toastContainer);
                }, 1000); // Give 1 second after mouse leave
            }
        });
        
        // Limit number of toasts
        limitToasts(toastContainer, config.maxToasts);
        
        return toast;
    }
    
    function removeToast(toast, container) {
        if (toast.parentNode) {
            toast.style.animation = 'slideOutRight 0.3s ease-out';
            setTimeout(() => {
                if (toast.parentNode) {
                    toast.parentNode.removeChild(toast);
                }
                // Clean up container if empty
                if (container.children.length === 0) {
                    container.remove();
                }
            }, 300);
        }
    }
    
    function limitToasts(container, maxToasts) {
        const toasts = container.querySelectorAll('.toast-notification');
        if (toasts.length > maxToasts) {
            const excessCount = toasts.length - maxToasts;
            for (let i = 0; i < excessCount; i++) {
                const oldestToast = toasts[i];
                removeToast(oldestToast, container);
            }
        }
    }
    
    function getIconClass(type) {
        const iconMap = {
            success: 'fas fa-check-circle',
            error: 'fas fa-exclamation-circle',
            warning: 'fas fa-exclamation-triangle',
            info: 'fas fa-info-circle'
        };
        return iconMap[type] || iconMap.info;
    }
    
    // Convenience methods
    window.notify = {
        success: (message, options) => showNotification(message, 'success', options),
        error: (message, options) => showNotification(message, 'error', options),
        warning: (message, options) => showNotification(message, 'warning', options),
        info: (message, options) => showNotification(message, 'info', options)
    };
    
    // Initialize on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializeNotifications);
    } else {
        initializeNotifications();
    }
    
    function initializeNotifications() {
        // Add global styles if not already present
        if (!document.getElementById('notification-styles')) {
            const styles = document.createElement('style');
            styles.id = 'notification-styles';
            styles.textContent = `
                .toast-content {
                    display: flex;
                    align-items: center;
                    gap: 8px;
                    flex: 1;
                }
                
                .toast-content i {
                    font-size: 16px;
                    flex-shrink: 0;
                }
                
                .toast-message {
                    flex: 1;
                    line-height: 1.4;
                }
            `;
            document.head.appendChild(styles);
        }
    }
})();