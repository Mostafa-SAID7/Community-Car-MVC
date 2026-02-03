/**
 * Toaster Notification Service
 * Handles toast notifications with a modern UI, legacy compatibility, and sound effects.
 */
(function (CC) {
    class ToasterService extends CC.Utils.BaseService {
        constructor() {
            super('Toaster');
            this.toasts = [];
            this.options = {
                position: 'top-right',
                maxToasts: 5,
                duration: 5000,
                showProgress: true
            };
            this.soundEnabled = true;
            this.container = null;
        }

        init() {
            if (this.initialized) return;
            super.init();

            this.createContainer();
            this.injectStyles();
            this.setupEventListeners();
            this.checkForMetaMessages();
        }

        checkForMetaMessages() {
            const typeMeta = document.querySelector('meta[name="toaster-type"]');
            const titleMeta = document.querySelector('meta[name="toaster-title"]');
            const messageMeta = document.querySelector('meta[name="toaster-message"]');

            if (typeMeta && messageMeta) {
                const type = typeMeta.content;
                const title = titleMeta ? titleMeta.content : '';
                const message = messageMeta.content;

                if (type && message) {
                    // Delay slightly to ensure UI is ready
                    setTimeout(() => this.show({ type, title, message }), 100);
                }
            }
        }

        createContainer() {
            this.container = document.createElement('div');
            this.container.className = `toaster-container toaster-${this.options.position}`;
            document.body.appendChild(this.container);
        }

        injectStyles() {
            const styles = document.createElement('style');
            styles.textContent = `
                .toaster-container { position: fixed; z-index: 9999; display: flex; flex-direction: column; gap: 0.5rem; pointer-events: none; width: 100%; max-width: 400px; padding: 1rem; }
                .toaster-top-right { top: 0; right: 0; }
                .toaster-top-left { top: 0; left: 0; }
                .toaster-bottom-right { bottom: 0; right: 0; flex-direction: column-reverse; }
                .toaster-bottom-left { bottom: 0; left: 0; flex-direction: column-reverse; }
                .toaster-top-center { top: 0; left: 50%; transform: translateX(-50%); }
                .toaster-bottom-center { bottom: 0; left: 50%; transform: translateX(-50%); flex-direction: column-reverse; }

                .toast-item {
                    pointer-events: auto;
                    background: hsl(var(--card));
                    color: hsl(var(--card-foreground));
                    border: 1px solid hsl(var(--border));
                    border-radius: 0.75rem;
                    box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
                    overflow: hidden;
                    position: relative;
                    transform: translateX(100%);
                    opacity: 0;
                    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
                    display: flex;
                    flex-direction: column;
                }
                .toast-item.show { transform: translateX(0); opacity: 1; }
                .toast-item.removing { transform: translateX(100%); opacity: 0; height: 0; margin: 0; padding: 0; border: 0; }

                .toast-content { display: flex; padding: 1rem; gap: 0.75rem; position: relative; }
                .toast-icon { flex-shrink: 0; width: 1.5rem; height: 1.5rem; display: flex; items-center: center; justify-content: center; }
                .toast-body { flex: 1; min-width: 0; }
                .toast-title { font-weight: 600; font-size: 0.875rem; margin-bottom: 0.25rem; }
                .toast-message { font-size: 0.8125rem; opacity: 0.9; line-height: 1.4; }
                
                .toast-close { 
                    position: absolute; top: 0.75rem; right: 0.75rem; 
                    background: transparent; border: none; cursor: pointer; opacity: 0.5; transition: opacity 0.2s;
                }
                .toast-close:hover { opacity: 1; }

                .toast-progress { height: 3px; background: currentColor; opacity: 0.3; width: 100%; transition: width linear; }

                /* Variants */
                .toast-success { color: #10b981; }
                .toast-success .toast-progress { background-color: #10b981; }
                
                .toast-error { color: #ef4444; }
                .toast-error .toast-progress { background-color: #ef4444; }
                
                .toast-warning { color: #f59e0b; }
                .toast-warning .toast-progress { background-color: #f59e0b; }
                
                .toast-info { color: #3b82f6; }
                .toast-info .toast-progress { background-color: #3b82f6; }

                /* Dark mode adjustments override */
                :root[class~="dark"] .toast-item { background: hsl(var(--card)); border-color: hsl(var(--border)); }
                
                @media (max-width: 640px) {
                    .toaster-container { padding: 0.5rem; max-width: 100%; }
                    .toast-item { width: 100%; }
                }
            `;
            document.head.appendChild(styles);
        }

        setupEventListeners() {
            window.addEventListener('resize', () => this.updatePosition());
        }

        updatePosition() {
            if (window.innerWidth <= 640) {
                this.container.style.transform = 'none';
                this.container.style.left = '0';
                this.container.style.right = '0';
            }
        }

        show(options) {
            if (typeof options === 'string') options = { message: options };
            const config = { type: 'info', duration: this.options.duration, closable: true, ...options };

            if (!config.message && !config.title) return;

            // Trim
            if (this.toasts.length >= this.options.maxToasts) {
                this.remove(this.toasts[0]);
            }

            const toast = this.createToast(config);
            this.toasts.push(toast);
            this.container.appendChild(toast.element);

            // Animation
            requestAnimationFrame(() => toast.element.classList.add('show'));

            // Auto close
            if (config.duration > 0) {
                toast.timer = setTimeout(() => this.remove(toast), config.duration);
            }

            if (this.soundEnabled && config.sound !== false) this.playSound(config.type);

            return toast;
        }

        createToast(config) {
            const el = document.createElement('div');
            el.className = `toast-item toast-${config.type}`;
            el.setAttribute('role', 'alert');

            const iconMap = {
                success: '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>',
                error: '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><line x1="15" y1="9" x2="9" y2="15"/><line x1="9" y1="9" x2="15" y2="15"/></svg>',
                warning: '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg>',
                info: '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><line x1="12" y1="16" x2="12" y2="12"/><line x1="12" y1="8" x2="12.01" y2="8"/></svg>'
            };

            const icon = iconMap[config.type] || iconMap.info;

            el.innerHTML = `
                <div class="toast-content">
                    <div class="toast-icon">${icon}</div>
                    <div class="toast-body">
                        ${config.title ? `<div class="toast-title">${this.escapeHtml(config.title)}</div>` : ''}
                        <div class="toast-message">${this.escapeHtml(config.message)}</div>
                    </div>
                    ${config.closable ? `<button class="toast-close"><svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg></button>` : ''}
                </div>
                ${config.duration > 0 ? `<div class="toast-progress"></div>` : ''}
            `;

            // Events
            if (config.closable) {
                el.querySelector('.toast-close').addEventListener('click', () => this.remove({ element: el }));
            }

            // Progress Bar
            if (config.duration > 0) {
                const bar = el.querySelector('.toast-progress');
                // Immediate set width to 100% then animate to 0
                requestAnimationFrame(() => {
                    bar.style.transitionDuration = `${config.duration}ms`;
                    bar.style.width = '0%';
                });

                // Hover pause
                el.addEventListener('mouseenter', () => {
                    bar.style.animationPlayState = 'paused'; // Transition pause is harder, assuming simple duration reset logic or ignore pause for MV P
                });
            }

            return { element: el, config };
        }

        remove(toast) {
            if (!toast || !toast.element) return;
            const idx = this.toasts.indexOf(toast);
            if (idx > -1) this.toasts.splice(idx, 1);

            toast.element.classList.add('removing');
            setTimeout(() => {
                if (toast.element.parentElement) toast.element.remove();
            }, 300);
        }

        playSound(type) {
            // Placeholder - requires actual assets
            const sounds = {
                success: '/sounds/success.mp3',
                error: '/sounds/error.mp3',
                warning: '/sounds/warning.mp3',
                info: '/sounds/info.mp3'
            };
            const audio = new Audio(sounds[type] || sounds.info);
            audio.volume = 0.3;
            audio.play().catch(() => { });
        }

        escapeHtml(text) {
            if (!text) return '';
            const div = document.createElement('div');
            div.textContent = text;
            return div.innerHTML;
        }

        // Aliases
        success(msg, title, opt) { return this.show({ type: 'success', message: msg, title, ...opt }); }
        error(msg, title, opt) { return this.show({ type: 'error', message: msg, title, ...opt }); }
        warning(msg, title, opt) { return this.show({ type: 'warning', message: msg, title, ...opt }); }
        info(msg, title, opt) { return this.show({ type: 'info', message: msg, title, ...opt }); }
        validation(errors, title = 'Validation Errors', opt) {
            let msg = Array.isArray(errors) ? errors.join('<br>') : (typeof errors === 'object' ? Object.values(errors).join('<br>') : errors);
            return this.show({ type: 'error', message: msg, title, duration: 8000, ...opt });
        }
        clear() { this.toasts.forEach(t => this.remove(t)); }
    }

    CC.Services.Toaster = new ToasterService();
    document.addEventListener('DOMContentLoaded', () => CC.Services.Toaster.init());

    // Compatibility
    window.toaster = CC.Services.Toaster;
    if (!window.toastr) {
        window.toastr = {
            success: (m, t) => window.toaster.success(m, t),
            error: (m, t) => window.toaster.error(m, t),
            warning: (m, t) => window.toaster.warning(m, t),
            info: (m, t) => window.toaster.info(m, t)
        };
    }
    window.notify = window.toaster; // Use same instance

})(window.CommunityCar);