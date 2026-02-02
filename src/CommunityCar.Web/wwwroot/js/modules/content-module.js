/**
 * Content Modules
 * Unified controllers for News, Guides, and Reviews.
 */

class NewsController extends CC.Utils.BaseComponent {
    constructor() {
        super('NewsController');
        this.service = CC.Services.News;
        this.filters = {
            search: '',
            category: '',
            sortBy: 'newest',
            page: 1
        };
        this.searchTimeout = null;
    }

    init() {
        this.cacheElements();
        this.bindEvents();
        this.setupInitialState();
    }

    cacheElements() {
        this.container = document.getElementById('news-list-container');
        this.searchInput = document.querySelector('#newsSearchForm input[name="search"]');
    }

    bindEvents() {
        this.delegate('click', '[data-action="like-news"]', async (e, target) => {
            const id = target.dataset.id;
            const result = await this.service.toggleLike(id);
            if (result.success) this.updateLikeUI(target, result.isLiked, result.count);
        });

        this.delegate('click', '[data-action="bookmark-news"]', async (e, target) => {
            const id = target.dataset.id;
            const result = await this.service.toggleBookmark(id);
            if (result.success) this.updateBookmarkUI(target, result.isBookmarked);
        });

        // Search Input (Debounced)
        this.searchInput?.addEventListener('input', (e) => {
            this.filters.search = e.target.value;
            this.filters.page = 1;
            this.debouncedSearch();
        });

        // Dropdown Filters
        const dropdowns = ['category', 'sortBy'];
        dropdowns.forEach(name => {
            document.querySelector(`#newsSearchForm select[name="${name}"]`)?.addEventListener('change', (e) => {
                this.filters[name] = e.target.value;
                this.filters.page = 1;
                this.updateLiveView();
            });
        });

        // Pagination (Delegated)
        this.delegate('click', '#news-list-container nav a', (e, target) => {
            e.preventDefault();
            const url = new URL(target.href, window.location.origin);
            const page = url.searchParams.get('page');
            if (page) {
                this.filters.page = parseInt(page);
                this.updateLiveView();
                window.scrollTo({ top: this.container.offsetTop - 100, behavior: 'smooth' });
            }
        });
    }

    setupInitialState() {
        if (this.searchInput) this.filters.search = this.searchInput.value;
        const category = document.querySelector('#newsSearchForm select[name="category"]');
        if (category) this.filters.category = category.value;
        const sortBy = document.querySelector('#newsSearchForm select[name="sortBy"]');
        if (sortBy) this.filters.sortBy = sortBy.value;
    }

    debouncedSearch() {
        clearTimeout(this.searchTimeout);
        this.searchTimeout = setTimeout(() => this.updateLiveView(), 500);
    }

    async updateLiveView() {
        if (!this.container) return;
        this.container.classList.add('opacity-40', 'pointer-events-none');

        try {
            const html = await this.service.searchNews(this.filters);
            if (html) {
                this.container.innerHTML = html;
                if (window.lucide) window.lucide.createIcons({ parent: this.container });
                this.updateUrl();
            }
        } catch (error) {
            CC.Utils.Notifier.error('Failed to update news');
        } finally {
            this.container.classList.remove('opacity-40', 'pointer-events-none');
        }
    }

    updateUrl() {
        const url = new URL(window.location);
        Object.entries(this.filters).forEach(([key, value]) => {
            if (value && value !== 'newest' && value !== 1) {
                url.searchParams.set(key, value);
            } else {
                url.searchParams.delete(key);
            }
        });
        window.history.replaceState({}, '', url);
    }

    updateLikeUI(button, isLiked, count) {
        const icon = button.querySelector('[data-lucide]');
        const countLabel = button.querySelector('.like-count');
        if (icon) {
            icon.setAttribute('data-lucide', isLiked ? 'heart-handshake' : 'heart');
            if (window.lucide) window.lucide.createIcons({ parent: button });
        }
        if (countLabel) countLabel.textContent = count;
        button.classList.toggle('text-primary', isLiked);
    }

    updateBookmarkUI(button, isBookmarked) {
        const icon = button.querySelector('[data-lucide]');
        if (icon) {
            icon.setAttribute('data-lucide', isBookmarked ? 'bookmark-check' : 'bookmark');
            if (window.lucide) window.lucide.createIcons({ parent: button });
        }
        button.classList.toggle('text-primary', isBookmarked);
    }

    async handleDelete(id, type) {
        if (await this.confirm(`Are you sure you want to delete this ${type}?`)) {
            const result = await this.service.post(`/${type}s/Delete/${id}`);
            if (result.success) {
                CC.Utils.Notifier.success(`${type} deleted`);
                location.reload();
            }
        }
    }

    async confirm(message) {
        if (window.AlertModal) {
            return await new Promise(resolve => window.AlertModal.confirm(message, 'Confirm Action', resolve));
        }
        return confirm(message);
    }
}

class GuidesController extends CC.Utils.BaseComponent {
    constructor() {
        super('GuidesController');
        this.service = CC.Services.Guides;
        this.filters = {
            search: '',
            category: '',
            difficulty: '',
            filter: 'all', // For MyGuides
            sortBy: 'newest',
            page: 1
        };
        this.searchTimeout = null;
        this.baseUrl = window.location.pathname.includes('/my-guides') ? 'my-guides' : '';
    }

    init() {
        this.cacheElements();
        this.bindEvents();
        this.setupInitialState();
    }

    cacheElements() {
        this.container = document.getElementById('guides-content');
        this.searchInput = document.querySelector('input[name="search"]');
    }

    bindEvents() {
        this.delegate('click', '[data-action="bookmark-guide"]', async (e, target) => {
            await this.handleBookmarkToggle(target.dataset.id, target);
        });

        this.delegate('click', '[data-action="rate-guide"]', async (e, target) => {
            const id = target.dataset.id;
            const rating = target.dataset.rating;
            const result = await this.service.rateGuide(id, rating);
            if (result.success) CC.Utils.Notifier.success('Rating submitted');
        });

        this.delegate('click', '[data-action="delete-guide"]', async (e, target) => {
            await this.handleDelete(target.dataset.id);
        });

        this.delegate('click', '[data-action="publish-guide"]', async (e, target) => {
            await this.handlePublish(target.dataset.id);
        });

        // Search Input (Debounced)
        this.searchInput?.addEventListener('input', (e) => {
            this.filters.search = e.target.value;
            this.filters.page = 1;
            this.debouncedSearch();
        });

        // Dropdown Filters
        const dropdowns = ['category', 'difficulty', 'sortBy'];
        dropdowns.forEach(name => {
            document.querySelector(`select[name="${name}"]`)?.addEventListener('change', (e) => {
                this.filters[name] = e.target.value;
                this.filters.page = 1;
                this.updateLiveView();
            });
        });

        // Pagination (Delegated)
        this.delegate('click', '#guides-content nav a', (e, target) => {
            e.preventDefault();
            const page = target.dataset.page || new URL(target.href, window.location.origin).searchParams.get('page');
            if (page) {
                this.filters.page = parseInt(page);
                this.updateLiveView();
                window.scrollTo({ top: this.container.offsetTop - 100, behavior: 'smooth' });
            }
        });

        // Tab Filters (for MyGuides)
        this.delegate('click', '[data-action="filter-guides"]', (e, target) => {
            e.preventDefault();
            this.filters.filter = target.dataset.filter;
            this.filters.page = 1;
            this.updateLiveView();

            // Update Active State
            document.querySelectorAll('[data-action="filter-guides"]').forEach(el => {
                const isActive = el === target;
                el.classList.toggle('bg-background', isActive);
                el.classList.toggle('text-foreground', isActive);
                el.classList.toggle('shadow-md', isActive);
                el.classList.toggle('ring-1', isActive);
                el.classList.toggle('text-muted-foreground', !isActive);
            });
        });
    }

    setupInitialState() {
        if (this.searchInput) this.filters.search = this.searchInput.value;
        const category = document.querySelector('select[name="category"]');
        if (category) this.filters.category = category.value;
        const difficulty = document.querySelector('select[name="difficulty"]');
        if (difficulty) this.filters.difficulty = difficulty.value;
        const sortBy = document.querySelector('select[name="sortBy"]');
        if (sortBy) this.filters.sortBy = sortBy.value;
    }

    debouncedSearch() {
        clearTimeout(this.searchTimeout);
        this.searchTimeout = setTimeout(() => this.updateLiveView(), 500);
    }

    async updateLiveView() {
        if (!this.container) return;
        this.container.classList.add('opacity-40', 'pointer-events-none');

        try {
            const params = { ...this.filters };
            const html = await this.service.searchGuides(params, this.baseUrl);
            if (html) {
                this.container.innerHTML = html;
                if (window.lucide) window.lucide.createIcons({ parent: this.container });
                this.updateUrl();
            }
        } catch (error) {
            CC.Utils.Notifier.error('Failed to update guides');
        } finally {
            this.container.classList.remove('opacity-40', 'pointer-events-none');
        }
    }

    updateUrl() {
        const url = new URL(window.location);
        Object.entries(this.filters).forEach(([key, value]) => {
            if (value && value !== 'newest' && value !== 1) {
                url.searchParams.set(key, value);
            } else {
                url.searchParams.delete(key);
            }
        });
        window.history.replaceState({}, '', url);
    }

    async handleBookmarkToggle(id, button) {
        const result = await this.service.toggleBookmark(id);
        if (result.success) this.updateBookmarkUI(button, result.isBookmarked);
    }

    async handleDelete(id) {
        if (await this.confirm('Are you sure you want to delete this guide?')) {
            const result = await this.service.post(`/Guides/Delete/${id}`);
            if (result.success) {
                CC.Utils.Notifier.success('Guide deleted');
                location.reload();
            }
        }
    }

    async handlePublish(id) {
        const result = await this.service.post(`/Guides/Publish/${id}`);
        if (result.success) {
            CC.Utils.Notifier.success('Guide published');
            location.reload();
        }
    }

    updateBookmarkUI(button, isBookmarked) {
        const icon = button.querySelector('[data-lucide]');
        if (icon) {
            icon.setAttribute('data-lucide', isBookmarked ? 'bookmark-check' : 'bookmark');
            if (window.lucide) window.lucide.createIcons({ parent: button });
        }
        button.classList.toggle('text-primary', isBookmarked);
    }

    async confirm(message) {
        if (window.AlertModal) {
            return await new Promise(resolve => window.AlertModal.confirm(message, 'Confirm Action', resolve));
        }
        return confirm(message);
    }
}

class ReviewsController extends CC.Utils.BaseComponent {
    constructor() {
        super('ReviewsController');
        this.service = CC.Services.Reviews;
        this.filters = {
            search: '',
            rating: '',
            carMake: '',
            isVerifiedPurchase: false,
            isRecommended: false,
            sortBy: 'Newest',
            page: 1
        };
        this.searchTimeout = null;
    }

    init() {
        this.cacheElements();
        this.bindEvents();
        this.setupInitialState();
    }

    cacheElements() {
        this.container = document.getElementById('reviews-list-container');
        this.skeleton = document.getElementById('reviews-skeleton');
        this.searchInput = document.getElementById('reviewsSearchInput');
        this.filtersPanel = document.getElementById('filtersPanel');
    }

    bindEvents() {
        // Helpful votes
        this.delegate('click', '[data-action="helpful-review"]', async (e, target) => {
            const id = target.dataset.id;
            const result = await this.service.markHelpful(id);
            if (result.success) {
                CC.Utils.Notifier.success(result.message || 'Marked as helpful');
                const count = target.querySelector('.helpful-count');
                if (count) count.textContent = parseInt(count.textContent) + 1;
                target.disabled = true;
                target.classList.add('opacity-50');
            }
        });

        // Search Input (Debounced)
        this.searchInput?.addEventListener('input', (e) => {
            this.filters.search = e.target.value;
            this.filters.page = 1;
            this.debouncedSearch();
        });

        // Dropdown Filters
        const dropdowns = ['sortFilter', 'ratingFilter', 'carMakeFilter'];
        dropdowns.forEach(id => {
            document.getElementById(id)?.addEventListener('change', (e) => {
                const key = id.replace('Filter', '');
                this.filters[key] = e.target.value;
                this.filters.page = 1;
                this.updateLiveView();
            });
        });

        // Checkbox Filters
        const checkboxes = ['verifiedFilter', 'recommendedFilter'];
        checkboxes.forEach(id => {
            document.getElementById(id)?.addEventListener('change', (e) => {
                const key = id.replace('Filter', '');
                this.filters[key] = e.target.checked;
                this.filters.page = 1;
                this.updateLiveView();
            });
        });

        // Pagination (Delegated)
        this.delegate('click', '#reviews-list-container nav a', (e, target) => {
            e.preventDefault();
            const url = new URL(target.href, window.location.origin);
            const page = url.searchParams.get('page');
            if (page) {
                this.filters.page = parseInt(page);
                this.updateLiveView();
                window.scrollTo({ top: this.container.offsetTop - 100, behavior: 'smooth' });
            }
        });

        // Delete
        this.delegate('click', '[data-action="delete-review"]', async (e, target) => {
            await this.handleDelete(target.dataset.id);
        });
    }

    setupInitialState() {
        // Sync filters with current UI values (in case of page reload with params)
        if (this.searchInput) this.filters.search = this.searchInput.value;
        const sort = document.getElementById('sortFilter'); if (sort) this.filters.sortBy = sort.value;
        const rating = document.getElementById('ratingFilter'); if (rating) this.filters.rating = rating.value;
        const make = document.getElementById('carMakeFilter'); if (make) this.filters.carMake = make.value;
        const verified = document.getElementById('verifiedFilter'); if (verified) this.filters.isVerifiedPurchase = verified.checked;
        const recommended = document.getElementById('recommendedFilter'); if (recommended) this.filters.isRecommended = recommended.checked;
    }

    debouncedSearch() {
        clearTimeout(this.searchTimeout);
        this.searchTimeout = setTimeout(() => this.updateLiveView(), 500);
    }

    async updateLiveView() {
        if (!this.container) return;

        // Show Skeleton
        if (CC.Utils.Skeleton) {
            this.container.classList.add('opacity-40', 'pointer-events-none');
            // Alternatively, swap content with actual skeletons
            // this.container.innerHTML = '';
            // CC.Utils.Skeleton.show(this.container, 'Post', 4);
        }

        try {
            const html = await this.service.searchReviews(this.filters);
            if (html) {
                this.container.innerHTML = html;
                // Re-initialize Lucide icons in the new content
                if (window.lucide) window.lucide.createIcons({ parent: this.container });

                // Update URL without reload for deep linking
                this.updateUrl();
            }
        } catch (error) {
            CC.Utils.Notifier.error('Failed to update reviews');
        } finally {
            this.container.classList.remove('opacity-40', 'pointer-events-none');
        }
    }

    updateUrl() {
        const url = new URL(window.location);
        Object.entries(this.filters).forEach(([key, value]) => {
            if (value && value !== 'Newest' && value !== 1) {
                url.searchParams.set(key, value);
            } else {
                url.searchParams.delete(key);
            }
        });
        window.history.replaceState({}, '', url);
    }

    async handleDelete(id) {
        if (await this.confirm('Are you sure you want to delete this review?')) {
            const result = await this.service.post(`/Reviews/Delete/${id}`);
            if (result.success) {
                CC.Utils.Notifier.success('Review deleted');
                this.updateLiveView(); // Refresh list via AJAX instead of reload
            }
        }
    }

    async confirm(message) {
        if (window.AlertModal) {
            return await new Promise(resolve => window.AlertModal.confirm(message, 'Confirm Action', resolve));
        }
        return confirm(message);
    }
}

CC.Modules.News = new NewsController();
CC.Modules.Guides = new GuidesController();
CC.Modules.Reviews = new ReviewsController();
