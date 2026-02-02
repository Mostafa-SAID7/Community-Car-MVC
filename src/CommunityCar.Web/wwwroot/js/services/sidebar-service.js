/**
 * Sidebar Service
 * Manages the main application sidebar state and mobile interactions
 */
(function (CC) {
    class SidebarService extends CC.Utils.BaseService {
        constructor(config = {}) {
            super('Sidebar');
            this.config = {
                toggleId: 'sidebar-toggle',
                sidebarId: 'main-sidebar',
                backdropId: 'main-sidebar-backdrop',
                closeBtnId: 'main-sidebar-close',
                ...config
            };

            // Defer element lookup to init/ready to ensure DOM exists
            if (document.readyState !== 'loading') {
                this.init();
            } else {
                document.addEventListener('DOMContentLoaded', () => this.init());
            }
        }

        init() {
            if (this.initialized) return;

            this.sidebarToggle = document.getElementById(this.config.toggleId);
            this.sidebar = document.getElementById(this.config.sidebarId);
            this.sidebarBackdrop = document.getElementById(this.config.backdropId);
            this.sidebarCloseBtn = document.getElementById(this.config.closeBtnId);

            if (this.sidebar && this.sidebarToggle) {
                super.init();
                this.setupEventListeners();
                this.handleResize();
            }
        }

        setupEventListeners() {
            // Toggle button
            this.sidebarToggle.addEventListener('click', (e) => {
                e.preventDefault();
                e.stopPropagation();
                this.toggle();
            });

            // Close button (mobile)
            if (this.sidebarCloseBtn) {
                this.sidebarCloseBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    this.close();
                });
            }

            // Backdrop click
            if (this.sidebarBackdrop) {
                this.sidebarBackdrop.addEventListener('click', () => this.close());
            }

            // Close on Escape key
            document.addEventListener('keydown', (e) => {
                if (e.key === 'Escape') {
                    if (this.isMobile() && this.isOpen()) {
                        this.close();
                    }
                }
            });

            // Close on link click (mobile)
            const links = this.sidebar.querySelectorAll('a');
            links.forEach(link => {
                link.addEventListener('click', () => {
                    if (this.isMobile()) {
                        this.close();
                    }
                });
            });

            // Window resize
            window.addEventListener('resize', () => this.handleResize());
        }

        toggle() {
            if (!this.isMobile()) return;

            if (this.isOpen()) {
                this.close();
            } else {
                this.open();
            }
        }

        open() {
            const isRtl = this.isRtl();

            if (isRtl) {
                this.sidebar.classList.remove('rtl:translate-x-[calc(100%+2rem)]');
            } else {
                this.sidebar.classList.remove('-translate-x-[calc(100%+2rem)]');
            }
            this.sidebar.classList.add('translate-x-0');

            if (this.sidebarBackdrop) {
                this.sidebarBackdrop.classList.remove('hidden');
            }

            document.body.style.overflow = 'hidden';
        }

        close() {
            const isRtl = this.isRtl();

            this.sidebar.classList.remove('translate-x-0');
            if (isRtl) {
                this.sidebar.classList.add('rtl:translate-x-[calc(100%+2rem)]');
            } else {
                this.sidebar.classList.add('-translate-x-[calc(100%+2rem)]');
            }

            if (this.sidebarBackdrop) {
                this.sidebarBackdrop.classList.add('hidden');
            }

            document.body.style.overflow = '';
        }

        handleResize() {
            const isMobile = this.isMobile();

            if (!isMobile) {
                // Desktop state
                this.sidebar.classList.remove('-translate-x-[calc(100%+2rem)]', 'rtl:translate-x-[calc(100%+2rem)]');
                this.sidebar.classList.add('translate-x-0');

                if (this.sidebarBackdrop) {
                    this.sidebarBackdrop.classList.add('hidden');
                }
                document.body.style.overflow = '';
            } else {
                // Mobile state - ensure hidden by default if not explicitly open
                if (!this.isOpen() && !this.isDirectlyHidden()) {
                    this.close();
                }
            }
        }

        // Helpers
        isMobile() {
            return window.innerWidth < 768;
        }

        isRtl() {
            return document.documentElement.getAttribute('dir') === 'rtl';
        }

        isOpen() {
            return this.sidebar.classList.contains('translate-x-0');
        }

        isDirectlyHidden() {
            // Checks if one of the hidden classes is already applied
            return this.sidebar.classList.contains('-translate-x-[calc(100%+2rem)]') ||
                this.sidebar.classList.contains('rtl:translate-x-[calc(100%+2rem)]');
        }
    }

    // Initialize Singleton
    CC.Services.Sidebar = new SidebarService();

})(window.CommunityCar);
