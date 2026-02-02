/**
 * Stories Service
 * Handles Stories API calls.
 */
(function (CC) {
    class StoriesService extends CC.Utils.BaseService {
        constructor() {
            super('Stories');
        }

        async getFeed(count = 20) {
            return this.get(`/stories/GetStoryFeed?count=${count}`);
        }

        async getActive() {
            return this.get('/stories/active');
        }

        async toggleLike(storyId, isLiked) {
            const method = isLiked ? 'DELETE' : 'POST';
            return this.fetch(`/stories/${storyId}/like`, { method });
        }

        async share(storyId) {
            return this.post(`/stories/${storyId}/share`);
        }

        async markViewed(storyId) {
            return this.post(`/stories/${storyId}/view`);
        }

        async createStory(formData) {
            return this.post('/stories/create', formData);
        }
    }

    CC.Services.Stories = new StoriesService();
})(window.CommunityCar);
