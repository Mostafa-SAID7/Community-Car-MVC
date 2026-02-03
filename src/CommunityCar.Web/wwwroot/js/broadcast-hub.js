/**
 * BroadcastHub Client - Handles real-time group access and post updates
 */
class BroadcastHubClient {
    constructor() {
        this.connection = null;
        this.isConnected = false;
        this.reconnectAttempts = 0;
        this.maxReconnectAttempts = 5;
        this.reconnectDelay = 1000;
        this.subscribedGroups = new Set();
        this.subscribedPostTypes = new Set();
        
        this.init();
    }

    async init() {
        try {
            // Initialize SignalR connection
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl("/hubs/broadcast")
                .withAutomaticReconnect([0, 2000, 10000, 30000])
                .configureLogging(signalR.LogLevel.Information)
                .build();

            // Set up event handlers
            this.setupEventHandlers();
            
            // Start connection
            await this.start();
        } catch (error) {
            console.error('Failed to initialize BroadcastHub:', error);
            this.scheduleReconnect();
        }
    }

    setupEventHandlers() {
        // Connection events
        this.connection.onreconnecting(() => {
            console.log('BroadcastHub reconnecting...');
            this.isConnected = false;
            this.showConnectionStatus('Reconnecting...', 'warning');
        });

        this.connection.onreconnected(() => {
            console.log('BroadcastHub reconnected');
            this.isConnected = true;
            this.reconnectAttempts = 0;
            this.showConnectionStatus('Connected', 'success');
            this.resubscribeToGroups();
        });

        this.connection.onclose(() => {
            console.log('BroadcastHub connection closed');
            this.isConnected = false;
            this.showConnectionStatus('Disconnected', 'error');
            this.scheduleReconnect();
        });

        // Group access events
        this.connection.on('PostAccessGranted', (data) => {
            console.log('Post access granted:', data);
            this.handleAccessGranted(data);
        });

        this.connection.on('PostAccessDenied', (data) => {
            console.log('Post access denied:', data);
            this.handleAccessDenied(data);
        });

        this.connection.on('PostAccessRequested', (data) => {
            console.log('Post access requested:', data);
            this.handleAccessRequested(data);
        });

        this.connection.on('PostAccessError', (data) => {
            console.error('Post access error:', data);
            this.handleAccessError(data);
        });

        // Legacy group access events for backward compatibility
        this.connection.on('GroupPostAccessGranted', (data) => {
            console.log('Group access granted (legacy):', data);
            this.handleAccessGranted(data);
        });

        this.connection.on('GroupPostAccessDenied', (data) => {
            console.log('Group access denied (legacy):', data);
            this.handleAccessDenied(data);
        });

        this.connection.on('GroupPostAccessRequested', (data) => {
            console.log('Group access requested (legacy):', data);
            this.handleAccessRequested(data);
        });

        this.connection.on('GroupPostAccessError', (data) => {
            console.error('Group access error (legacy):', data);
            this.handleAccessError(data);
        });

        // Post update events
        this.connection.on('GroupPostsUpdate', (data) => {
            console.log('Group posts updated:', data);
            this.handleGroupPostsUpdate(data);
        });

        this.connection.on('AccessiblePostsUpdate', (data) => {
            console.log('Accessible posts updated:', data);
            this.handleAccessiblePostsUpdate(data);
        });

        this.connection.on('NewPostBroadcast', (postData) => {
            console.log('New post broadcast:', postData);
            this.handleNewPost(postData);
        });

        this.connection.on('PostInteractionBroadcast', (data) => {
            console.log('Post interaction broadcast:', data);
            this.handlePostInteraction(data);
        });

        // Member events
        this.connection.on('NewMemberJoined', (data) => {
            console.log('New member joined:', data);
            this.handleNewMember(data);
        });

        this.connection.on('JoinRequestApproved', (data) => {
            console.log('Join request approved:', data);
            this.handleJoinRequestApproved(data);
        });

        this.connection.on('JoinRequestDenied', (data) => {
            console.log('Join request denied:', data);
            this.handleJoinRequestDenied(data);
        });

        // Admin events
        this.connection.on('NewJoinRequest', (data) => {
            console.log('New join request:', data);
            this.handleNewJoinRequest(data);
        });

        // User group access events
        this.connection.on('UserGroupAccessUpdate', (data) => {
            console.log('User group access updated:', data);
            this.handleUserGroupAccessUpdate(data);
        });

        // Error events
        this.connection.on('AccessDenied', (message) => {
            console.error('Access denied:', message);
            this.showNotification('Access Denied', message, 'error');
        });
    }

    async start() {
        try {
            await this.connection.start();
            this.isConnected = true;
            this.reconnectAttempts = 0;
            console.log('BroadcastHub connected successfully');
            this.showConnectionStatus('Connected', 'success');
            
            // Load user's group access on connection
            await this.getUserGroupAccess();
        } catch (error) {
            console.error('Failed to start BroadcastHub:', error);
            this.scheduleReconnect();
        }
    }

    scheduleReconnect() {
        if (this.reconnectAttempts < this.maxReconnectAttempts) {
            this.reconnectAttempts++;
            const delay = this.reconnectDelay * Math.pow(2, this.reconnectAttempts - 1);
            
            console.log(`Scheduling reconnect attempt ${this.reconnectAttempts} in ${delay}ms`);
            setTimeout(() => this.start(), delay);
        } else {
            console.error('Max reconnect attempts reached');
            this.showConnectionStatus('Connection Failed', 'error');
        }
    }

    async resubscribeToGroups() {
        if (this.subscribedGroups.size > 0) {
            const groupIds = Array.from(this.subscribedGroups);
            for (const groupId of groupIds) {
                await this.joinGroupBroadcast(groupId);
            }
        }

        if (this.subscribedPostTypes.size > 0) {
            const postTypes = Array.from(this.subscribedPostTypes);
            await this.subscribeToPostUpdates(postTypes);
        }
    }

    // Public API methods
    async accessPosts(groupId, accessLevel = 'read') {
        if (!this.isConnected) {
            this.showNotification('Connection Error', 'Not connected to server', 'error');
            return;
        }

        try {
            await this.connection.invoke('AccessPosts', groupId, accessLevel);
        } catch (error) {
            console.error('Failed to access posts:', error);
            this.showNotification('Request Failed', 'Failed to access posts', 'error');
        }
    }

    async requestGroupPostAccess(groupId, accessLevel = 'read') {
        // Legacy method - redirect to new AccessPosts method
        return await this.accessPosts(groupId, accessLevel);
    }

    async joinGroupBroadcast(groupId) {
        if (!this.isConnected) return;

        try {
            await this.connection.invoke('JoinGroupBroadcast', groupId);
            this.subscribedGroups.add(groupId);
        } catch (error) {
            console.error('Failed to join group broadcast:', error);
        }
    }

    async leaveGroupBroadcast(groupId) {
        if (!this.isConnected) return;

        try {
            await this.connection.invoke('LeaveGroupBroadcast', groupId);
            this.subscribedGroups.delete(groupId);
        } catch (error) {
            console.error('Failed to leave group broadcast:', error);
        }
    }

    async getAccessiblePosts(page = 1, pageSize = 10, category = null, sortBy = 'recent') {
        if (!this.isConnected) return;

        try {
            await this.connection.invoke('GetAccessiblePosts', page, pageSize, category, sortBy);
        } catch (error) {
            console.error('Failed to get accessible posts:', error);
        }
    }

    async subscribeToPostUpdates(postTypes, groupIds = null) {
        if (!this.isConnected) return;

        try {
            await this.connection.invoke('SubscribeToPostUpdates', postTypes, groupIds);
            postTypes.forEach(type => this.subscribedPostTypes.add(type));
        } catch (error) {
            console.error('Failed to subscribe to post updates:', error);
        }
    }

    async unsubscribeFromPostUpdates(postTypes, groupIds = null) {
        if (!this.isConnected) return;

        try {
            await this.connection.invoke('UnsubscribeFromPostUpdates', postTypes, groupIds);
            postTypes.forEach(type => this.subscribedPostTypes.delete(type));
        } catch (error) {
            console.error('Failed to unsubscribe from post updates:', error);
        }
    }

    async getUserGroupAccess() {
        if (!this.isConnected) return;

        try {
            await this.connection.invoke('GetUserGroupAccess');
        } catch (error) {
            console.error('Failed to get user group access:', error);
        }
    }

    async processJoinRequest(groupId, requestUserId, approved, reason = '') {
        if (!this.isConnected) return;

        try {
            await this.connection.invoke('ProcessJoinRequest', groupId, requestUserId, approved, reason);
        } catch (error) {
            console.error('Failed to process join request:', error);
        }
    }

    // Event handlers
    handleAccessGranted(data) {
        this.showNotification('Access Granted', data.Message || 'You now have access to this group', 'success');
        
        // Trigger custom event
        window.dispatchEvent(new CustomEvent('groupAccessGranted', { detail: data }));
        
        // Auto-refresh posts if on a group page
        if (window.location.pathname.includes('/groups/')) {
            this.refreshCurrentPage();
        }
    }

    handleAccessDenied(data) {
        this.showNotification('Access Denied', data.Reason || 'Access to this group was denied', 'error');
        
        // Trigger custom event
        window.dispatchEvent(new CustomEvent('groupAccessDenied', { detail: data }));
    }

    handleAccessRequested(data) {
        this.showNotification('Request Sent', data.Message || 'Your join request has been sent to group administrators', 'info');
        
        // Trigger custom event
        window.dispatchEvent(new CustomEvent('groupAccessRequested', { detail: data }));
    }

    handleAccessError(data) {
        this.showNotification('Error', data.Error || 'An error occurred while processing your request', 'error');
    }

    handleGroupPostsUpdate(data) {
        // Trigger custom event for components to handle
        window.dispatchEvent(new CustomEvent('groupPostsUpdated', { detail: data }));
    }

    handleAccessiblePostsUpdate(data) {
        // Trigger custom event for components to handle
        window.dispatchEvent(new CustomEvent('accessiblePostsUpdated', { detail: data }));
    }

    handleNewPost(postData) {
        // Trigger custom event for real-time post updates
        window.dispatchEvent(new CustomEvent('newPostReceived', { detail: postData }));
        
        // Show notification for new posts in subscribed groups
        this.showNotification('New Post', 'A new post was added to one of your groups', 'info');
    }

    handlePostInteraction(data) {
        // Trigger custom event for real-time interaction updates
        window.dispatchEvent(new CustomEvent('postInteractionReceived', { detail: data }));
    }

    handleNewMember(data) {
        // Trigger custom event
        window.dispatchEvent(new CustomEvent('newMemberJoined', { detail: data }));
    }

    handleJoinRequestApproved(data) {
        this.showNotification('Request Approved', data.Message || 'Your join request has been approved!', 'success');
        
        // Trigger custom event
        window.dispatchEvent(new CustomEvent('joinRequestApproved', { detail: data }));
        
        // Refresh the page to show new content
        setTimeout(() => this.refreshCurrentPage(), 1000);
    }

    handleJoinRequestDenied(data) {
        this.showNotification('Request Denied', data.Reason || 'Your join request was denied', 'error');
        
        // Trigger custom event
        window.dispatchEvent(new CustomEvent('joinRequestDenied', { detail: data }));
    }

    handleNewJoinRequest(data) {
        // Show notification to admins
        this.showNotification('New Join Request', 'Someone wants to join your group', 'info');
        
        // Trigger custom event for admin panels
        window.dispatchEvent(new CustomEvent('newJoinRequestReceived', { detail: data }));
    }

    handleUserGroupAccessUpdate(data) {
        // Store user's group access information
        window.userGroupAccess = data.Groups;
        
        // Trigger custom event
        window.dispatchEvent(new CustomEvent('userGroupAccessUpdated', { detail: data }));
    }

    // UI Helper methods
    showConnectionStatus(message, type) {
        const statusElement = document.getElementById('connection-status');
        if (statusElement) {
            statusElement.textContent = message;
            statusElement.className = `connection-status ${type}`;
        }
    }

    showNotification(title, message, type = 'info') {
        // Try to use existing notification system
        if (typeof window !== 'undefined' && window.showNotification && typeof window.showNotification === 'function') {
            window.showNotification(title, message, type);
            return;
        }

        // Fallback to simple alert or console
        if (type === 'error') {
            console.error(`${title}: ${message}`);
        } else {
            console.log(`${title}: ${message}`);
        }

        // Create a simple toast notification
        this.createToastNotification(title, message, type);
    }

    createToastNotification(title, message, type) {
        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;
        toast.innerHTML = `
            <div class="toast-header">
                <strong>${title}</strong>
                <button type="button" class="toast-close" onclick="this.parentElement.parentElement.remove()">×</button>
            </div>
            <div class="toast-body">${message}</div>
        `;

        // Add to toast container or body
        let container = document.getElementById('toast-container');
        if (!container) {
            container = document.createElement('div');
            container.id = 'toast-container';
            container.className = 'toast-container';
            document.body.appendChild(container);
        }

        container.appendChild(toast);

        // Auto-remove after 5 seconds
        setTimeout(() => {
            if (toast.parentElement) {
                toast.remove();
            }
        }, 5000);
    }

    refreshCurrentPage() {
        // Trigger a soft refresh of dynamic content
        window.dispatchEvent(new CustomEvent('refreshContent'));
    }

    // Cleanup
    async disconnect() {
        if (this.connection) {
            await this.connection.stop();
            this.isConnected = false;
        }
    }
}

// Initialize the BroadcastHub client when the page loads
let broadcastHub;

document.addEventListener('DOMContentLoaded', function() {
    broadcastHub = new BroadcastHubClient();
    
    // Make it globally available
    window.broadcastHub = broadcastHub;
});

// Cleanup on page unload
window.addEventListener('beforeunload', function() {
    if (broadcastHub) {
        broadcastHub.disconnect();
    }
});