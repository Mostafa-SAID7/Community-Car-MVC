/**
 * Layout Interactions
 * Handles mobile sidebar, search, and UI synchronization between desktop and mobile elements.
 */
document.addEventListener('DOMContentLoaded', function () {
    // Initialize Lucide icons if present
    if (typeof lucide !== 'undefined') {
        lucide.createIcons();
    }

    // Handle mobile viewport height for CSS --vh variable
    function setMobileViewportHeight() {
        const vh = window.innerHeight * 0.01;
        document.documentElement.style.setProperty('--vh', `${vh}px`);
    }
    setMobileViewportHeight();
    window.addEventListener('resize', setMobileViewportHeight);
    window.addEventListener('orientationchange', setMobileViewportHeight);

    // Mobile Search Overlay
    const mobileSearchToggle = document.getElementById('mobile-search-toggle');
    const mobileSearchOverlay = document.getElementById('mobile-search-overlay');
    const mobileSearchClose = document.getElementById('mobile-search-close');
    const mobileSearchInput = document.getElementById('mobile-search-input');

    if (mobileSearchToggle && mobileSearchOverlay) {
        mobileSearchToggle.addEventListener('click', function () {
            mobileSearchOverlay.classList.remove('hidden');
            setTimeout(() => {
                mobileSearchInput?.focus();
            }, 100);
        });

        mobileSearchClose?.addEventListener('click', function () {
            mobileSearchOverlay.classList.add('hidden');
        });

        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape' && !mobileSearchOverlay.classList.contains('hidden')) {
                mobileSearchOverlay.classList.add('hidden');
            }
        });

        mobileSearchInput?.addEventListener('input', function (e) {
            // Search logic here or dispatch event
            const query = e.target.value;
            document.dispatchEvent(new CustomEvent('mobile-search', { detail: query }));
        });
    }

    // Language Toggle Sync (Mobile to Desktop)
    const mobileLangToggle = document.getElementById('mobile-lang-toggle');
    const desktopLangToggle = document.getElementById('lang-toggle');

    if (mobileLangToggle && desktopLangToggle) {
        mobileLangToggle.addEventListener('click', function () {
            desktopLangToggle.click();
        });

        // Initial sync of text
        mobileLangToggle.textContent = desktopLangToggle.textContent;

        // Observer to keep them in sync if one changes
        const langObserver = new MutationObserver(function (mutations) {
            mutations.forEach(function (mutation) {
                if (mutation.target === desktopLangToggle) {
                    mobileLangToggle.textContent = desktopLangToggle.textContent;
                }
            });
        });
        langObserver.observe(desktopLangToggle, { childList: true });
    }

    // Notification Sync (Mobile to Desktop)
    const mobileNotificationBell = document.querySelector('.mobile-notification-bell');
    const mobileNotificationDropdown = document.querySelector('.mobile-notification-dropdown');
    const desktopNotificationBell = document.querySelector('.notification-bell');

    if (mobileNotificationBell && desktopNotificationBell) {
        mobileNotificationBell.addEventListener('click', function () {
            if (mobileNotificationDropdown) {
                mobileNotificationDropdown.classList.toggle('hidden');
            }
        });

        const desktopBadge = document.querySelector('.notification-badge');
        const mobileBadge = document.querySelector('.mobile-notification-badge');

        if (desktopBadge && mobileBadge) {
            const syncBadge = () => {
                mobileBadge.style.display = desktopBadge.style.display;
                mobileBadge.textContent = desktopBadge.textContent;
            };

            const badgeObserver = new MutationObserver(function (mutations) {
                mutations.forEach(() => syncBadge());
            });

            badgeObserver.observe(desktopBadge, { attributes: true, childList: true, subtree: true });
            syncBadge(); // Initial sync
        }
    }

    // Global Skeleton Handlers for common patterns
    const skeletonConfig = [
        { id: 'profile-skeleton', container: 'profile-content' },
        { id: 'loading-skeleton', container: 'main-content' },
        { id: 'dashboard-skeleton', container: 'dashboard-content' }
    ];

    if (typeof Skeleton !== 'undefined') {
        skeletonConfig.forEach(config => {
            const skeletonEl = document.getElementById(config.id);
            const containerEl = document.getElementById(config.container);
            if (skeletonEl && containerEl) {
                Skeleton.initPageLoad(config.id, config.container);
            }
        });
    }

    // Global Dropdown Handler
    document.addEventListener('click', function (e) {
        // Toggle Dropdown
        const toggleBtn = e.target.closest('[data-action="toggle-dropdown"]');
        if (toggleBtn) {
            e.preventDefault();
            const parent = toggleBtn.closest('.relative');
            const menu = parent?.querySelector('.dropdown-content');
            if (menu) {
                // Close other open dropdowns first
                document.querySelectorAll('.dropdown-content:not(.hidden)').forEach(m => {
                    if (m !== menu) m.classList.add('hidden');
                });
                menu.classList.toggle('hidden');
            }
            return;
        }

        // Close dropdowns when clicking outside
        if (!e.target.closest('.dropdown-content')) {
            document.querySelectorAll('.dropdown-content:not(.hidden)').forEach(menu => {
                menu.classList.add('hidden');
            });
        }
    });
});
