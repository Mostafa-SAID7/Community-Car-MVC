// Gallery Management JavaScript
class GalleryManager {
    constructor() {
        this.currentView = 'grid';
        this.tags = [];
        this.currentFilter = {
            mediaType: '',
            visibility: ''
        };

        this.init();
    }

    init() {
        this.bindEvents();
        this.initializeFilters();
        this.initializeDragAndDrop();

        // Initialize skeleton loading
        if (typeof Skeleton !== 'undefined') {
            Skeleton.initPageLoad('gallery-skeleton', 'galleryContainer');
        }
    }

    bindEvents() {
        // Event delegation for all data-action buttons
        document.body.addEventListener('click', (event) => {
            const target = event.target.closest('[data-action]');
            if (!target) return;

            const action = target.dataset.action;
            const itemId = target.dataset.itemId;

            switch (action) {
                case 'open-upload':
                    this.openUploadModal();
                    break;
                case 'close-upload':
                    this.closeUploadModal();
                    break;
                case 'toggle-view':
                    this.toggleView(target.dataset.view);
                    break;
                case 'open-media':
                    this.openMediaModal(itemId);
                    break;
                case 'toggle-visibility':
                    this.toggleItemVisibility(itemId);
                    break;
                case 'edit-item':
                    this.editItem(itemId);
                    break;
                case 'delete-item':
                    this.deleteItem(itemId);
                    break;
                case 'remove-tag':
                    this.removeTag(target.dataset.tag);
                    break;
            }
        });

        // Upload area click to trigger file input
        const uploadArea = document.getElementById('uploadArea');
        if (uploadArea) {
            uploadArea.addEventListener('click', () => {
                const fileInput = document.getElementById('mediaFile');
                if (fileInput) fileInput.click();
            });
        }

        // Filter dropdowns
        document.getElementById('mediaTypeFilter')?.addEventListener('change', () => this.applyFilters());
        document.getElementById('visibilityFilter')?.addEventListener('change', () => this.applyFilters());

        // Upload modal backdrop click
        document.getElementById('uploadModal')?.addEventListener('click', (e) => {
            if (e.target.id === 'uploadModal') {
                this.closeUploadModal();
            }
        });

        // Tags input
        const tagsInput = document.getElementById('tagsInput');
        if (tagsInput) {
            tagsInput.addEventListener('keypress', (e) => {
                if (e.key === 'Enter' || e.key === ',') {
                    e.preventDefault();
                    this.addTag(e.target.value.trim());
                    e.target.value = '';
                }
            });
        }

        // File input
        const fileInput = document.getElementById('mediaFile');
        if (fileInput) {
            fileInput.addEventListener('change', (e) => this.handleFileSelect(e.target));
        }
    }

    initializeFilters() {
        this.applyFilters();
    }

    initializeDragAndDrop() {
        const uploadArea = document.getElementById('uploadArea');
        if (!uploadArea) return;

        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            uploadArea.addEventListener(eventName, this.preventDefaults, false);
        });

        ['dragenter', 'dragover'].forEach(eventName => {
            uploadArea.addEventListener(eventName, () => uploadArea.classList.add('drag-over'), false);
        });

        ['dragleave', 'drop'].forEach(eventName => {
            uploadArea.addEventListener(eventName, () => uploadArea.classList.remove('drag-over'), false);
        });

        uploadArea.addEventListener('drop', (e) => this.handleDrop(e), false);
    }

    preventDefaults(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    handleDrop(e) {
        const files = e.dataTransfer.files;
        if (files.length > 0) {
            const fileInput = document.getElementById('mediaFile');
            if (fileInput) {
                fileInput.files = files;
                this.handleFileSelect(fileInput);
            }
        }
    }

    toggleView(view) {
        this.currentView = view;
        const container = document.getElementById('galleryContainer');
        const gridBtn = document.getElementById('gridViewBtn');
        const listBtn = document.getElementById('listViewBtn');

        if (!container || !gridBtn || !listBtn) return;

        if (view === 'grid') {
            container.className = 'grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6';
            gridBtn.className = 'p-2 bg-primary text-primary-foreground rounded-lg transition-all';
            listBtn.className = 'p-2 bg-background border border-border text-muted-foreground hover:text-foreground rounded-lg transition-all';
        } else {
            container.className = 'space-y-4';
            gridBtn.className = 'p-2 bg-background border border-border text-muted-foreground hover:text-foreground rounded-lg transition-all';
            listBtn.className = 'p-2 bg-primary text-primary-foreground rounded-lg transition-all';

            // Update items for list view
            this.updateListView();
        }
    }

    updateListView() {
        const items = document.querySelectorAll('.gallery-item');
        items.forEach(item => {
            if (this.currentView === 'list') {
                item.classList.add('flex', 'flex-row', 'items-center');
                item.classList.remove('flex-col');

                const mediaContainer = item.querySelector('.aspect-square');
                if (mediaContainer) {
                    mediaContainer.classList.remove('aspect-square');
                    mediaContainer.classList.add('w-24', 'h-24', 'flex-shrink-0');
                }
            } else {
                item.classList.remove('flex', 'flex-row', 'items-center');
                item.classList.add('flex-col');

                const mediaContainer = item.querySelector('.w-24');
                if (mediaContainer) {
                    mediaContainer.classList.add('aspect-square');
                    mediaContainer.classList.remove('w-24', 'h-24', 'flex-shrink-0');
                }
            }
        });
    }

    applyFilters() {
        const mediaTypeFilter = document.getElementById('mediaTypeFilter');
        const visibilityFilter = document.getElementById('visibilityFilter');

        if (!mediaTypeFilter || !visibilityFilter) return;

        this.currentFilter.mediaType = mediaTypeFilter.value;
        this.currentFilter.visibility = visibilityFilter.value;

        const items = document.querySelectorAll('.gallery-item');

        items.forEach(item => {
            let show = true;

            if (this.currentFilter.mediaType && item.dataset.mediaType !== this.currentFilter.mediaType) {
                show = false;
            }

            if (this.currentFilter.visibility && item.dataset.isPublic !== this.currentFilter.visibility) {
                show = false;
            }

            if (show) {
                item.classList.remove('hidden');
                item.classList.add('block');
            } else {
                item.classList.add('hidden');
                item.classList.remove('block');
            }
        });

        this.updateEmptyState();
    }

    updateEmptyState() {
        const items = document.querySelectorAll('.gallery-item');
        const visibleItems = Array.from(items).filter(item => !item.classList.contains('hidden'));
        const emptyState = document.querySelector('.col-span-full');

        if (emptyState) {
            if (visibleItems.length === 0) {
                emptyState.classList.remove('hidden');
                emptyState.classList.add('block');
            } else {
                emptyState.classList.add('hidden');
                emptyState.classList.remove('block');
            }
        }
    }

    openUploadModal() {
        const modal = document.getElementById('uploadModal');
        if (modal) {
            modal.classList.remove('hidden');
            modal.classList.add('flex');
        }
    }

    closeUploadModal() {
        const modal = document.getElementById('uploadModal');
        if (modal) {
            modal.classList.add('hidden');
            modal.classList.remove('flex');

            // Reset form
            const form = modal.querySelector('form');
            if (form) {
                form.reset();
            }

            // Reset file preview
            const filePreview = document.getElementById('filePreview');
            const uploadArea = document.getElementById('uploadArea');

            if (filePreview && uploadArea) {
                filePreview.classList.add('hidden');
                uploadArea.classList.remove('hidden');
            }

            // Reset tags
            this.tags = [];
            this.updateTagsDisplay();
        }
    }

    handleFileSelect(input) {
        const file = input.files[0];
        if (!file) return;

        const preview = document.getElementById('filePreview');
        const uploadArea = document.getElementById('uploadArea');

        if (!preview || !uploadArea) return;

        preview.innerHTML = '';

        if (file.type.startsWith('image/')) {
            const img = document.createElement('img');
            img.src = URL.createObjectURL(file);
            img.className = 'max-w-full h-32 object-cover rounded-lg mx-auto';
            preview.appendChild(img);
        } else if (file.type.startsWith('video/')) {
            const video = document.createElement('video');
            video.src = URL.createObjectURL(file);
            video.className = 'max-w-full h-32 object-cover rounded-lg mx-auto';
            video.controls = true;
            preview.appendChild(video);
        } else {
            const icon = document.createElement('div');
            icon.className = 'flex items-center justify-center gap-2 text-foreground';
            icon.innerHTML = `<i data-lucide="file" class="w-8 h-8"></i><span>${file.name}</span>`;
            preview.appendChild(icon);
        }

        const fileName = document.createElement('p');
        fileName.textContent = file.name;
        fileName.className = 'text-sm text-muted-foreground mt-2';
        preview.appendChild(fileName);

        uploadArea.classList.add('hidden');
        preview.classList.remove('hidden');

        // Re-initialize Lucide icons
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }
    }

    addTag(tag) {
        if (tag && !this.tags.includes(tag) && this.tags.length < 10) {
            this.tags.push(tag);
            this.updateTagsDisplay();
        }
    }

    removeTag(tag) {
        this.tags = this.tags.filter(t => t !== tag);
        this.updateTagsDisplay();
    }

    updateTagsDisplay() {
        const container = document.getElementById('tagsList');
        const hiddenInput = document.getElementById('tagsHidden');

        if (!container || !hiddenInput) return;

        container.innerHTML = '';
        hiddenInput.value = JSON.stringify(this.tags);

        this.tags.forEach(tag => {
            const tagElement = document.createElement('span');
            tagElement.className = 'bg-primary/10 text-primary text-sm font-medium px-3 py-1 rounded-full flex items-center gap-2';
            tagElement.innerHTML = `
                #${tag}
                <button type="button" data-action="remove-tag" data-tag="${tag}" class="text-primary hover:text-primary/70">
                    <i data-lucide="x" class="w-3 h-3"></i>
                </button>
            `;
            container.appendChild(tagElement);
        });

        // Re-initialize Lucide icons
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }
    }

    async openMediaModal(itemId) {
        try {
            const response = await fetch(`/profile/gallery/item/${itemId}`);
            const result = await response.json();

            if (result.success) {
                this.showMediaModal(result.data);
            } else {
                console.error('Failed to load media item');
            }
        } catch (error) {
            console.error('Error loading media item:', error);
        }
    }

    showMediaModal(item) {
        // Create and show media modal
        const modal = document.createElement('div');
        modal.className = 'fixed inset-0 bg-black/80 backdrop-blur-sm z-50 flex items-center justify-center p-4';
        modal.innerHTML = `
            <div class="bg-background border border-border rounded-2xl max-w-4xl w-full max-h-[90vh] overflow-hidden">
                <div class="p-4 border-b border-border flex items-center justify-between">
                    <h2 class="text-xl font-bold text-foreground">${item.title}</h2>
                    <button onclick="this.closest('.fixed').remove()" class="text-muted-foreground hover:text-foreground transition-colors">
                        <i data-lucide="x" class="w-6 h-6"></i>
                    </button>
                </div>
                <div class="p-6">
                    <div class="aspect-video bg-muted rounded-lg overflow-hidden mb-4">
                        ${item.mediaType === 1 ?
                `<img src="${item.mediaUrl}" alt="${item.title}" class="w-full h-full object-contain" />` :
                item.mediaType === 2 ?
                    `<video src="${item.mediaUrl}" controls class="w-full h-full"></video>` :
                    `<audio src="${item.mediaUrl}" controls class="w-full"></audio>`
            }
                    </div>
                    ${item.description ? `<p class="text-muted-foreground mb-4">${item.description}</p>` : ''}
                    <div class="flex items-center justify-between text-sm text-muted-foreground">
                        <div class="flex items-center gap-4">
                            <span class="flex items-center gap-1">
                                <i data-lucide="eye" class="w-4 h-4"></i>
                                ${item.viewCount}
                            </span>
                            <span class="flex items-center gap-1">
                                <i data-lucide="heart" class="w-4 h-4"></i>
                                ${item.likeCount}
                            </span>
                        </div>
                        <span>${item.timeAgo}</span>
                    </div>
                </div>
            </div>
        `;

        document.body.appendChild(modal);

        // Re-initialize Lucide icons
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }

        // Close on backdrop click
        modal.addEventListener('click', (e) => {
            if (e.target === modal) {
                modal.remove();
            }
        });
    }

    async toggleItemVisibility(itemId) {
        try {
            const response = await fetch(`/profile/gallery/toggle-visibility/${itemId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                }
            });

            const result = await response.json();

            if (result.success) {
                // Update UI
                const item = document.querySelector(`[onclick*="${itemId}"]`)?.closest('.gallery-item');
                if (item) {
                    const isPublic = item.dataset.isPublic === 'true';
                    item.dataset.isPublic = (!isPublic).toString();

                    // Update visibility icon
                    const visibilityIcon = item.querySelector('[data-lucide="globe"], [data-lucide="lock"]');
                    if (visibilityIcon) {
                        visibilityIcon.setAttribute('data-lucide', isPublic ? 'lock' : 'globe');
                        if (typeof lucide !== 'undefined') {
                            lucide.createIcons();
                        }
                    }
                }

                this.showToast(isPublic ? 'Item made private' : 'Item made public', 'success');
            } else {
                this.showToast('Failed to update visibility', 'error');
            }
        } catch (error) {
            console.error('Error toggling visibility:', error);
            this.showToast('Failed to update visibility', 'error');
        }
    }

    async deleteItem(itemId) {
        if (window.AlertModal) {
            window.AlertModal.confirm('Are you sure you want to delete this item? This action cannot be undone.', 'Delete Item', (confirmed) => {
                if (confirmed) {
                    this.executeDeleteItem(itemId);
                }
            });
        } else if (confirm('Are you sure you want to delete this item? This action cannot be undone.')) {
            this.executeDeleteItem(itemId);
        }
    }

    async executeDeleteItem(itemId) {
        try {
            const response = await fetch(`/profile/gallery/delete/${itemId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                }
            });

            const result = await response.json();

            if (result.success) {
                // Remove item from UI
                const item = document.querySelector(`[onclick*="${itemId}"]`)?.closest('.gallery-item');
                if (item) {
                    item.remove();
                }

                this.updateEmptyState();
                this.showToast('Item deleted successfully', 'success');
            } else {
                this.showToast('Failed to delete item', 'error');
            }
        } catch (error) {
            console.error('Error deleting item:', error);
            this.showToast('Failed to delete item', 'error');
        }
    }

    editItem(itemId) {
        // Implementation for editing item
        console.log('Editing item:', itemId);
        this.showToast('Edit functionality coming soon!', 'info');
    }

    showToast(message, type = 'info') {
        const toast = document.createElement('div');
        toast.className = `fixed top-4 right-4 z-50 px-6 py-3 rounded-lg font-medium text-white transition-all transform translate-x-full opacity-0 ${type === 'success' ? 'bg-green-500' :
            type === 'error' ? 'bg-red-500' :
                type === 'warning' ? 'bg-yellow-500' :
                    'bg-blue-500'
            }`;
        toast.textContent = message;

        document.body.appendChild(toast);

        // Animate in
        setTimeout(() => {
            toast.classList.remove('translate-x-full', 'opacity-0');
        }, 100);

        // Animate out and remove
        setTimeout(() => {
            toast.classList.add('translate-x-full', 'opacity-0');
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    }
}


// Initialize gallery manager when DOM is loaded
document.addEventListener('DOMContentLoaded', function () {
    window.gallery = new GalleryManager();

    // Initialize Lucide icons
    if (typeof lucide !== 'undefined') {
        lucide.createIcons();
    }
});