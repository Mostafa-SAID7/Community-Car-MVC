/**
 * Global Skeleton Loading Helper
 * Provides functions to show/hide skeletons on elements
 */

window.Skeleton = {
    /**
     * Shows a skeleton for a given element or container
     * @param {string|HTMLElement} container - The container selector or element to put the skeleton in
     * @param {string} type - The skeleton type ('Post', 'Story', 'Circle', 'Line', 'Card')
     * @param {number} count - How many skeletons to show
     */
    show: function (container, type = 'Post', count = 1) {
        const target = typeof container === 'string' ? document.querySelector(container) : container;
        if (!target) return;

        // Clear target if desired (usually you want to keep the old content and append or replace)
        // For simplicity, we create a temporary template if it doesn't exist

        let template = document.getElementById('global-skeleton-templates');
        if (!template) {
            // Create hidden templates if not present in the DOM
            this.createGlobalTemplates();
            template = document.getElementById('global-skeleton-templates');
        }

        const skeletonSource = template.querySelector(`[data-skeleton-type="${type}"]`);
        if (!skeletonSource) {
            console.error(`Skeleton type "${type}" not found`);
            return;
        }

        const wrapper = document.createElement('div');
        wrapper.className = 'skeleton-wrapper space-y-4 w-full';

        for (let i = 0; i < count; i++) {
            const clone = skeletonSource.cloneNode(true);
            clone.removeAttribute('data-skeleton-type');
            wrapper.appendChild(clone);
        }

        target.appendChild(wrapper);
    },

    /**
     * Removes all skeletons from a container
     * @param {string|HTMLElement} container 
     */
    hide: function (container) {
        const target = typeof container === 'string' ? document.querySelector(container) : container;
        if (!target) return;

        const skeletons = target.querySelectorAll('.skeleton-wrapper');
        skeletons.forEach(s => s.remove());
    },

    /**
     * Intercepts page transitions to show a skeleton (optional experiment)
     */
    initPageTransition: function () {
        window.addEventListener('beforeunload', () => {
            // Optional: show a mini loader or full page skeleton
            // Useful for slower page transitions
        });
    },

    createGlobalTemplates: function () {
        const div = document.createElement('div');
        div.id = 'global-skeleton-templates';
        div.className = 'hidden';

        // We'll populate this via a partial in the layout for maximum flexibility
        // But for now, just an empty container that will be filled by the partial
        document.body.appendChild(div);
    },

    /**
     * Initializes a skeleton-to-content transition on page load
     * @param {string} skeletonId - The ID of the skeleton container
     * @param {string} contentId - The ID of the content container
     * @param {number} delay - Optional delay in ms (default 500ms for smoothness)
     */
    initPageLoad: function (skeletonId, contentId, delay = 500) {
        const skeleton = document.getElementById(skeletonId);
        const content = document.getElementById(contentId);

        if (!skeleton || !content) return;

        // Immediately show skeleton and hide content
        skeleton.classList.remove('hidden');
        content.classList.add('hidden');

        // Wait for window load or fallback to timeout
        const showContent = () => {
            setTimeout(() => {
                skeleton.classList.add('hidden');
                content.classList.remove('hidden');
                // Trigger animations if any
                content.classList.add('animate-in', 'fade-in', 'duration-500');
            }, delay);
        };

        if (document.readyState === 'complete') {
            showContent();
        } else {
            window.addEventListener('load', showContent);
            // Fallback in case load event already fired or hangs
            setTimeout(showContent, 2000);
        }
    }
};

// Initialize on DOM load
document.addEventListener('DOMContentLoaded', () => {
    // Skeleton.initPageTransition();
});
