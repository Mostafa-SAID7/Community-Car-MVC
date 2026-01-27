/**
 * Interaction Panel Manager
 * Handles reactions, comments, and sharing functionality
 */
class InteractionManager {
    constructor() {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => this.init());
        } else {
            this.init();
        }
    }

    init() {
        this.setupEventHandlers();
    }

    setupEventHandlers() {
        document.body.addEventListener('click', (event) => {
            const target = event.target.closest('[data-action]');
            if (!target) return;

            const action = target.dataset.action;
            const panel = target.closest('.interaction-panel');

            // Interaction context
            const entityId = panel ? panel.dataset.entityId : null;
            const entityType = panel ? panel.dataset.entityType : null;

            switch (action) {
                case 'toggle-reaction':
                    this.toggleReaction(target, entityId, entityType);
                    break;
                case 'submit-reaction':
                    const reactionType = target.dataset.reactionType;
                    this.submitReaction(entityId, entityType, reactionType);
                    break;
                case 'toggle-comments':
                    this.toggleComments(panel);
                    break;
                case 'submit-comment':
                    this.submitComment(panel, entityId, entityType);
                    break;
                case 'cancel-comment':
                    this.toggleComments(panel);
                    break;
                case 'toggle-share':
                    this.toggleShare(target);
                    break;
                case 'share-platform':
                    this.shareToPlatform(target.dataset.platform, target.dataset.url);
                    break;
                case 'copy-link':
                    this.copyLink(target.dataset.url);
                    break;
            }
        });

        // Hover events for reactions and share
        document.body.addEventListener('mouseover', (event) => {
            const target = event.target.closest('[data-hover-action]');
            if (!target) return;

            if (target.dataset.hoverAction === 'show-reaction-options') {
                const optionsId = target.dataset.target;
                const options = document.querySelector(optionsId);
                if (options) options.style.display = 'flex';
            }
        });

        document.body.addEventListener('mouseout', (event) => {
            const target = event.target.closest('[data-hover-action]');
            if (!target) return;

            if (target.dataset.hoverAction === 'show-reaction-options') {
                const optionsId = target.dataset.target;
                const options = document.querySelector(optionsId);
                // logic to check if moving to options... complicated in vanilla JS delegation
                // simplified: CSS hover is often better for menus, but if we stick to JS:
                if (options) options.style.display = 'none';
            }
        });

        // Better approach for menus with JS delegation is often click-toggle or distinct mouseenter/leave on container
        // Refactoring to use click or CSS for menus is preferred, but adapting existing logic:

        document.querySelectorAll('.group/picker').forEach(group => {
            group.addEventListener('mouseenter', () => {
                const options = group.querySelector('.reaction-options');
                if (options) options.style.display = 'flex';
            });
            group.addEventListener('mouseleave', () => {
                const options = group.querySelector('.reaction-options');
                if (options) options.style.display = 'none';
            });
        });
    }

    toggleReaction(button, entityId, entityType) {
        // Toggle default like
        this.submitReaction(entityId, entityType, 'Like');
    }

    async submitReaction(entityId, entityType, reactionType) {
        // Mock API call
        console.log(`Reacting ${reactionType} to ${entityType} ${entityId}`);
        // In real app: fetch POST /api/interactions/react
    }

    toggleComments(panel) {
        const commentSection = panel.querySelector('.comments-section');
        const form = commentSection.querySelector('.comment-form');
        const list = commentSection.querySelector('.comments-list');

        if (form) form.classList.toggle('hidden');
        if (list) list.classList.toggle('hidden');
    }

    async submitComment(panel, entityId, entityType) {
        const textarea = panel.querySelector('.comment-input');
        const content = textarea.value.trim();

        if (!content) return;

        console.log(`Commenting on ${entityType} ${entityId}: ${content}`);

        // Mock success
        textarea.value = '';
        this.toggleComments(panel); // Close or refresh list
        // In real app: fetch POST /api/interactions/comment
    }

    toggleShare(button) {
        // Expecting button to have a sibling or finding via ID
        const options = button.nextElementSibling;
        if (options && options.classList.contains('share-options')) {
            const isHidden = options.classList.contains('hidden') || getComputedStyle(options).display === 'none';
            // Simple toggle for now, matching the inline style logic
            options.style.display = isHidden ? 'flex' : 'none';
        }
    }

    shareToPlatform(platform, url) {
        console.log(`Sharing to ${platform}: ${url}`);
        // Implement sharing logic
    }

    copyLink(url) {
        navigator.clipboard.writeText(url || window.location.href);
        // Show tooltip/toast
        console.log('Link copied');
    }
}

// Global instance
window.interactionManager = new InteractionManager();

// Function expected by existing code
window.initializeInteractionPanel = function () {
    // Re-init if needed or just use the global instance
    console.log('Interaction Panel Initialized');
};