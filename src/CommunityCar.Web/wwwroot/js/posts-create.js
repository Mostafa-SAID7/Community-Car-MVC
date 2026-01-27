function selectPostType(type, element) {
    // Remove active state from all options
    document.querySelectorAll('[onclick*="selectPostType"]').forEach(el => {
        el.classList.remove('border-primary', 'bg-primary/5');
        el.classList.add('border-border/30');
    });

    // Add active state to selected option
    element.classList.remove('border-border/30');
    element.classList.add('border-primary', 'bg-primary/5');

    // Update placeholder text based on type
    const contentTextarea = document.getElementById('content');
    const titleInput = document.getElementById('title');

    if (!contentTextarea || !titleInput) return;

    // These placeholders should ideally be passed from the view via data-attributes
    // or we can use the ones currently in the inputs if we don't want to break localization easily.
    // For now, we'll keep the logic but rely on data-attributes for full localization support if needed.
    const placeholders = {
        'Text': {
            title: titleInput.getAttribute('data-placeholder-text') || 'Give your broadcast a compelling title...',
            content: contentTextarea.getAttribute('data-placeholder-text') || 'What\'s on your mind? Share your thoughts with the community...'
        },
        'Image': {
            title: titleInput.getAttribute('data-placeholder-image') || 'Describe your photo...',
            content: contentTextarea.getAttribute('data-placeholder-image') || 'Add some context to your photo...'
        },
        'Link': {
            title: titleInput.getAttribute('data-placeholder-link') || 'Enter the link title...',
            content: contentTextarea.getAttribute('data-placeholder-link') || 'Share why this link is interesting...'
        },
        'Poll': {
            title: titleInput.getAttribute('data-placeholder-poll') || 'Ask a question...',
            content: contentTextarea.getAttribute('data-placeholder-poll') || 'Explain what this poll is about...'
        }
    };

    const currentPlaceholders = placeholders[type] || placeholders['Text'];
    titleInput.placeholder = currentPlaceholders.title;
    contentTextarea.placeholder = currentPlaceholders.content;
}

document.addEventListener('DOMContentLoaded', function () {
    // Character counter
    const contentTextarea = document.getElementById('content');
    const counter = document.getElementById('contentCounter');

    if (contentTextarea && counter) {
        contentTextarea.addEventListener('input', function () {
            const current = this.value.length;
            const max = parseInt(this.getAttribute('maxlength')) || 5000;
            counter.textContent = `${current} / ${max}`;

            if (current > max * 0.9) {
                counter.classList.add('text-red-500');
            } else {
                counter.classList.remove('text-red-500');
            }
        });

        // Initial count
        const current = contentTextarea.value.length;
        const max = parseInt(contentTextarea.getAttribute('maxlength')) || 5000;
        counter.textContent = `${current} / ${max}`;
    }

    // Initialize Lucide icons
    if (typeof lucide !== 'undefined') {
        lucide.createIcons();
    }
});

// Make globally accessible for onclick handlers
window.selectPostType = selectPostType;
