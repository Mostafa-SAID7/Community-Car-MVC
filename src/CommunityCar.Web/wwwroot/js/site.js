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

// ========================================
// MAIN SIDEBAR HANDLING
// ========================================
// Unified sidebar toggle system for all screen sizes
// Mobile (<768px): Toggle button opens/closes sidebar with backdrop
// Desktop (>=768px): Sidebar always visible, button does nothing

const sidebarToggle = document.getElementById('sidebar-toggle');
const sidebar = document.getElementById('main-sidebar');
const sidebarBackdrop = document.getElementById('main-sidebar-backdrop');
const sidebarCloseBtn = document.getElementById('main-sidebar-close');

if (sidebar && sidebarToggle) {
    // Toggle sidebar (mobile only)
    function toggleSidebar() {
        const isMobile = window.innerWidth < 768;

        // Only toggle on mobile screens
        if (!isMobile) return;

        const isRtl = document.documentElement.getAttribute('dir') === 'rtl';
        const isOpen = sidebar.classList.contains('translate-x-0');

        if (isOpen) {
            closeSidebar();
        } else {
            openSidebar();
        }
    }

    // Open sidebar (mobile only)
    function openSidebar() {
        const isRtl = document.documentElement.getAttribute('dir') === 'rtl';

        // Show sidebar
        if (isRtl) {
            sidebar.classList.remove('rtl:translate-x-[calc(100%+2rem)]');
        } else {
            sidebar.classList.remove('-translate-x-[calc(100%+2rem)]');
        }
        sidebar.classList.add('translate-x-0');

        // Show backdrop
        if (sidebarBackdrop) {
            sidebarBackdrop.classList.remove('hidden');
        }

        // Prevent body scroll
        document.body.style.overflow = 'hidden';
    }

    // Close sidebar (mobile only)
    function closeSidebar() {
        const isRtl = document.documentElement.getAttribute('dir') === 'rtl';

        // Hide sidebar
        sidebar.classList.remove('translate-x-0');
        if (isRtl) {
            sidebar.classList.add('rtl:translate-x-[calc(100%+2rem)]');
        } else {
            sidebar.classList.add('-translate-x-[calc(100%+2rem)]');
        }

        // Hide backdrop
        if (sidebarBackdrop) {
            sidebarBackdrop.classList.add('hidden');
        }

        // Restore body scroll
        document.body.style.overflow = '';
    }

    // Toggle button click
    sidebarToggle.addEventListener('click', (e) => {
        e.preventDefault();
        e.stopPropagation();
        toggleSidebar();
    });

    // Close button click (mobile only)
    if (sidebarCloseBtn) {
        sidebarCloseBtn.addEventListener('click', (e) => {
            e.preventDefault();
            e.stopPropagation();
            closeSidebar();
        });
    }

    // Backdrop click
    if (sidebarBackdrop) {
        sidebarBackdrop.addEventListener('click', closeSidebar);
    }

    // Escape key
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            const isMobile = window.innerWidth < 768;
            const isOpen = sidebar.classList.contains('translate-x-0');
            if (isMobile && isOpen) {
                closeSidebar();
            }
        }
    });

    // Close sidebar when clicking links (mobile only)
    const sidebarLinks = sidebar.querySelectorAll('a');
    sidebarLinks.forEach(link => {
        link.addEventListener('click', () => {
            const isMobile = window.innerWidth < 768;
            if (isMobile) {
                closeSidebar();
            }
        });
    });

    // Handle window resize
    window.addEventListener('resize', () => {
        const isMobile = window.innerWidth < 768;

        if (!isMobile) {
            // On desktop, ensure sidebar is visible and clean up mobile state
            sidebar.classList.remove('-translate-x-[calc(100%+2rem)]', 'rtl:translate-x-[calc(100%+2rem)]');
            sidebar.classList.add('translate-x-0');

            // Hide backdrop
            if (sidebarBackdrop) {
                sidebarBackdrop.classList.add('hidden');
            }

            // Restore body scroll
            document.body.style.overflow = '';
        } else {
            // On mobile, ensure sidebar is hidden by default
            const isOpen = sidebar.classList.contains('translate-x-0') &&
                !sidebar.classList.contains('-translate-x-[calc(100%+2rem)]') &&
                !sidebar.classList.contains('rtl:translate-x-[calc(100%+2rem)]');

            if (!isOpen) {
                closeSidebar();
            }
        }
    });
}
