// Feed JavaScript Functionality

let currentPage = 1;
let isLoading = false;
let feedType = 'personalized';

// Initialize feed functionality
function initializeFeed() {
    feedType = new URLSearchParams(window.location.search).get('feedType') || 'personalized';
    
    // Mark posts as seen when they come into view
    observePostVisibility();
    
    // Auto-refresh stories every 30 seconds
    setInterval(refreshStories, 30000);
    
    // Initialize infinite scroll
    initializeInfiniteScroll();
}

// Story functionality (now handled by stories.js)
function refreshStories() {
    if (window.storiesFunctions && window.storiesFunctions.refreshStories) {
        window.storiesFunctions.refreshStories();
    }
}

// Content interaction functions
function toggleLike(contentId, contentType) {
    const button = event.target.closest('.action-btn');
    const isLiked = button.classList.contains('liked');
    
    fetch('/api/feed/interact', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            contentId: contentId,
            contentType: contentType,
            interactionType: isLiked ? 'unlike' : 'like'
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            button.classList.toggle('liked');
            updateLikeCount(contentId, !isLiked);
            showNotification(
                isLiked ? (window.feedLocalizer?.RemovedLike || 'Removed like') : (window.feedLocalizer?.Liked || 'Liked!'),
                'success'
            );
        } else {
            showNotification(window.feedLocalizer?.FailedToUpdateLike || 'Failed to update like', 'error');
        }
    })
    .catch(error => {
        console.error('Error toggling like:', error);
        showNotification(window.feedLocalizer?.ErrorUpdatingLike || 'Error updating like', 'error');
    });
}

function toggleComments(contentId) {
    const commentsSection = document.getElementById(`comments-${contentId}`);
    if (commentsSection.style.display === 'none') {
        commentsSection.style.display = 'block';
        loadComments(contentId);
    } else {
        commentsSection.style.display = 'none';
    }
}

function shareContent(contentId, contentType) {
    // First record the share interaction
    fetch('/api/feed/interact', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            contentId: contentId,
            contentType: contentType,
            interactionType: 'share'
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            // Then handle the actual sharing
            if (navigator.share) {
                navigator.share({
                    title: 'Check out this post',
                    url: window.location.href + `#post-${contentId}`
                });
            } else {
                // Fallback: copy to clipboard
                navigator.clipboard.writeText(window.location.href + `#post-${contentId}`)
                    .then(() => {
                        showNotification(window.feedLocalizer?.LinkCopiedToClipboard || 'Link copied to clipboard!', 'success');
                    })
                    .catch(() => {
                        showNotification(window.feedLocalizer?.UnableToCopyLink || 'Unable to copy link', 'error');
                    });
            }
        } else {
            showNotification(window.feedLocalizer?.FailedToShare || 'Failed to share content', 'error');
        }
    })
    .catch(error => {
        console.error('Error sharing content:', error);
        showNotification(window.feedLocalizer?.ErrorSharingContent || 'Error sharing content', 'error');
    });
}

function bookmarkContent(contentId, contentType) {
    const button = event.target.closest('.action-btn, .dropdown-item');
    const isBookmarked = button.classList.contains('bookmarked');
    
    fetch('/api/feed/bookmark', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            contentId: contentId,
            contentType: contentType
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            if (button.classList.contains('action-btn')) {
                button.classList.toggle('bookmarked');
            }
            showNotification(
                isBookmarked ? (window.feedLocalizer?.RemovedFromBookmarks || 'Removed from bookmarks') : (window.feedLocalizer?.AddedToBookmarks || 'Added to bookmarks'),
                'success'
            );
        } else {
            showNotification(window.feedLocalizer?.FailedToBookmark || 'Failed to bookmark content', 'error');
        }
    })
    .catch(error => {
        console.error('Error bookmarking content:', error);
        showNotification(window.feedLocalizer?.ErrorBookmarkingContent || 'Error bookmarking content', 'error');
    });
}

function hideContent(contentId) {
    fetch('/api/feed/hide', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            contentId: contentId,
            contentType: 'Post'
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            const feedItem = document.querySelector(`[data-content-id="${contentId}"]`);
            if (feedItem) {
                feedItem.style.animation = 'slideUp 0.3s ease-out reverse';
                setTimeout(() => {
                    feedItem.remove();
                }, 300);
            }
            showNotification(window.feedLocalizer?.ContentHidden || 'Content hidden', 'success');
        } else {
            showNotification(window.feedLocalizer?.FailedToHideContent || 'Failed to hide content', 'error');
        }
    })
    .catch(error => {
        console.error('Error hiding content:', error);
        showNotification(window.feedLocalizer?.ErrorHidingContent || 'Error hiding content', 'error');
    });
}

function reportContent(contentId, contentType) {
    // Show a simple prompt for report reason
    const reason = prompt(window.feedLocalizer?.ReportReason || 'Please provide a reason for reporting this content:');
    if (!reason || reason.trim() === '') {
        return;
    }

    fetch('/api/feed/report', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            contentId: contentId,
            contentType: contentType,
            reason: reason.trim()
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            showNotification(window.feedLocalizer?.ContentReported || 'Content reported. Thank you for helping keep our community safe.', 'success');
        } else {
            showNotification(window.feedLocalizer?.FailedToReportContent || 'Failed to report content', 'error');
        }
    })
    .catch(error => {
        console.error('Error reporting content:', error);
        showNotification(window.feedLocalizer?.ErrorReportingContent || 'Error reporting content', 'error');
    });
}

// Comment functionality
function handleCommentSubmit(event, contentId, contentType) {
    if (event.key === 'Enter' && event.target.value.trim()) {
        const comment = event.target.value.trim();
        submitComment(contentId, contentType, comment)
            .then(success => {
                if (success) {
                    event.target.value = '';
                    loadComments(contentId);
                    updateCommentCount(contentId, 1);
                }
            });
    }
}

function submitComment(contentId, contentType, comment) {
    return fetch('/feed/api/comments', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            contentId: contentId,
            contentType: contentType,
            comment: comment
        })
    })
    .then(response => response.json())
    .then(data => data.success)
    .catch(error => {
        console.error('Error submitting comment:', error);
        return false;
    });
}

function loadComments(contentId) {
    fetch(`/feed/api/comments/${contentId}`)
        .then(response => response.json())
        .then(comments => {
            const commentsList = document.querySelector(`#comments-${contentId} .comments-list`);
            if (commentsList) {
                commentsList.innerHTML = comments.map(createCommentElement).join('');
            }
        })
        .catch(error => {
            console.error('Error loading comments:', error);
        });
}

function createCommentElement(comment) {
    return `
        <div class="comment mb-3">
            <div class="d-flex">
                <div class="user-avatar me-2">
                    <img src="${comment.authorAvatar || '/images/default-avatar.png'}" alt="${comment.authorName}" />
                </div>
                <div class="comment-content">
                    <div class="comment-bubble">
                        <strong>${comment.authorName}</strong>
                        <p class="mb-1">${comment.content}</p>
                    </div>
                    <small class="text-muted">${comment.timeAgo}</small>
                </div>
            </div>
        </div>
    `;
}

// API interaction functions
function interactWithContent(contentId, contentType, interactionType) {
    return fetch('/feed/api/interact', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            contentId: contentId,
            contentType: contentType,
            interactionType: interactionType
        })
    })
    .then(response => response.json())
    .then(data => data.success)
    .catch(error => {
        console.error('Error interacting with content:', error);
        return false;
    });
}

function markContentAsSeen(contentId, contentType) {
    fetch('/feed/api/mark-seen', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            contentId: contentId,
            contentType: contentType
        })
    })
    .catch(error => {
        console.error('Error marking content as seen:', error);
    });
}

// Infinite scroll and loading
function initializeInfiniteScroll() {
    window.addEventListener('scroll', () => {
        if (window.innerHeight + window.scrollY >= document.body.offsetHeight - 1000) {
            loadMoreContent();
        }
    });
}

function loadMoreContent() {
    if (isLoading) return;
    
    isLoading = true;
    const loadButton = document.querySelector('button[onclick="loadMoreContent()"]');
    if (loadButton) {
        const originalContent = loadButton.innerHTML;
        loadButton.innerHTML = `<i class="fas fa-spinner fa-spin w-5 h-5"></i> ${window.feedLocalizer?.Loading || 'Loading...'}`;
        loadButton.disabled = true;
        
        // Show skeleton loading
        showSkeletonLoading(10);
        
        currentPage++;
        
        fetch(`/feed?feedType=${feedType}&page=${currentPage}&pageSize=10`)
            .then(response => response.text())
            .then(html => {
                // Parse the HTML to extract feed items
                const parser = new DOMParser();
                const doc = parser.parseFromString(html, 'text/html');
                const newItems = doc.querySelectorAll('[data-content-id]');
                
                // Remove skeleton loading
                removeSkeletonLoading();
                
                // Append new items
                const container = document.getElementById('feed-container') || document.querySelector('.flex.flex-col.gap-12');
                newItems.forEach(item => {
                    container.appendChild(item.cloneNode(true));
                });
                
                // Check if there are more items
                const hasMore = doc.querySelector('.py-12.text-center button');
                if (!hasMore) {
                    loadButton.style.display = 'none';
                }
            })
            .catch(error => {
                console.error('Error loading more content:', error);
                currentPage--; // Revert page increment on error
                removeSkeletonLoading();
            })
            .finally(() => {
                isLoading = false;
                loadButton.innerHTML = originalContent;
                loadButton.disabled = false;
            });
    }
}

function showSkeletonLoading(count) {
    const container = document.getElementById('feed-container') || document.querySelector('.flex.flex-col.gap-12');
    const template = document.getElementById('skeleton-template');
    
    if (template && container) {
        for (let i = 0; i < count; i++) {
            const skeleton = template.cloneNode(true);
            skeleton.id = `skeleton-${i}`;
            skeleton.classList.remove('hidden');
            skeleton.classList.add('skeleton-item');
            container.appendChild(skeleton);
        }
    }
}

function removeSkeletonLoading() {
    document.querySelectorAll('.skeleton-item').forEach(skeleton => {
        skeleton.remove();
    });
}

function appendFeedItems(feedItems) {
    const feedContainer = document.querySelector('.feed-items');
    if (!feedContainer) return;
    
    feedItems.forEach(item => {
        const feedItemElement = createFeedItemElement(item);
        feedContainer.appendChild(feedItemElement);
    });
}

function createFeedItemElement(item) {
    // This would be a complex function to create the full feed item HTML
    // For now, return a simplified version
    const div = document.createElement('div');
    div.className = 'card feed-item mb-4 animate-fade-in';
    div.setAttribute('data-content-id', item.id);
    div.setAttribute('data-content-type', item.contentType);
    
    // TODO: Implement full feed item HTML generation
    div.innerHTML = `
        <div class="card-body">
            <h5 class="card-title">${item.title}</h5>
            <p class="card-text">${item.summary || item.content.substring(0, 200)}...</p>
        </div>
    `;
    
    return div;
}

// Post visibility observer for marking as seen
function observePostVisibility() {
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const contentId = entry.target.getAttribute('data-content-id');
                const contentType = entry.target.getAttribute('data-content-type');
                if (contentId && contentType) {
                    markContentAsSeen(contentId, contentType);
                }
            }
        });
    }, {
        threshold: 0.5,
        rootMargin: '0px 0px -100px 0px'
    });
    
    document.querySelectorAll('.feed-item').forEach(item => {
        observer.observe(item);
    });
}

// Create post modal functions
function openCreatePostModal(type = 'text') {
    // TODO: Implement create post modal
    console.log('Opening create post modal for type:', type);
}

// Friend request functions
function sendFriendRequest(userId) {
    fetch('/api/friends/request', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ userId: userId })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            const button = event.target.closest('button');
            button.innerHTML = '<i class="fas fa-clock"></i>';
            button.disabled = true;
            button.classList.remove('btn-primary');
            button.classList.add('btn-secondary');
            showNotification(window.feedLocalizer?.FriendRequestSent || 'Friend request sent!', 'success');
        }
    })
    .catch(error => {
        console.error('Error sending friend request:', error);
        showNotification(window.feedLocalizer?.ErrorSendingFriendRequest || 'Error sending friend request', 'error');
    });
}

// Image modal functions
function openImageModal(imageUrl) {
    // TODO: Implement image modal/lightbox
    console.log('Opening image modal for:', imageUrl);
}

// Utility functions
function updateLikeCount(contentId, increment) {
    const feedItem = document.querySelector(`[data-content-id="${contentId}"]`);
    if (feedItem) {
        const likeCountElement = feedItem.querySelector('.stat-item .fa-heart').nextSibling;
        if (likeCountElement) {
            const currentCount = parseInt(likeCountElement.textContent.trim()) || 0;
            likeCountElement.textContent = ` ${currentCount + (increment ? 1 : -1)}`;
        }
    }
}

function updateCommentCount(contentId, increment) {
    const feedItem = document.querySelector(`[data-content-id="${contentId}"]`);
    if (feedItem) {
        const commentCountElement = feedItem.querySelector('.stats-right .stat-item');
        if (commentCountElement) {
            const currentCount = parseInt(commentCountElement.textContent.split(' ')[0]) || 0;
            commentCountElement.textContent = `${currentCount + increment} comments`;
        }
    }
}

function refreshFeedStats() {
    fetch('/feed/api/stats')
        .then(response => response.json())
        .then(stats => {
            updateStatsDisplay(stats);
        })
        .catch(error => {
            console.error('Error refreshing feed stats:', error);
        });
}

function updateStatsDisplay(stats) {
    // Update stats in sidebar
    const statsCard = document.querySelector('.sidebar-card .stat-row');
    if (statsCard) {
        // TODO: Update individual stat values
    }
}

function showNotification(message, type = 'info') {
    // Use the global notification system
    if (window.notify && typeof window.notify[type] === 'function') {
        window.notify[type](message);
    } else {
        // Fallback to console if notification system isn't loaded yet
        console.log(`${type.toUpperCase()}: ${message}`);
    }
}

// Export functions for global access
window.feedFunctions = {
    toggleLike,
    toggleComments,
    shareContent,
    bookmarkContent,
    hideContent,
    reportContent,
    handleCommentSubmit,
    openCreatePostModal,
    sendFriendRequest,
    openImageModal,
    loadMoreContent
};

// Make functions globally accessible
Object.assign(window, window.feedFunctions);