// Search functionality
class SearchManager {
    constructor() {
        this.searchInput = document.getElementById('searchInput');
        this.searchSuggestions = document.getElementById('searchSuggestions');
        this.searchResults = document.getElementById('searchResults');
        this.searchStats = document.getElementById('searchStats');
        this.searchLoading = document.getElementById('searchLoading');
        this.noResults = document.getElementById('noResults');
        this.searchPagination = document.getElementById('searchPagination');
        this.trendingItems = document.getElementById('trendingItems');
        
        this.currentQuery = '';
        this.currentFilters = {
            entityTypes: [],
            dateRange: '',
            sortBy: 'relevance',
            page: 1,
            pageSize: 20
        };
        
        this.debounceTimer = null;
        this.suggestionsCache = new Map();
        
        this.init();
    }
    
    init() {
        this.setupEventListeners();
        this.loadTrendingItems();
        this.setupFilters();
        
        // Check for URL parameters
        const urlParams = new URLSearchParams(window.location.search);
        const query = urlParams.get('q');
        if (query) {
            this.searchInput.value = query;
            this.performSearch(query);
        }
    }
    
    setupEventListeners() {
        // Search input
        this.searchInput.addEventListener('input', (e) => {
            this.handleSearchInput(e.target.value);
        });
        
        this.searchInput.addEventListener('keydown', (e) => {
            if (e.key === 'Enter') {
                e.preventDefault();
                this.performSearch(e.target.value);
                this.hideSuggestions();
            } else if (e.key === 'Escape') {
                this.hideSuggestions();
            }
        });
        
        // Search filters
        document.querySelectorAll('.search-filter').forEach(filter => {
            filter.addEventListener('click', (e) => {
                this.handleFilterClick(e.target);
            });
        });
        
        // Sidebar filters
        document.getElementById('dateRangeFilter').addEventListener('change', (e) => {
            this.currentFilters.dateRange = e.target.value;
            this.performSearch(this.currentQuery);
        });
        
        document.getElementById('sortFilter').addEventListener('change', (e) => {
            this.currentFilters.sortBy = e.target.value;
            this.performSearch(this.currentQuery);
        });
        
        document.getElementById('clearFilters').addEventListener('click', () => {
            this.clearAllFilters();
        });
        
        // Click outside to hide suggestions
        document.addEventListener('click', (e) => {
            if (!this.searchInput.contains(e.target) && !this.searchSuggestions.contains(e.target)) {
                this.hideSuggestions();
            }
        });
    }
    
    setupFilters() {
        const contentTypeFilters = document.getElementById('contentTypeFilters');
        const entityTypes = [
            { value: 'Post', label: 'Posts' },
            { value: 'Question', label: 'Questions' },
            { value: 'Guide', label: 'Guides' },
            { value: 'News', label: 'News' },
            { value: 'Review', label: 'Reviews' },
            { value: 'Story', label: 'Stories' },
            { value: 'Event', label: 'Events' },
            { value: 'Group', label: 'Groups' }
        ];
        
        entityTypes.forEach(type => {
            const checkbox = document.createElement('label');
            checkbox.className = 'flex items-center cursor-pointer';
            checkbox.innerHTML = `
                <input type="checkbox" value="${type.value}" class="entity-type-filter mr-2 rounded border-gray-300 dark:border-gray-600 text-red-500 focus:ring-red-500">
                <span class="text-sm text-gray-700 dark:text-gray-300">${type.label}</span>
            `;
            
            checkbox.querySelector('input').addEventListener('change', (e) => {
                this.handleEntityTypeFilter(e.target.value, e.target.checked);
            });
            
            contentTypeFilters.appendChild(checkbox);
        });
    }
    
    handleSearchInput(query) {
        clearTimeout(this.debounceTimer);
        
        if (query.length < 2) {
            this.hideSuggestions();
            return;
        }
        
        this.debounceTimer = setTimeout(() => {
            this.loadSuggestions(query);
        }, 300);
    }
    
    async loadSuggestions(query) {
        if (this.suggestionsCache.has(query)) {
            this.displaySuggestions(this.suggestionsCache.get(query));
            return;
        }
        
        try {
            const response = await fetch(`/api/shared/search/suggestions?query=${encodeURIComponent(query)}&maxResults=8`);
            const result = await response.json();
            
            if (result.success) {
                this.suggestionsCache.set(query, result.data);
                this.displaySuggestions(result.data);
            }
        } catch (error) {
            console.error('Error loading suggestions:', error);
        }
    }
    
    displaySuggestions(suggestions) {
        if (!suggestions || suggestions.length === 0) {
            this.hideSuggestions();
            return;
        }
        
        const html = suggestions.map(suggestion => `
            <div class="suggestion-item px-4 py-2 hover:bg-gray-50 dark:hover:bg-gray-700 cursor-pointer flex items-center justify-between"
                 data-suggestion="${suggestion.text}">
                <div class="flex items-center">
                    <span class="text-sm text-gray-900 dark:text-white">${suggestion.text}</span>
                    <span class="ml-2 text-xs text-gray-500 dark:text-gray-400">${suggestion.type}</span>
                </div>
                <span class="text-xs text-gray-400">${suggestion.count}</span>
            </div>
        `).join('');
        
        this.searchSuggestions.innerHTML = html;
        this.searchSuggestions.classList.remove('hidden');
        
        // Add click handlers
        this.searchSuggestions.querySelectorAll('.suggestion-item').forEach(item => {
            item.addEventListener('click', () => {
                const suggestion = item.dataset.suggestion;
                this.searchInput.value = suggestion;
                this.performSearch(suggestion);
                this.hideSuggestions();
            });
        });
    }
    
    hideSuggestions() {
        this.searchSuggestions.classList.add('hidden');
    }
    
    handleFilterClick(filterElement) {
        // Remove active class from all filters
        document.querySelectorAll('.search-filter').forEach(f => f.classList.remove('active'));
        
        // Add active class to clicked filter
        filterElement.classList.add('active');
        
        // Update entity type filter
        const filterType = filterElement.dataset.filter;
        if (filterType === 'all') {
            this.currentFilters.entityTypes = [];
        } else {
            this.currentFilters.entityTypes = [filterType];
        }
        
        // Perform search if there's a query
        if (this.currentQuery) {
            this.performSearch(this.currentQuery);
        }
    }
    
    handleEntityTypeFilter(entityType, checked) {
        if (checked) {
            if (!this.currentFilters.entityTypes.includes(entityType)) {
                this.currentFilters.entityTypes.push(entityType);
            }
        } else {
            this.currentFilters.entityTypes = this.currentFilters.entityTypes.filter(t => t !== entityType);
        }
        
        // Update main filter buttons
        this.updateMainFilterButtons();
        
        // Perform search if there's a query
        if (this.currentQuery) {
            this.performSearch(this.currentQuery);
        }
    }
    
    updateMainFilterButtons() {
        const allFilter = document.querySelector('.search-filter[data-filter="all"]');
        const otherFilters = document.querySelectorAll('.search-filter:not([data-filter="all"])');
        
        // Remove active from all
        document.querySelectorAll('.search-filter').forEach(f => f.classList.remove('active'));
        
        if (this.currentFilters.entityTypes.length === 0) {
            allFilter.classList.add('active');
        } else if (this.currentFilters.entityTypes.length === 1) {
            const activeFilter = document.querySelector(`.search-filter[data-filter="${this.currentFilters.entityTypes[0].toLowerCase()}"]`);
            if (activeFilter) {
                activeFilter.classList.add('active');
            }
        }
    }
    
    clearAllFilters() {
        this.currentFilters = {
            entityTypes: [],
            dateRange: '',
            sortBy: 'relevance',
            page: 1,
            pageSize: 20
        };
        
        // Reset UI
        document.getElementById('dateRangeFilter').value = '';
        document.getElementById('sortFilter').value = 'relevance';
        document.querySelectorAll('.entity-type-filter').forEach(cb => cb.checked = false);
        document.querySelectorAll('.search-filter').forEach(f => f.classList.remove('active'));
        document.querySelector('.search-filter[data-filter="all"]').classList.add('active');
        
        // Perform search if there's a query
        if (this.currentQuery) {
            this.performSearch(this.currentQuery);
        }
    }
    
    async performSearch(query, page = 1) {
        if (!query.trim()) {
            this.clearResults();
            return;
        }
        
        this.currentQuery = query;
        this.currentFilters.page = page;
        
        this.showLoading();
        
        try {
            const searchParams = new URLSearchParams({
                query: query,
                page: page,
                pageSize: this.currentFilters.pageSize,
                sortBy: this.currentFilters.sortBy,
                sortDirection: 'DESC'
            });
            
            if (this.currentFilters.entityTypes.length > 0) {
                this.currentFilters.entityTypes.forEach(type => {
                    searchParams.append('entityTypes', type);
                });
            }
            
            if (this.currentFilters.dateRange) {
                const dateRange = this.getDateRange(this.currentFilters.dateRange);
                if (dateRange.fromDate) searchParams.append('fromDate', dateRange.fromDate);
                if (dateRange.toDate) searchParams.append('toDate', dateRange.toDate);
            }
            
            const response = await fetch(`/api/shared/search/universal?${searchParams}`);
            const result = await response.json();
            
            if (result.success) {
                this.displayResults(result.data);
                this.updateURL(query);
            } else {
                this.showError(result.message);
            }
        } catch (error) {
            console.error('Search error:', error);
            this.showError('An error occurred while searching. Please try again.');
        } finally {
            this.hideLoading();
        }
    }
    
    displayResults(data) {
        if (!data.items || data.items.length === 0) {
            this.showNoResults();
            return;
        }
        
        // Show search stats
        this.searchStats.innerHTML = `
            Found ${data.totalCount.toLocaleString()} results in ${data.searchDuration.toFixed(2)}ms
        `;
        this.searchStats.classList.remove('hidden');
        
        // Display results
        const html = data.items.map(item => this.renderSearchResult(item)).join('');
        this.searchResults.innerHTML = html;
        this.searchResults.classList.remove('hidden');
        
        // Display pagination
        this.displayPagination(data);
        
        // Hide no results
        this.noResults.classList.add('hidden');
    }
    
    renderSearchResult(item) {
        const tags = item.tags.map(tag => `<span class="search-result-tag">${tag}</span>`).join('');
        const entityTypeIcon = this.getEntityTypeIcon(item.entityType);
        
        return `
            <div class="search-result-item">
                <div class="flex items-start justify-between">
                    <div class="flex-1">
                        <div class="flex items-center mb-2">
                            ${entityTypeIcon}
                            <span class="text-xs text-gray-500 dark:text-gray-400 ml-2">${item.entityType}</span>
                            <span class="text-xs text-gray-400 ml-2">•</span>
                            <span class="text-xs text-gray-500 dark:text-gray-400 ml-2">${this.formatDate(item.createdAt)}</span>
                            ${item.authorName ? `<span class="text-xs text-gray-400 ml-2">•</span><span class="text-xs text-gray-500 dark:text-gray-400 ml-2">by ${item.authorName}</span>` : ''}
                        </div>
                        <h3 class="search-result-title">
                            <a href="${item.url}" class="hover:underline">${item.title}</a>
                        </h3>
                        <p class="search-result-description">${item.description}</p>
                        ${tags ? `<div class="search-result-tags">${tags}</div>` : ''}
                    </div>
                    <div class="ml-4 text-right">
                        <div class="text-sm font-medium text-red-500">
                            ${(item.relevanceScore * 10).toFixed(0)}% match
                        </div>
                    </div>
                </div>
            </div>
        `;
    }
    
    getEntityTypeIcon(entityType) {
        const icons = {
            'Post': '<svg class="h-4 w-4 text-blue-500" fill="currentColor" viewBox="0 0 20 20"><path d="M2 5a2 2 0 012-2h7a2 2 0 012 2v4a2 2 0 01-2 2H9l-3 3v-3H4a2 2 0 01-2-2V5z"></path></svg>',
            'Question': '<svg class="h-4 w-4 text-green-500" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-8-3a1 1 0 00-.867.5 1 1 0 11-1.731-1A3 3 0 0113 8a3.001 3.001 0 01-2 2.83V11a1 1 0 11-2 0v-1a1 1 0 011-1 1 1 0 100-2zm0 8a1 1 0 100-2 1 1 0 000 2z" clip-rule="evenodd"></path></svg>',
            'Guide': '<svg class="h-4 w-4 text-purple-500" fill="currentColor" viewBox="0 0 20 20"><path d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>',
            'News': '<svg class="h-4 w-4 text-red-500" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M2 5a2 2 0 012-2h8a2 2 0 012 2v10a2 2 0 002 2H4a2 2 0 01-2-2V5zm3 1h6v4H5V6zm6 6H5v2h6v-2z" clip-rule="evenodd"></path></svg>',
            'Review': '<svg class="h-4 w-4 text-yellow-500" fill="currentColor" viewBox="0 0 20 20"><path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"></path></svg>',
            'Story': '<svg class="h-4 w-4 text-indigo-500" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M4 4a2 2 0 012-2h4.586A2 2 0 0112 2.586L15.414 6A2 2 0 0116 7.414V16a2 2 0 01-2 2H6a2 2 0 01-2-2V4zm2 6a1 1 0 011-1h6a1 1 0 110 2H7a1 1 0 01-1-1zm1 3a1 1 0 100 2h6a1 1 0 100-2H7z" clip-rule="evenodd"></path></svg>',
            'Event': '<svg class="h-4 w-4 text-orange-500" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M6 2a1 1 0 00-1 1v1H4a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V6a2 2 0 00-2-2h-1V3a1 1 0 10-2 0v1H7V3a1 1 0 00-1-1zm0 5a1 1 0 000 2h8a1 1 0 100-2H6z" clip-rule="evenodd"></path></svg>',
            'Group': '<svg class="h-4 w-4 text-teal-500" fill="currentColor" viewBox="0 0 20 20"><path d="M13 6a3 3 0 11-6 0 3 3 0 016 0zM18 8a2 2 0 11-4 0 2 2 0 014 0zM14 15a4 4 0 00-8 0v3h8v-3z"></path></svg>'
        };
        
        return icons[entityType] || '<svg class="h-4 w-4 text-gray-500" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M4 4a2 2 0 012-2h4.586A2 2 0 0112 2.586L15.414 6A2 2 0 0116 7.414V16a2 2 0 01-2 2H6a2 2 0 01-2-2V4z" clip-rule="evenodd"></path></svg>';
    }
    
    displayPagination(data) {
        if (data.totalPages <= 1) {
            this.searchPagination.classList.add('hidden');
            return;
        }
        
        const currentPage = data.page;
        const totalPages = data.totalPages;
        const maxVisiblePages = 5;
        
        let startPage = Math.max(1, currentPage - Math.floor(maxVisiblePages / 2));
        let endPage = Math.min(totalPages, startPage + maxVisiblePages - 1);
        
        if (endPage - startPage + 1 < maxVisiblePages) {
            startPage = Math.max(1, endPage - maxVisiblePages + 1);
        }
        
        let html = '<div class="flex items-center justify-center space-x-2">';
        
        // Previous button
        if (currentPage > 1) {
            html += `<button class="pagination-btn" data-page="${currentPage - 1}">Previous</button>`;
        }
        
        // Page numbers
        for (let i = startPage; i <= endPage; i++) {
            const isActive = i === currentPage;
            html += `<button class="pagination-btn ${isActive ? 'active' : ''}" data-page="${i}">${i}</button>`;
        }
        
        // Next button
        if (currentPage < totalPages) {
            html += `<button class="pagination-btn" data-page="${currentPage + 1}">Next</button>`;
        }
        
        html += '</div>';
        
        this.searchPagination.innerHTML = html;
        this.searchPagination.classList.remove('hidden');
        
        // Add click handlers
        this.searchPagination.querySelectorAll('.pagination-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                const page = parseInt(btn.dataset.page);
                this.performSearch(this.currentQuery, page);
            });
        });
    }
    
    async loadTrendingItems() {
        try {
            const response = await fetch('/api/shared/search/trending?maxResults=6');
            const result = await response.json();
            
            if (result.success && result.data.length > 0) {
                this.displayTrendingItems(result.data);
            }
        } catch (error) {
            console.error('Error loading trending items:', error);
        }
    }
    
    displayTrendingItems(items) {
        const html = items.map(item => `
            <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4 hover:bg-gray-100 dark:hover:bg-gray-600 transition-colors">
                <div class="flex items-center mb-2">
                    ${this.getEntityTypeIcon(item.entityType)}
                    <span class="text-xs text-gray-500 dark:text-gray-400 ml-2">${item.entityType}</span>
                </div>
                <h4 class="font-medium text-gray-900 dark:text-white mb-1">
                    <a href="${item.url}" class="hover:text-red-500 dark:hover:text-red-400">${item.title}</a>
                </h4>
                <p class="text-sm text-gray-600 dark:text-gray-400 mb-2">${item.description}</p>
                <div class="flex items-center justify-between text-xs text-gray-500 dark:text-gray-400">
                    <span>${item.interactionCount} interactions</span>
                    <span>${item.viewCount} views</span>
                </div>
            </div>
        `).join('');
        
        this.trendingItems.innerHTML = html;
    }
    
    getDateRange(range) {
        const now = new Date();
        const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
        
        switch (range) {
            case 'today':
                return { fromDate: today.toISOString(), toDate: null };
            case 'week':
                const weekAgo = new Date(today);
                weekAgo.setDate(weekAgo.getDate() - 7);
                return { fromDate: weekAgo.toISOString(), toDate: null };
            case 'month':
                const monthAgo = new Date(today);
                monthAgo.setMonth(monthAgo.getMonth() - 1);
                return { fromDate: monthAgo.toISOString(), toDate: null };
            case 'year':
                const yearAgo = new Date(today);
                yearAgo.setFullYear(yearAgo.getFullYear() - 1);
                return { fromDate: yearAgo.toISOString(), toDate: null };
            default:
                return { fromDate: null, toDate: null };
        }
    }
    
    formatDate(dateString) {
        const date = new Date(dateString);
        const now = new Date();
        const diffTime = Math.abs(now - date);
        const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
        
        if (diffDays === 1) return 'Yesterday';
        if (diffDays < 7) return `${diffDays} days ago`;
        if (diffDays < 30) return `${Math.ceil(diffDays / 7)} weeks ago`;
        if (diffDays < 365) return `${Math.ceil(diffDays / 30)} months ago`;
        return `${Math.ceil(diffDays / 365)} years ago`;
    }
    
    updateURL(query) {
        const url = new URL(window.location);
        url.searchParams.set('q', query);
        window.history.replaceState({}, '', url);
    }
    
    showLoading() {
        this.searchLoading.classList.remove('hidden');
        this.searchResults.classList.add('hidden');
        this.noResults.classList.add('hidden');
        this.searchStats.classList.add('hidden');
        this.searchPagination.classList.add('hidden');
    }
    
    hideLoading() {
        this.searchLoading.classList.add('hidden');
    }
    
    showNoResults() {
        this.noResults.classList.remove('hidden');
        this.searchResults.classList.add('hidden');
        this.searchStats.classList.add('hidden');
        this.searchPagination.classList.add('hidden');
    }
    
    showError(message) {
        this.searchResults.innerHTML = `
            <div class="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg p-4">
                <div class="flex">
                    <svg class="h-5 w-5 text-red-400" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"></path>
                    </svg>
                    <div class="ml-3">
                        <h3 class="text-sm font-medium text-red-800 dark:text-red-200">Search Error</h3>
                        <p class="mt-1 text-sm text-red-700 dark:text-red-300">${message}</p>
                    </div>
                </div>
            </div>
        `;
        this.searchResults.classList.remove('hidden');
    }
    
    clearResults() {
        this.searchResults.classList.add('hidden');
        this.noResults.classList.add('hidden');
        this.searchStats.classList.add('hidden');
        this.searchPagination.classList.add('hidden');
    }
}

// Initialize search when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new SearchManager();
});

// Add pagination button styles
const style = document.createElement('style');
style.textContent = `
    .pagination-btn {
        @apply px-3 py-2 text-sm font-medium text-gray-500 bg-white border border-gray-300 hover:bg-gray-50 hover:text-gray-700 dark:bg-gray-800 dark:border-gray-600 dark:text-gray-400 dark:hover:bg-gray-700 dark:hover:text-white;
    }
    
    .pagination-btn.active {
        @apply bg-red-500 border-red-500 text-white hover:bg-red-600;
    }
    
    .pagination-btn:first-child {
        @apply rounded-l-md;
    }
    
    .pagination-btn:last-child {
        @apply rounded-r-md;
    }
`;
document.head.appendChild(style);