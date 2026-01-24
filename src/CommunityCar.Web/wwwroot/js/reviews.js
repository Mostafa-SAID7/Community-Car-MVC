// Reviews JavaScript functionality
document.addEventListener('DOMContentLoaded', function() {
    initializeReviews();
});

function initializeReviews() {
    // Initialize search functionality
    initializeSearch();
    
    // Initialize filtering
    initializeFilters();
    
    // Initialize rating interactions
    initializeRatingInteractions();
    
    // Initialize image modal
    initializeImageModal();
    
    // Initialize form validation
    initializeFormValidation();
}

// Search functionality
function initializeSearch() {
    const searchForm = document.getElementById('searchForm');
    const searchInput = document.getElementById('searchInput');
    
    if (searchForm && searchInput) {
        // Auto-submit search after typing stops
        let searchTimeout;
        searchInput.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                searchForm.submit();
            }, 500);
        });
    }
}

// Filter functionality
function initializeFilters() {
    const filterForm = document.getElementById('filterForm');
    const filterInputs = document.querySelectorAll('.filter-input');
    
    filterInputs.forEach(input => {
        input.addEventListener('change', function() {
            if (filterForm) {
                filterForm.submit();
            }
        });
    });
    
    // Clear filters
    const clearFiltersBtn = document.getElementById('clearFilters');
    if (clearFiltersBtn) {
        clearFiltersBtn.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Clear all filter inputs
            filterInputs.forEach(input => {
                if (input.type === 'checkbox') {
                    input.checked = false;
                } else {
                    input.value = '';
                }
            });
            
            // Submit form to clear filters
            if (filterForm) {
                filterForm.submit();
            }
        });
    }
}

// Rating interactions
function initializeRatingInteractions() {
    // Helpful/Not helpful buttons
    document.querySelectorAll('[data-action="helpful"]').forEach(button => {
        button.addEventListener('click', function() {
            const reviewId = this.dataset.reviewId;
            const isHelpful = this.dataset.helpful === 'true';
            markHelpful(reviewId, isHelpful);
        });
    });
    
    // Flag review buttons
    document.querySelectorAll('[data-action="flag"]').forEach(button => {
        button.addEventListener('click', function() {
            const reviewId = this.dataset.reviewId;
            flagReview(reviewId);
        });
    });
}

// Mark review as helpful
function markHelpful(reviewId, isHelpful) {
    const formData = new FormData();
    formData.append('isHelpful', isHelpful);
    formData.append('__RequestVerificationToken', getAntiForgeryToken());
    
    fetch(`/reviews/${reviewId}/helpful`, {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            showNotification(data.message, 'success');
            
            // Update helpful count in UI
            const helpfulBtn = document.querySelector(`[data-review-id="${reviewId}"][data-action="helpful"]`);
            if (helpfulBtn) {
                const countElement = helpfulBtn.querySelector('.helpful-count');
                if (countElement) {
                    const currentCount = parseInt(countElement.textContent) || 0;
                    countElement.textContent = currentCount + (isHelpful ? 1 : -1);
                }
                
                // Disable button to prevent multiple clicks
                helpfulBtn.disabled = true;
                helpfulBtn.classList.add('opacity-50');
            }
        } else {
            showNotification(data.message, 'error');
        }
    })
    .catch(error => {
        console.error('Error marking review as helpful:', error);
        showNotification('An error occurred. Please try again.', 'error');
    });
}

// Flag review
function flagReview(reviewId) {
    if (!confirm('Are you sure you want to flag this review as inappropriate?')) {
        return;
    }
    
    const formData = new FormData();
    formData.append('__RequestVerificationToken', getAntiForgeryToken());
    
    fetch(`/reviews/${reviewId}/flag`, {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            showNotification(data.message, 'success');
            
            // Hide flag button
            const flagBtn = document.querySelector(`[data-review-id="${reviewId}"][data-action="flag"]`);
            if (flagBtn) {
                flagBtn.style.display = 'none';
            }
        } else {
            showNotification(data.message, 'error');
        }
    })
    .catch(error => {
        console.error('Error flagging review:', error);
        showNotification('An error occurred. Please try again.', 'error');
    });
}

// Image modal functionality
function initializeImageModal() {
    // Create modal if it doesn't exist
    if (!document.getElementById('imageModal')) {
        const modal = document.createElement('div');
        modal.id = 'imageModal';
        modal.className = 'fixed inset-0 bg-black bg-opacity-75 flex items-center justify-center z-50 hidden';
        modal.innerHTML = `
            <div class="relative max-w-4xl max-h-full p-4">
                <img id="modalImage" src="" alt="" class="max-w-full max-h-full object-contain">
                <button onclick="closeImageModal()" class="absolute top-4 right-4 text-white bg-black bg-opacity-50 rounded-full p-2 hover:bg-opacity-75">
                    <i data-lucide="x" class="w-6 h-6"></i>
                </button>
            </div>
        `;
        document.body.appendChild(modal);
    }
}

// Open image modal
function openImageModal(imageSrc) {
    const modal = document.getElementById('imageModal');
    const modalImage = document.getElementById('modalImage');
    
    if (modal && modalImage) {
        modalImage.src = imageSrc;
        modal.classList.remove('hidden');
        document.body.style.overflow = 'hidden';
    }
}

// Close image modal
function closeImageModal() {
    const modal = document.getElementById('imageModal');
    
    if (modal) {
        modal.classList.add('hidden');
        document.body.style.overflow = '';
    }
}

// Form validation
function initializeFormValidation() {
    const reviewForm = document.querySelector('form[action*="reviews"]');
    
    if (reviewForm) {
        reviewForm.addEventListener('submit', function(e) {
            if (!validateReviewForm()) {
                e.preventDefault();
            }
        });
        
        // Real-time validation
        const requiredFields = reviewForm.querySelectorAll('[required]');
        requiredFields.forEach(field => {
            field.addEventListener('blur', function() {
                validateField(this);
            });
        });
    }
}

// Validate review form
function validateReviewForm() {
    let isValid = true;
    const form = document.querySelector('form[action*="reviews"]');
    
    if (!form) return true;
    
    // Validate required fields
    const requiredFields = form.querySelectorAll('[required]');
    requiredFields.forEach(field => {
        if (!validateField(field)) {
            isValid = false;
        }
    });
    
    // Validate rating
    const ratingField = form.querySelector('[name="Rating"]');
    if (ratingField && (!ratingField.value || ratingField.value < 1 || ratingField.value > 5)) {
        showFieldError(ratingField, 'Please select a rating between 1 and 5 stars.');
        isValid = false;
    }
    
    // Validate comment length
    const commentField = form.querySelector('[name="Comment"]');
    if (commentField && commentField.value.length < 10) {
        showFieldError(commentField, 'Review comment must be at least 10 characters long.');
        isValid = false;
    }
    
    return isValid;
}

// Validate individual field
function validateField(field) {
    const value = field.value.trim();
    let isValid = true;
    
    // Clear previous errors
    clearFieldError(field);
    
    // Check required fields
    if (field.hasAttribute('required') && !value) {
        showFieldError(field, 'This field is required.');
        isValid = false;
    }
    
    // Check email format
    if (field.type === 'email' && value && !isValidEmail(value)) {
        showFieldError(field, 'Please enter a valid email address.');
        isValid = false;
    }
    
    // Check URL format
    if (field.type === 'url' && value && !isValidUrl(value)) {
        showFieldError(field, 'Please enter a valid URL.');
        isValid = false;
    }
    
    return isValid;
}

// Show field error
function showFieldError(field, message) {
    clearFieldError(field);
    
    const errorElement = document.createElement('div');
    errorElement.className = 'field-error text-red-500 text-sm mt-1';
    errorElement.textContent = message;
    
    field.parentNode.appendChild(errorElement);
    field.classList.add('border-red-500');
}

// Clear field error
function clearFieldError(field) {
    const existingError = field.parentNode.querySelector('.field-error');
    if (existingError) {
        existingError.remove();
    }
    field.classList.remove('border-red-500');
}

// Utility functions
function getAntiForgeryToken() {
    const token = document.querySelector('input[name="__RequestVerificationToken"]');
    return token ? token.value : '';
}

function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

function isValidUrl(url) {
    try {
        new URL(url);
        return true;
    } catch {
        return false;
    }
}

function showNotification(message, type = 'info') {
    // Create notification element
    const notification = document.createElement('div');
    notification.className = `fixed top-4 right-4 p-4 rounded-lg shadow-lg z-50 ${
        type === 'success' ? 'bg-green-500 text-white' :
        type === 'error' ? 'bg-red-500 text-white' :
        'bg-blue-500 text-white'
    }`;
    notification.textContent = message;
    
    document.body.appendChild(notification);
    
    // Auto-remove after 5 seconds
    setTimeout(() => {
        if (notification.parentNode) {
            notification.parentNode.removeChild(notification);
        }
    }, 5000);
}

// Load more reviews (for infinite scroll)
function loadMoreReviews() {
    const loadMoreBtn = document.getElementById('loadMoreBtn');
    const currentPage = parseInt(loadMoreBtn.dataset.currentPage) || 1;
    const nextPage = currentPage + 1;
    
    loadMoreBtn.disabled = true;
    loadMoreBtn.textContent = 'Loading...';
    
    const url = new URL(window.location);
    url.searchParams.set('page', nextPage);
    
    fetch(url.toString(), {
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        }
    })
    .then(response => response.text())
    .then(html => {
        const reviewsList = document.getElementById('reviewsList');
        if (reviewsList) {
            reviewsList.insertAdjacentHTML('beforeend', html);
            loadMoreBtn.dataset.currentPage = nextPage;
        }
        
        // Check if there are more pages
        const hasMorePages = document.querySelector('[data-has-more-pages]');
        if (!hasMorePages || hasMorePages.dataset.hasMorePages === 'false') {
            loadMoreBtn.style.display = 'none';
        } else {
            loadMoreBtn.disabled = false;
            loadMoreBtn.textContent = 'Load More Reviews';
        }
    })
    .catch(error => {
        console.error('Error loading more reviews:', error);
        loadMoreBtn.disabled = false;
        loadMoreBtn.textContent = 'Load More Reviews';
        showNotification('Error loading more reviews. Please try again.', 'error');
    });
}

// Export functions for global access
window.markHelpful = markHelpful;
window.flagReview = flagReview;
window.openImageModal = openImageModal;
window.closeImageModal = closeImageModal;
window.loadMoreReviews = loadMoreReviews;