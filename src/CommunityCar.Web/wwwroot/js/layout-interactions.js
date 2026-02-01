/**
 * Layout Interactions
 * Handles mobile sidebar, search, and UI synchronization between desktop and mobile elements.
 * Simplified version - no complex desktop sidebar toggle functionality.
 */

document.addEventListener('DOMContentLoaded', function () {
    // Ensure main content is always visible on page load
    const mainContent = document.getElementById('main-content');
    if (mainContent) {
        mainContent.style.opacity = '1';
        mainContent.style.visibility = 'visible';
        mainContent.style.display = 'block';
    }

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

        mobileSearchInput?.addEventListener('input', function (e) {
            // Search logic here or dispatch event
            const query = e.target.value;
            document.dispatchEvent(new CustomEvent('mobile-search', { detail: query }));
        });
    }

    // Unified Escape key handler for all overlays and sidebars
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            // Close mobile search overlay
            if (mobileSearchOverlay && !mobileSearchOverlay.classList.contains('hidden')) {
                mobileSearchOverlay.classList.add('hidden');
            }
            // Close dashboard mobile sidebar
            const dashboardSidebar = document.getElementById('dashboard-sidebar');
            if (dashboardSidebar && !dashboardSidebar.classList.contains('-translate-x-[calc(100%+2rem)]')) {
                // Close using the backdrop click if available
                const sidebarBackdrop = document.getElementById('sidebar-backdrop');
                if (sidebarBackdrop) {
                    sidebarBackdrop.click();
                }
            }
            // NOTE: Main sidebar escape handling is in site.js
        }
    });

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

    // Profile Dropdown
    const profileToggle = document.querySelector('.profile-toggle');
    const profileDropdown = document.querySelector('.profile-dropdown');

    if (profileToggle && profileDropdown) {
        profileToggle.addEventListener('click', function (e) {
            e.stopPropagation();
            // Close other dropdowns first
            document.querySelectorAll('.dropdown-content:not(.profile-dropdown)').forEach(dropdown => {
                dropdown.classList.add('hidden');
            });

            // Check if we're in RTL mode and adjust positioning if needed
            const isRTL = document.documentElement.dir === 'rtl' || document.documentElement.classList.contains('rtl');
            if (isRTL) {
                // Ensure RTL positioning classes are applied
                profileDropdown.classList.add('rtl:right-auto', 'rtl:left-0');
            }

            profileDropdown.classList.toggle('hidden');
        });

        // Close profile dropdown when clicking outside
        document.addEventListener('click', function (e) {
            if (!e.target.closest('.profile-wrapper')) {
                profileDropdown.classList.add('hidden');
            }
        });
    }

    // Mobile Profile Dropdown
    const mobileProfileToggle = document.querySelector('.mobile-profile-toggle');
    const mobileProfileDropdown = document.querySelector('.mobile-profile-dropdown');

    if (mobileProfileToggle && mobileProfileDropdown) {
        mobileProfileToggle.addEventListener('click', function (e) {
            e.stopPropagation();
            // Close other dropdowns first
            document.querySelectorAll('.dropdown-content:not(.mobile-profile-dropdown)').forEach(dropdown => {
                dropdown.classList.add('hidden');
            });

            // Check if we're in RTL mode and adjust positioning if needed
            const isRTL = document.documentElement.dir === 'rtl' || document.documentElement.classList.contains('rtl');
            if (isRTL) {
                // Ensure RTL positioning classes are applied
                mobileProfileDropdown.classList.add('rtl:right-auto', 'rtl:left-0');
            }

            mobileProfileDropdown.classList.toggle('hidden');
        });

        // Close mobile profile dropdown when clicking outside
        document.addEventListener('click', function (e) {
            if (!e.target.closest('.mobile-profile-wrapper')) {
                mobileProfileDropdown.classList.add('hidden');
            }
        });
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

                // Check if we're in RTL mode and adjust positioning if needed
                const isRTL = document.documentElement.dir === 'rtl' || document.documentElement.classList.contains('rtl');
                if (isRTL) {
                    // Ensure RTL positioning classes are applied
                    menu.classList.add('rtl:right-auto', 'rtl:left-0');
                }

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

    // Dashboard Right Sidebar Toggle
    const dashRightToggle = document.getElementById('dashboard-right-sidebar-toggle');
    const dashRightSidebar = document.querySelector('aside.sidebar-right');

    if (dashRightToggle && dashRightSidebar) {
        dashRightToggle.addEventListener('click', function () {
            dashRightSidebar.classList.toggle('hidden');
            dashRightSidebar.classList.toggle('flex');
            dashRightSidebar.classList.toggle('translate-x-0');
        });
    }

    // Mobile Dashboard Sidebar Toggle
    const mobileSidebarToggle = document.getElementById('mobile-sidebar-toggle');
    const mobileSidebarClose = document.getElementById('mobile-sidebar-close');
    const dashboardSidebar = document.getElementById('dashboard-sidebar');
    const sidebarBackdrop = document.getElementById('sidebar-backdrop');

    function openMobileSidebar() {
        if (dashboardSidebar && sidebarBackdrop) {
            dashboardSidebar.classList.remove('-translate-x-[calc(100%+2rem)]');
            dashboardSidebar.classList.add('translate-x-0');
            dashboardSidebar.classList.add('w-56'); // Show full width on mobile
            dashboardSidebar.classList.add('sidebar-mobile-open'); // Add class to show text labels
            sidebarBackdrop.classList.remove('hidden');
            document.body.style.overflow = 'hidden'; // Prevent background scrolling
            document.body.classList.add('sidebar-visible'); // Add class for CSS targeting

            // Mobile spacing is now handled by CSS - no need for JavaScript adjustments
        }
    }

    function closeMobileSidebar() {
        if (dashboardSidebar && sidebarBackdrop) {
            dashboardSidebar.classList.add('-translate-x-[calc(100%+2rem)]');
            dashboardSidebar.classList.remove('translate-x-0');
            dashboardSidebar.classList.remove('w-56');
            dashboardSidebar.classList.remove('sidebar-mobile-open'); // Remove class to hide text labels
            sidebarBackdrop.classList.add('hidden');
            document.body.style.overflow = ''; // Restore scrolling
            document.body.classList.remove('sidebar-visible'); // Remove class

            // Mobile spacing is now handled by CSS - no need for JavaScript adjustments
        }
    }

    if (mobileSidebarToggle) {
        mobileSidebarToggle.addEventListener('click', function () {
            openMobileSidebar();
        });
    }

    if (mobileSidebarClose) {
        mobileSidebarClose.addEventListener('click', function () {
            closeMobileSidebar();
        });
    }

    if (sidebarBackdrop) {
        sidebarBackdrop.addEventListener('click', function () {
            closeMobileSidebar();
        });
    }


    // Close mobile sidebar when clicking on navigation links (mobile only)
    if (dashboardSidebar) {
        const sidebarLinks = dashboardSidebar.querySelectorAll('a, button[type="submit"]');
        sidebarLinks.forEach(link => {
            link.addEventListener('click', function () {
                // Only close on mobile screens
                if (window.innerWidth < 768) {
                    closeMobileSidebar();
                }
            });
        });
    }

    // Handle window resize to ensure proper sidebar state and spacing
    window.addEventListener('resize', function () {
        const mainContent = document.getElementById('main-content');

        if (window.innerWidth >= 768) {
            // On desktop, ensure sidebars are in correct state
            if (dashboardSidebar) {
                closeMobileSidebar();
            }
            // NOTE: Main sidebar close is handled in site.js

            // Reset any mobile-specific spacing - let CSS handle desktop spacing
            if (mainContent) {
                mainContent.style.paddingLeft = '';
                mainContent.style.paddingRight = '';
                mainContent.style.marginLeft = '';
                mainContent.style.marginRight = '';
                // Ensure main content is always visible
                mainContent.style.opacity = '1';
                mainContent.style.visibility = 'visible';
                mainContent.style.display = 'block';
                mainContent.style.pointerEvents = '';
            }

            // Reset body classes and overflow
            document.body.classList.remove('sidebar-visible');
            document.body.style.overflow = '';
        } else {
            // On mobile, ensure main content is visible
            if (mainContent) {
                mainContent.style.opacity = '1';
                mainContent.style.visibility = 'visible';
                mainContent.style.display = 'block';
            }
            // NOTE: Main sidebar state on mobile is handled in site.js
        }
    });

    // NOTE: Main layout sidebar toggle (#sidebar-toggle) is handled in site.js
    // This file only handles dashboard-specific sidebar toggling to avoid conflicts

    // ========================================
    // SIMPLIFIED LAYOUT - NO COMPLEX DESKTOP TOGGLE
    // ========================================
    // 
    // Large screens now use the same simple approach as other screen sizes.
    // No complex state management or margin calculations needed.
    // 
    // ========================================
});
