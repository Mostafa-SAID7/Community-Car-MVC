/**
 * Profile View Tracker
 * Automatically tracks profile views and view duration
 */
class ProfileViewTracker {
    constructor() {
        this.viewStartTime = Date.now();
        this.viewId = null;
        this.profileUserId = null;
        this.isTracking = false;
        this.heartbeatInterval = null;
        this.viewSource = this.detectViewSource();
        
        this.init();
    }

    init() {
        // Get profile user ID from page data
        this.profileUserId = this.getProfileUserId();
        
        if (this.profileUserId) {
            this.startTracking();
            this.setupEventListeners();
        }
    }

    getProfileUserId() {
        // Try to get from meta tag
        const metaTag = document.querySelector('meta[name="profile-user-id"]');
        if (metaTag) {
            return metaTag.getAttribute('content');
        }

        // Try to get from data attribute on body
        const bodyData = document.body.getAttribute('data-profile-user-id');
        if (bodyData) {
            return bodyData;
        }

        // Try to extract from URL pattern
        const urlMatch = window.location.pathname.match(/\/Profile\/Details\/([a-f0-9-]+)/i);
        if (urlMatch) {
            return urlMatch[1];
        }

        return null;
    }

    detectViewSource() {
        const referrer = document.referrer;
        const currentUrl = window.location.href;

        if (!referrer) {
            return 'direct';
        }

        if (referrer.includes('google.com') || referrer.includes('bing.com') || referrer.includes('yahoo.com')) {
            return 'search';
        }

        if (referrer.includes('facebook.com') || referrer.includes('twitter.com') || referrer.includes('linkedin.com')) {
            return 'social';
        }

        if (referrer.includes('/followers') || referrer.includes('/following')) {
            return 'followers';
        }

        if (referrer.includes('/feed') || referrer.includes('/news')) {
            return 'feed';
        }

        if (referrer.includes('/recommendations') || referrer.includes('/suggested')) {
            return 'recommendation';
        }

        // Check if from same domain
        try {
            const referrerDomain = new URL(referrer).hostname;
            const currentDomain = new URL(currentUrl).hostname;
            
            if (referrerDomain === currentDomain) {
                return 'internal';
            }
        } catch (e) {
            // Invalid URL
        }

        return 'external';
    }

    async startTracking() {
        if (this.isTracking) return;

        try {
            const response = await fetch('/Profile/Views/Record', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify({
                    profileUserId: this.profileUserId,
                    location: await this.getUserLocation(),
                    viewSource: this.viewSource
                })
            });

            if (response.ok) {
                const result = await response.json();
                if (result.success) {
                    this.isTracking = true;
                    this.startHeartbeat();
                    console.log('Profile view tracking started');
                }
            }
        } catch (error) {
            console.warn('Failed to start profile view tracking:', error);
            
            // Try anonymous tracking as fallback
            this.startAnonymousTracking();
        }
    }

    async startAnonymousTracking() {
        try {
            const response = await fetch('/Profile/Views/RecordAnonymous', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    profileUserId: this.profileUserId,
                    location: await this.getUserLocation(),
                    viewSource: this.viewSource
                })
            });

            if (response.ok) {
                const result = await response.json();
                if (result.success) {
                    this.isTracking = true;
                    console.log('Anonymous profile view tracking started');
                }
            }
        } catch (error) {
            console.warn('Failed to start anonymous profile view tracking:', error);
        }
    }

    startHeartbeat() {
        // Update view duration every 30 seconds
        this.heartbeatInterval = setInterval(() => {
            this.updateViewDuration();
        }, 30000);
    }

    async updateViewDuration() {
        if (!this.isTracking || !this.viewId) return;

        const duration = Math.floor((Date.now() - this.viewStartTime) / 1000);

        try {
            await fetch('/Profile/Views/UpdateDuration', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify({
                    viewId: this.viewId,
                    durationSeconds: duration
                })
            });
        } catch (error) {
            console.warn('Failed to update view duration:', error);
        }
    }

    setupEventListeners() {
        // Track when user leaves the page
        window.addEventListener('beforeunload', () => {
            this.stopTracking();
        });

        // Track when page becomes hidden/visible
        document.addEventListener('visibilitychange', () => {
            if (document.hidden) {
                this.pauseTracking();
            } else {
                this.resumeTracking();
            }
        });

        // Track scroll depth as engagement indicator
        let maxScrollDepth = 0;
        window.addEventListener('scroll', () => {
            const scrollDepth = Math.round((window.scrollY / (document.body.scrollHeight - window.innerHeight)) * 100);
            if (scrollDepth > maxScrollDepth) {
                maxScrollDepth = scrollDepth;
                this.trackEngagement('scroll', { depth: scrollDepth });
            }
        });

        // Track clicks as engagement
        document.addEventListener('click', (e) => {
            if (e.target.closest('a, button')) {
                this.trackEngagement('click', { element: e.target.tagName });
            }
        });
    }

    pauseTracking() {
        if (this.heartbeatInterval) {
            clearInterval(this.heartbeatInterval);
            this.heartbeatInterval = null;
        }
    }

    resumeTracking() {
        if (this.isTracking && !this.heartbeatInterval) {
            this.startHeartbeat();
        }
    }

    stopTracking() {
        if (this.heartbeatInterval) {
            clearInterval(this.heartbeatInterval);
        }

        // Final duration update
        this.updateViewDuration();
        this.isTracking = false;
    }

    trackEngagement(type, data) {
        // Could be extended to track specific engagement metrics
        console.log('Engagement tracked:', type, data);
    }

    async getUserLocation() {
        // Try to get user's location (with permission)
        return new Promise((resolve) => {
            if ('geolocation' in navigator) {
                navigator.geolocation.getCurrentPosition(
                    (position) => {
                        resolve(`${position.coords.latitude},${position.coords.longitude}`);
                    },
                    () => {
                        resolve(null);
                    },
                    { timeout: 5000 }
                );
            } else {
                resolve(null);
            }
        });
    }

    getAntiForgeryToken() {
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
    }
}

// Initialize profile view tracking when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    // Only initialize on profile pages
    if (window.location.pathname.includes('/Profile/') || 
        document.querySelector('[data-profile-user-id]') ||
        document.querySelector('meta[name="profile-user-id"]')) {
        
        window.profileViewTracker = new ProfileViewTracker();
    }
});

// Export for manual initialization if needed
window.ProfileViewTracker = ProfileViewTracker;