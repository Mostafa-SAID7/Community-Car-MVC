/**
 * Search Service
 * Handles API interactions for search functionality.
 */
class SearchService {
    constructor() {
        this.baseUrl = '/shared/search';
    }

    /**
     * Get search suggestions
     * @param {string} query 
     * @param {number} maxResults 
     */
    async getSuggestions(query, maxResults = 8) {
        const response = await fetch(`${this.baseUrl}/suggestions?query=${encodeURIComponent(query)}&maxResults=${maxResults}`);
        if (!response.ok) throw new Error('Failed to fetch suggestions');
        return await response.json();
    }

    /**
     * Perform universal search
     * @param {URLSearchParams} params 
     */
    async search(params) {
        const response = await fetch(`${this.baseUrl}/universal?${params.toString()}`);
        if (!response.ok) throw new Error('Failed to perform search');
        return await response.json();
    }

    /**
     * Get trending items
     * @param {number} maxResults 
     */
    async getTrending(maxResults = 6) {
        const response = await fetch(`${this.baseUrl}/trending?maxResults=${maxResults}`);
        if (!response.ok) throw new Error('Failed to fetch trending items');
        return await response.json();
    }
}

// Register Service
window.CC = window.CC || {};
window.CC.Services = window.CC.Services || {};
window.CC.Services.Search = new SearchService();
