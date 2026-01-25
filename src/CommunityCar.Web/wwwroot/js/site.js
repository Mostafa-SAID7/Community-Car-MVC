// Theme Toggle Logic
const themeToggleButtons = [
    { toggle: document.getElementById('theme-toggle'), sun: document.getElementById('sun-icon'), moon: document.getElementById('moon-icon') },
    { toggle: document.getElementById('mobile-theme-toggle'), sun: document.getElementById('mobile-sun-icon'), moon: document.getElementById('mobile-moon-icon') },
    { toggle: document.getElementById('dashboard-theme-toggle'), sun: document.getElementById('dashboard-sun-icon'), moon: document.getElementById('dashboard-moon-icon') }
];

const html = document.documentElement;

function updateTheme(theme) {
    // Update HTML class for Tailwind dark mode
    if (theme === 'dark') {
        html.classList.add('dark');
        html.classList.remove('light');
    } else {
        html.classList.remove('dark');
        html.classList.add('light');
    }

    // Update all toggle button icons
    themeToggleButtons.forEach(({ sun, moon }) => {
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

// Initialize theme from localStorage or system preference
let currentTheme = localStorage.getItem('theme');
if (!currentTheme) {
    // Check system preference
    currentTheme = window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    // Save the detected preference
    localStorage.setItem('theme', currentTheme);
}

// Apply theme immediately
html.classList.remove('light', 'dark');
html.classList.add(currentTheme);

// Wait for DOM to be ready before updating icons
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => updateTheme(currentTheme));
} else {
    updateTheme(currentTheme);
}

// Add event listeners to all theme toggle buttons
themeToggleButtons.forEach(({ toggle }) => {
    if (toggle) {
        toggle.addEventListener('click', () => {
            const isDark = html.classList.contains('dark');
            const newTheme = isDark ? 'light' : 'dark';

            // Update theme
            updateTheme(newTheme);

            // Save preference
            localStorage.setItem('theme', newTheme);
        });
    }
});

// Language/RTL Toggle Logic
const langToggle = document.getElementById('lang-toggle');

// Helper to get cookie
function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
}

// Helper to set cookie
function setCookie(name, value, days) {
    const date = new Date();
    date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
    const expires = "; expires=" + date.toUTCString();
    document.cookie = name + "=" + (value || "") + expires + "; path=/; samesite=lax";
}

// Initialize from cookie or default
const cultureCookie = getCookie('.AspNetCore.Culture');
let currentLang = 'en';
let currentDir = 'ltr';

console.log('Culture cookie found:', cultureCookie);

if (cultureCookie) {
    currentLang = cultureCookie.includes('ar') ? 'ar' : 'en';
    currentDir = currentLang === 'ar' ? 'rtl' : 'ltr';
} else {
    // Fallback to localStorage or default
    currentLang = localStorage.getItem('lang') || 'en';
    currentDir = currentLang === 'ar' ? 'rtl' : 'ltr';
}

console.log('Detected language:', { currentLang, currentDir });

// Apply language and direction
html.setAttribute('lang', currentLang);
html.setAttribute('dir', currentDir);
document.body.setAttribute('dir', currentDir);

// Update toggle button text
if (langToggle) langToggle.textContent = currentLang === 'en' ? 'AR' : 'EN';

// Apply RTL-specific styles
if (currentDir === 'rtl') {
    document.body.classList.add('rtl');
} else {
    document.body.classList.remove('rtl');
}

if (langToggle) {
    langToggle.addEventListener('click', () => {
        const isEn = html.getAttribute('lang') === 'en';
        const newLang = isEn ? 'ar-EG' : 'en-US';
        const newCulture = `c=${newLang}|uic=${newLang}`;
        const newDir = isEn ? 'rtl' : 'ltr';

        console.log('Language toggle clicked:', { isEn, newLang, newCulture, newDir });

        // Set the ASP.NET Core culture cookie
        setCookie('.AspNetCore.Culture', newCulture, 365);

        // Also update localStorage for persistence
        localStorage.setItem('lang', isEn ? 'ar' : 'en');
        localStorage.setItem('dir', newDir);

        // Apply direction immediately for smoother transition
        html.setAttribute('dir', newDir);
        html.setAttribute('lang', isEn ? 'ar' : 'en');
        document.body.setAttribute('dir', newDir);

        if (newDir === 'rtl') {
            document.body.classList.add('rtl');
        } else {
            document.body.classList.remove('rtl');
        }

        console.log('Cookie set, reloading page...');

        // Reload to apply server-side localization
        window.location.reload();
    });
}

// Toastr Configuration
if (typeof toastr !== 'undefined') {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
}

// Global Notification Helper
window.notify = {
    success: (msg, title) => typeof toastr !== 'undefined' && toastr.success(msg, title),
    error: (msg, title) => typeof toastr !== 'undefined' && toastr.error(msg, title),
    info: (msg, title) => typeof toastr !== 'undefined' && toastr.info(msg, title),
    warning: (msg, title) => typeof toastr !== 'undefined' && toastr.warning(msg, title)
};

// Handle TempData toaster messages
document.addEventListener('DOMContentLoaded', function () {
    // Check for TempData toaster messages
    const toasterType = document.querySelector('meta[name="toaster-type"]')?.content;
    const toasterTitle = document.querySelector('meta[name="toaster-title"]')?.content;
    const toasterMessage = document.querySelector('meta[name="toaster-message"]')?.content;

    if (toasterType && toasterMessage && window.toaster) {
        setTimeout(() => {
            switch (toasterType) {
                case 'success':
                    window.toaster.success(toasterMessage, toasterTitle);
                    break;
                case 'error':
                    window.toaster.error(toasterMessage, toasterTitle);
                    break;
                case 'warning':
                    window.toaster.warning(toasterMessage, toasterTitle);
                    break;
                case 'info':
                    window.toaster.info(toasterMessage, toasterTitle);
                    break;
                case 'validation':
                    window.toaster.validation(toasterMessage, toasterTitle);
                    break;
                default:
                    window.toaster.info(toasterMessage, toasterTitle);
            }
        }, 100); // Small delay to ensure toaster is initialized
    }
});

// Sidebar handling for mobile
const sidebarToggle = document.getElementById('sidebar-toggle');
const rightSidebarToggle = document.getElementById('right-sidebar-toggle');
const mainContainer = document.getElementById('main-container');
const leftSidebar = document.querySelector('.sidebar');
const rightSidebar = document.querySelector('.sidebar-right');

if ((sidebarToggle || rightSidebarToggle) && mainContainer) {
    console.log('Sidebar system initialized');

    // Create backdrop if it doesn't exist
    let backdrop = document.querySelector('.sidebar-backdrop');
    if (!backdrop) {
        backdrop = document.createElement('div');
        backdrop.className = 'sidebar-backdrop fixed inset-0 bg-black/50 z-30 opacity-0 pointer-events-none transition-opacity duration-300';
        document.body.appendChild(backdrop);
    }

    if (sidebarToggle && leftSidebar) {
        sidebarToggle.addEventListener('click', (e) => {
            e.preventDefault();
            e.stopPropagation();

            const isMobile = window.innerWidth < 768;
            const isRtl = document.documentElement.getAttribute('dir') === 'rtl';

            if (isMobile) {
                if (isRtl) {
                    leftSidebar.classList.toggle('translate-x-full');
                    leftSidebar.classList.toggle('translate-x-0');
                } else {
                    leftSidebar.classList.toggle('-translate-x-full');
                    leftSidebar.classList.toggle('translate-x-0');
                }
            } else {
                if (isRtl) {
                    leftSidebar.classList.toggle('md:translate-x-0');
                    leftSidebar.classList.toggle('md:translate-x-[calc(100%+2rem)]');
                } else {
                    leftSidebar.classList.toggle('md:translate-x-0');
                    leftSidebar.classList.toggle('md:-translate-x-[calc(100%+2rem)]');
                }
            }

            backdrop.classList.toggle('opacity-0');
            backdrop.classList.toggle('pointer-events-none');
            backdrop.classList.toggle('opacity-100');
            backdrop.classList.toggle('pointer-events-auto');
        });
    }

    if (rightSidebarToggle && rightSidebar) {
        rightSidebarToggle.addEventListener('click', (e) => {
            e.preventDefault();
            e.stopPropagation();

            const isRtl = document.documentElement.getAttribute('dir') === 'rtl';

            // On mobile/tablet, the right sidebar is hidden with 'hidden xl:block'
            // We need to override this
            rightSidebar.classList.toggle('hidden');
            rightSidebar.classList.toggle('flex'); // Assuming it's a flex container or just block

            if (isRtl) {
                rightSidebar.classList.toggle('-translate-x-[calc(100%+2rem)]');
                rightSidebar.classList.toggle('translate-x-0');
            } else {
                rightSidebar.classList.toggle('translate-x-[calc(100%+2rem)]');
                rightSidebar.classList.toggle('translate-x-0');
            }

            backdrop.classList.toggle('opacity-0');
            backdrop.classList.toggle('pointer-events-none');
            backdrop.classList.toggle('opacity-100');
            backdrop.classList.toggle('pointer-events-auto');
        });
    }

    function closeSidebar() {
        const isMobile = window.innerWidth < 1280; // Corrected to xl breakpoint for right sidebar consistency
        const isRtl = document.documentElement.getAttribute('dir') === 'rtl';

        if (leftSidebar) {
            if (window.innerWidth < 768) {
                if (isRtl) {
                    leftSidebar.classList.add('translate-x-full');
                } else {
                    leftSidebar.classList.add('-translate-x-full');
                }
                leftSidebar.classList.remove('translate-x-0');
            }
        }

        if (rightSidebar) {
            if (window.innerWidth < 1280) {
                rightSidebar.classList.add('hidden');
                if (isRtl) {
                    rightSidebar.classList.add('-translate-x-[calc(100%+2rem)]');
                } else {
                    rightSidebar.classList.add('translate-x-[calc(100%+2rem)]');
                }
                rightSidebar.classList.remove('translate-x-0');
            }
        }

        backdrop.classList.add('opacity-0', 'pointer-events-none');
        backdrop.classList.remove('opacity-100', 'pointer-events-auto');
    }

    // Close sidebar when clicking backdrop
    backdrop.addEventListener('click', closeSidebar);

    // Close on escape
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            closeSidebar();
        }
    });

    // Handle window resize to reset sidebar state
    window.addEventListener('resize', () => {
        const width = window.innerWidth;
        const isRtl = document.documentElement.getAttribute('dir') === 'rtl';

        if (width >= 1280) { // xl
            if (rightSidebar) {
                rightSidebar.classList.remove('hidden', 'translate-x-0', 'translate-x-[calc(100%+2rem)]', '-translate-x-[calc(100%+2rem)]');
                rightSidebar.classList.add('xl:block');
            }
            backdrop.classList.add('opacity-0', 'pointer-events-none');
            backdrop.classList.remove('opacity-100', 'pointer-events-auto');
        }

        if (width >= 768) { // md
            if (leftSidebar) {
                leftSidebar.classList.remove('-translate-x-full', 'translate-x-0', 'translate-x-full');
                leftSidebar.classList.add('md:translate-x-0');
            }
        }
    });
}

