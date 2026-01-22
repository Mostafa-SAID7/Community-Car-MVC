// Maps functionality
let currentLocation = null;
let searchResults = [];

// Initialize maps functionality
document.addEventListener('DOMContentLoaded', function() {
    initializeMaps();
});

function initializeMaps() {
    // Initialize any map-related functionality
    console.log('Maps initialized');
}

// Get current location
function getCurrentLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
            function(position) {
                currentLocation = {
                    latitude: position.coords.latitude,
                    longitude: position.coords.longitude
                };
                
                // Update the map placeholder with location info
                updateMapPlaceholder(`Location found: ${currentLocation.latitude.toFixed(4)}, ${currentLocation.longitude.toFixed(4)}`);
                
                // Search for nearby locations
                searchNearbyLocations();
            },
            function(error) {
                console.error('Error getting location:', error);
                showToast('Unable to get your location. Please enable location services.', 'error');
            }
        );
    } else {
        showToast('Geolocation is not supported by this browser.', 'error');
    }
}

// Update map placeholder
function updateMapPlaceholder(message) {
    const mapContainer = document.getElementById('mapContainer');
    const placeholder = mapContainer.querySelector('.d-flex');
    if (placeholder) {
        const messageElement = placeholder.querySelector('p');
        if (messageElement) {
            messageElement.textContent = message;
        }
    }
}

// Search for nearby locations
function searchNearbyLocations() {
    if (!currentLocation) {
        showToast('Please get your location first', 'warning');
        return;
    }
    
    const searchParams = new URLSearchParams({
        latitude: currentLocation.latitude,
        longitude: currentLocation.longitude,
        radiusKm: document.getElementById('radiusKm').value || 10,
        page: 1,
        pageSize: 20
    });
    
    fetch(`/maps/search?${searchParams}`)
        .then(response => response.json())
        .then(data => {
            searchResults = data.items || [];
            displaySearchResults(data);
            updateMapPlaceholder(`Found ${data.totalCount} locations nearby`);
        })
        .catch(error => {
            console.error('Error searching locations:', error);
            showToast('Error searching for locations', 'error');
        });
}

// Search locations with filters
function searchLocations() {
    const searchTerm = document.getElementById('searchTerm').value;
    const poiType = document.getElementById('poiType').value;
    const poiCategory = document.getElementById('poiCategory').value;
    const radiusKm = document.getElementById('radiusKm').value;
    const verifiedOnly = document.getElementById('verifiedOnly').checked;
    const open24Hours = document.getElementById('open24Hours').checked;
    
    const searchParams = new URLSearchParams({
        page: 1,
        pageSize: 20
    });
    
    if (searchTerm) searchParams.append('searchTerm', searchTerm);
    if (poiType) searchParams.append('type', poiType);
    if (poiCategory) searchParams.append('category', poiCategory);
    if (radiusKm) searchParams.append('radiusKm', radiusKm);
    if (verifiedOnly) searchParams.append('isVerified', 'true');
    if (open24Hours) searchParams.append('isOpen24Hours', 'true');
    
    if (currentLocation) {
        searchParams.append('latitude', currentLocation.latitude);
        searchParams.append('longitude', currentLocation.longitude);
    }
    
    fetch(`/maps/search?${searchParams}`)
        .then(response => response.json())
        .then(data => {
            searchResults = data.items || [];
            displaySearchResults(data);
        })
        .catch(error => {
            console.error('Error searching locations:', error);
            showToast('Error searching for locations', 'error');
        });
}

// Display search results
function displaySearchResults(data) {
    const mapContainer = document.getElementById('mapContainer');
    
    // Create results overlay
    let resultsOverlay = document.getElementById('searchResultsOverlay');
    if (!resultsOverlay) {
        resultsOverlay = document.createElement('div');
        resultsOverlay.id = 'searchResultsOverlay';
        resultsOverlay.className = 'position-absolute top-0 start-0 w-100 h-100 bg-white bg-opacity-95 p-3 overflow-auto';
        resultsOverlay.style.zIndex = '10';
        mapContainer.appendChild(resultsOverlay);
    }
    
    let html = `
        <div class="d-flex justify-content-between align-items-center mb-3">
            <h6 class="mb-0">Search Results (${data.totalCount})</h6>
            <button type="button" class="btn btn-sm btn-outline-secondary" onclick="closeSearchResults()">
                <i data-lucide="x" size="16"></i>
            </button>
        </div>
    `;
    
    if (data.items && data.items.length > 0) {
        data.items.forEach(poi => {
            html += `
                <div class="card mb-2 cursor-pointer" onclick="showLocationDetails('${poi.id}')">
                    <div class="card-body p-3">
                        <div class="d-flex justify-content-between align-items-start">
                            <div class="flex-grow-1">
                                <h6 class="mb-1">${poi.name}</h6>
                                <p class="text-muted small mb-1">${poi.description}</p>
                                <div class="d-flex align-items-center gap-2">
                                    ${poi.isVerified ? '<span class="badge bg-success-subtle text-success">Verified</span>' : ''}
                                    ${poi.isOpen24Hours ? '<span class="badge bg-info-subtle text-info">24/7</span>' : ''}
                                    ${poi.distanceKm ? `<small class="text-muted">${poi.distanceKm.toFixed(1)} km away</small>` : ''}
                                </div>
                            </div>
                            <div class="text-end">
                                ${poi.averageRating > 0 ? `
                                    <div class="d-flex align-items-center">
                                        <i data-lucide="star" size="14" class="text-warning me-1"></i>
                                        <small>${poi.averageRating.toFixed(1)}</small>
                                    </div>
                                ` : ''}
                                <small class="text-muted">${poi.checkInCount} check-ins</small>
                            </div>
                        </div>
                    </div>
                </div>
            `;
        });
    } else {
        html += '<p class="text-muted text-center">No locations found matching your criteria.</p>';
    }
    
    resultsOverlay.innerHTML = html;
    
    // Re-initialize Lucide icons
    if (typeof lucide !== 'undefined') {
        lucide.createIcons();
    }
}

// Close search results
function closeSearchResults() {
    const resultsOverlay = document.getElementById('searchResultsOverlay');
    if (resultsOverlay) {
        resultsOverlay.remove();
    }
}

// Show location details
function showLocationDetails(poiId) {
    fetch(`/maps/poi/${poiId}`)
        .then(response => response.json())
        .then(poi => {
            const detailsCard = document.getElementById('locationDetails');
            const detailsContent = document.getElementById('locationDetailsContent');
            
            let html = `
                <div class="mb-3">
                    <h5 class="mb-1">${poi.name}</h5>
                    <p class="text-muted mb-2">${poi.description}</p>
                    ${poi.address ? `<p class="small mb-1"><i data-lucide="map-pin" size="14" class="me-1"></i>${poi.address}</p>` : ''}
                    ${poi.phoneNumber ? `<p class="small mb-1"><i data-lucide="phone" size="14" class="me-1"></i>${poi.phoneNumber}</p>` : ''}
                    ${poi.website ? `<p class="small mb-1"><i data-lucide="globe" size="14" class="me-1"></i><a href="${poi.website}" target="_blank">Website</a></p>` : ''}
                </div>
                
                <div class="mb-3">
                    <div class="d-flex justify-content-between align-items-center mb-2">
                        <span>Rating</span>
                        <div class="d-flex align-items-center">
                            ${poi.averageRating > 0 ? `
                                <div class="me-2">
                                    ${Array.from({length: 5}, (_, i) => 
                                        `<i data-lucide="star" size="14" class="${i < poi.averageRating ? 'text-warning' : 'text-muted'}"></i>`
                                    ).join('')}
                                </div>
                                <small class="text-muted">(${poi.reviewCount})</small>
                            ` : '<small class="text-muted">No ratings yet</small>'}
                        </div>
                    </div>
                    <div class="d-flex justify-content-between align-items-center mb-2">
                        <span>Views</span>
                        <span>${poi.viewCount}</span>
                    </div>
                    <div class="d-flex justify-content-between align-items-center">
                        <span>Check-ins</span>
                        <span>${poi.checkInCount}</span>
                    </div>
                </div>
                
                ${poi.services && poi.services.length > 0 ? `
                    <div class="mb-3">
                        <h6>Services</h6>
                        <div class="d-flex flex-wrap gap-1">
                            ${poi.services.map(service => `<span class="badge bg-primary-subtle text-primary">${service}</span>`).join('')}
                        </div>
                    </div>
                ` : ''}
                
                <div class="d-grid gap-2">
                    <button type="button" class="btn btn-primary btn-sm" onclick="checkInToLocation('${poi.id}')">
                        <i data-lucide="check-circle" size="16"></i>
                        Check In
                    </button>
                    <button type="button" class="btn btn-outline-secondary btn-sm" onclick="viewCheckIns('${poi.id}')">
                        <i data-lucide="users" size="16"></i>
                        View Check-ins
                    </button>
                </div>
            `;
            
            detailsContent.innerHTML = html;
            detailsCard.style.display = 'block';
            
            // Re-initialize Lucide icons
            if (typeof lucide !== 'undefined') {
                lucide.createIcons();
            }
        })
        .catch(error => {
            console.error('Error loading location details:', error);
            showToast('Error loading location details', 'error');
        });
}

// Check in to location
function checkInToLocation(poiId) {
    // Simple check-in - in a real app, you'd show a modal with options
    const checkInData = {
        comment: '',
        rating: null,
        isPrivate: false
    };
    
    fetch(`/maps/poi/${poiId}/checkin`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
        },
        body: JSON.stringify(checkInData)
    })
    .then(response => {
        if (response.ok) {
            return response.json();
        }
        throw new Error('Check-in failed');
    })
    .then(checkIn => {
        showToast('Successfully checked in!', 'success');
        // Refresh location details to update check-in count
        showLocationDetails(poiId);
    })
    .catch(error => {
        console.error('Error checking in:', error);
        showToast('Error checking in. Please try again.', 'error');
    });
}

// Submit new POI
function submitPOI() {
    const form = document.getElementById('addPOIForm');
    const formData = new FormData(form);
    
    const poiData = {
        name: formData.get('name'),
        description: formData.get('description'),
        latitude: parseFloat(formData.get('latitude')),
        longitude: parseFloat(formData.get('longitude')),
        type: parseInt(formData.get('type')),
        category: 1, // Default to Automotive
        address: formData.get('address'),
        phoneNumber: formData.get('phoneNumber'),
        website: formData.get('website')
    };
    
    fetch('/maps/poi', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
        },
        body: JSON.stringify(poiData)
    })
    .then(response => {
        if (response.ok) {
            return response.json();
        }
        throw new Error('Failed to create location');
    })
    .then(poi => {
        showToast('Location added successfully!', 'success');
        const modal = bootstrap.Modal.getInstance(document.getElementById('addPOIModal'));
        if (modal) modal.hide();
        form.reset();
        // Refresh search results
        if (currentLocation) {
            searchNearbyLocations();
        }
    })
    .catch(error => {
        console.error('Error creating POI:', error);
        showToast('Error adding location. Please try again.', 'error');
    });
}

// Submit new route
function submitRoute() {
    const form = document.getElementById('addRouteForm');
    const formData = new FormData(form);
    
    const routeData = {
        name: formData.get('name'),
        description: formData.get('description'),
        type: parseInt(formData.get('type')),
        difficulty: parseInt(formData.get('difficulty')) || 1,
        distanceKm: parseFloat(formData.get('distanceKm')) || 0,
        estimatedDurationMinutes: parseInt(formData.get('estimatedDurationMinutes')) || 0,
        isScenic: formData.get('isScenic') === 'on',
        hasTolls: formData.get('hasTolls') === 'on',
        isOffRoad: formData.get('isOffRoad') === 'on'
    };
    
    fetch('/maps/routes', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
        },
        body: JSON.stringify(routeData)
    })
    .then(response => {
        if (response.ok) {
            return response.json();
        }
        throw new Error('Failed to create route');
    })
    .then(route => {
        showToast('Route added successfully!', 'success');
        const modal = bootstrap.Modal.getInstance(document.getElementById('addRouteModal'));
        if (modal) modal.hide();
        form.reset();
    })
    .catch(error => {
        console.error('Error creating route:', error);
        showToast('Error adding route. Please try again.', 'error');
    });
}

// Show toast notification
function showToast(message, type = 'info') {
    // Use the global showToast function from custom-bootstrap.js
    if (window.showToast) {
        window.showToast(message, type);
    } else {
        // Fallback to console if custom toast not available
        console.log(`${type.toUpperCase()}: ${message}`);
    }
}
    });
}