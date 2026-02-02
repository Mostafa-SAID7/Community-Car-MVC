/**
 * Layout Controller
 * Handles general layout interactions: Mobile Viewport, Search Overlay, Dropdowns, and UI Sync.
 * Replaces layout-interactions.js
 */
(function (CC) {
    class LayoutController extends CC.Utils.BaseComponent {
        constructor() {
            super('LayoutController');
        }

        init() {
            if (this.initialized) return;
            super.init();

            this.handleMobileViewport();
            this.setupMobileSearch();
            this.setupDropdowns();
            this.setupLanguageSync();
            this.setupResizeHandlers();

            // Initial cleanup
            this.ensureMainContentVisible();
        }

        handleMobileViewport() {
            const setVh = () => {
                const vh = window.innerHeight * 0.01;
                document.documentElement.style.setProperty('--vh', `${vh}px`);
            };
            setVh();
            window.addEventListener('resize', setVh);
            window.addEventListener('orientationchange', setVh);
        }

        setupMobileSearch() {
            const toggle = document.getElementById('mobile-search-toggle');
            const overlay = document.getElementById('mobile-search-overlay');
            const close = document.getElementById('mobile-search-close');
            const input = document.getElementById('mobile-search-input');

            if (toggle && overlay) {
                toggle.addEventListener('click', () => {
                    overlay.classList.remove('hidden');
                    setTimeout(() => input?.focus(), 100);
                });

                close?.addEventListener('click', () => overlay.classList.add('hidden'));

                // Global escape handler handles closing, but we can add specific logic here if needed
            }
        }

        setupDropdowns() {
            // Global delegated click handler for dropdowns
            document.addEventListener('click', (e) => {
                const toggleBtn = e.target.closest('[data-action="toggle-dropdown"]');

                // Clicking a toggle
                if (toggleBtn) {
                    e.preventDefault();
                    e.stopPropagation();
                    const parent = toggleBtn.closest('.relative');
                    const menu = parent?.querySelector('.dropdown-content');

                    if (menu) {
                        // Close others
                        document.querySelectorAll('.dropdown-content:not(.hidden)').forEach(m => {
                            if (m !== menu) m.classList.add('hidden');
                        });

                        // RTL adjustment
                        if (document.documentElement.dir === 'rtl') {
                            menu.classList.add('rtl:right-auto', 'rtl:left-0');
                        }

                        menu.classList.toggle('hidden');
                    }
                    return;
                }

                // Clicking outside
                if (!e.target.closest('.dropdown-content')) {
                    document.querySelectorAll('.dropdown-content:not(.hidden)').forEach(menu => {
                        // Don't close if clicking a toggle (handled above) 
                        // but above we returned, so this logic runs only if NOT clicking toggle.
                        menu.classList.add('hidden');
                    });
                }
            });

            // Escape key to close all
            document.addEventListener('keydown', (e) => {
                if (e.key === 'Escape') {
                    document.querySelectorAll('.dropdown-content:not(.hidden)').forEach(menu => menu.classList.add('hidden'));

                    const mobileSearch = document.getElementById('mobile-search-overlay');
                    if (mobileSearch) mobileSearch.classList.add('hidden');
                }
            });
        }

        setupLanguageSync() {
            // Sync mobile/desktop language toggles
            const mobileToggle = document.getElementById('mobile-lang-toggle');
            const desktopToggle = document.getElementById('lang-toggle');

            if (mobileToggle && desktopToggle) {
                // Click sync
                mobileToggle.addEventListener('click', () => desktopToggle.click());

                // Value sync via Observer
                mobileToggle.textContent = desktopToggle.textContent;
                const observer = new MutationObserver(() => {
                    mobileToggle.textContent = desktopToggle.textContent;
                });
                observer.observe(desktopToggle, { childList: true, subtree: true, characterData: true });
            }
        }

        setupResizeHandlers() {
            window.addEventListener('resize', () => {
                if (window.innerWidth >= 768) {
                    this.ensureMainContentVisible();
                    document.body.classList.remove('sidebar-visible');
                    document.body.style.overflow = '';
                }
            });
        }

        ensureMainContentVisible() {
            const main = document.getElementById('main-content');
            if (main) {
                main.style.opacity = '1';
                main.style.visibility = 'visible';
                main.style.display = 'block';
            }
        }
    }

    CC.Components.Layout = new LayoutController();

})(window.CommunityCar);
