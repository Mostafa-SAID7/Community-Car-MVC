/**
 * Localization Service
 * Handles language switching and RTL/LTR direction functionality.
 */
(function (CC) {
    class LocalizationService extends CC.Utils.BaseService {
        constructor() {
            super('Localization');

            // Elements
            this.langToggle = document.getElementById('lang-toggle');
            this.mobileLangToggle = document.getElementById('mobile-lang-toggle');
            this.html = document.documentElement;

            // Initialization
            if (document.readyState !== 'loading') {
                this.init();
            } else {
                document.addEventListener('DOMContentLoaded', () => this.init());
            }
        }

        init() {
            if (this.initialized) return;
            super.init();

            this.initializeState();
            this.setupEventListeners();
        }

        initializeState() {
            // Initialize from cookie or default
            const cultureCookie = CC.Services.Cookie ? CC.Services.Cookie.get('.AspNetCore.Culture') : null;
            let currentLang = 'en';
            let currentDir = 'ltr';

            if (CC.Config.debug) console.log('Culture cookie found:', cultureCookie);

            if (cultureCookie) {
                currentLang = cultureCookie.includes('ar') ? 'ar' : 'en';
                currentDir = currentLang === 'ar' ? 'rtl' : 'ltr';
            } else {
                // Fallback to localStorage or default
                currentLang = localStorage.getItem('lang') || 'en';
                currentDir = currentLang === 'ar' ? 'rtl' : 'ltr';
            }

            if (CC.Config.debug) console.log('Detected language:', { currentLang, currentDir });

            // Apply language and direction
            this.html.setAttribute('lang', currentLang);
            this.html.setAttribute('dir', currentDir);
            document.body.setAttribute('dir', currentDir);

            // Update toggle button text
            const langText = currentLang === 'en' ? 'AR' : 'EN';
            if (this.langToggle) this.langToggle.textContent = langText;
            if (this.mobileLangToggle) this.mobileLangToggle.textContent = langText;

            // Apply RTL-specific styles
            if (currentDir === 'rtl') {
                document.body.classList.add('rtl');
            } else {
                document.body.classList.remove('rtl');
            }
        }

        setupEventListeners() {
            if (this.langToggle) {
                this.langToggle.addEventListener('click', (e) => {
                    e.preventDefault();
                    this.toggleLanguage();
                });
            }

            if (this.mobileLangToggle) {
                this.mobileLangToggle.addEventListener('click', (e) => {
                    e.preventDefault();
                    this.toggleLanguage();
                });
            }
        }

        toggleLanguage() {
            if (!CC.Services.Cookie) {
                console.error("CookieService not available for language toggle");
                return;
            }

            const isEn = this.html.getAttribute('lang') === 'en';
            const newLang = isEn ? 'ar-EG' : 'en-US';
            const newCulture = `c=${newLang}|uic=${newLang}`;
            const newDir = isEn ? 'rtl' : 'ltr';

            if (CC.Config.debug) console.log('Language toggle clicked:', { isEn, newLang, newCulture, newDir });

            // Set the ASP.NET Core culture cookie
            CC.Services.Cookie.set('.AspNetCore.Culture', newCulture, 365);

            // Also update localStorage for persistence
            localStorage.setItem('lang', isEn ? 'ar' : 'en');
            localStorage.setItem('dir', newDir);

            // Apply direction immediately for smoother transition
            this.html.setAttribute('dir', newDir);
            this.html.setAttribute('lang', isEn ? 'ar' : 'en');
            document.body.setAttribute('dir', newDir);

            if (newDir === 'rtl') {
                document.body.classList.add('rtl');
            } else {
                document.body.classList.remove('rtl');
            }

            if (CC.Config.debug) console.log('Cookie set, reloading page...');

            // Reload to apply server-side localization
            window.location.reload();
        }
    }

    // Initialize Singleton
    CC.Services.Localization = new LocalizationService();

})(window.CommunityCar);
