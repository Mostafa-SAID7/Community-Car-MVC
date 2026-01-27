function initializeCreateStory() {
    const mediaUploadArea = document.getElementById('mediaUploadArea');
    const mediaInput = document.getElementById('mediaInput');
    const uploadPlaceholder = document.getElementById('uploadPlaceholder');
    const mediaPreview = document.getElementById('mediaPreview');
    const imagePreview = document.getElementById('imagePreview');
    const videoPreview = document.getElementById('videoPreview');
    const removeMedia = document.getElementById('removeMedia');
    const addLocation = document.getElementById('addLocation');
    const locationFields = document.getElementById('locationFields');
    const getCurrentLocation = document.getElementById('getCurrentLocation');
    const tagsInput = document.getElementById('tagsInput');
    const tagsHidden = document.getElementById('tagsHidden');
    const tagsList = document.getElementById('tagsList');

    // Media upload functionality
    mediaUploadArea?.addEventListener('click', () => mediaInput?.click());
    mediaUploadArea?.addEventListener('dragover', handleDragOver);
    mediaUploadArea?.addEventListener('drop', handleDrop);
    mediaInput?.addEventListener('change', handleFileSelect);
    removeMedia?.addEventListener('click', clearMedia);

    // Location functionality
    addLocation?.addEventListener('change', function () {
        if (this.checked) {
            locationFields?.classList.remove('hidden');
        } else {
            locationFields?.classList.add('hidden');
        }
    });

    getCurrentLocation?.addEventListener('click', function () {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (position) {
                const latInput = document.querySelector('input[name="Latitude"]');
                const lonInput = document.querySelector('input[name="Longitude"]');
                if (latInput) latInput.value = position.coords.latitude;
                if (lonInput) lonInput.value = position.coords.longitude;
            });
        }
    });

    // Tags functionality
    tagsInput?.addEventListener('keydown', function (e) {
        if (e.key === 'Enter' || e.key === ',') {
            e.preventDefault();
            addTag(this.value.trim());
            this.value = '';
        }
    });

    tagsInput?.addEventListener('blur', function () {
        if (this.value.trim()) {
            addTag(this.value.trim());
            this.value = '';
        }
    });

    let tags = [];

    function addTag(tagText) {
        if (tagText && !tags.includes(tagText)) {
            tags.push(tagText);
            updateTagsDisplay();
            updateTagsHidden();
        }
    }

    function removeTag(tagText) {
        tags = tags.filter(tag => tag !== tagText);
        updateTagsDisplay();
        updateTagsHidden();
    }

    function updateTagsDisplay() {
        if (!tagsList) return;
        tagsList.innerHTML = '';
        tags.forEach(tag => {
            const tagElement = document.createElement('span');
            tagElement.className = 'inline-flex items-center gap-1 px-3 py-1 bg-primary/10 text-primary rounded-full text-sm';
            tagElement.innerHTML = `
                #${tag}
                <button type="button" class="w-4 h-4 hover:bg-primary/20 rounded-full flex items-center justify-center remove-tag-btn" data-tag="${tag}">
                    <i data-lucide="x" class="w-3 h-3"></i>
                </button>
            `;
            tagsList.appendChild(tagElement);
        });

        // Re-initialize Lucide for new icons
        if (typeof lucide !== 'undefined') {
            lucide.createIcons({
                root: tagsList
            });
        }

        // Add event listeners to remove buttons
        tagsList.querySelectorAll('.remove-tag-btn').forEach(btn => {
            btn.addEventListener('click', function () {
                removeTag(this.getAttribute('data-tag'));
            });
        });
    }

    function updateTagsHidden() {
        if (tagsHidden) {
            tagsHidden.value = tags.join(',');
        }
    }

    // Make removeTag globally accessible if needed, but preferred to use data-attributes
    window.removeTag = removeTag;

    function handleDragOver(e) {
        e.preventDefault();
        e.stopPropagation();
        mediaUploadArea.classList.add('border-primary');
    }

    function handleDrop(e) {
        e.preventDefault();
        e.stopPropagation();
        mediaUploadArea.classList.remove('border-primary');

        const files = e.dataTransfer.files;
        if (files.length > 0) {
            handleFile(files[0]);
        }
    }

    function handleFileSelect(e) {
        const file = e.target.files[0];
        if (file) {
            handleFile(file);
        }
    }

    function handleFile(file) {
        if (!file.type.startsWith('image/') && !file.type.startsWith('video/')) {
            if (window.toaster) window.toaster.error('Please select an image or video file.');
            else alert('Please select an image or video file.');
            return;
        }

        if (file.size > 50 * 1024 * 1024) { // 50MB limit
            if (window.toaster) window.toaster.error('File size must be less than 50MB.');
            else alert('File size must be less than 50MB.');
            return;
        }

        const reader = new FileReader();
        reader.onload = function (e) {
            uploadPlaceholder.classList.add('hidden');
            mediaPreview.classList.remove('hidden');

            const typeSelect = document.querySelector('select[name="Type"]');
            if (file.type.startsWith('image/')) {
                imagePreview.src = e.target.result;
                imagePreview.classList.remove('hidden');
                videoPreview.classList.add('hidden');
                if (typeSelect) typeSelect.value = 'Photo';
            } else if (file.type.startsWith('video/')) {
                videoPreview.src = e.target.result;
                videoPreview.classList.remove('hidden');
                imagePreview.classList.add('hidden');
                if (typeSelect) typeSelect.value = 'Video';
            }
        };
        reader.readAsDataURL(file);
    }

    function clearMedia() {
        mediaInput.value = '';
        uploadPlaceholder.classList.remove('hidden');
        mediaPreview.classList.add('hidden');
        imagePreview.classList.add('hidden');
        videoPreview.classList.add('hidden');
        imagePreview.src = '';
        videoPreview.src = '';
    }
}

document.addEventListener('DOMContentLoaded', function () {
    initializeCreateStory();
});
