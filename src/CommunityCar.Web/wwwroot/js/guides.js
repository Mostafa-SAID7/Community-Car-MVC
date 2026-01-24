/**
 * Guides JavaScript functionality
 * Handles guide interactions, bookmarking, rating, and other features
 */

class GuidesManager {
    constructor() {
        this.init();
    }

    init() {
        this.bindEvents();
        this.initializeComponents();
    }

    bindEvents() {
        // Bookmark functionality
        document.addEventListener('click', (e) => {
            if (e.target.closest('[data-action="bookmark"]')) {
                e.preventDefault();
                const button = e.target.closest('[data-action="bookmark"]');
                const guideId = button.dataset.guideId;
                this.toggleBookmark(guideId, button);
            }
        });

        // Rating functionality
        document.addEventListener('click', (e) => {
            if (e.target.closest('[data-action="rate"]')) {
                e.preventDefault();
                const button = e.target.closest('[data-action="rate"]');
                const guideId = button.dataset.guideId;
                const rating = parseInt(button.dataset.rating);
                this.rateGuide(guideId, rating);
            }
        });

        // Share functionality
        document.addEventListener('click', (e) => {
            if (e.target.closest('[data-action="share"]')) {
                e.preventDefault();
                const button = e.target.closest('[data-action="share"]');
                const guideId = button.dataset.guideId;
                this.shareGuide(guideId);
            }
        });

        // Filter form submission
        const filterForm = document.querySelector('#guidesFilterForm');
        if (filterForm) {
            filterForm.addEventListener('submit', (e) => {
                this.handleFilterSubmit(e);
            });
        }

        // Search input with debounce
        const searchInput = document.querySelector('input[name="search"]');
        if (searchInput) {
            let searchTimeout;
            searchInput.addEventListener('input', (e) => {
                clearTimeout(searchTimeout);
                searchTimeout = setTimeout(() => {
                    this.handleSearch(e.target.value);
                }, 500);
            });
        }
    }

    initializeComponents() {
        // Initialize tooltips
        this.initializeTooltips();
        
        // Initialize lazy loading for images
        this.initializeLazyLoading();
        
        // Initialize infinite scroll if needed
        this.initializeInfiniteScroll();
    }

    async toggleBookmark(guideId, button) {
        try {
            const isBookmarked = button.classList.contains('bookmarked');
            const action = isBookmarked ? 'unbookmark' : 'bookmark';
            
            // Optimistic UI update
            this.updateBookmarkUI(button, !isBookmarked);
            
            const response = await fetch(`/guides/${guideId}/${action}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                }
            });

            if (!response.ok) {
                // Revert UI on failure
                this.updateBookmarkUI(button, isBookmarked);
                throw new Error('Failed to update bookmark');
            }

            const result = await response.json();
            this.showNotification(result.message || window.getGuideString('GuideBookmarkedSuccessfully'), 'success');
            
        } catch (error) {
            console.error('Error toggling bookmark:', error);
            this.showNotification(window.getGuideString('Error'), 'error');
        }
    }

    updateBookmarkUI(button, isBookmarked) {
        const icon = button.querySelector('i[data-lucide="bookmark"]');
        const text = button.querySelector('.bookmark-text');
        
        if (isBookmarked) {
            button.classList.add('bookmarked', 'text-yellow-500', 'bg-yellow-500/10');
            button.classList.remove('text-muted-foreground');
            if (icon) icon.classList.add('fill-current');
            if (text) text.textContent = window.getGuideString('UnbookmarkGuide');
        } else {
            button.classList.remove('bookmarked', 'text-yellow-500', 'bg-yellow-500/10');
            button.classList.add('text-muted-foreground');
            if (icon) icon.classList.remove('fill-current');
            if (text) text.textContent = window.getGuideString('BookmarkGuide');
        }
    }

    async rateGuide(guideId, rating) {
        try {
            if (rating < 1 || rating > 5) {
                this.showNotification(window.getGuideString('InvalidRating'), 'error');
                return;
            }

            const response = await fetch(`/guides/${guideId}/rate`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: `rating=${rating}`
            });

            if (!response.ok) {
                throw new Error('Failed to rate guide');
            }

            // Update rating UI
            this.updateRatingUI(rating);
            this.showNotification(window.getGuideString('GuideRatedSuccessfully'), 'success');
            
        } catch (error) {
            console.error('Error rating guide:', error);
            this.showNotification(window.getGuideString('Error'), 'error');
        }
    }

    updateRatingUI(rating) {
        const ratingButtons = document.querySelectorAll('[data-action="rate"]');
        ratingButtons.forEach((button, index) => {
            const star = button.querySelector('i[data-lucide="star"]');
            const starRating = parseInt(button.dataset.rating);
            
            if (starRating <= rating) {
                button.classList.add('text-yellow-500');
                button.classList.remove('text-muted-foreground');
                if (star) star.classList.add('fill-current');
            } else {
                button.classList.remove('text-yellow-500');
                button.classList.add('text-muted-foreground');
                if (star) star.classList.remove('fill-current');
            }
        });
    }

    async shareGuide(guideId) {
        try {
            const guideUrl = `${window.location.origin}/guides/${guideId}`;
            const guideTitle = document.querySelector('h1')?.textContent || 'Check out this guide';
            
            if (navigator.share) {
                await navigator.share({
                    title: guideTitle,
                    url: guideUrl
                });
            } else {
                await navigator.clipboard.writeText(guideUrl);
                this.showNotification(window.getGuideString('LinkCopied'), 'success');
            }
        } catch (error) {
            console.error('Error sharing guide:', error);
            this.showNotification(window.getGuideString('ShareFailed'), 'error');
        }
    }

    handleFilterSubmit(e) {
        e.preventDefault();
        const formData = new FormData(e.target);
        const params = new URLSearchParams();
        
        for (const [key, value] of formData.entries()) {
            if (value) {
                params.append(key, value);
            }
        }
        
        const url = `${window.location.pathname}?${params.toString()}`;
        window.location.href = url;
    }

    handleSearch(searchTerm) {
        const currentUrl = new URL(window.location);
        if (searchTerm) {
            currentUrl.searchParams.set('search', searchTerm);
        } else {
            currentUrl.searchParams.delete('search');
        }
        currentUrl.searchParams.set('page', '1'); // Reset to first page
        
        // Update URL without page reload for better UX
        window.history.pushState({}, '', currentUrl);
        
        // Optionally trigger AJAX search here
        this.performSearch(searchTerm);
    }

    async performSearch(searchTerm) {
        try {
            const params = new URLSearchParams(window.location.search);
            params.set('search', searchTerm);
            params.set('page', '1');
            
            const response = await fetch(`/guides/search?${params.toString()}`, {
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });
            
            if (response.ok) {
                const html = await response.text();
                const parser = new DOMParser();
                const doc = parser.parseFromString(html, 'text/html');
                const newContent = doc.querySelector('.guides-grid');
                const currentContent = document.querySelector('.guides-grid');
                
                if (newContent && currentContent) {
                    currentContent.innerHTML = newContent.innerHTML;
                    this.initializeComponents(); // Re-initialize components for new content
                }
            }
        } catch (error) {
            console.error('Search error:', error);
        }
    }

    initializeTooltips() {
        // Initialize tooltips for buttons and interactive elements
        const tooltipElements = document.querySelectorAll('[data-tooltip]');
        tooltipElements.forEach(element => {
            // Simple tooltip implementation
            element.addEventListener('mouseenter', (e) => {
                this.showTooltip(e.target, e.target.dataset.tooltip);
            });
            
            element.addEventListener('mouseleave', () => {
                this.hideTooltip();
            });
        });
    }

    showTooltip(element, text) {
        const tooltip = document.createElement('div');
        tooltip.className = 'absolute z-50 px-2 py-1 text-xs font-medium text-white bg-gray-900 rounded shadow-lg tooltip';
        tooltip.textContent = text;
        
        document.body.appendChild(tooltip);
        
        const rect = element.getBoundingClientRect();
        tooltip.style.left = `${rect.left + rect.width / 2 - tooltip.offsetWidth / 2}px`;
        tooltip.style.top = `${rect.top - tooltip.offsetHeight - 5}px`;
    }

    hideTooltip() {
        const tooltip = document.querySelector('.tooltip');
        if (tooltip) {
            tooltip.remove();
        }
    }

    initializeLazyLoading() {
        const images = document.querySelectorAll('img[data-src]');
        
        if ('IntersectionObserver' in window) {
            const imageObserver = new IntersectionObserver((entries, observer) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        const img = entry.target;
                        img.src = img.dataset.src;
                        img.classList.remove('lazy');
                        imageObserver.unobserve(img);
                    }
                });
            });
            
            images.forEach(img => imageObserver.observe(img));
        } else {
            // Fallback for browsers without IntersectionObserver
            images.forEach(img => {
                img.src = img.dataset.src;
            });
        }
    }

    initializeInfiniteScroll() {
        const loadMoreButton = document.querySelector('[data-action="load-more"]');
        if (!loadMoreButton) return;
        
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    this.loadMoreGuides();
                }
            });
        }, {
            rootMargin: '100px'
        });
        
        observer.observe(loadMoreButton);
    }

    async loadMoreGuides() {
        const loadMoreButton = document.querySelector('[data-action="load-more"]');
        if (!loadMoreButton || loadMoreButton.disabled) return;
        
        try {
            loadMoreButton.disabled = true;
            loadMoreButton.textContent = window.getGuideString('Loading');
            
            const currentPage = parseInt(loadMoreButton.dataset.page) || 1;
            const nextPage = currentPage + 1;
            
            const params = new URLSearchParams(window.location.search);
            params.set('page', nextPage.toString());
            
            const response = await fetch(`/guides?${params.toString()}`, {
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });
            
            if (response.ok) {
                const html = await response.text();
                const parser = new DOMParser();
                const doc = parser.parseFromString(html, 'text/html');
                const newGuides = doc.querySelectorAll('.guide-card');
                const guidesContainer = document.querySelector('.guides-grid');
                
                newGuides.forEach(guide => {
                    guidesContainer.appendChild(guide);
                });
                
                loadMoreButton.dataset.page = nextPage.toString();
                
                // Check if there are more pages
                const hasMore = doc.querySelector('[data-action="load-more"]');
                if (!hasMore) {
                    loadMoreButton.remove();
                }
            }
        } catch (error) {
            console.error('Error loading more guides:', error);
        } finally {
            if (loadMoreButton) {
                loadMoreButton.disabled = false;
                loadMoreButton.textContent = 'Load More Guides';
            }
        }
    }

    getAntiForgeryToken() {
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
    }

    showNotification(message, type = 'info') {
        // Use existing notification system if available
        if (window.notify) {
            window.notify[type](message);
            return;
        }
        
        // Fallback notification
        const notification = document.createElement('div');
        notification.className = `fixed top-4 right-4 z-50 px-4 py-2 rounded-lg text-white font-medium ${
            type === 'success' ? 'bg-green-500' : 
            type === 'error' ? 'bg-red-500' : 
            'bg-blue-500'
        }`;
        notification.textContent = message;
        
        document.body.appendChild(notification);
        
        setTimeout(() => {
            notification.remove();
        }, 3000);
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.guidesManager = new GuidesManager();
});

// Global functions for backward compatibility
window.toggleBookmark = async function(guideId, button) {
    if (window.guidesManager) {
        await window.guidesManager.toggleBookmark(guideId, button);
    }
};

window.rateGuide = async function(guideId, rating) {
    if (window.guidesManager) {
        await window.guidesManager.rateGuide(guideId, rating);
    }
};

window.shareGuide = async function(guideId) {
    if (window.guidesManager) {
        await window.guidesManager.shareGuide(guideId);
    }
};