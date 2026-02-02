/**
 * Social Service
 * Handles follow/unfollow, friend requests, and blocking via API.
 */
class SocialService extends CC.Services.BaseService {
    constructor() {
        super('SocialService');
    }

    /**
     * Follow/Unfollow a user
     * @param {string} userId - Target user ID
     */
    async toggleFollow(userId) {
        try {
            const response = await this.post(`/profile/follow/${userId}`);
            if (response.success) {
                this.emit('follow:toggled', { userId, isFollowing: response.isFollowing });
            }
            return response;
        } catch (error) {
            this.handleError(error, 'follow:error');
            throw error;
        }
    }

    /**
     * Send a friend request
     * @param {string} userId - Target user ID
     */
    async sendFriendRequest(userId) {
        try {
            const response = await this.post(`/friends/request/${userId}`);
            if (response.success) {
                this.emit('friendRequest:sent', { userId });
            }
            return response;
        } catch (error) {
            this.handleError(error, 'friendRequest:error');
            throw error;
        }
    }

    /**
     * Respond to a friend request
     * @param {string} requestId - Request ID
     * @param {boolean} accept - Whether to accept or reject
     */
    async respondToFriendRequest(requestId, accept) {
        try {
            const action = accept ? 'accept' : 'reject';
            const response = await this.post(`/friends/request/${requestId}/${action}`);
            if (response.success) {
                this.emit('friendRequest:responded', { requestId, accept });
            }
            return response;
        } catch (error) {
            this.handleError(error, 'friendRequest:error');
            throw error;
        }
    }

    /**
     * Remove a friend
     * @param {string} userId - Target user ID
     */
    async removeFriend(userId) {
        try {
            const response = await this.post(`/friends/remove/${userId}`);
            if (response.success) {
                this.emit('friend:removed', { userId });
            }
            return response;
        } catch (error) {
            this.handleError(error, 'friendRequest:error');
            throw error;
        }
    }

    /**
     * Block/Unblock a user
     * @param {string} userId - Target user ID
     */
    async toggleBlock(userId) {
        try {
            const response = await this.post(`/friends/block/${userId}`);
            if (response.success) {
                this.emit('block:toggled', { userId, isBlocked: response.isBlocked });
            }
            return response;
        } catch (error) {
            this.handleError(error, 'block:error');
            throw error;
        }
    }

    /**
     * Get friend status
     * @param {string} userId 
     */
    async getStatus(userId) {
        return await this.get(`/friends/status/${userId}`);
    }
}

CC.Services.Social = new SocialService();
