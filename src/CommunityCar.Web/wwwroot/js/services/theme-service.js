/**
 * Theme Service
 * Manages dark/light mode and synchronizes toggle buttons
 */
(function (CC) {
    class ThemeService extends CC.Utils.BaseService {
        constructor() {
            super('Theme');
            this.html = document.documentElement;
            this.toggleButtons = [
                { toggle: document.getElementById('theme-toggle'), sun: document.getElementById('sun-icon'), moon: document.getElementById('moon-icon') },
                { toggle: document.getElementById('mobile-theme-toggle'), sun: document.getElementById('mobile-sun-icon'), moon: document.getElementById('mobile-moon-icon') },
                { toggle: document.getElementById('dashboard-theme-toggle'), sun: document.getElementById('dashboard-sun-icon'), moon: document.getElementById('dashboard-moon-icon') }
            ];

            // Initialization is handled by base or explicit call? 
            // In site.js we usually init. For now, we self-init if DOM is ready, or wait.
            if (document.readyState !== 'loading') {
                this.init();
            } else {
                document.addEventListener('DOMContentLoaded', () => this.init());
            }
        }

        init() {
            if (this.initialized) return;
            super.init();

            // Initialize from localStorage or system preference
            let currentTheme = localStorage.getItem('theme');
            if (!currentTheme) {
                currentTheme = window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
                localStorage.setItem('theme', currentTheme);
            }

            this.applyTheme(currentTheme);
            this.setupEventListeners();
        }

        applyTheme(theme) {
            // Update HTML class
            if (theme === 'dark') {
                this.html.classList.add('dark');
                this.html.classList.remove('light');
            } else {
                this.html.classList.remove('dark');
                this.html.classList.add('light');
            }

            // Update icons
            this.updateIcons(theme);
        }

        updateIcons(theme) {
            this.toggleButtons.forEach(({ sun, moon }) => {
                if (!sun || !moon) return;
                if (theme === 'dark') {
                    sun.classList.add('hidden');
                    moon.classList.remove('hidden');
                } else {
                    sun.classList.remove('hidden');
                    moon.classList.add('hidden');
                }
            });
        }

        setupEventListeners() {
            this.toggleButtons.forEach(({ toggle }) => {
                if (toggle) {
                    // Use a named handler or bound function to prevent dups if init called twice (though BaseService protects us)
                    toggle.addEventListener('click', () => this.toggleTheme());
                }
            });
        }

        toggleTheme() {
            const isDark = this.html.classList.contains('dark');
            const newTheme = isDark ? 'light' : 'dark';

            this.applyTheme(newTheme);
            localStorage.setItem('theme', newTheme);
        }
    }

    // Initialize Singleton
    CC.Services.Theme = new ThemeService();

})(window.CommunityCar);
