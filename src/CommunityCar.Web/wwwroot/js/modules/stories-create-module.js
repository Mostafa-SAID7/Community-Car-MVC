/**
 * Stories Create Controller
 * Handles story creation UI, media previews, tags, and location.
 */
(function (CC) {
    class StoriesCreateController extends CC.Utils.BaseComponent {
        constructor() {
            super('StoriesCreate');
            this.service = CC.Services.Stories;
            this.tags = [];
        }

        init() {
            if (this.initialized) return;
            super.init();

            this.setupMediaUpload();
            this.setupLocation();
            this.setupTags();
        }

        setupMediaUpload() {
            const area = document.getElementById('mediaUploadArea');
            const input = document.getElementById('mediaInput');
            const removeBtn = document.getElementById('removeMedia');

            if (!area || !input) return;

            area.addEventListener('click', () => input.click());
            area.addEventListener('dragover', (e) => {
                e.preventDefault();
                area.classList.add('border-primary');
            });
            area.addEventListener('dragleave', () => area.classList.remove('border-primary'));
            area.addEventListener('drop', (e) => {
                e.preventDefault();
                area.classList.remove('border-primary');
                if (e.dataTransfer.files.length) this.handleFile(e.dataTransfer.files[0]);
            });

            input.addEventListener('change', (e) => {
                if (e.target.files.length) this.handleFile(e.target.files[0]);
            });

            removeBtn?.addEventListener('click', () => this.clearMedia());
        }

        handleFile(file) {
            if (!file.type.startsWith('image/') && !file.type.startsWith('video/')) {
                CC.Services.Toaster?.error('Please select an image or video file.');
                return;
            }

            if (file.size > 50 * 1024 * 1024) {
                CC.Services.Toaster?.error('File size must be less than 50MB.');
                return;
            }

            const reader = new FileReader();
            reader.onload = (e) => {
                document.getElementById('uploadPlaceholder')?.classList.add('hidden');
                document.getElementById('mediaPreview')?.classList.remove('hidden');

                const imagePreview = document.getElementById('imagePreview');
                const videoPreview = document.getElementById('videoPreview');
                const typeSelect = document.querySelector('select[name="Type"]');

                if (file.type.startsWith('image/')) {
                    if (imagePreview) {
                        imagePreview.src = e.target.result;
                        imagePreview.classList.remove('hidden');
                    }
                    videoPreview?.classList.add('hidden');
                    if (typeSelect) typeSelect.value = 'Photo';
                } else {
                    if (videoPreview) {
                        videoPreview.src = e.target.result;
                        videoPreview.classList.remove('hidden');
                    }
                    imagePreview?.classList.add('hidden');
                    if (typeSelect) typeSelect.value = 'Video';
                }
            };
            reader.readAsDataURL(file);
        }

        clearMedia() {
            const input = document.getElementById('mediaInput');
            if (input) input.value = '';
            document.getElementById('uploadPlaceholder')?.classList.remove('hidden');
            document.getElementById('mediaPreview')?.classList.add('hidden');
        }

        setupLocation() {
            const addLocation = document.getElementById('addLocation');
            const fields = document.getElementById('locationFields');
            const getBtn = document.getElementById('getCurrentLocation');

            addLocation?.addEventListener('change', (e) => {
                fields?.classList.toggle('hidden', !e.target.checked);
            });

            getBtn?.addEventListener('click', () => {
                if (navigator.geolocation) {
                    navigator.geolocation.getCurrentPosition((pos) => {
                        const lat = document.querySelector('input[name="Latitude"]');
                        const lon = document.querySelector('input[name="Longitude"]');
                        if (lat) lat.value = pos.coords.latitude;
                        if (lon) lon.value = pos.coords.longitude;
                    });
                }
            });
        }

        setupTags() {
            const input = document.getElementById('tagsInput');
            if (!input) return;

            input.addEventListener('keydown', (e) => {
                if (e.key === 'Enter' || e.key === ',') {
                    e.preventDefault();
                    this.addTag(input.value.trim());
                    input.value = '';
                }
            });

            input.addEventListener('blur', () => {
                if (input.value.trim()) {
                    this.addTag(input.value.trim());
                    input.value = '';
                }
            });
        }

        addTag(tag) {
            if (tag && !this.tags.includes(tag)) {
                this.tags.push(tag);
                this.updateTagsUI();
            }
        }

        removeTag(tag) {
            this.tags = this.tags.filter(t => t !== tag);
            this.updateTagsUI();
        }

        updateTagsUI() {
            const list = document.getElementById('tagsList');
            const hidden = document.getElementById('tagsHidden');
            if (!list) return;

            list.innerHTML = this.tags.map(tag => `
                <span class="inline-flex items-center gap-1 px-3 py-1 bg-primary/10 text-primary rounded-full text-sm">
                    #${tag}
                    <button type="button" class="w-4 h-4 hover:bg-primary/20 rounded-full flex items-center justify-center" onclick="CC.Modules.StoriesCreate.removeTag('${tag}')">
                        <i data-lucide="x" class="w-3 h-3"></i>
                    </button>
                </span>
            `).join('');

            if (hidden) hidden.value = this.tags.join(',');
            if (window.lucide) window.lucide.createIcons({ root: list });
        }
    }

    CC.Modules.StoriesCreate = new StoriesCreateController();
})(window.CommunityCar);
