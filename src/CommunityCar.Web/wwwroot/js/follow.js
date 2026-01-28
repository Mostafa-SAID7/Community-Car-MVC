// Follow/Unfollow functionality
class FollowManager {
    constructor() {
        this.init();
    }

    init() {
        // Add event listeners for follow buttons
        document.addEventListener('click', (e) => {
            if (e.target.matches('[data-follow-action]')) {
                e.preventDefault();
                const userId = e.target.dataset.userId;
                if (userId) {
                    this.toggleFollow(userId);
                }
            }
        });
    }

    async toggleFollow(userId) {
        const btn = document.getElementById(`follow-btn-${userId}`);
        if (!btn) return;

        const originalText = btn.textContent;
        const isFollowing = btn.dataset.following === 'true';
        
        // Update button state
        btn.disabled = true;
        btn.textContent = 'Loading...';
        
        try {
            const response = await fetch(`/Profile/Follow/toggle/${userId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                }
            });
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            const result = await response.json();
            
            if (result.success) {
                this.updateFollowButton(btn, result.isFollowing);
                this.updateFollowersCount(userId, result.followersCount);
                
                // Show success notification
                this.showNotification(
                    result.isFollowing ? 'Now following user' : 'Unfollowed user',
                    'success'
                );
            } else {
                throw new Error(result.message || 'Failed to toggle follow');
            }
        } catch (error) {
            console.error('Error toggling follow:', error);
            btn.textContent = originalText;
            this.showNotification('Failed to update follow status', 'error');
        } finally {
            btn.disabled = false;
        }
    }

    updateFollowButton(btn, isFollowing) {
        btn.dataset.following = isFollowing.toString();
        
        if (isFollowing) {
            btn.innerHTML = `
                <span class="following-text">Following</span>
                <span class="unfollow-text hidden">Unfollow</span>
            `;
            btn.className = 'follow-btn px-4 py-2 rounded-2xl text-sm font-black uppercase tracking-widest transition-all hover:scale-105 active:scale-95 bg-muted text-foreground hover:bg-muted/80';
        } else {
            btn.innerHTML = '<span class="follow-text">Follow</span>';
            btn.className = 'follow-btn px-4 py-2 rounded-2xl text-sm font-black uppercase tracking-widest transition-all hover:scale-105 active:scale-95 bg-primary text-primary-foreground hover:bg-primary/90';
        }

        // Re-add hover effects for following state
        if (isFollowing) {
            this.addHoverEffects(btn);
        }
    }

    addHoverEffects(btn) {
        btn.addEventListener('mouseenter', function() {
            const followingText = this.querySelector('.following-text');
            const unfollowText = this.querySelector('.unfollow-text');
            if (followingText && unfollowText) {
                followingText.classList.add('hidden');
                unfollowText.classList.remove('hidden');
                this.classList.remove('bg-muted', 'hover:bg-muted/80');
                this.classList.add('bg-red-500', 'hover:bg-red-600', 'text-white');
            }
        });
        
        btn.addEventListener('mouseleave', function() {
            const followingText = this.querySelector('.following-text');
            const unfollowText = this.querySelector('.unfollow-text');
            if (followingText && unfollowText) {
                followingText.classList.remove('hidden');
                unfollowText.classList.add('hidden');
                this.classList.remove('bg-red-500', 'hover:bg-red-600', 'text-white');
                this.classList.add('bg-muted', 'hover:bg-muted/80');
            }
        });
    }

    updateFollowersCount(userId, newCount) {
        const countElement = document.getElementById(`followers-count-${userId}`);
        if (countElement) {
            countElement.textContent = newCount;
        }
    }

    getAntiForgeryToken() {
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
    }

    showNotification(message, type = 'info') {
        // Create notification element
        const notification = document.createElement('div');
        notification.className = `fixed top-4 right-4 z-50 px-6 py-3 rounded-2xl text-sm font-black uppercase tracking-widest transition-all duration-300 transform translate-x-full ${
            type === 'success' ? 'bg-green-500 text-white' :
            type === 'error' ? 'bg-red-500 text-white' :
            'bg-blue-500 text-white'
        }`;
        notification.textContent = message;
        
        document.body.appendChild(notification);
        
        // Animate in
        setTimeout(() => {
            notification.classList.remove('translate-x-full');
        }, 100);
        
        // Remove after 3 seconds
        setTimeout(() => {
            notification.classList.add('translate-x-full');
            setTimeout(() => {
                document.body.removeChild(notification);
            }, 300);
        }, 3000);
    }

    // Load follow stats for a user
    async loadFollowStats(userId) {
        try {
            const response = await fetch(`/Profile/Follow/stats/${userId}`);
            if (response.ok) {
                return await response.json();
            }
        } catch (error) {
            console.error('Error loading follow stats:', error);
        }
        return null;
    }
}

// Global follow function for backward compatibility
async function toggleFollow(userId) {
    if (window.followManager) {
        await window.followManager.toggleFollow(userId);
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    window.followManager = new FollowManager();
});