// Main site entry point
// Most services now self-initialize using the CommunityCar.Services namespace.
// this file is kept for backward compatibility and any page-specific glue code that doesn't fit a service.

document.addEventListener('DOMContentLoaded', () => {
    // Core services (Theme, Sidebar, Cookie, Toaster, Localization) 
    // should be loaded and initialized by now due to their self-init logic.

    // Global Aliases for Backward Compatibility
    if (window.CommunityCar) {
        const CC = window.CommunityCar;
        window.Skeleton = CC.Utils.Skeleton;
        window.ErrorBoundary = CC.Utils.ErrorBoundary;

        if (CC.Config.debug) {
            console.log('Community Car App Initialized');
            console.log('Active Services:', Object.keys(CC.Services));
        }
    }
});
