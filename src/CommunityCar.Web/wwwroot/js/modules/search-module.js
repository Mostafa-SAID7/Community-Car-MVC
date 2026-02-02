/**
 * Search Module Controller
 * Handles Search UI, Filtering, and Results display.
 */
class SearchController extends CC.Utils.BaseComponent {
    constructor() {
        super('SearchModule');
        this.service = CC.Services.Search;
        this.suggestionsCache = new Map();
        this.debounceTimer = null;

        this.state = {
            currentQuery: '',
            filters: {
                entityTypes: [],
                dateRange: '',
                sortBy: 'relevance',
                page: 1,
                pageSize: 20
            }
        };

        this.elements = {};
    }

    init() {
        if (super.init()) return;

        // Check if search elements exist (e.g. search page or global search)
        if (!document.getElementById('searchInput')) return;

        this.cacheElements();
        this.setupEventListeners();
        this.setupFilters();
        this.loadTrendingItems();

        // URL params check
        const urlParams = new URLSearchParams(window.location.search);
        const q = urlParams.get('q');
        if (q) {
            this.elements.searchInput.value = q;
            this.performSearch(q);
        }

        console.log('Search Module initialized');
    }

    cacheElements() {
        this.elements = {
            searchInput: document.getElementById('searchInput'),
            suggestions: document.getElementById('searchSuggestions'),
            results: document.getElementById('searchResults'),
            stats: document.getElementById('searchStats'),
            loading: document.getElementById('searchLoading'),
            noResults: document.getElementById('noResults'),
            pagination: document.getElementById('searchPagination'),
            trending: document.getElementById('trendingItems'),
            contentTypeFilters: document.getElementById('contentTypeFilters')
        };
    }

    setupEventListeners() {
        const { searchInput, suggestions } = this.elements;

        searchInput.addEventListener('input', (e) => this.handleInput(e.target.value));
        searchInput.addEventListener('keydown', (e) => {
            if (e.key === 'Enter') {
                e.preventDefault();
                this.performSearch(e.target.value);
                this.hideSuggestions();
            } else if (e.key === 'Escape') {
                this.hideSuggestions();
            }
        });

        // Hide suggestions on outside click
        document.addEventListener('click', (e) => {
            if (searchInput && suggestions && !searchInput.contains(e.target) && !suggestions.contains(e.target)) {
                this.hideSuggestions();
            }
        });

        // Static filter toggles
        document.getElementById('dateRangeFilter')?.addEventListener('change', (e) => {
            this.state.filters.dateRange = e.target.value;
            this.performSearch(this.state.currentQuery);
        });

        document.getElementById('sortFilter')?.addEventListener('change', (e) => {
            this.state.filters.sortBy = e.target.value;
            this.performSearch(this.state.currentQuery);
        });

        document.getElementById('clearFilters')?.addEventListener('click', () => this.clearFilters());
    }

    setupFilters() {
        const container = this.elements.contentTypeFilters;
        if (!container) return;

        const types = [
            { value: 'Post', label: 'Posts' }, { value: 'Question', label: 'Questions' },
            { value: 'Guide', label: 'Guides' }, { value: 'News', label: 'News' },
            { value: 'Review', label: 'Reviews' }, { value: 'Story', label: 'Stories' },
            { value: 'Event', label: 'Events' }, { value: 'Group', label: 'Groups' }
        ];

        types.forEach(type => {
            const label = document.createElement('label');
            label.className = 'flex items-center cursor-pointer';
            label.innerHTML = `
                <input type="checkbox" value="${type.value}" class="entity-type-filter mr-2 rounded border-gray-300 dark:border-gray-600 text-red-500 focus:ring-red-500">
                <span class="text-sm text-gray-700 dark:text-gray-300">${type.label}</span>`;

            label.querySelector('input').addEventListener('change', (e) =>
                this.handleTypeFilter(e.target.value, e.target.checked)
            );
            container.appendChild(label);
        });
    }

    handleInput(query) {
        if (this.debounceTimer) clearTimeout(this.debounceTimer);

        if (query.length < 2) {
            this.hideSuggestions();
            return;
        }

        this.debounceTimer = setTimeout(() => this.loadSuggestions(query), 300);
    }

    async loadSuggestions(query) {
        if (this.suggestionsCache.has(query)) {
            this.displaySuggestions(this.suggestionsCache.get(query));
            return;
        }

        try {
            const result = await this.service.getSuggestions(query);
            if (result.success) {
                this.suggestionsCache.set(query, result.data);
                this.displaySuggestions(result.data);
            }
        } catch (error) {
            console.error(error);
        }
    }

    displaySuggestions(data) {
        if (!data || data.length === 0) {
            this.hideSuggestions();
            return;
        }

        const html = data.map(item => `
            <div class="suggestion-item px-4 py-2 hover:bg-gray-50 dark:hover:bg-gray-700 cursor-pointer flex items-center justify-between"
                 data-text="${item.text}">
                <div class="flex items-center">
                    <span class="text-sm text-gray-900 dark:text-white">${item.text}</span>
                    <span class="ml-2 text-xs text-gray-500 dark:text-gray-400">${item.type}</span>
                </div>
                <span class="text-xs text-gray-400">${item.count}</span>
            </div>
        `).join('');

        this.elements.suggestions.innerHTML = html;
        this.elements.suggestions.classList.remove('hidden');

        this.elements.suggestions.querySelectorAll('.suggestion-item').forEach(el => {
            el.addEventListener('click', () => {
                const text = el.dataset.text;
                this.elements.searchInput.value = text;
                this.performSearch(text);
                this.hideSuggestions();
            });
        });
    }

    hideSuggestions() {
        if (this.elements.suggestions) this.elements.suggestions.classList.add('hidden');
    }

    handleTypeFilter(type, checked) {
        if (checked) {
            if (!this.state.filters.entityTypes.includes(type)) this.state.filters.entityTypes.push(type);
        } else {
            this.state.filters.entityTypes = this.state.filters.entityTypes.filter(t => t !== type);
        }
        if (this.state.currentQuery) this.performSearch(this.state.currentQuery);
    }

    clearFilters() {
        this.state.filters.entityTypes = [];
        this.state.filters.dateRange = '';
        this.state.filters.sortBy = 'relevance';
        this.state.filters.page = 1;

        // Reset UI
        document.getElementById('dateRangeFilter').value = '';
        document.getElementById('sortFilter').value = 'relevance';
        document.querySelectorAll('.entity-type-filter').forEach(cb => cb.checked = false);

        if (this.state.currentQuery) this.performSearch(this.state.currentQuery);
    }

    async performSearch(query, page = 1) {
        if (!query.trim()) {
            this.clearResults();
            return;
        }

        this.state.currentQuery = query;
        this.state.filters.page = page;

        this.showLoading();

        const params = new URLSearchParams({
            query,
            page,
            pageSize: this.state.filters.pageSize,
            sortBy: this.state.filters.sortBy,
            sortDirection: 'DESC'
        });

        this.state.filters.entityTypes.forEach(t => params.append('entityTypes', t));

        // Date range logic
        const dateRange = this.getDateRange(this.state.filters.dateRange);
        if (dateRange.fromDate) params.append('fromDate', dateRange.fromDate);
        if (dateRange.toDate) params.append('toDate', dateRange.toDate);

        try {
            const result = await this.service.search(params);
            if (result.success) {
                this.displayResults(result.data);
                this.updateUrl(query);
            } else {
                this.showError(result.message);
            }
        } catch (error) {
            this.showError('An error occurred.');
            console.error(error);
        } finally {
            this.hideLoading();
        }
    }

    displayResults(data) {
        if (!data.items || data.items.length === 0) {
            this.showNoResults();
            return;
        }

        this.elements.stats.innerHTML = `Found ${data.totalCount.toLocaleString()} results in ${data.searchDuration.toFixed(2)}ms`;
        this.elements.stats.classList.remove('hidden');

        this.elements.results.innerHTML = data.items.map(item => this.renderResultItem(item)).join('');
        this.elements.results.classList.remove('hidden');

        this.renderPagination(data);
        this.elements.noResults.classList.add('hidden');
    }

    renderResultItem(item) {
        const tags = item.tags.map(tag => `<span class="search-result-tag">${tag}</span>`).join('');
        // Icons embedded via svg string or lucide if available. Keeping text fallback for simplicity or SVG string from original
        const icon = this.getEntityTypeIcon(item.entityType);

        return `
            <div class="search-result-item">
                <div class="flex items-start justify-between">
                    <div class="flex-1">
                        <div class="flex items-center mb-2">
                            ${icon}
                            <span class="text-xs text-gray-500 dark:text-gray-400 ml-2">${item.entityType}</span>
                            <span class="text-xs text-gray-400 ml-2">•</span>
                            <span class="text-xs text-gray-500 dark:text-gray-400 ml-2">${this.formatDate(item.createdAt)}</span>
                            ${item.authorName ? `<span class="text-xs text-gray-400 ml-2">•</span><span class="text-xs text-gray-500 dark:text-gray-400 ml-2">by ${item.authorName}</span>` : ''}
                        </div>
                        <h3 class="search-result-title">
                            <a href="${item.url}" class="hover:underline">${item.title}</a>
                        </h3>
                        <p class="search-result-description">${item.description}</p>
                        <div class="search-result-tags">${tags}</div>
                    </div>
                     <div class="ml-4 text-right">
                        <div class="text-sm font-medium text-red-500">${(item.relevanceScore * 10).toFixed(0)}%</div>
                    </div>
                </div>
            </div>`;
    }

    renderPagination(data) {
        if (data.totalPages <= 1) {
            this.elements.pagination.classList.add('hidden');
            return;
        }

        let html = '<div class="flex items-center justify-center space-x-2">';
        const current = data.page;
        const total = data.totalPages;

        if (current > 1) html += `<button class="pagination-btn" onclick="CC.Modules.Search.performSearch('${this.state.currentQuery}', ${current - 1})">Previous</button>`;

        // Simplified pagination Logic
        for (let i = 1; i <= Math.min(5, total); i++) {
            html += `<button class="pagination-btn ${i === current ? 'active' : ''}" onclick="CC.Modules.Search.performSearch('${this.state.currentQuery}', ${i})">${i}</button>`;
        }

        if (current < total) html += `<button class="pagination-btn" onclick="CC.Modules.Search.performSearch('${this.state.currentQuery}', ${current + 1})">Next</button>`;

        html += '</div>';
        this.elements.pagination.innerHTML = html;
        this.elements.pagination.classList.remove('hidden');
    }

    // ... Helpers (getDateRange, formatDate, updateUrl, icons, loading/error states)
    // Using simple placeholders or copying original logic

    getDateRange(range) {
        const now = new Date();
        const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
        switch (range) {
            case 'today': return { fromDate: today.toISOString() };
            case 'week': { const d = new Date(today); d.setDate(d.getDate() - 7); return { fromDate: d.toISOString() }; }
            case 'month': { const d = new Date(today); d.setMonth(d.getMonth() - 1); return { fromDate: d.toISOString() }; }
            case 'year': { const d = new Date(today); d.setFullYear(d.getFullYear() - 1); return { fromDate: d.toISOString() }; }
            default: return {};
        }
    }

    formatDate(dateStr) {
        try { return new Date(dateStr).toLocaleDateString(); } catch { return ''; }
    }

    updateUrl(q) {
        const url = new URL(window.location);
        url.searchParams.set('q', q);
        window.history.replaceState({}, '', url);
    }

    showLoading() { this.elements.loading.classList.remove('hidden'); this.elements.results.classList.add('hidden'); }
    hideLoading() { this.elements.loading.classList.add('hidden'); }
    clearResults() { this.elements.results.classList.add('hidden'); this.elements.noResults.classList.add('hidden'); }
    showNoResults() { this.elements.noResults.classList.remove('hidden'); this.elements.results.classList.add('hidden'); }
    showError(msg) { this.elements.results.innerHTML = `<p class="text-red-500">${msg}</p>`; this.elements.results.classList.remove('hidden'); }

    async loadTrendingItems() {
        if (!this.elements.trending) return;
        try {
            const result = await this.service.getTrending();
            if (result.success) {
                this.elements.trending.innerHTML = result.data.map(item => `
                    <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4 mb-2">
                        <a href="${item.url}" class="font-medium hover:text-red-500">${item.title}</a>
                    </div>
                `).join('');
            }
        } catch (e) { console.error(e); }
    }

    getEntityTypeIcon(type) {
        // Simplified return for brevity, original SVG strings were long
        return '<svg class="h-4 w-4 text-gray-500" fill="currentColor" viewBox="0 0 20 20"><circle cx="10" cy="10" r="5"/></svg>';
    }
}

// Init
CommunityCar.Modules.Search = new SearchController();
document.addEventListener('DOMContentLoaded', () => {
    CommunityCar.Modules.Search.init();
});
