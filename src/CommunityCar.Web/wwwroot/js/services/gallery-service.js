/**
 * Gallery Service
 * Handles gallery media operations: upload, delete, filter, visibility.
 */
class GalleryService extends CC.Services.BaseService {
    constructor() {
        super('GalleryService');
    }

    /**
     * Upload media files
     * @param {File[]} files 
     * @param {object} options 
     */
    async uploadMedia(files, options = {}) {
        const formData = new FormData();
        files.forEach(file => formData.append('files', file));
        if (options.albumId) formData.append('albumId', options.albumId);
        if (options.isPublic !== undefined) formData.append('isPublic', options.isPublic);

        try {
            const response = await this.post('/Gallery/Upload', formData, {
                // BaseService.post might need to handle FormData correctly (no JSON stringify)
                // Assuming BaseService.post handles FormData if passed
            });
            if (response.success) {
                this.emit('media:uploaded', response.data);
            }
            return response;
        } catch (error) {
            this.handleError(error, 'upload:error');
            throw error;
        }
    }

    /**
     * Delete media item
     * @param {string} id 
     */
    async deleteMedia(id) {
        try {
            const response = await this.post(`/Gallery/Delete/${id}`);
            if (response.success) {
                this.emit('media:deleted', id);
            }
            return response;
        } catch (error) {
            this.handleError(error, 'delete:error');
            throw error;
        }
    }

    /**
     * Toggle media visibility
     * @param {string} id 
     */
    async toggleVisibility(id) {
        try {
            const response = await this.post(`/Gallery/ToggleVisibility/${id}`);
            if (response.success) {
                this.emit('media:visibilityToggled', { id, isPublic: response.isPublic });
            }
            return response;
        } catch (error) {
            this.handleError(error, 'visibility:error');
            throw error;
        }
    }

    /**
     * Get gallery items with filtering
     * @param {object} filters 
     */
    async getGallery(filters = {}) {
        const query = new URLSearchParams(filters).toString();
        return await this.get(`/api/gallery?${query}`);
    }
}

CC.Services.Gallery = new GalleryService();
