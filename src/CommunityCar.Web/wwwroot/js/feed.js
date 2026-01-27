// Feed JavaScript Functionality

let currentPage = 1;
let isLoading = false;
let feedType = 'personalized';

// Initialize feed functionality
function initializeFeed() {
    feedType = new URLSearchParams(window.location.search).get('feedType') || 'personalized';

    // Mark posts as seen when they come into view
    observePostVisibility();

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
function toggleLike(contentId, contentType, event) {
    const button = event.target.closest('.action-btn');
    const isLiked = button.classList.contains('liked');

    fetch('/feed/interact', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
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
    if (commentsSection.classList.contains('hidden')) {
        commentsSection.classList.remove('hidden');
        commentsSection.classList.add('block');
        loadComments(contentId);
    } else {
        commentsSection.classList.add('hidden');
        commentsSection.classList.remove('block');
    }
}

function shareContent(contentId, contentType) {
    // First record the share interaction
    fetch('/feed/interact', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
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
                const url = window.location.href + `#post-${contentId}`;
                if (navigator.share) {
                    navigator.share({
                        title: 'Check out this post',
                        url: url
                    })
                        .catch(console.error);
                } else {
                    // Fallback: copy to clipboard
                    navigator.clipboard.writeText(url)
                        .then(() => {
                            window.AlertModal.success(window.feedLocalizer?.LinkCopiedToClipboard || 'Link copied to clipboard!', 'Share');
                        })
                        .catch(() => {
                            window.AlertModal.error(window.feedLocalizer?.UnableToCopyLink || 'Unable to copy link', 'Share Error');
                        });
                }
            } else {
                window.AlertModal.error(window.feedLocalizer?.FailedToShare || 'Failed to share content', 'Share Error');
            }
        })
        .catch(error => {
            console.error('Error sharing content:', error);
            window.AlertModal.error(window.feedLocalizer?.ErrorSharingContent || 'Error sharing content', 'Share Error');
        });
}

function bookmarkContent(contentId, contentType, event) {
    const button = event.target.closest('.action-btn, .dropdown-item');

    fetch('/feed/bookmark', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        },
        body: JSON.stringify({
            contentId: contentId,
            contentType: contentType
        })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const isBookmarked = data.isBookmarked; // Assuming API returns current state
                if (button && button.classList.contains('action-btn')) {
                    button.classList.toggle('bookmarked', isBookmarked);
                }

                const message = isBookmarked
                    ? (window.feedLocalizer?.AddedToBookmarks || 'Added to Bookmarks')
                    : (window.feedLocalizer?.RemovedFromBookmarks || 'Removed from Bookmarks');

                window.AlertModal.success(message, 'Bookmarks');
            } else {
                window.AlertModal.error(window.feedLocalizer?.FailedToBookmark || 'Failed to update bookmark', 'Error');
            }
        })
        .catch(error => {
            console.error('Error bookmarking content:', error);
            window.AlertModal.error(window.feedLocalizer?.ErrorBookmarkingContent || 'Error updating bookmark', 'Error');
        });
}

function hideContent(contentId) {
    const confirmMessage = window.feedLocalizer?.ConfirmHideContent || 'Are you sure you want to hide this content?';

    window.AlertModal.confirm(confirmMessage, 'Hide Content', function (confirmed) {
        if (confirmed) {
            fetch('/feed/hide', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
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
                        window.AlertModal.success(window.feedLocalizer?.ContentHidden || 'Content hidden successfully', 'Success');
                    } else {
                        window.AlertModal.error(window.feedLocalizer?.FailedToHideContent || 'Failed to hide content', 'Error');
                    }
                })
                .catch(error => {
                    console.error('Error hiding content:', error);
                    window.AlertModal.error(window.feedLocalizer?.ErrorHidingContent || 'Error hiding content', 'Error');
                });
        }
    });
}

function reportContent(contentId, contentType) {
    const promptMessage = window.feedLocalizer?.ReportReason || 'Please provide a reason for reporting this content:';

    window.AlertModal.prompt(promptMessage, 'Report Content', function (reason) {
        if (reason && typeof reason === 'string' && reason.trim() !== '') {
            fetch('/feed/report', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
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
                        window.AlertModal.success(window.feedLocalizer?.ContentReported || 'Content reported. Thank you for helping keep our community safe.', 'Report Submitted');
                    } else {
                        window.AlertModal.error(window.feedLocalizer?.FailedToReportContent || 'Failed to report content', 'Error');
                    }
                })
                .catch(error => {
                    console.error('Error reporting content:', error);
                    window.AlertModal.error(window.feedLocalizer?.ErrorReportingContent || 'Error reporting content', 'Error');
                });
        }
    }, 'Enter report reason...');
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
    return fetch('/Comments/add', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        },
        body: JSON.stringify({
            EntityId: contentId,
            EntityType: contentType,
            Content: comment
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
    // We need contentType for the . Try to find it from DOM
    const item = document.querySelector(`[data-content-id="${contentId}"]`);
    const contentType = item ? item.dataset.contentType : 'Post'; // Default to Post

    fetch(`/Comments/get/${contentId}?entityType=${contentType}`, {
        headers: {
            'Accept': 'application/json'
        }
    })
        .then(response => response.json())
        .then(response => {
            if (response.success && response.data) {
                const comments = response.data;
                const commentsList = document.querySelector(`#comments-${contentId} .comments-list`);
                if (commentsList) {
                    commentsList.innerHTML = comments.map(createCommentElement).join('');
                }
            }
        })
        .catch(error => {
            console.error('Error loading comments:', error);
        });
}

function createCommentElement(comment) {
    const isReply = comment.parentCommentId != null;
    const itemClass = isReply ? 'reply-item flex gap-3 p-3 bg-muted/10 rounded-lg' : 'comment-item flex gap-3 p-4 bg-muted/30 rounded-lg';
    const avatarSize = isReply ? 'w-8 h-8' : 'w-10 h-10';

    // Format mentions in content
    let content = comment.content;
    if (content) {
        content = content.replace(/@([\w\s]+)/g, '<span class="text-primary font-bold cursor-pointer hover:underline">@$1</span>');
    }

    if (isReply) {
        return `
            <div class="reply-item flex gap-3 p-3 bg-muted/10 rounded-lg animate-in fade-in slide-in-from-top-2 duration-300" data-comment-id="${comment.id}">
                <div class="flex-shrink-0">
                    <img src="${comment.authorAvatar || '/images/default-avatar.png'}" 
                         alt="${comment.authorName}" 
                         class="w-8 h-8 rounded-full object-cover" />
                </div>
                <div class="flex-1 text-sm">
                    <div class="flex items-center gap-2 mb-1">
                        <span class="font-medium text-foreground">${comment.authorName}</span>
                        <span class="text-xs text-muted-foreground">${comment.timeAgo}</span>
                    </div>
                    <div class="text-foreground mb-1">
                        ${content}
                    </div>
                    <div class="flex items-center gap-3">
                        <button class="text-muted-foreground hover:text-primary transition-colors reply-btn text-[10px] uppercase font-bold" 
                                data-comment-id="${comment.parentCommentId}"
                                data-author-name="${comment.authorName}">
                            Reply
                        </button>
                    </div>
                </div>
            </div>
        `;
    }

    return `
        <div class="comment-item-container space-y-3 animate-in fade-in slide-in-from-top-2 duration-300" data-comment-id="${comment.id}">
            <div class="comment-item flex gap-3 p-4 bg-muted/30 rounded-lg" data-comment-id="${comment.id}">
                <div class="flex-shrink-0">
                    <img src="${comment.authorAvatar || '/images/default-avatar.png'}" 
                         alt="${comment.authorName}" 
                         class="w-10 h-10 rounded-full object-cover" />
                </div>
                <div class="flex-1">
                    <div class="flex items-center gap-2 mb-2">
                        <span class="font-medium text-foreground">${comment.authorName}</span>
                        <span class="text-sm text-muted-foreground">${comment.timeAgo}</span>
                    </div>
                    <div class="text-foreground mb-2">
                        ${content}
                    </div>
                    <div class="flex items-center gap-4 text-sm">
                        <button class="text-muted-foreground hover:text-primary transition-colors reply-btn" data-comment-id="${comment.id}" data-author-name="${comment.authorName}">
                            <i data-lucide="reply" class="w-4 h-4 inline mr-1"></i>
                            Reply
                        </button>
                    </div>
                </div>
            </div>
            <div class="replies-container ml-8 space-y-3 border-l-2 border-muted pl-4"></div>
        </div>
    `;
}

// API interaction functions
function interactWithContent(contentId, contentType, interactionType) {
    return fetch('/feed/interact', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
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
    fetch('/feed/mark-seen', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
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

        fetch(`/feed?feedType=${feedType}&page=${currentPage}&pageSize=10`, {
            headers: {
                'Accept': 'application/json'
            }
        })
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

                // Reinitialize shared interactions for new items
                if (window.sharedInteractions) {
                    window.sharedInteractions.initializeCounters();
                }

                // Check if there are more items
                const hasMore = doc.querySelector('.py-12.text-center button');
                if (!hasMore) {
                    loadButton.classList.add('hidden');
                }

                // Re-initialize icons
                if (typeof lucide !== 'undefined') {
                    lucide.createIcons();
                }

                // Re-observe for visibility
                observePostVisibility();
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

function loadMoreFeedContent() {
    loadMoreContent();
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
    fetch('/friends/send-request', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        },
        body: JSON.stringify({ receiverId: userId })
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
    fetch('/feed/stats', {
        headers: {
            'Accept': 'application/json'
        }
    })
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
    if (window.AlertModal) {
        switch (type) {
            case 'success': window.AlertModal.success(message); break;
            case 'error': window.AlertModal.error(message); break;
            case 'warning': window.AlertModal.warning(message); break;
            default: window.AlertModal.info(message); break;
        }
    } else if (window.notify && typeof window.notify[type] === 'function') {
        window.notify[type](message);
    } else {
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

// Initialize SignalR for Real-time Comments using the shared global connection
document.addEventListener('DOMContentLoaded', () => {
    const checkConnection = setInterval(() => {
        if (typeof signalR !== 'undefined' && window.notificationConnection && window.notificationConnection.state === signalR.HubConnectionState.Connected) {
            clearInterval(checkConnection);
            initializeRealTimeComments();
        }
    }, 500);
});

function initializeRealTimeComments() {
    console.log("🚀 Initializing Real-time Comments (using shared connection)");

    joinFeedGroups();

    // Listen for new items being added to the DOM to join their groups
    const observer = new MutationObserver((mutations) => {
        let shouldJoin = false;
        mutations.forEach(mutation => {
            if (mutation.addedNodes.length) {
                mutation.addedNodes.forEach(node => {
                    if (node.nodeType === 1 && (node.hasAttribute('data-content-id') || node.querySelector('[data-content-id]'))) {
                        shouldJoin = true;
                    }
                });
            }
        });
        if (shouldJoin) joinFeedGroups();
    });

    const feedContainer = document.getElementById('feed-container');
    if (feedContainer) {
        observer.observe(feedContainer, { childList: true, subtree: true });
    }

    const connection = window.notificationConnection;

    connection.on("ReceiveComment", (comment) => {
        console.log("📨 Real-time comment received:", comment);

        // Check if this comment already exists in DOM (to avoid duplication for the sender)
        if (document.querySelector(`[data-comment-id="${comment.id}"]`)) {
            console.log("⚠️ Comment already exists in DOM, skipping.");
            return;
        }

        let appended = false;
        if (comment.parentCommentId) {
            // Find the parent's replies container
            const parentItem = document.querySelector(`.comment-item[data-comment-id="${comment.parentCommentId}"]`);
            if (parentItem) {
                const parentContainer = parentItem.closest('.comment-item-container');
                let repliesList = parentContainer.querySelector('.replies-container');
                if (!repliesList) {
                    repliesList = document.createElement('div');
                    repliesList.className = 'replies-container ml-8 space-y-3 border-l-2 border-muted pl-4 mt-3';
                    parentContainer.appendChild(repliesList);
                }

                const html = createCommentElement(comment);
                repliesList.insertAdjacentHTML('beforeend', html);
                appended = true;
            }
        } else {
            const commentsSection = document.getElementById(`comments-${comment.entityId}`);
            const commentsList = commentsSection ? commentsSection.querySelector('.comments-list') : null;

            if (commentsList) {
                // Remove empty state if exists
                const emptyState = commentsList.querySelector('.text-center.py-8');
                if (emptyState) emptyState.remove();

                const html = createCommentElement(comment);
                commentsList.insertAdjacentHTML('afterbegin', html); // Prepend top-level comments
                appended = true;
            }
        }

        if (appended) {
            // Update count visually
            updateCommentCount(comment.entityId, 1);

            // Re-initialize icons
            if (typeof lucide !== 'undefined') {
                lucide.createIcons();
            }

            // Show a brief toast if the user is not the author
            const currentUserId = document.querySelector('meta[name="user-id"]')?.content;
            if (comment.authorId !== currentUserId) {
                showNotification(`New comment from ${comment.authorName}`, 'info');
            }
        }
    });
}

function joinFeedGroups() {
    const connection = window.notificationConnection;
    if (typeof signalR === 'undefined' || !connection || connection.state !== signalR.HubConnectionState.Connected) return;

    document.querySelectorAll('[data-content-id]:not([data-group-joined])').forEach(el => {
        const id = el.dataset.contentId;
        const type = el.dataset.contentType;
        if (id && type) {
            connection.invoke("JoinEntityGroup", type, id)
                .then(() => {
                    el.setAttribute('data-group-joined', 'true');
                    console.log(`📡 Joined real-time group: ${type}_${id}`);
                })
                .catch(e => console.error(`Error joining group ${type}_${id}:`, e));
        }
    });
}
window.joinFeedGroups = joinFeedGroups;