/**
 * CC.Modules.Post
 * Controller for post creation UI and logic.
 */
(function (CC) {
    class PostController extends CC.Utils.BaseComponent {
        constructor() {
            super('PostController');
            this.service = CC.Services.Post;
        }

        init() {
            if (this.initialized) return;
            super.init();

            this.setupCharacterCounter();
            this.setupPostTypeSelection();
            this.initializeLucide();
        }

        setupCharacterCounter() {
            const contentTextarea = document.getElementById('content');
            const counter = document.getElementById('contentCounter');

            if (contentTextarea && counter) {
                const updateCounter = () => {
                    const current = contentTextarea.value.length;
                    const max = parseInt(contentTextarea.getAttribute('maxlength')) || 5000;
                    counter.textContent = `${current} / ${max}`;

                    if (current > max * 0.9) {
                        counter.classList.add('text-red-500');
                    } else {
                        counter.classList.remove('text-red-500');
                    }
                };

                contentTextarea.addEventListener('input', updateCounter);
                updateCounter(); // Initial count
            }
        }

        setupPostTypeSelection() {
            this.delegate('click', '[data-action="select-post-type"]', (e, target) => {
                const type = target.dataset.type;
                this.selectPostType(type, target);
            });
        }

        selectPostType(type, element) {
            // Remove active state from all options
            document.querySelectorAll('[data-action="select-post-type"]').forEach(el => {
                el.classList.remove('border-primary', 'bg-primary/5');
                el.classList.add('border-border/30');
            });

            // Add active state to selected option
            element.classList.remove('border-border/30');
            element.classList.add('border-primary', 'bg-primary/5');

            // Update placeholders
            const titleInput = document.getElementById('title');
            const contentTextarea = document.getElementById('content');

            if (titleInput && contentTextarea) {
                titleInput.placeholder = element.dataset.placeholderTitle || titleInput.placeholder;
                contentTextarea.placeholder = element.dataset.placeholderContent || contentTextarea.placeholder;
            }

            // Update hidden type field if exists
            const typeInput = document.getElementById('PostType');
            if (typeInput) typeInput.value = type;
        }

        initializeLucide() {
            if (window.lucide) window.lucide.createIcons();
        }
    }

    CC.Modules.Post = new PostController();

    // Legacy support for selectPostType
    window.selectPostType = (type, element) => CC.Modules.Post.selectPostType(type, element);
})(window.CommunityCar);
