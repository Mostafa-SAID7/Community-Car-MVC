/**
 * CC.Services.Post
 * Service for handling post creation and interactions.
 */
(function (CC) {
    class PostService extends CC.Utils.BaseService {
        constructor() {
            super('Posts');
        }

        /**
         * Creates a new post.
         * @param {FormData} formData 
         * @returns {Promise<object>}
         */
        async createPost(formData) {
            return await this.post('/Create', formData, {
                headers: {
                    // Let the browser set the content type for FormData (multipart/form-data)
                }
            });
        }

        /**
         * Validates post data before submission.
         * @param {object} data 
         * @returns {object} { isValid: boolean, message: string }
         */
        validatePost(data) {
            if (!data.title || data.title.trim().length === 0) {
                return { isValid: false, message: 'Title is required' };
            }
            if (!data.content || data.content.trim().length === 0) {
                return { isValid: false, message: 'Content is required' };
            }
            return { isValid: true };
        }
    }

    CC.Services.Post = new PostService();
})(window.CommunityCar);
