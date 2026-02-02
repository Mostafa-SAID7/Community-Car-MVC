/**
 * Gallery Controller
 * Manages gallery UI, upload previews, filtering, and media modals.
 */
class GalleryController extends CC.Utils.BaseComponent {
    constructor() {
        super('GalleryController');
        this.service = CC.Services.Gallery;
    }

    init() {
        this.bindEvents();
        this.setupSubscriptions();
        this.initDropzone();
    }

    bindEvents() {
        // Filter actions
        this.delegate('click', '[data-gallery-filter]', (e, target) => {
            const filter = target.dataset.galleryFilter;
            this.handleFilter(filter, target);
        });

        // Media actions
        this.delegate('click', '[data-action="delete-media"]', async (e, target) => {
            const id = target.dataset.id;
            await this.handleDeleteMedia(id, target);
        });

        this.delegate('click', '[data-action="toggle-visibility"]', async (e, target) => {
            const id = target.dataset.id;
            await this.handleToggleVisibility(id, target);
        });

        // View toggle
        this.delegate('click', '[data-action="toggle-gallery-view"]', (e, target) => {
            const view = target.dataset.view;
            this.toggleView(view, target);
        });

        // Media Modal
        this.delegate('click', '[data-action="view-media"]', (e, target) => {
            const mediaUrl = target.dataset.url;
            const mediaType = target.dataset.type;
            this.openMediaModal(mediaUrl, mediaType);
        });
    }

    setupSubscriptions() {
        this.service.on('media:uploaded', () => {
            CC.Utils.Notifier.success('Media uploaded successfully');
            location.reload(); // Simple refresh for now
        });

        this.service.on('media:deleted', (id) => {
            const item = document.querySelector(`[data-media-item="${id}"]`);
            if (item) {
                item.classList.add('fade-out');
                setTimeout(() => item.remove(), 300);
            }
        });
    }

    initDropzone() {
        const dropzone = document.getElementById('gallery-dropzone');
        if (!dropzone) return;

        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            dropzone.addEventListener(eventName, e => {
                e.preventDefault();
                e.stopPropagation();
            }, false);
        });

        dropzone.addEventListener('drop', (e) => {
            const files = Array.from(e.dataTransfer.files);
            this.handleFiles(files);
        });

        const fileInput = dropzone.querySelector('input[type="file"]');
        if (fileInput) {
            fileInput.addEventListener('change', (e) => {
                const files = Array.from(e.target.files);
                this.handleFiles(files);
            });
        }
    }

    async handleFiles(files) {
        if (files.length === 0) return;

        const validFiles = files.filter(f => f.size < 5 * 1024 * 1024); // 5MB limit as per view
        if (validFiles.length < files.length) {
            CC.Utils.Notifier.warning('Some files were skipped because they exceed the 5MB limit.');
        }

        try {
            CC.Utils.Notifier.info(`Uploading ${validFiles.length} files...`);
            // Custom upload logic if needed, or use service
            const formData = new FormData();
            validFiles.forEach(file => formData.append('file', file)); // The view used 'file' not 'files'

            const response = await this.service.uploadMedia(validFiles, { useSingleFileField: true });
            if (response.success) {
                CC.Utils.Notifier.success('Upload successful');
                location.reload();
            }
        } catch (error) {
            CC.Utils.Notifier.error('Failed to upload files.');
        }
    }

    async setAsProfilePicture(id) {
        try {
            const response = await this.service.post(`/profile/gallery/set-profile-picture/${id}`);
            if (response.success) {
                CC.Utils.Notifier.success('Profile picture updated');
                // Refresh images
                document.querySelectorAll('.profile-avatar img').forEach(img => {
                    const src = img.src.split('?')[0];
                    img.src = `${src}?t=${Date.now()}`;
                });
            }
        } catch (error) {
            CC.Utils.Notifier.error('Failed to set profile picture');
        }
    }

    async handleDeleteMedia(id, button) {
        if (await this.confirm('Are you sure you want to delete this media?')) {
            this.service.deleteMedia(id);
        }
    }

    async handleToggleVisibility(id, button) {
        const result = await this.service.toggleVisibility(id);
        if (result.success) {
            const icon = button.querySelector('[data-lucide]');
            if (icon) {
                icon.setAttribute('data-lucide', result.isPublic ? 'eye' : 'eye-off');
                if (window.lucide) window.lucide.createIcons({ parent: button });
            }
            CC.Utils.Notifier.success(`Visibility set to ${result.isPublic ? 'Public' : 'Private'}`);
        }
    }

    handleFilter(filter, button) {
        // UI logic for filtering (showing/hiding items locally or reloading)
        const items = document.querySelectorAll('[data-media-item]');
        items.forEach(item => {
            if (filter === 'all' || item.dataset.type === filter) {
                item.classList.remove('hidden');
            } else {
                item.classList.add('hidden');
            }
        });

        // Update active class on buttons
        const filterButtons = document.querySelectorAll('[data-gallery-filter]');
        filterButtons.forEach(btn => btn.classList.remove('active', 'bg-primary', 'text-white'));
        button.classList.add('active', 'bg-primary', 'text-white');
    }

    toggleView(view, button) {
        const container = document.getElementById('gallery-container');
        if (!container) return;

        if (view === 'grid') {
            container.classList.remove('list-view');
            container.classList.add('grid-view');
        } else {
            container.classList.remove('grid-view');
            container.classList.add('list-view');
        }

        const viewButtons = document.querySelectorAll('[data-action="toggle-gallery-view"]');
        viewButtons.forEach(btn => btn.classList.remove('bg-muted'));
        button.classList.add('bg-muted');
    }

    openMediaModal(url, type) {
        // Assuming a global modal system or simple alert for now
        // In a real app, this would trigger a lightbox/modal component
        console.log(`Opening ${type} media: ${url}`);
    }

    async confirm(message) {
        if (window.AlertModal) {
            return await new Promise(resolve => window.AlertModal.confirm(message, 'Confirm Action', resolve));
        }
        return confirm(message);
    }
}

CC.Modules.Gallery = new GalleryController();
