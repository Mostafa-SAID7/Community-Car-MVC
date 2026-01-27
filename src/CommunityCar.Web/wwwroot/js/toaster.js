/**
 * Modern Toaster Notification System
 * Provides comprehensive toast notifications with validation support
 */
class ToasterSystem {
    constructor(options = {}) {
        this.options = {
            position: 'top-right',
            duration: 5000,
            maxToasts: 5,
            showProgress: true,
            enableSounds: true,
            enableAnimations: true,
            rtlSupport: true,
            ...options
        };

        this.toasts = [];
        this.container = null;
        this.soundEnabled = this.options.enableSounds;

        this.init();
    }

    init() {
        this.createContainer();
        this.setupStyles();
        this.setupEventListeners();

        // Initialize global toaster instance
        if (!window.toaster) {
            window.toaster = this;
        }
    }

    createContainer() {
        this.container = document.createElement('div');
        this.container.className = `toaster-container toaster-${this.options.position}`;
        this.container.setAttribute('aria-live', 'polite');
        this.container.setAttribute('aria-atomic', 'false');

        document.body.appendChild(this.container);
    }

    setupStyles() {
        if (document.getElementById('toaster-styles')) return;

        const styles = document.createElement('style');
        styles.id = 'toaster-styles';
        styles.textContent = `
            .toaster-container {
                position: fixed;
                z-index: 9999;
                pointer-events: none;
                max-width: 420px;
                width: 100%;
            }
            
            .toaster-top-right {
                top: 1rem;
                right: 1rem;
            }
            
            .toaster-top-left {
                top: 1rem;
                left: 1rem;
            }
            
            .toaster-bottom-right {
                bottom: 1rem;
                right: 1rem;
            }
            
            .toaster-bottom-left {
                bottom: 1rem;
                left: 1rem;
            }
            
            .toaster-top-center {
                top: 1rem;
                left: 50%;
                transform: translateX(-50%);
            }
            
            .toaster-bottom-center {
                bottom: 1rem;
                left: 50%;
                transform: translateX(-50%);
            }
            
            .toast-item {
                pointer-events: auto;
                margin-bottom: 0.75rem;
                border-radius: 0.75rem;
                box-shadow: var(--shadow-xl);
                backdrop-filter: blur(16px);
                border: 1px solid var(--border);
                overflow: hidden;
                position: relative;
                min-height: 64px;
                max-width: 100%;
                transform: translateX(100%);
                opacity: 0;
                transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
            }
            
            .toast-item.rtl {
                transform: translateX(-100%);
            }
            
            .toast-item.show {
                transform: translateX(0);
                opacity: 1;
            }
            
            .toast-item.removing {
                transform: translateX(100%);
                opacity: 0;
                margin-bottom: 0;
                max-height: 0;
                padding: 0;
            }
            
            .toast-item.rtl.removing {
                transform: translateX(-100%);
            }
            
            .toast-content {
                display: flex;
                align-items: flex-start;
                padding: 1rem;
                gap: 0.75rem;
                position: relative;
            }
            
            .toast-icon {
                flex-shrink: 0;
                width: 2rem;
                height: 2rem;
                border-radius: 50%;
                display: flex;
                align-items: center;
                justify-content: center;
                font-size: 1rem;
            }
            
            .toast-body {
                flex: 1;
                min-width: 0;
            }
            
            .toast-title {
                font-weight: 600;
                font-size: 0.875rem;
                line-height: 1.25rem;
                margin-bottom: 0.25rem;
                word-wrap: break-word;
            }
            
            .toast-message {
                font-size: 0.8125rem;
                line-height: 1.25rem;
                opacity: 0.9;
                word-wrap: break-word;
            }
            
            .toast-actions {
                display: flex;
                gap: 0.5rem;
                margin-top: 0.75rem;
            }
            
            .toast-action {
                padding: 0.375rem 0.75rem;
                border-radius: 0.375rem;
                font-size: 0.75rem;
                font-weight: 500;
                border: none;
                cursor: pointer;
                transition: all 0.2s;
                text-decoration: none;
                display: inline-flex;
                align-items: center;
                gap: 0.25rem;
            }
            
            .toast-action:hover {
                transform: translateY(-1px);
            }
            
            .toast-close {
                position: absolute;
                top: 0.75rem;
                right: 0.75rem;
                width: 1.5rem;
                height: 1.5rem;
                border: none;
                background: hsla(var(--background), 0.2);
                border-radius: 50%;
                cursor: pointer;
                display: flex;
                align-items: center;
                justify-content: center;
                font-size: 0.75rem;
                transition: all 0.2s;
                opacity: 0.7;
            }
            
            .toast-close:hover {
                opacity: 1;
                background: hsla(var(--background), 0.4);
            }
            
            .toast-progress {
                position: absolute;
                bottom: 0;
                left: 0;
                height: 3px;
                background: hsla(var(--background), 0.3);
                transition: width linear;
                border-radius: 0 0 0.75rem 0.75rem;
            }
            
            /* Toast Types - Using design system colors */
            .toast-success {
                background: hsl(var(--chart-success, 142 76% 36%));
                color: white;
            }
            
            .toast-success .toast-icon {
                background: hsla(var(--background), 0.2);
            }
            
            .toast-error {
                background: hsl(var(--destructive));
                color: var(--destructive-foreground);
            }
            
            .toast-error .toast-icon {
                background: hsla(var(--background), 0.2);
            }
            
            .toast-warning {
                background: hsl(38 92% 50%);
                color: white;
            }
            
            .toast-warning .toast-icon {
                background: hsla(var(--background), 0.2);
            }
            
            .toast-info {
                background: hsl(var(--primary));
                color: var(--primary-foreground);
            }
            
            .toast-info .toast-icon {
                background: hsla(var(--background), 0.2);
            }
            
            .toast-validation {
                background: hsl(262 83% 58%);
                color: white;
            }
            
            .toast-validation .toast-icon {
                background: hsla(var(--background), 0.2);
            }
            
            /* Action Button Styles */
            .toast-action-primary {
                background: hsla(var(--background), 0.2);
                color: white;
            }
            
            .toast-action-primary:hover {
                background: hsla(var(--background), 0.3);
            }
            
            .toast-action-secondary {
                background: transparent;
                color: hsla(var(--background), 0.9);
                border: 1px solid hsla(var(--background), 0.3);
            }
            
            .toast-action-secondary:hover {
                background: hsla(var(--background), 0.1);
                color: white;
            }
            
            /* Mobile Responsive */
            @media (max-width: 640px) {
                .toaster-container {
                    left: 1rem !important;
                    right: 1rem !important;
                    max-width: none;
                    transform: none !important;
                }
                
                .toast-content {
                    padding: 0.875rem;
                }
                
                .toast-title {
                    font-size: 0.8125rem;
                }
                
                .toast-message {
                    font-size: 0.75rem;
                }
            }
            
            /* Dark mode support */
            .dark .toast-item {
                border-color: rgba(255, 255, 255, 0.05);
            }
            
            /* RTL Support */
            [dir="rtl"] .toast-close {
                right: auto;
                left: 0.75rem;
            }
            
            [dir="rtl"] .toast-progress {
                left: auto;
                right: 0;
                border-radius: 0 0 0.75rem 0.75rem;
            }
        `;

        document.head.appendChild(styles);
    }

    setupEventListeners() {
        // Handle window resize for mobile responsiveness
        window.addEventListener('resize', () => {
            this.updateContainerPosition();
        });

        // Handle RTL changes
        const observer = new MutationObserver((mutations) => {
            mutations.forEach((mutation) => {
                if (mutation.type === 'attributes' && mutation.attributeName === 'dir') {
                    this.updateRTLSupport();
                }
            });
        });

        observer.observe(document.documentElement, {
            attributes: true,
            attributeFilter: ['dir']
        });
    }

    updateContainerPosition() {
        // Update container positioning for mobile
        if (window.innerWidth <= 640) {
            this.container.style.transform = 'none';
        }
    }

    updateRTLSupport() {
        const isRTL = document.documentElement.dir === 'rtl';
        this.toasts.forEach(toast => {
            if (isRTL) {
                toast.element.classList.add('rtl');
            } else {
                toast.element.classList.remove('rtl');
            }
        });
    }

    // Main toast creation method
    show(options) {
        if (typeof options === 'string') {
            options = { message: options };
        }

        const config = {
            type: 'info',
            title: '',
            message: '',
            duration: this.options.duration,
            actions: [],
            closable: true,
            showProgress: this.options.showProgress,
            sound: true,
            validation: null,
            ...options
        };

        // Validate required fields
        if (!config.message && !config.title) {
            console.warn('Toast message or title is required');
            return null;
        }

        // Limit number of toasts
        if (this.toasts.length >= this.options.maxToasts) {
            this.remove(this.toasts[0]);
        }

        const toast = this.createToast(config);
        this.toasts.push(toast);

        this.container.appendChild(toast.element);

        // Trigger show animation
        requestAnimationFrame(() => {
            toast.element.classList.add('show');
        });

        // Auto-remove if duration is set
        if (config.duration > 0) {
            toast.timer = setTimeout(() => {
                this.remove(toast);
            }, config.duration);
        }

        // Play sound
        if (config.sound && this.soundEnabled) {
            this.playSound(config.type);
        }

        return toast;
    }

    createToast(config) {
        const toast = {
            id: this.generateId(),
            config,
            element: null,
            timer: null
        };

        const element = document.createElement('div');
        element.className = `toast-item toast-${config.type}`;
        element.setAttribute('role', 'alert');
        element.setAttribute('aria-live', 'assertive');

        // Add RTL support
        if (document.documentElement.dir === 'rtl') {
            element.classList.add('rtl');
        }

        const icon = this.getIcon(config.type);
        const actionsHTML = this.createActionsHTML(config.actions, toast);

        element.innerHTML = `
            <div class="toast-content">
                <div class="toast-icon">
                    <i data-lucide="${icon}"></i>
                </div>
                <div class="toast-body">
                    ${config.title ? `<div class="toast-title">${this.escapeHtml(config.title)}</div>` : ''}
                    <div class="toast-message">${this.escapeHtml(config.message)}</div>
                    ${actionsHTML ? `<div class="toast-actions">${actionsHTML}</div>` : ''}
                </div>
                ${config.closable ? `
                    <button class="toast-close" type="button" aria-label="Close">
                        <i data-lucide="x"></i>
                    </button>
                ` : ''}
            </div>
            ${config.showProgress && config.duration > 0 ? '<div class="toast-progress"></div>' : ''}
        `;

        toast.element = element;

        // Set up event listeners
        this.setupToastEventListeners(toast);

        // Initialize Lucide icons
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }

        return toast;
    }

    setupToastEventListeners(toast) {
        const element = toast.element;

        // Close button
        const closeBtn = element.querySelector('.toast-close');
        if (closeBtn) {
            closeBtn.addEventListener('click', () => {
                this.remove(toast);
            });
        }

        // Action buttons
        const actionBtns = element.querySelectorAll('.toast-action');
        actionBtns.forEach((btn, index) => {
            const action = toast.config.actions[index];
            if (action && action.handler) {
                btn.addEventListener('click', (e) => {
                    e.preventDefault();
                    action.handler(toast, e);

                    if (action.closeOnClick !== false) {
                        this.remove(toast);
                    }
                });
            }
        });

        // Progress bar animation
        if (toast.config.showProgress && toast.config.duration > 0) {
            const progressBar = element.querySelector('.toast-progress');
            if (progressBar) {
                progressBar.style.width = '100%';
                requestAnimationFrame(() => {
                    progressBar.style.transitionDuration = `${toast.config.duration}ms`;
                    progressBar.style.width = '0%';
                });
            }
        }

        // Pause on hover
        element.addEventListener('mouseenter', () => {
            if (toast.timer) {
                clearTimeout(toast.timer);
                toast.timer = null;
            }
        });

        element.addEventListener('mouseleave', () => {
            if (toast.config.duration > 0 && !toast.timer) {
                toast.timer = setTimeout(() => {
                    this.remove(toast);
                }, 1000); // Resume with shorter duration
            }
        });
    }

    createActionsHTML(actions, toast) {
        if (!actions || actions.length === 0) return '';

        return actions.map((action, index) => {
            const className = `toast-action toast-action-${action.style || 'primary'}`;
            const icon = action.icon ? `<i data-lucide="${action.icon}"></i>` : '';

            if (action.url) {
                return `<a href="${action.url}" class="${className}">${icon}${this.escapeHtml(action.text)}</a>`;
            } else {
                return `<button type="button" class="${className}" data-action-index="${index}">${icon}${this.escapeHtml(action.text)}</button>`;
            }
        }).join('');
    }

    remove(toast) {
        if (!toast || !toast.element) return;

        // Clear timer
        if (toast.timer) {
            clearTimeout(toast.timer);
            toast.timer = null;
        }

        // Remove from array
        const index = this.toasts.indexOf(toast);
        if (index > -1) {
            this.toasts.splice(index, 1);
        }

        // Animate out
        toast.element.classList.add('removing');

        setTimeout(() => {
            if (toast.element && toast.element.parentElement) {
                toast.element.parentElement.removeChild(toast.element);
            }
        }, 300);
    }

    // Convenience methods
    success(message, title, options = {}) {
        return this.show({
            type: 'success',
            title,
            message,
            ...options
        });
    }

    error(message, title, options = {}) {
        return this.show({
            type: 'error',
            title,
            message,
            duration: 8000, // Longer duration for errors
            ...options
        });
    }

    warning(message, title, options = {}) {
        return this.show({
            type: 'warning',
            title,
            message,
            ...options
        });
    }

    info(message, title, options = {}) {
        return this.show({
            type: 'info',
            title,
            message,
            ...options
        });
    }

    validation(errors, title = 'Validation Errors', options = {}) {
        let message = '';

        if (Array.isArray(errors)) {
            message = errors.join('\n');
        } else if (typeof errors === 'object') {
            message = Object.values(errors).flat().join('\n');
        } else {
            message = errors.toString();
        }

        return this.show({
            type: 'validation',
            title,
            message,
            duration: 10000, // Longer duration for validation errors
            ...options
        });
    }

    // Form validation helper
    showValidationErrors(form, errors) {
        // Clear existing validation toasts
        this.clearValidationToasts();

        // Show validation toast
        const toast = this.validation(errors, 'Please fix the following errors:');
        toast.isValidation = true;

        // Highlight form fields with errors
        if (typeof errors === 'object' && !Array.isArray(errors)) {
            Object.keys(errors).forEach(fieldName => {
                const field = form.querySelector(`[name="${fieldName}"]`);
                if (field) {
                    field.classList.add('error');

                    // Remove error class on input
                    const removeError = () => {
                        field.classList.remove('error');
                        field.removeEventListener('input', removeError);
                        field.removeEventListener('change', removeError);
                    };

                    field.addEventListener('input', removeError);
                    field.addEventListener('change', removeError);
                }
            });
        }

        return toast;
    }

    clearValidationToasts() {
        this.toasts.filter(toast => toast.isValidation).forEach(toast => {
            this.remove(toast);
        });
    }

    // Utility methods
    getIcon(type) {
        const icons = {
            success: 'check-circle',
            error: 'x-circle',
            warning: 'alert-triangle',
            info: 'info',
            validation: 'alert-circle'
        };
        return icons[type] || icons.info;
    }

    playSound(type) {
        try {
            const sounds = {
                success: '/sounds/success.mp3',
                error: '/sounds/error.mp3',
                warning: '/sounds/warning.mp3',
                info: '/sounds/info.mp3',
                validation: '/sounds/error.mp3'
            };

            const audio = new Audio(sounds[type] || sounds.info);
            audio.volume = 0.3;
            audio.play().catch(() => {
                // Ignore audio play errors
            });
        } catch (error) {
            // Ignore audio errors
        }
    }

    generateId() {
        return 'toast_' + Math.random().toString(36).substr(2, 9);
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Configuration methods
    setPosition(position) {
        this.container.className = `toaster-container toaster-${position}`;
        this.options.position = position;
    }

    enableSounds() {
        this.soundEnabled = true;
    }

    disableSounds() {
        this.soundEnabled = false;
    }

    clear() {
        this.toasts.forEach(toast => this.remove(toast));
    }
}

// Initialize toaster system when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    // Initialize global toaster
    window.toaster = new ToasterSystem();

    // Legacy compatibility with existing toastr calls
    if (!window.toastr) {
        window.toastr = {
            success: (message, title) => window.toaster.success(message, title),
            error: (message, title) => window.toaster.error(message, title),
            warning: (message, title) => window.toaster.warning(message, title),
            info: (message, title) => window.toaster.info(message, title)
        };
    }

    // Enhanced notify object
    window.notify = {
        success: (message, title, options) => window.toaster.success(message, title, options),
        error: (message, title, options) => window.toaster.error(message, title, options),
        warning: (message, title, options) => window.toaster.warning(message, title, options),
        info: (message, title, options) => window.toaster.info(message, title, options),
        validation: (errors, title, options) => window.toaster.validation(errors, title, options),
        clear: () => window.toaster.clear()
    };
});