// Maps functionality
let currentLocation = null;
let searchResults = [];

// Initialize maps functionality
document.addEventListener('DOMContentLoaded', function () {
    initializeMaps();
    initializeModals();
    setupModalLinks();
});

function setupModalLinks() {
    // Global listener for modal toggle buttons
    document.addEventListener('click', function (e) {
        const toggleBtn = e.target.closest('[data-bs-toggle="modal"]');
        if (toggleBtn) {
            e.preventDefault();
            const targetId = toggleBtn.getAttribute('data-bs-target').replace('#', '');
            showModal(targetId);
        }

        // Global listener for modal dismiss buttons
        const dismissBtn = e.target.closest('[data-bs-dismiss="modal"]');
        if (dismissBtn) {
            e.preventDefault();
            const modal = dismissBtn.closest('.modal');
            if (modal) {
                hideModal(modal.id);
            }
        }
    });
}

function showModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.remove('hidden');
        modal.classList.add('flex');
        document.body.style.overflow = 'hidden';

        // Re-initialize Lucide icons in the modal
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }

        // Trigger 'shown.bs.modal' for any custom logic (like auto-populating coordinates)
        const event = new CustomEvent('shown.bs.modal');
        modal.dispatchEvent(event);
    }
}

function initializeMaps() {
    // Initialize any map-related functionality
    console.log('Maps initialized');

    // Auto-populate coordinates if user clicks "Get My Location"
    const getCurrentLocationBtn = document.querySelector('button[onclick="getCurrentLocation()"]');
    if (getCurrentLocationBtn) {
        getCurrentLocationBtn.addEventListener('click', getCurrentLocation);
    }
}

function initializeModals() {
    // Add event listeners for modal forms
    const addPOIForm = document.getElementById('addPOIForm');
    const addRouteForm = document.getElementById('addRouteForm');

    if (addPOIForm) {
        addPOIForm.addEventListener('submit', function (e) {
            e.preventDefault();
            submitPOI();
        });
    }

    if (addRouteForm) {
        addRouteForm.addEventListener('submit', function (e) {
            e.preventDefault();
            submitRoute();
        });
    }

    // Auto-populate coordinates when modals open
    const addPOIModal = document.getElementById('addPOIModal');
    const addRouteModal = document.getElementById('addRouteModal');
    const addReviewModal = document.getElementById('addReviewModal');

    if (addPOIModal) {
        addPOIModal.addEventListener('shown.bs.modal', function () {
            if (currentLocation) {
                document.getElementById('poiLatitude').value = currentLocation.latitude.toFixed(6);
                document.getElementById('poiLongitude').value = currentLocation.longitude.toFixed(6);
            }
        });
    }

    if (addReviewModal) {
        // Handle star rating clicks
        const starBtns = addReviewModal.querySelectorAll('.star-btn');
        const ratingInput = document.getElementById('reviewRating');

        starBtns.forEach(btn => {
            btn.addEventListener('click', function () {
                const rating = this.getAttribute('data-rating');
                ratingInput.value = rating;

                // Update stars visual state
                starBtns.forEach(s => {
                    const sRating = s.getAttribute('data-rating');
                    if (sRating <= rating) {
                        s.classList.remove('text-muted-foreground/30');
                        s.classList.add('text-amber-500');
                    } else {
                        s.classList.add('text-muted-foreground/30');
                        s.classList.remove('text-amber-500');
                    }
                });
            });
        });

        // Set target info when modal opens
        addReviewModal.addEventListener('show.bs.modal', function (event) {
            const button = event.relatedTarget;
            // Get target info from URL or page context if not on button
            let targetId = button?.getAttribute('data-target-id');
            let targetType = button?.getAttribute('data-target-type');

            if (!targetId) {
                // Fallback: try to find Model ID from page
                const pathParts = window.location.pathname.split('/');
                targetId = pathParts[pathParts.length - 1];
                targetType = window.location.pathname.includes('/poi/') ? 'POI' : 'Route';
            }

            document.getElementById('reviewTargetId').value = targetId;
            document.getElementById('reviewTargetType').value = targetType;

            // Reset form
            document.getElementById('addReviewForm').reset();
            ratingInput.value = '';
            starBtns.forEach(s => {
                s.classList.add('text-muted-foreground/30');
                s.classList.remove('text-amber-500');
            });
        });

        const addReviewForm = document.getElementById('addReviewForm');
        if (addReviewForm) {
            addReviewForm.addEventListener('submit', function (e) {
                e.preventDefault();
                submitReview();
            });
        }
    }
}

// Get current location
function getCurrentLocation() {
    if (navigator.geolocation) {
        // Show loading state
        const button = event.target.closest('button');
        const originalContent = button.innerHTML;
        button.innerHTML = '<i data-lucide="loader-2" class="w-4 h-4 animate-spin"></i> Getting Location...';
        button.disabled = true;

        navigator.geolocation.getCurrentPosition(
            function (position) {
                currentLocation = {
                    latitude: position.coords.latitude,
                    longitude: position.coords.longitude
                };

                // Update the map placeholder with location info
                updateMapPlaceholder(`Location found: ${currentLocation.latitude.toFixed(4)}, ${currentLocation.longitude.toFixed(4)}`);

                // Restore button
                button.innerHTML = originalContent;
                button.disabled = false;

                // Re-initialize Lucide icons
                if (typeof lucide !== 'undefined') {
                    lucide.createIcons();
                }

                // Search for nearby locations
                searchNearbyLocations();

                showToast('Location found successfully!', 'success');
            },
            function (error) {
                console.error('Error getting location:', error);

                // Restore button
                button.innerHTML = originalContent;
                button.disabled = false;

                // Re-initialize Lucide icons
                if (typeof lucide !== 'undefined') {
                    lucide.createIcons();
                }

                let errorMessage = 'Unable to get your location. ';
                switch (error.code) {
                    case error.PERMISSION_DENIED:
                        errorMessage += 'Please enable location services.';
                        break;
                    case error.POSITION_UNAVAILABLE:
                        errorMessage += 'Location information is unavailable.';
                        break;
                    case error.TIMEOUT:
                        errorMessage += 'Location request timed out.';
                        break;
                    default:
                        errorMessage += 'An unknown error occurred.';
                        break;
                }
                showToast(errorMessage, 'error');
            },
            {
                enableHighAccuracy: true,
                timeout: 10000,
                maximumAge: 300000 // 5 minutes
            }
        );
    } else {
        showToast('Geolocation is not supported by this browser.', 'error');
    }
}

// Update map placeholder
function updateMapPlaceholder(message) {
    const mapContainer = document.getElementById('mapContainer');
    if (mapContainer) {
        const messageElement = mapContainer.querySelector('p');
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

    const radiusKm = document.getElementById('radiusKm')?.value || 10;
    const searchParams = new URLSearchParams({
        latitude: currentLocation.latitude,
        longitude: currentLocation.longitude,
        radiusKm: radiusKm,
        page: 1,
        pageSize: 20
    });

    fetch(`/maps/search?${searchParams}`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            searchResults = data.items || [];
            displaySearchResults(data);
            updateMapPlaceholder(`Found ${data.totalCount || 0} locations nearby`);
        })
        .catch(error => {
            console.error('Error searching locations:', error);
            showToast('Error searching for locations', 'error');
        });
}

// Search locations with filters
function searchLocations() {
    const searchTerm = document.getElementById('searchTerm')?.value || '';
    const poiType = document.getElementById('poiType')?.value || '';
    const radiusKm = document.getElementById('radiusKm')?.value || 10;
    const verifiedOnly = document.getElementById('verifiedOnly')?.checked || false;
    const showRoutes = document.getElementById('showRoutes')?.checked || false;

    const searchParams = new URLSearchParams({
        page: 1,
        pageSize: 20
    });

    if (searchTerm) searchParams.append('searchTerm', searchTerm);
    if (poiType) searchParams.append('type', poiType);
    if (radiusKm) searchParams.append('radiusKm', radiusKm);
    if (verifiedOnly) searchParams.append('isVerified', 'true');

    if (currentLocation) {
        searchParams.append('latitude', currentLocation.latitude);
        searchParams.append('longitude', currentLocation.longitude);
    }

    fetch(`/maps/search?${searchParams}`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            searchResults = data.items || [];
            displaySearchResults(data);
            showToast(`Found ${data.totalCount || 0} locations`, 'info');
        })
        .catch(error => {
            console.error('Error searching locations:', error);
            showToast('Error searching for locations', 'error');
        });
}

// Display search results
function displaySearchResults(data) {
    const mapContainer = document.getElementById('mapContainer');
    if (!mapContainer) return;

    // Create results overlay
    let resultsOverlay = document.getElementById('searchResultsOverlay');
    if (!resultsOverlay) {
        resultsOverlay = document.createElement('div');
        resultsOverlay.id = 'searchResultsOverlay';
        resultsOverlay.className = 'absolute inset-0 bg-card/95 backdrop-blur-md border border-border rounded-2xl p-6 overflow-auto z-10';
        mapContainer.appendChild(resultsOverlay);
    }

    let html = `
        <div class="flex justify-between items-center mb-6">
            <h6 class="text-lg font-black text-foreground">Search Results (${data.totalCount || 0})</h6>
            <button type="button" class="inline-flex items-center justify-center w-8 h-8 rounded-xl bg-background border border-border hover:border-red-500 hover:text-red-600 transition-all" onclick="closeSearchResults()">
                <i data-lucide="x" class="w-4 h-4"></i>
            </button>
        </div>
        <div class="space-y-4">
    `;

    if (data.items && data.items.length > 0) {
        data.items.forEach(poi => {
            html += `
                <div class="bg-background/50 border border-border rounded-xl p-4 hover:border-primary/30 transition-all cursor-pointer group" onclick="showLocationDetails('${poi.id}')">
                    <div class="flex justify-between items-start">
                        <div class="flex-1 min-w-0">
                            <h6 class="text-sm font-black text-foreground group-hover:text-primary transition-colors truncate">${poi.name}</h6>
                            <p class="text-xs text-muted-foreground mt-1 line-clamp-2">${poi.description}</p>
                            <div class="flex items-center gap-2 mt-2">
                                ${poi.isVerified ? '<span class="inline-flex items-center gap-1 px-2 py-1 bg-green-500/10 text-green-600 rounded-lg text-[9px] font-black uppercase tracking-widest"><i data-lucide="shield-check" class="w-3 h-3"></i>Verified</span>' : ''}
                                ${poi.isOpen24Hours ? '<span class="inline-flex items-center gap-1 px-2 py-1 bg-blue-500/10 text-blue-600 rounded-lg text-[9px] font-black uppercase tracking-widest"><i data-lucide="clock" class="w-3 h-3"></i>24/7</span>' : ''}
                                ${poi.distanceKm ? `<span class="text-[9px] font-black uppercase tracking-widest text-muted-foreground opacity-60">${poi.distanceKm.toFixed(1)} km away</span>` : ''}
                            </div>
                        </div>
                        <div class="text-right ml-4">
                            ${poi.averageRating > 0 ? `
                                <div class="flex items-center justify-end mb-1">
                                    <i data-lucide="star" class="w-3 h-3 text-amber-500 mr-1"></i>
                                    <span class="text-xs font-bold">${poi.averageRating.toFixed(1)}</span>
                                </div>
                            ` : ''}
                            <span class="text-[9px] font-black uppercase tracking-widest text-muted-foreground opacity-60">${poi.checkInCount} check-ins</span>
                        </div>
                    </div>
                </div>
            `;
        });
    } else {
        html += '<div class="text-center py-12"><p class="text-muted-foreground">No locations found matching your criteria.</p></div>';
    }

    html += '</div>';
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
    fetch(`/maps/poi/${poiId}/json`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(poi => {
            // Create or update details panel
            let detailsPanel = document.getElementById('locationDetailsPanel');
            const isRtl = document.documentElement.dir === 'rtl';

            if (!detailsPanel) {
                detailsPanel = document.createElement('div');
                detailsPanel.id = 'locationDetailsPanel';
                detailsPanel.className = `fixed top-4 ${isRtl ? 'left-4' : 'right-4'} w-80 bg-card border border-border rounded-2xl shadow-2xl p-6 z-50 max-h-[80vh] overflow-auto`;
                detailsPanel.dir = isRtl ? 'rtl' : 'ltr';
                document.body.appendChild(detailsPanel);
            }

            let html = `
                <div class="flex justify-between items-start mb-4">
                    <h5 class="text-lg font-black text-foreground">${poi.name}</h5>
                    <button type="button" class="inline-flex items-center justify-center w-8 h-8 rounded-xl bg-background border border-border hover:border-red-500 hover:text-red-600 transition-all" onclick="closeLocationDetails()">
                        <i data-lucide="x" class="w-4 h-4"></i>
                    </button>
                </div>
                
                <div class="space-y-4">
                    <p class="text-sm text-muted-foreground">${poi.description}</p>
                    
                    ${poi.address ? `<div class="flex items-start gap-2"><i data-lucide="map-pin" class="w-4 h-4 text-muted-foreground mt-0.5"></i><span class="text-sm">${poi.address}</span></div>` : ''}
                    ${poi.phoneNumber ? `<div class="flex items-center gap-2"><i data-lucide="phone" class="w-4 h-4 text-muted-foreground"></i><span class="text-sm">${poi.phoneNumber}</span></div>` : ''}
                    ${poi.website ? `<div class="flex items-center gap-2"><i data-lucide="globe" class="w-4 h-4 text-muted-foreground"></i><a href="${poi.website}" target="_blank" class="text-sm text-primary hover:underline">Website</a></div>` : ''}
                    
                    <div class="bg-background/50 rounded-xl p-4 space-y-2">
                        <div class="flex justify-between items-center">
                            <span class="text-sm font-medium">Rating</span>
                            <div class="flex items-center gap-2">
                                ${poi.averageRating > 0 ? `
                                    <div class="flex items-center">
                                        ${Array.from({ length: 5 }, (_, i) =>
                `<i data-lucide="star" class="w-3 h-3 ${i < poi.averageRating ? 'text-amber-500' : 'text-muted-foreground opacity-30'}"></i>`
            ).join('')}
                                    </div>
                                    <span class="text-sm font-bold">${poi.averageRating.toFixed(1)}</span>
                                ` : '<span class="text-sm text-muted-foreground">No ratings</span>'}
                            </div>
                        </div>
                        <div class="flex justify-between items-center">
                            <span class="text-sm font-medium">Check-ins</span>
                            <span class="text-sm font-bold">${poi.checkInCount}</span>
                        </div>
                        <div class="flex justify-between items-center">
                            <span class="text-sm font-medium">Views</span>
                            <span class="text-sm font-bold">${poi.viewCount}</span>
                        </div>
                    </div>
                    
                    ${poi.services && poi.services.length > 0 ? `
                        <div>
                            <h6 class="text-sm font-black uppercase tracking-widest text-foreground mb-2">Services</h6>
                            <div class="flex flex-wrap gap-1">
                                ${poi.services.map(service => `<span class="px-2 py-1 bg-primary/10 text-primary rounded-lg text-xs font-medium">${service}</span>`).join('')}
                            </div>
                        </div>
                    ` : ''}
                    
                    <div class="space-y-2">
                        <button type="button" class="w-full inline-flex items-center justify-center gap-2 px-4 py-3 bg-primary text-primary-foreground hover:bg-primary/90 rounded-xl text-[10px] font-black uppercase tracking-widest transition-all" onclick="checkInToLocation('${poi.id}')">
                            <i data-lucide="check-circle" class="w-4 h-4"></i>
                            Check In
                        </button>
                        <button type="button" class="w-full inline-flex items-center justify-center gap-2 px-4 py-3 bg-background border border-border hover:border-primary hover:text-primary rounded-xl text-[10px] font-black uppercase tracking-widest transition-all" onclick="viewCheckIns('${poi.id}')">
                            <i data-lucide="users" class="w-4 h-4"></i>
                            View Check-ins
                        </button>
                    </div>
                </div>
            `;

            detailsPanel.innerHTML = html;

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

// Show route details
function showRouteDetails(routeId) {
    fetch(`/maps/routes/${routeId}/json`)
        .then(response => {
            if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
            return response.json();
        })
        .then(route => {
            let detailsPanel = document.getElementById('locationDetailsPanel');
            const isRtl = document.documentElement.dir === 'rtl';

            if (!detailsPanel) {
                detailsPanel = document.createElement('div');
                detailsPanel.id = 'locationDetailsPanel';
                detailsPanel.className = `fixed top-4 ${isRtl ? 'left-4' : 'right-4'} w-80 bg-card border border-border rounded-2xl shadow-2xl p-6 z-50 max-h-[80vh] overflow-auto animate-in slide-in-from-right duration-300`;
                detailsPanel.dir = isRtl ? 'rtl' : 'ltr';
                document.body.appendChild(detailsPanel);
            }

            let html = `
                <div class="flex justify-between items-start mb-4">
                    <h5 class="text-lg font-black text-foreground">${route.name}</h5>
                    <button type="button" class="inline-flex items-center justify-center w-8 h-8 rounded-xl bg-background border border-border hover:border-red-500 hover:text-red-600 transition-all" onclick="closeLocationDetails()">
                        <i data-lucide="x" class="w-4 h-4"></i>
                    </button>
                </div>
                
                <div class="space-y-4">
                    <p class="text-sm text-muted-foreground">${route.description}</p>
                    <div class="grid grid-cols-2 gap-3">
                        <div class="bg-background/50 rounded-xl p-3 text-center border border-border/40">
                            <span class="text-[9px] font-black uppercase tracking-widest text-muted-foreground block mb-1">Distance</span>
                            <span class="text-sm font-bold text-foreground">${route.distanceKm.toFixed(1)} KM</span>
                        </div>
                        <div class="bg-background/50 rounded-xl p-3 text-center border border-border/40">
                            <span class="text-[9px] font-black uppercase tracking-widest text-muted-foreground block mb-1">Duration</span>
                            <span class="text-sm font-bold text-foreground">${Math.floor(route.estimatedDurationMinutes / 60)}h ${route.estimatedDurationMinutes % 60}m</span>
                        </div>
                    </div>
                    <div class="bg-background/50 rounded-xl p-4 space-y-2 text-sm">
                        <div class="flex justify-between"><span>Difficulty</span><span class="font-bold text-primary">${route.difficulty}</span></div>
                        <div class="flex justify-between"><span>Type</span><span class="font-bold">${route.type}</span></div>
                        <div class="flex justify-between pt-2 border-t border-border/40"><span>Rating</span><div class="flex items-center gap-1">${Array.from({ length: 5 }, (_, i) => `<i data-lucide="star" class="w-3 h-3 ${i < route.averageRating ? 'text-amber-500 fill-current' : 'text-muted-foreground opacity-30'}"></i>`).join('')}</div></div>
                    </div>
                    <div class="space-y-2 pt-2">
                        <a href="/maps/routes/${route.id}" class="w-full inline-flex items-center justify-center gap-2 px-4 py-3 bg-primary text-primary-foreground hover:bg-primary/90 rounded-xl text-[10px] font-black uppercase tracking-widest transition-all shadow-glow-primary">
                            <i data-lucide="navigation" class="w-4 h-4"></i> Start Navigator
                        </a>
                    </div>
                </div>`;
            detailsPanel.innerHTML = html;
            if (typeof lucide !== 'undefined') lucide.createIcons();
        })
        .catch(error => { console.error('Error loading route details:', error); showToast('Error loading route details', 'error'); });
}

// Hotspots functionality removed - no longer using infinite scroll

// Show route details
function showRouteDetails(routeId) {
    fetch(`/maps/routes/${routeId}/json`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(route => {
            // Create or update details panel (reusing the same panel ID as POI)
            let detailsPanel = document.getElementById('locationDetailsPanel');
            const isRtl = document.documentElement.dir === 'rtl';

            if (!detailsPanel) {
                detailsPanel = document.createElement('div');
                detailsPanel.id = 'locationDetailsPanel';
                detailsPanel.className = `fixed top-4 ${isRtl ? 'left-4' : 'right-4'} w-80 bg-card border border-border rounded-2xl shadow-2xl p-6 z-50 max-h-[80vh] overflow-auto animate-in slide-in-from-right duration-300`;
                detailsPanel.dir = isRtl ? 'rtl' : 'ltr';
                document.body.appendChild(detailsPanel);
            }

            let html = `
                <div class="flex justify-between items-start mb-4">
                    <h5 class="text-lg font-black text-foreground">${route.name}</h5>
                    <button type="button" class="inline-flex items-center justify-center w-8 h-8 rounded-xl bg-background border border-border hover:border-red-500 hover:text-red-600 transition-all" onclick="closeLocationDetails()">
                        <i data-lucide="x" class="w-4 h-4"></i>
                    </button>
                </div>
                
                <div class="space-y-4">
                    <p class="text-sm text-muted-foreground">${route.description}</p>
                    
                    <div class="grid grid-cols-2 gap-3">
                        <div class="bg-background/50 rounded-xl p-3 text-center border border-border/40">
                            <span class="text-[9px] font-black uppercase tracking-widest text-muted-foreground block mb-1">Distance</span>
                            <span class="text-sm font-bold text-foreground">${route.distanceKm.toFixed(1)} KM</span>
                        </div>
                        <div class="bg-background/50 rounded-xl p-3 text-center border border-border/40">
                            <span class="text-[9px] font-black uppercase tracking-widest text-muted-foreground block mb-1">Duration</span>
                            <span class="text-sm font-bold text-foreground">${Math.floor(route.estimatedDurationMinutes / 60)}h ${route.estimatedDurationMinutes % 60}m</span>
                        </div>
                    </div>

                    <div class="bg-background/50 rounded-xl p-4 space-y-2">
                        <div class="flex justify-between items-center text-sm">
                            <span class="font-medium">Difficulty</span>
                            <span class="font-bold text-primary">${route.difficulty}</span>
                        </div>
                        <div class="flex justify-between items-center text-sm">
                            <span class="font-medium">Type</span>
                            <span class="font-bold">${route.type}</span>
                        </div>
                        <div class="flex justify-between items-center text-sm pt-2 border-t border-border/40">
                            <span class="font-medium">Rating</span>
                            <div class="flex items-center gap-1">
                                ${Array.from({ length: 5 }, (_, i) =>
                `<i data-lucide="star" class="w-3 h-3 ${i < route.averageRating ? 'text-amber-500 fill-current' : 'text-muted-foreground opacity-30'}"></i>`
            ).join('')}
                            </div>
                        </div>
                    </div>

                    <div class="flex flex-wrap gap-1.5">
                        ${route.isScenic ? '<span class="px-2 py-1 bg-amber-500/10 text-amber-600 rounded-lg text-[10px] font-black uppercase tracking-widest border border-amber-500/20">Scenic</span>' : ''}
                        ${route.isOffRoad ? '<span class="px-2 py-1 bg-orange-500/10 text-orange-600 rounded-lg text-[10px] font-black uppercase tracking-widest border border-orange-500/20">Off-Road</span>' : ''}
                        ${route.hasTolls ? '<span class="px-2 py-1 bg-red-500/10 text-red-600 rounded-lg text-[10px] font-black uppercase tracking-widest border border-red-500/20">Tolls</span>' : ''}
                    </div>

                    <div class="space-y-2 pt-2">
                        <a href="/maps/routes/${route.id}" class="w-full inline-flex items-center justify-center gap-2 px-4 py-3 bg-primary text-primary-foreground hover:bg-primary/90 rounded-xl text-[10px] font-black uppercase tracking-widest transition-all shadow-glow-primary">
                            <i data-lucide="navigation" class="w-4 h-4"></i>
                            Start Navigator
                        </a>
                        <button type="button" class="w-full inline-flex items-center justify-center gap-2 px-4 py-3 bg-background border border-border hover:border-primary hover:text-primary rounded-xl text-[10px] font-black uppercase tracking-widest transition-all" onclick="showToast('Route saved to library', 'success')">
                            <i data-lucide="bookmark" class="w-4 h-4"></i>
                            Save Route
                        </button>
                    </div>
                </div>
            `;

            detailsPanel.innerHTML = html;

            // Re-initialize Lucide icons
            if (typeof lucide !== 'undefined') {
                lucide.createIcons();
            }
        })
        .catch(error => {
            console.error('Error loading route details:', error);
            showToast('Error loading route details', 'error');
        });
}

// Close location details
function closeLocationDetails() {
    const detailsPanel = document.getElementById('locationDetailsPanel');
    if (detailsPanel) {
        detailsPanel.remove();
    }
}

// View check-ins
function viewCheckIns(poiId) {
    window.location.href = `/maps/poi/${poiId}/checkins`;
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
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        },
        body: JSON.stringify(checkInData)
    })
        .then(response => {
            if (response.ok) {
                return response.json();
            }
            if (response.status === 401) {
                throw new Error('Please log in to check in');
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
            showToast(error.message || 'Error checking in. Please try again.', 'error');
        });
}

// Submit new POI
function submitPOI() {
    const form = document.getElementById('addPOIForm');
    if (!form) return;

    const formData = new FormData(form);

    // Validate required fields
    const requiredFields = ['name', 'description', 'latitude', 'longitude', 'type'];
    for (const field of requiredFields) {
        if (!formData.get(field)) {
            showToast(`Please fill in the ${field} field`, 'error');
            return;
        }
    }

    const poiData = {
        name: formData.get('name'),
        description: formData.get('description'),
        latitude: parseFloat(formData.get('latitude')),
        longitude: parseFloat(formData.get('longitude')),
        type: parseInt(formData.get('type')),
        category: 1, // Default to Automotive
        address: formData.get('address') || '',
        phoneNumber: formData.get('phoneNumber') || '',
        website: formData.get('website') || ''
    };

    // Validate coordinates
    if (isNaN(poiData.latitude) || isNaN(poiData.longitude)) {
        showToast('Please enter valid coordinates', 'error');
        return;
    }

    fetch('/maps/poi', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        },
        body: JSON.stringify(poiData)
    })
        .then(response => {
            if (response.ok) {
                return response.json();
            }
            if (response.status === 401) {
                throw new Error('Please log in to add locations');
            }
            throw new Error('Failed to create location');
        })
        .then(poi => {
            showToast('Location added successfully!', 'success');
            showToast('Location added successfully!', 'success');
            hideModal('addPOIModal');
            form.reset();
            // Refresh search results if we have current location
            if (currentLocation) {
                searchNearbyLocations();
            }
        })
        .catch(error => {
            console.error('Error creating POI:', error);
            showToast(error.message || 'Error adding location. Please try again.', 'error');
        });
}

// Submit new route
function submitRoute() {
    const form = document.getElementById('addRouteForm');
    if (!form) return;

    const formData = new FormData(form);

    // Validate required fields
    const requiredFields = ['name', 'description', 'type'];
    for (const field of requiredFields) {
        if (!formData.get(field)) {
            showToast(`Please fill in the ${field} field`, 'error');
            return;
        }
    }

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
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        },
        body: JSON.stringify(routeData)
    })
        .then(response => {
            if (response.ok) {
                return response.json();
            }
            if (response.status === 401) {
                throw new Error('Please log in to add routes');
            }
            throw new Error('Failed to create route');
        })
        .then(route => {
            showToast('Route added successfully!', 'success');
            showToast('Route added successfully!', 'success');
            hideModal('addRouteModal');
            form.reset();
        })
        .catch(error => {
            console.error('Error creating route:', error);
            showToast(error.message || 'Error adding route. Please try again.', 'error');
        });
}

// Submit new review
function submitReview() {
    const form = document.getElementById('addReviewForm');
    if (!form) return;

    const formData = new FormData(form);
    const reviewData = {
        targetId: formData.get('targetId'),
        targetType: formData.get('targetType'),
        rating: parseInt(formData.get('rating')),
        title: formData.get('title'),
        comment: formData.get('comment'),
        isRecommended: formData.get('isRecommended') === 'on'
    };

    // Validate
    if (!reviewData.rating) {
        showToast('Please select a rating', 'error');
        return;
    }
    if (!reviewData.title || !reviewData.comment) {
        showToast('Please fill in both title and comment', 'error');
        return;
    }

    const submitBtn = form.querySelector('button[type="submit"]');
    const originalBtnHtml = submitBtn.innerHTML;
    submitBtn.innerHTML = '<i data-lucide="loader-2" class="w-4 h-4 animate-spin"></i> Submitting...';
    submitBtn.disabled = true;

    fetch('/reviews', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        },
        body: JSON.stringify(reviewData)
    })
        .then(response => {
            if (response.ok) return response.json();
            if (response.status === 401) throw new Error('Please log in to submit a review');
            return response.text().then(text => { throw new Error(text || 'Failed to submit review'); });
        })
        .then(review => {
            showToast('Review submitted successfully!', 'success');
            hideModal('addReviewModal');
            form.reset();
            // Refresh page to show new review
            setTimeout(() => location.reload(), 1500);
        })
        .catch(error => {
            console.error('Error submitting review:', error);
            showToast(error.message || 'Error submitting review. Please try again.', 'error');
            submitBtn.innerHTML = originalBtnHtml;
            submitBtn.disabled = false;
            if (typeof lucide !== 'undefined') lucide.createIcons();
        });
}

// Show toast notification
function showToast(message, type = 'info') {
    // Use the global notification system if available
    if (window.notify && typeof window.notify[type] === 'function') {
        window.notify[type](message);
    } else {
        // Fallback to console if custom toast not available
        console.log(`${type.toUpperCase()}: ${message}`);

        // Simple fallback toast with RTL support
        const toast = document.createElement('div');
        const isRtl = document.documentElement.dir === 'rtl';

        toast.className = `fixed top-4 ${isRtl ? 'left-4' : 'right-4'} z-50 px-6 py-3 rounded-xl shadow-lg text-white font-medium transition-all ${type === 'success' ? 'bg-green-600' :
            type === 'error' ? 'bg-red-600' :
                type === 'warning' ? 'bg-amber-600' :
                    'bg-blue-600'
            }`;
        toast.textContent = message;
        toast.dir = isRtl ? 'rtl' : 'ltr';
        document.body.appendChild(toast);

        // Animate in
        setTimeout(() => {
            toast.style.transform = 'translateY(0)';
            toast.style.opacity = '1';
        }, 10);

        // Remove after delay
        setTimeout(() => {
            toast.style.transform = 'translateY(-100%)';
            toast.style.opacity = '0';
            setTimeout(() => {
                if (toast.parentNode) {
                    toast.remove();
                }
            }, 300);
        }, 5000);
    }
}

// Helper to hide modals without Bootstrap
function hideModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        // Try Tailwind/Custom hidden class toggle
        modal.classList.add('hidden');
        modal.classList.remove('show', 'flex'); // Handle various potential states
        document.body.style.overflow = ''; // Restore scrolling

        // Remove backdrop if intrinsic
        const backdrop = document.querySelector('.modal-backdrop');
        if (backdrop) backdrop.remove();
    }
}