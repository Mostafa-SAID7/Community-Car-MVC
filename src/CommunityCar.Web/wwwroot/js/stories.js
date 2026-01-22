// Stories functionality
let currentStoryId = null;
let uploadedMediaUrl = null;

// Initialize stories functionality
document.addEventListener('DOMContentLoaded', function() {
    initializeStoryForm();
    loadStoriesData();
});

// Initialize story creation form
function initializeStoryForm() {
    // Caption character counter
    const captionInput = document.getElementById('storyCaption');
    const captionCount = document.getElementById('captionCount');
    
    if (captionInput && captionCount) {
        captionInput.addEventListener('input', function() {
            captionCount.textContent = this.value.length;
        });
    }

    // File upload handler
    const mediaInput = document.getElementById('storyMedia');
    if (mediaInput) {
        mediaInput.addEventListener('change', handleMediaUpload);
    }

    // Current location handler
    const useLocationCheckbox = document.getElementById('useCurrentLocation');
    if (useLocationCheckbox) {
        useLocationCheckbox.addEventListener('change', function() {
            if (this.checked) {
                getCurrentLocation();
            }
        });
    }
}

// Open create story modal
function openCreateStoryModal() {
    const modal = new bootstrap.Modal(document.getElementById('createStoryModal'));
    modal.show();
    
    // Reset form
    document.getElementById('createStoryForm').reset();
    document.getElementById('captionCount').textContent = '0';
    uploadedMediaUrl = null;
}

// Handle media file upload
async function handleMediaUpload(event) {
    const file = event.target.files[0];
    if (!file) return;

    // Validate file type
    const validTypes = ['image/jpeg', 'image/png', 'image/gif', 'video/mp4', 'video/webm'];
    if (!validTypes.includes(file.type)) {
        showToast('Please select a valid image or video file', 'error');
        return;
    }

    // Validate file size (10MB limit)
    if (file.size > 10 * 1024 * 1024) {
        showToast('File size must be less than 10MB', 'error');
        return;
    }

    try {
        // TODO: Implement actual file upload to server
        // For now, create a temporary URL
        uploadedMediaUrl = URL.createObjectURL(file);
        
        // Update story type based on file type
        const storyTypeSelect = document.getElementById('storyType');
        if (file.type.startsWith('image/')) {
            storyTypeSelect.value = '0'; // Image
        } else if (file.type.startsWith('video/')) {
            storyTypeSelect.value = '1'; // Video
        }
        
        showToast('Media uploaded successfully', 'success');
    } catch (error) {
        console.error('Upload error:', error);
        showToast('Failed to upload media', 'error');
    }
}

// Get current location
function getCurrentLocation() {
    if (!navigator.geolocation) {
        showToast('Geolocation is not supported by this browser', 'error');
        return;
    }

    navigator.geolocation.getCurrentPosition(
        function(position) {
            const lat = position.coords.latitude;
            const lng = position.coords.longitude;
            
            // TODO: Reverse geocode to get location name
            document.getElementById('locationName').value = `${lat.toFixed(4)}, ${lng.toFixed(4)}`;
            
            // Store coordinates for later use
            document.getElementById('locationName').dataset.latitude = lat;
            document.getElementById('locationName').dataset.longitude = lng;
            
            showToast('Location added successfully', 'success');
        },
        function(error) {
            console.error('Geolocation error:', error);
            showToast('Failed to get current location', 'error');
        }
    );
}

// Create new story
async function createStory() {
    if (!uploadedMediaUrl) {
        showToast('Please upload media for your story', 'error');
        return;
    }

    const createBtn = document.getElementById('createStoryBtn');
    const spinner = document.getElementById('createStorySpinner');
    
    // Show loading state
    createBtn.disabled = true;
    spinner.classList.remove('d-none');

    try {
        // Collect form data
        const formData = {
            mediaUrl: uploadedMediaUrl, // TODO: Replace with actual uploaded URL
            type: parseInt(document.getElementById('storyType').value),
            duration: parseInt(document.getElementById('storyDuration').value),
            caption: document.getElementById('storyCaption').value.trim() || null,
            carMake: document.getElementById('carMake').value.trim() || null,
            carModel: document.getElementById('carModel').value.trim() || null,
            carYear: document.getElementById('carYear').value ? parseInt(document.getElementById('carYear').value) : null,
            eventType: document.getElementById('eventType').value || null,
            locationName: document.getElementById('locationName').value.trim() || null,
            latitude: document.getElementById('locationName').dataset.latitude ? parseFloat(document.getElementById('locationName').dataset.latitude) : null,
            longitude: document.getElementById('locationName').dataset.longitude ? parseFloat(document.getElementById('locationName').dataset.longitude) : null,
            visibility: parseInt(document.getElementById('storyVisibility').value),
            allowReplies: document.getElementById('allowReplies').checked,
            allowSharing: document.getElementById('allowSharing').checked,
            tags: parseTags(document.getElementById('storyTags').value),
            mentionedUsers: [],
            additionalMediaUrls: [],
            isFeatured: false,
            isHighlighted: false
        };

        // Send request to server
        const response = await fetch('/feed/api/stories/create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(formData)
        });

        const result = await response.json();

        if (result.success) {
            showToast('Story created successfully!', 'success');
            
            // Close modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('createStoryModal'));
            modal.hide();
            
            // Refresh stories display
            loadStoriesData();
        } else {
            showToast(result.message || 'Failed to create story', 'error');
        }
    } catch (error) {
        console.error('Create story error:', error);
        showToast('Failed to create story', 'error');
    } finally {
        // Hide loading state
        createBtn.disabled = false;
        spinner.classList.add('d-none');
    }
}

// Parse tags from input string
function parseTags(tagsString) {
    if (!tagsString) return [];
    
    return tagsString
        .split(',')
        .map(tag => tag.trim().replace(/^#/, ''))
        .filter(tag => tag.length > 0);
}

// Load stories data
async function loadStoriesData() {
    try {
        const response = await fetch('/feed/api/stories');
        const stories = await response.json();
        
        if (stories && Array.isArray(stories)) {
            updateStoriesDisplay(stories);
        }
    } catch (error) {
        console.error('Load stories error:', error);
    }
}

// Update stories display
function updateStoriesDisplay(stories) {
    const container = document.getElementById('storiesContainer');
    if (!container) return;

    // Keep the "Add Story" button and clear the rest
    const addStoryBtn = container.querySelector('.flex-shrink-0');
    container.innerHTML = '';
    if (addStoryBtn) {
        container.appendChild(addStoryBtn);
    }

    // Add stories
    stories.forEach(story => {
        const storyElement = createStoryElement(story);
        container.appendChild(storyElement);
    });
}

// Create story element
function createStoryElement(story) {
    const div = document.createElement('div');
    div.className = 'flex-shrink-0';
    div.style.width = '80px';
    
    const thumbnailUrl = story.thumbnailUrl || story.mediaUrl || '';
    const backgroundStyle = thumbnailUrl ? `background-image: url('${thumbnailUrl}'); background-size: cover; background-position: center;` : '';
    
    div.innerHTML = `
        <div class="rounded-circle bg-primary-subtle d-flex align-items-center justify-content-center mb-2 cursor-pointer position-relative" 
             style="width: 80px; height: 80px; border: 3px solid var(--color-red-500); ${backgroundStyle}"
             onclick="openStoryViewer('${story.id}')">
            ${!thumbnailUrl ? '<i data-lucide="user" class="text-primary"></i>' : ''}
            ${!story.isExpired ? '<div class="position-absolute bottom-0 start-0 w-100 bg-success rounded-bottom" style="height: 4px;"></div>' : ''}
        </div>
        <div class="text-center">
            <small class="text-muted">${story.authorName || 'User'}</small>
        </div>
    `;
    
    return div;
}

// Open story viewer
async function openStoryViewer(storyId) {
    currentStoryId = storyId;
    
    try {
        // Fetch story details
        const response = await fetch(`/feed/api/stories/${storyId}`);
        const story = await response.json();
        
        if (!story) {
            showToast('Story not found', 'error');
            return;
        }

        // Update modal content
        document.getElementById('storyAuthorName').textContent = story.authorName || 'Unknown User';
        document.getElementById('storyTimeAgo').textContent = story.timeAgo || 'Recently';
        document.getElementById('storyCaption').textContent = story.caption || '';
        document.getElementById('storyLikeCount').textContent = story.likeCount || 0;
        document.getElementById('storyViewCount').textContent = story.viewCount || 0;
        
        // Update media container
        const mediaContainer = document.getElementById('storyMediaContainer');
        if (story.type === 1) { // Video
            mediaContainer.innerHTML = `
                <video controls class="w-100" style="max-height: 400px;">
                    <source src="${story.mediaUrl}" type="video/mp4">
                    Your browser does not support the video tag.
                </video>
            `;
        } else { // Image
            mediaContainer.innerHTML = `
                <img src="${story.mediaUrl}" class="w-100" style="max-height: 400px; object-fit: contain;" alt="Story media">
            `;
        }
        
        // Update tags
        const tagsContainer = document.getElementById('storyTags');
        if (story.tags && story.tags.length > 0) {
            tagsContainer.innerHTML = story.tags.map(tag => `<span class="badge bg-secondary me-1">#${tag}</span>`).join('');
        } else {
            tagsContainer.innerHTML = '';
        }
        
        // Update location
        const locationContainer = document.getElementById('storyLocation');
        if (story.locationName) {
            locationContainer.innerHTML = `<i data-lucide="map-pin" class="me-1"></i>${story.locationName}`;
        } else {
            locationContainer.innerHTML = '';
        }
        
        // Show modal
        const modal = new bootstrap.Modal(document.getElementById('storyViewerModal'));
        modal.show();
        
        // Increment view count
        incrementStoryView(storyId);
        
    } catch (error) {
        console.error('Load story error:', error);
        showToast('Failed to load story', 'error');
    }
}

// Like story
async function likeStory() {
    if (!currentStoryId) return;
    
    try {
        const response = await fetch(`/feed/api/stories/${currentStoryId}/like`, {
            method: 'POST'
        });
        
        const result = await response.json();
        
        if (result.success) {
            // Update like count in UI
            const likeCountElement = document.getElementById('storyLikeCount');
            const currentCount = parseInt(likeCountElement.textContent) || 0;
            likeCountElement.textContent = currentCount + 1;
            
            showToast('Story liked!', 'success');
        }
    } catch (error) {
        console.error('Like story error:', error);
        showToast('Failed to like story', 'error');
    }
}

// Increment story view count
async function incrementStoryView(storyId) {
    try {
        await fetch(`/feed/api/stories/${storyId}/view`, {
            method: 'POST'
        });
    } catch (error) {
        console.error('Increment view error:', error);
    }
}

// Show toast notification
function showToast(message, type = 'info') {
    // Create toast element
    const toast = document.createElement('div');
    toast.className = `toast align-items-center text-white bg-${type === 'error' ? 'danger' : type === 'success' ? 'success' : 'primary'} border-0`;
    toast.setAttribute('role', 'alert');
    toast.setAttribute('aria-live', 'assertive');
    toast.setAttribute('aria-atomic', 'true');
    
    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">
                ${message}
            </div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
    `;
    
    // Add to toast container or create one
    let toastContainer = document.querySelector('.toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
        toastContainer.style.zIndex = '1100';
        document.body.appendChild(toastContainer);
    }
    
    toastContainer.appendChild(toast);
    
    // Show toast
    const bsToast = new bootstrap.Toast(toast);
    bsToast.show();
    
    // Remove from DOM after hiding
    toast.addEventListener('hidden.bs.toast', function() {
        toast.remove();
    });
}