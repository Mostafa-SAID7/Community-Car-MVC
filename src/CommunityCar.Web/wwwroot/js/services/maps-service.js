/**
 * Maps Service
 * Handles API interactions for Maps, POIs, and Routes.
 */
class MapsService {
    constructor() {
        this.baseUrl = '/maps';
    }

    /**
     * Search for locations
     * @param {URLSearchParams} params 
     */
    async search(params) {
        const response = await fetch(`${this.baseUrl}/search?${params.toString()}`);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    }

    /**
     * Get POI Details
     * @param {string} id 
     */
    async getPoiDetails(id) {
        const response = await fetch(`${this.baseUrl}/poi/${id}/json`);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    }

    /**
     * Get Route Details
     * @param {string} id 
     */
    async getRouteDetails(id) {
        const response = await fetch(`${this.baseUrl}/routes/${id}/json`);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    }

    /**
     * Create a new POI
     * @param {object} data
     * @param {string} token - Anti-forgery token
     */
    async createPoi(data, token) {
        const response = await fetch(`${this.baseUrl}/poi`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify(data)
        });

        if (response.ok) return await response.json();
        if (response.status === 401) throw new Error('Please log in to add locations');
        throw new Error('Failed to create location');
    }

    /**
     * Create a new Route
     * @param {object} data 
     * @param {string} token 
     */
    async createRoute(data, token) {
        const response = await fetch(`${this.baseUrl}/routes`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify(data)
        });

        if (response.ok) return await response.json();
        if (response.status === 401) throw new Error('Please log in to add routes');
        throw new Error('Failed to create route');
    }

    /**
     * Check in to a location
     * @param {string} poiId 
     * @param {object} data 
     * @param {string} token 
     */
    async checkIn(poiId, data, token) {
        const response = await fetch(`${this.baseUrl}/poi/${poiId}/checkin`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify(data)
        });

        if (response.ok) return await response.json();
        if (response.status === 401) throw new Error('Please log in to check in');
        throw new Error('Check-in failed');
    }
}

// Register Service
window.CC = window.CC || {};
window.CC.Services = window.CC.Services || {};
window.CC.Services.Maps = new MapsService();
