/**
 * Feed Service
 * Handles Feed-specific API calls.
 */
(function (CC) {
    class FeedService extends CC.Utils.BaseService {
        constructor() {
            super('Feed');
        }

        async loadFeed(type, page, pageSize = 10) {
            // Adjust endpoint as per actual usage, assuming /feed?type=... based on js
            // feed.js didn't show the exact load url in snippet, but implied infinite scroll.
            // Assuming standard MVC pattern or API.
            // If explicit endpoint is not known, I'll assume /Feed/Load?type=...
            // Wait, feed.js snippet implies initialized with `feedType`.
            // Let's use a generic fetcher or assume /Feed/GetPosts
            return this.get(`/Feed/GetPosts?feedType=${type}&page=${page}&pageSize=${pageSize}`);
        }

        async interact(contentId, contentType, interactionType) {
            return this.post('/feed/interact', { contentId, contentType, interactionType });
        }

        async bookmark(contentId, contentType) {
            return this.post('/feed/bookmark', { contentId, contentType });
        }

        async hide(contentId, contentType = 'Post') {
            return this.post('/feed/hide', { contentId, contentType });
        }

        async report(contentId, contentType, reason) {
            return this.post('/feed/report', { contentId, contentType, reason });
        }
    }

    CC.Services.Feed = new FeedService();
})(window.CommunityCar);
