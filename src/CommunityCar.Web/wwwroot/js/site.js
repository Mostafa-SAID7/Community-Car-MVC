// Theme Toggle Logic
const themeToggle = document.getElementById('theme-toggle');
const sunIcon = document.getElementById('sun-icon');
const moonIcon = document.getElementById('moon-icon');
const html = document.documentElement;

function updateIcons(theme) {
    if (!sunIcon || !moonIcon) return;
    if (theme === 'dark') {
        sunIcon.classList.add('hidden');
        moonIcon.classList.remove('hidden');
    } else {
        sunIcon.classList.remove('hidden');
        moonIcon.classList.add('hidden');
    }
}

// Initialize theme from localStorage
const currentTheme = localStorage.getItem('theme') || 'light';
html.className = currentTheme;
updateIcons(currentTheme);

if (themeToggle) {
    themeToggle.addEventListener('click', () => {
        const isDark = html.classList.contains('dark');
        const newTheme = isDark ? 'light' : 'dark';

        html.className = newTheme;
        localStorage.setItem('theme', newTheme);
        updateIcons(newTheme);
    });
}

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
document.addEventListener('DOMContentLoaded', function() {
    // Check for TempData toaster messages
    const toasterType = document.querySelector('meta[name="toaster-type"]')?.content;
    const toasterTitle = document.querySelector('meta[name="toaster-title"]')?.content;
    const toasterMessage = document.querySelector('meta[name="toaster-message"]')?.content;
    
    if (toasterType && toasterMessage && window.toaster) {
        setTimeout(() => {
            switch(toasterType) {
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
const mainContainer = document.getElementById('main-container');
const leftSidebar = document.querySelector('.sidebar');

if (sidebarToggle && mainContainer && leftSidebar) {
    console.log('Sidebar toggle initialized');

    // Create backdrop if it doesn't exist
    let backdrop = document.querySelector('.sidebar-backdrop');
    if (!backdrop) {
        backdrop = document.createElement('div');
        backdrop.className = 'sidebar-backdrop fixed inset-0 bg-black/50 z-30 opacity-0 pointer-events-none transition-opacity duration-300 md:hidden';
        document.body.appendChild(backdrop);
    }

    sidebarToggle.addEventListener('click', (e) => {
        e.preventDefault();
        e.stopPropagation();
        console.log('Sidebar toggle clicked');
        
        // Check if we're on mobile (screen width < 768px)
        const isMobile = window.innerWidth < 768;
        
        if (isMobile) {
            // Mobile: toggle between -translate-x-full and translate-x-0
            leftSidebar.classList.toggle('-translate-x-full');
            leftSidebar.classList.toggle('translate-x-0');
        } else {
            // Desktop: toggle between normal position and hidden
            leftSidebar.classList.toggle('md:translate-x-0');
            leftSidebar.classList.toggle('md:-translate-x-[calc(100%+2rem)]');
        }
        
        // Toggle backdrop (only visible on mobile)
        backdrop.classList.toggle('opacity-0');
        backdrop.classList.toggle('pointer-events-none');
        backdrop.classList.toggle('opacity-100');
        backdrop.classList.toggle('pointer-events-auto');
    });

    // Close sidebar when clicking backdrop
    backdrop.addEventListener('click', () => {
        const isMobile = window.innerWidth < 768;
        
        if (isMobile) {
            leftSidebar.classList.add('-translate-x-full');
            leftSidebar.classList.remove('translate-x-0');
        } else {
            leftSidebar.classList.add('md:-translate-x-[calc(100%+2rem)]');
            leftSidebar.classList.remove('md:translate-x-0');
        }
        
        backdrop.classList.add('opacity-0', 'pointer-events-none');
        backdrop.classList.remove('opacity-100', 'pointer-events-auto');
    });

    // Close on escape
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            const isMobile = window.innerWidth < 768;
            
            if (isMobile) {
                leftSidebar.classList.add('-translate-x-full');
                leftSidebar.classList.remove('translate-x-0');
            } else {
                leftSidebar.classList.add('md:-translate-x-[calc(100%+2rem)]');
                leftSidebar.classList.remove('md:translate-x-0');
            }
            
            backdrop.classList.add('opacity-0', 'pointer-events-none');
            backdrop.classList.remove('opacity-100', 'pointer-events-auto');
        }
    });

    // Close on link click
    document.addEventListener('click', (e) => {
        if (e.target.closest('.sidebar a')) {
            const isMobile = window.innerWidth < 768;
            
            if (isMobile) {
                leftSidebar.classList.add('-translate-x-full');
                leftSidebar.classList.remove('translate-x-0');
            } else {
                leftSidebar.classList.add('md:-translate-x-[calc(100%+2rem)]');
                leftSidebar.classList.remove('md:translate-x-0');
            }
            
            backdrop.classList.add('opacity-0', 'pointer-events-none');
            backdrop.classList.remove('opacity-100', 'pointer-events-auto');
        }
    });

    // Handle window resize to reset sidebar state
    window.addEventListener('resize', () => {
        const isMobile = window.innerWidth < 768;
        
        if (!isMobile) {
            // On desktop, ensure sidebar is visible and backdrop is hidden
            leftSidebar.classList.remove('-translate-x-full', 'translate-x-0');
            leftSidebar.classList.add('md:translate-x-0');
            backdrop.classList.add('opacity-0', 'pointer-events-none');
            backdrop.classList.remove('opacity-100', 'pointer-events-auto');
        } else {
            // On mobile, ensure sidebar is hidden by default
            leftSidebar.classList.add('-translate-x-full');
            leftSidebar.classList.remove('translate-x-0', 'md:translate-x-0', 'md:-translate-x-[calc(100%+2rem)]');
        }
    });
}

