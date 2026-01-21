// Theme Toggle Logic
const themeToggle = document.getElementById('theme-toggle');
const sunIcon = document.getElementById('sun-icon');
const moonIcon = document.getElementById('moon-icon');
const html = document.documentElement;

function updateIcons(theme) {
    if (!sunIcon || !moonIcon) return;
    if (theme === 'dark') {
        sunIcon.classList.add('d-none');
        moonIcon.classList.remove('d-none');
    } else {
        sunIcon.classList.remove('d-none');
        moonIcon.classList.add('d-none');
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

if (cultureCookie) {
    currentLang = cultureCookie.includes('ar') ? 'ar' : 'en';
    currentDir = currentLang === 'ar' ? 'rtl' : 'ltr';
} else {
    // Fallback to localStorage or default
    currentLang = localStorage.getItem('lang') || 'en';
    currentDir = currentLang === 'ar' ? 'rtl' : 'ltr';
}

html.setAttribute('lang', currentLang);
html.setAttribute('dir', currentDir);
if (langToggle) langToggle.textContent = currentLang === 'en' ? 'AR' : 'EN';

if (langToggle) {
    langToggle.addEventListener('click', () => {
        const isEn = html.getAttribute('lang') === 'en';
        const newLang = isEn ? 'ar-EG' : 'en-US';
        const newCulture = `c=${newLang}|uic=${newLang}`;

        // Set the ASP.NET Core culture cookie
        setCookie('.AspNetCore.Culture', newCulture, 365);

        // Also update localStorage for persistence
        localStorage.setItem('lang', isEn ? 'ar' : 'en');
        localStorage.setItem('dir', isEn ? 'rtl' : 'ltr');

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

// Sidebar handling for mobile
const sidebarToggle = document.getElementById('sidebar-toggle');
const appContainer = document.querySelector('.app-container');

if (sidebarToggle && appContainer) {
    console.log('Sidebar toggle initialized');

    // Check if backdrop already exists to avoid duplicates
    let backdrop = document.querySelector('.sidebar-backdrop');
    if (!backdrop) {
        backdrop = document.createElement('div');
        backdrop.className = 'sidebar-backdrop';
        appContainer.appendChild(backdrop);
    }

    sidebarToggle.addEventListener('click', (e) => {
        e.preventDefault();
        e.stopPropagation();
        console.log('Sidebar toggle clicked');
        appContainer.classList.toggle('sidebar-visible');
    });

    backdrop.addEventListener('click', () => {
        appContainer.classList.remove('sidebar-visible');
    });

    // Close on escape
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            appContainer.classList.remove('sidebar-visible');
        }
    });

    // Close on link click
    document.addEventListener('click', (e) => {
        if (e.target.closest('.sidebar a')) {
            appContainer.classList.remove('sidebar-visible');
        }
    });
}

