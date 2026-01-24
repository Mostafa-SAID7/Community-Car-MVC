// News functionality with enhanced features
class NewsManager {
    constructor() {
        this.initializeEventListeners();
        this.initializeSearch();
        this.initializeSkeletonLoading();
        this.initializeValidation();
    }

    initializeEventListeners() {
        // Search form submission
        const searchForm = document.getElementById('newsSearchForm');
        if (searchForm) {
            searchForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.performSearch();
            });
        }

        // Real-time search (debounced)
        const searchInput = document.querySelector('input[name="search"]');
        if (searchInput) {
            let searchTimeout;
            searchInput.addEventListener('input', (e) => {
                clearTimeout(searchTimeout);
                searchTimeout = setTimeout(() => {
                    this.performSearch();
                }, 500);
            });
        }

        // Filter changes
        document.querySelectorAll('select[name="category"], select[name="sortBy"]').forEach(select => {
            select.addEventListener('change', () => {
                this.performSearch();
            });
        });

        // Like buttons
        document.querySelectorAll('.like-button').forEach(button => {
            button.addEventListener('click', (e) => {
                e.preventDefault();
                const newsId = button.dataset.newsId;
                this.toggleLike(newsId, button);
            });
        });

        // Share buttons
        document.querySelectorAll('.share-button').forEach(button => {
            button.addEventListener('click', (e) => {
                e.preventDefault();
                const newsId = button.dataset.newsId;
                const title = button.dataset.title;
                const url = button.dataset.url;
                this.shareNews(title, url);
            });
        });

        // Bookmark buttons
        document.querySelectorAll('.bookmark-button').forEach(button => {
            button.addEventListener('click', (e) => {
                e.preventDefault();
                const newsId = button.dataset.newsId;
                this.toggleBookmark(newsId, button);
            });
        });
    }

    initializeSearch() {
        // Initialize any search-related functionality
        this.setupInfiniteScroll();
    }

    initializeSkeletonLoading() {
        // Create skeleton loading templates
        this.createSkeletonTemplates();
    }

    initializeValidation() {
        // Enhanced client-side validation
        this.setupFormValidation();
        this.setupRealTimeValidation();
    }

    createSkeletonTemplates() {
        // News card skeleton template
        this.newsCardSkeleton = `
            <div class="bg-card/40 backdrop-blur-md border border-border rounded-2xl shadow-xl overflow-hidden animate-pulse">
                <div class="h-48 bg-muted"></div>
                <div class="p-6">
                    <div class="flex items-center justify-between mb-3">
                        <div class="h-4 bg-muted rounded w-20"></div>
                        <div class="h-3 bg-muted rounded w-16"></div>
                    </div>
                    <div class="space-y-2 mb-3">
                        <div class="h-5 bg-muted rounded w-full"></div>
                        <div class="h-5 bg-muted rounded w-3/4"></div>
                    </div>
                    <div class="space-y-2 mb-4">
                        <div class="h-3 bg-muted rounded w-full"></div>
                        <div class="h-3 bg-muted rounded w-5/6"></div>
                        <div class="h-3 bg-muted rounded w-4/5"></div>
                    </div>
                    <div class="flex items-center justify-between">
                        <div class="flex items-center gap-3">
                            <div class="h-3 bg-muted rounded w-8"></div>
                            <div class="h-3 bg-muted rounded w-8"></div>
                            <div class="h-3 bg-muted rounded w-8"></div>
                        </div>
                        <div class="flex items-center gap-2">
                            <div class="h-6 w-6 bg-muted rounded"></div>
                            <div class="h-6 w-6 bg-muted rounded"></div>
                            <div class="h-6 w-6 bg-muted rounded"></div>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Search results skeleton
        this.searchResultsSkeleton = `
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                ${Array(6).fill(this.newsCardSkeleton).join('')}
            </div>
        `;
    }

    showSkeletonLoading(container) {
        if (container) {
            container.innerHTML = this.searchResultsSkeleton;
        }
    }

    hideSkeletonLoading() {
        // Skeleton will be replaced by actual content
    }

    setupFormValidation() {
        const forms = document.querySelectorAll('form');
        forms.forEach(form => {
            form.addEventListener('submit', (e) => {
                if (!this.validateForm(form)) {
                    e.preventDefault();
                    this.showToast('Please check the form for errors', 'error');
                }
            });
        });
    }

    setupRealTimeValidation() {
        // Real-time validation for form fields
        const inputs = document.querySelectorAll('input[required], textarea[required], select[required]');
        inputs.forEach(input => {
            input.addEventListener('blur', () => {
                this.validateField(input);
            });

            input.addEventListener('input', () => {
                // Clear error state on input
                if (input.classList.contains('border-red-500')) {
                    input.classList.remove('border-red-500');
                    this.removeFieldError(input);
                }
            });
        });
    }

    validateForm(form) {
        let isValid = true;
        const requiredFields = form.querySelectorAll('[required]');
        
        requiredFields.forEach(field => {
            if (!this.validateField(field)) {
                isValid = false;
            }
        });

        // Custom validation rules
        const emailFields = form.querySelectorAll('input[type="email"]');
        emailFields.forEach(field => {
            if (field.value && !this.isValidEmail(field.value)) {
                this.showFieldError(field, 'Please enter a valid email address');
                isValid = false;
            }
        });

        const urlFields = form.querySelectorAll('input[type="url"]');
        urlFields.forEach(field => {
            if (field.value && !this.isValidUrl(field.value)) {
                this.showFieldError(field, 'Please enter a valid URL');
                isValid = false;
            }
        });
        
        return isValid;
    }

    validateField(field) {
        let isValid = true;
        
        // Required field validation
        if (field.hasAttribute('required') && !field.value.trim()) {
            this.showFieldError(field, 'This field is required');
            isValid = false;
        }
        
        // Minimum length validation
        const minLength = field.getAttribute('minlength');
        if (minLength && field.value.length < parseInt(minLength)) {
            this.showFieldError(field, `Minimum length is ${minLength} characters`);
            isValid = false;
        }
        
        // Maximum length validation
        const maxLength = field.getAttribute('maxlength');
        if (maxLength && field.value.length > parseInt(maxLength)) {
            this.showFieldError(field, `Maximum length is ${maxLength} characters`);
            isValid = false;
        }
        
        if (isValid) {
            this.removeFieldError(field);
        }
        
        return isValid;
    }

    showFieldError(field, message) {
        field.classList.add('border-red-500');
        
        // Remove existing error message
        this.removeFieldError(field);
        
        // Add new error message
        const errorDiv = document.createElement('div');
        errorDiv.className = 'text-red-500 text-sm mt-1 field-error';
        errorDiv.textContent = message;
        
        field.parentNode.appendChild(errorDiv);
    }

    removeFieldError(field) {
        field.classList.remove('border-red-500');
        const errorDiv = field.parentNode.querySelector('.field-error');
        if (errorDiv) {
            errorDiv.remove();
        }
    }

    isValidEmail(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }

    isValidUrl(url) {
        try {
            new URL(url);
            return true;
        } catch {
            return false;
        }
    }

    async performSearch() {
        const form = document.getElementById('newsSearchForm');
        if (!form) return;

        const formData = new FormData(form);
        const params = new URLSearchParams(formData);
        const newsContent = document.getElementById('newsContent');

        // Show skeleton loading
        this.showSkeletonLoading(newsContent);

        try {
            const response = await fetch(`/news?${params.toString()}`, {
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (response.ok) {
                const html = await response.text();
                if (newsContent) {
                    // Simulate minimum loading time for better UX
                    setTimeout(() => {
                        newsContent.innerHTML = html;
                        this.reinitializeEventListeners();
                        this.showToast('Search completed', 'success');
                    }, 300);
                }
            } else {
                throw new Error('Search request failed');
            }
        } catch (error) {
            console.error('Search failed:', error);
            this.showToast('Search failed. Please try again.', 'error');
            
            // Show error state
            if (newsContent) {
                newsContent.innerHTML = `
                    <div class="text-center py-12">
                        <i data-lucide="alert-circle" class="w-16 h-16 text-muted-foreground mx-auto mb-4"></i>
                        <h3 class="text-lg font-medium text-foreground mb-2">Search Failed</h3>
                        <p class="text-muted-foreground mb-4">Unable to load search results. Please try again.</p>
                        <button onclick="window.location.reload()" class="btn-primary">
                            <i data-lucide="refresh-cw" class="w-4 h-4 mr-2"></i>
                            Retry
                        </button>
                    </div>
                `;
            }
        }
    }

    reinitializeEventListeners() {
        // Reinitialize event listeners for dynamically loaded content
        this.initializeEventListeners();
    }

    async toggleLike(newsId, button) {
        if (!this.isAuthenticated()) {
            this.showToast('Please log in to like articles', 'warning');
            setTimeout(() => {
                window.location.href = '/login';
            }, 1500);
            return;
        }

        const isLiked = button.classList.contains('liked');
        const action = isLiked ? 'unlike' : 'like';
        
        // Show loading state
        button.disabled = true;
        const originalContent = button.innerHTML;
        button.innerHTML = '<i data-lucide="loader-2" class="w-4 h-4 animate-spin"></i>';

        try {
            const response = await fetch(`/news/${newsId}/${action}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                }
            });

            const result = await response.json();
            if (result.success) {
                // Update button state
                if (isLiked) {
                    button.classList.remove('liked', 'text-red-500');
                    button.classList.add('text-muted-foreground');
                } else {
                    button.classList.add('liked', 'text-red-500');
                    button.classList.remove('text-muted-foreground');
                }

                // Update like count
                const likeCount = button.querySelector('.like-count');
                if (likeCount) {
                    likeCount.textContent = result.likeCount;
                }

                // Show feedback
                this.showToast(isLiked ? 'Article unliked' : 'Article liked', 'success');
            } else {
                this.showToast(result.message || 'An error occurred', 'error');
            }
        } catch (error) {
            console.error('Like toggle failed:', error);
            this.showToast('Network error. Please try again.', 'error');
        } finally {
            // Restore button state
            button.disabled = false;
            button.innerHTML = originalContent;
        }
    }

    shareNews(title, url) {
        if (navigator.share) {
            navigator.share({
                title: title,
                url: url
            }).then(() => {
                this.showToast('Article shared successfully', 'success');
            }).catch(error => {
                if (error.name !== 'AbortError') {
                    console.error('Share failed:', error);
                    this.fallbackShare(url);
                }
            });
        } else {
            this.fallbackShare(url);
        }
    }

    fallbackShare(url) {
        if (navigator.clipboard) {
            navigator.clipboard.writeText(url).then(() => {
                this.showToast('Link copied to clipboard', 'success');
            }).catch(() => {
                this.showShareModal(url);
            });
        } else {
            this.showShareModal(url);
        }
    }

    showShareModal(url) {
        // Enhanced share modal with better styling
        const modal = document.createElement('div');
        modal.className = 'fixed inset-0 bg-black/50 backdrop-blur-sm z-50 flex items-center justify-center p-4';
        modal.innerHTML = `
            <div class="bg-card border border-border rounded-2xl p-6 max-w-md w-full animate-in zoom-in-95 duration-300">
                <div class="flex items-center justify-between mb-4">
                    <h3 class="text-lg font-medium text-foreground">Share this article</h3>
                    <button onclick="this.closest('.fixed').remove()" class="btn-ghost btn-sm">
                        <i data-lucide="x" class="w-4 h-4"></i>
                    </button>
                </div>
                <div class="space-y-4">
                    <div>
                        <label class="text-sm font-medium text-foreground mb-2 block">Article URL</label>
                        <input type="text" value="${url}" readonly class="input w-full" onclick="this.select()">
                    </div>
                    <div>
                        <label class="text-sm font-medium text-foreground mb-2 block">Share on social media</label>
                        <div class="grid grid-cols-3 gap-2">
                            <button onclick="window.open('https://twitter.com/intent/tweet?url=${encodeURIComponent(url)}', '_blank')" class="btn-sm btn-secondary flex items-center justify-center gap-2">
                                <i data-lucide="twitter" class="w-4 h-4"></i>
                                Twitter
                            </button>
                            <button onclick="window.open('https://www.facebook.com/sharer/sharer.php?u=${encodeURIComponent(url)}', '_blank')" class="btn-sm btn-secondary flex items-center justify-center gap-2">
                                <i data-lucide="facebook" class="w-4 h-4"></i>
                                Facebook
                            </button>
                            <button onclick="window.open('https://www.linkedin.com/sharing/share-offsite/?url=${encodeURIComponent(url)}', '_blank')" class="btn-sm btn-secondary flex items-center justify-center gap-2">
                                <i data-lucide="linkedin" class="w-4 h-4"></i>
                                LinkedIn
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `;
        document.body.appendChild(modal);

        // Close on backdrop click
        modal.addEventListener('click', (e) => {
            if (e.target === modal) {
                modal.remove();
            }
        });

        // Initialize Lucide icons for the modal
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }
    }

    async toggleBookmark(newsId, button) {
        if (!this.isAuthenticated()) {
            this.showToast('Please log in to bookmark articles', 'warning');
            setTimeout(() => {
                window.location.href = '/login';
            }, 1500);
            return;
        }

        const isBookmarked = button.classList.contains('bookmarked');
        const action = isBookmarked ? 'unbookmark' : 'bookmark';
        
        // Show loading state
        button.disabled = true;
        const originalContent = button.innerHTML;
        button.innerHTML = '<i data-lucide="loader-2" class="w-4 h-4 animate-spin"></i>';

        try {
            const response = await fetch(`/news/${newsId}/${action}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                }
            });

            const result = await response.json();
            if (result.success) {
                // Update button state
                if (isBookmarked) {
                    button.classList.remove('bookmarked', 'text-yellow-500');
                    button.classList.add('text-muted-foreground');
                } else {
                    button.classList.add('bookmarked', 'text-yellow-500');
                    button.classList.remove('text-muted-foreground');
                }

                // Show feedback
                this.showToast(isBookmarked ? 'Bookmark removed' : 'Article bookmarked', 'success');
            } else {
                this.showToast(result.message || 'An error occurred', 'error');
            }
        } catch (error) {
            console.error('Bookmark toggle failed:', error);
            this.showToast('Network error. Please try again.', 'error');
        } finally {
            // Restore button state
            button.disabled = false;
            button.innerHTML = originalContent;
        }
    }

    setupInfiniteScroll() {
        let loading = false;
        let currentPage = 1;
        const pageSize = 12;

        const loadMore = async () => {
            if (loading) return;
            loading = true;

            try {
                const form = document.getElementById('newsSearchForm');
                const formData = new FormData(form);
                formData.set('page', currentPage + 1);
                formData.set('pageSize', pageSize);
                
                const params = new URLSearchParams(formData);
                const response = await fetch(`/news?${params.toString()}`, {
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });

                if (response.ok) {
                    const html = await response.text();
                    const tempDiv = document.createElement('div');
                    tempDiv.innerHTML = html;
                    
                    const newItems = tempDiv.querySelectorAll('.news-item');
                    if (newItems.length > 0) {
                        const newsGrid = document.querySelector('.news-grid');
                        if (newsGrid) {
                            newItems.forEach(item => newsGrid.appendChild(item));
                            currentPage++;
                            this.reinitializeEventListeners();
                        }
                    }
                }
            } catch (error) {
                console.error('Load more failed:', error);
                this.showToast('Failed to load more articles', 'error');
            } finally {
                loading = false;
            }
        };

        // Intersection Observer for infinite scroll
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting && !loading) {
                    loadMore();
                }
            });
        }, {
            rootMargin: '100px'
        });

        // Observe the last news item
        const observeLastItem = () => {
            const newsItems = document.querySelectorAll('.news-item');
            if (newsItems.length > 0) {
                const lastItem = newsItems[newsItems.length - 1];
                observer.observe(lastItem);
            }
        };

        // Initial observation
        observeLastItem();

        // Re-observe after new content is loaded
        const originalReinitialize = this.reinitializeEventListeners.bind(this);
        this.reinitializeEventListeners = function() {
            originalReinitialize();
            observer.disconnect();
            observeLastItem();
        };
    }

    isAuthenticated() {
        // Check if user is authenticated
        return document.querySelector('meta[name="user-authenticated"]')?.content === 'true' ||
               document.body.classList.contains('authenticated') ||
               document.querySelector('.user-menu') !== null;
    }

    getAntiForgeryToken() {
        return document.querySelector('input[name="__RequestVerificationToken"]')?.value ||
               document.querySelector('meta[name="__RequestVerificationToken"]')?.content;
    }

    showToast(message, type = 'info') {
        // Use the enhanced toaster system
        if (window.ToasterSystem) {
            window.ToasterSystem.show(message, type);
        } else if (window.notify) {
            window.notify[type](message);
        } else {
            // Fallback notification
            console.log(`${type.toUpperCase()}: ${message}`);
            
            // Simple fallback toast
            const toast = document.createElement('div');
            toast.className = `fixed top-4 right-4 z-50 p-4 rounded-lg shadow-lg text-white ${
                type === 'success' ? 'bg-green-500' : 
                type === 'error' ? 'bg-red-500' : 
                type === 'warning' ? 'bg-yellow-500' : 'bg-blue-500'
            }`;
            toast.textContent = message;
            document.body.appendChild(toast);
            
            setTimeout(() => {
                toast.remove();
            }, 3000);
        }
    }
}

// Initialize news manager when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.newsManager = new NewsManager();
});

// Export for use in other scripts
if (typeof module !== 'undefined' && module.exports) {
    module.exports = NewsManager;
}