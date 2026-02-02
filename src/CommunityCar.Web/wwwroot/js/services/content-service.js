/**
 * Content Services
 * Unified services for News, Guides, and Reviews.
 */

class NewsService extends CC.Services.BaseService {
    constructor() { super('NewsService'); }

    async searchNews(params = {}) {
        const query = new URLSearchParams();
        Object.entries(params).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                query.append(key, value);
            }
        });

        const lang = document.documentElement.lang || 'en';
        return await this.get(`/${lang}/news?${query.toString()}`, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });
    }

    async getNews(params) { return await this.get(`/News/GetNews?${new URLSearchParams(params)}`); }
    async toggleLike(id) { return await this.post(`/News/ToggleLike/${id}`); }
    async toggleBookmark(id) { return await this.post(`/News/ToggleBookmark/${id}`); }
}

class GuidesService extends CC.Services.BaseService {
    constructor() { super('GuidesService'); }

    async searchGuides(params = {}, basePath = '') {
        const query = new URLSearchParams();
        Object.entries(params).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                query.append(key, value);
            }
        });

        const lang = document.documentElement.lang || 'en';
        const url = basePath ? `/${lang}/guides/${basePath}?${query.toString()}` : `/${lang}/guides?${query.toString()}`;

        return await this.get(url, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });
    }

    async getGuides(params) { return await this.get(`/Guides/GetGuides?${new URLSearchParams(params)}`); }
    async toggleBookmark(id) { return await this.post(`/Guides/ToggleBookmark/${id}`); }
    async rateGuide(id, rating) { return await this.post(`/Guides/Rate/${id}`, { rating }); }
}

class ReviewsService extends CC.Services.BaseService {
    constructor() { super('ReviewsService'); }

    async searchReviews(params = {}) {
        const query = new URLSearchParams();
        Object.entries(params).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                query.append(key, value);
            }
        });
        return await this.get(`/reviews?${query.toString()}`, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });
    }

    async getReviews(params) { return await this.get(`/Reviews/GetReviews?${new URLSearchParams(params)}`); }
    async markHelpful(id) { return await this.post(`/Reviews/MarkHelpful/${id}`); }
    async reportReview(id, reason) { return await this.post(`/Reviews/Report/${id}`, { reason }); }
}

CC.Services.News = new NewsService();
CC.Services.Guides = new GuidesService();
CC.Services.Reviews = new ReviewsService();
