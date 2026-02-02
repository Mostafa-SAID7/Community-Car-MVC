/**
 * Profile View Tracker
 * Automatically tracks profile views and view duration
 */
(function (CC) {
    class ViewTracker extends CC.Utils.BaseComponent {
        constructor() {
            super('ProfileViewTracker');
            this.viewStartTime = Date.now();
            this.viewId = null;
            this.profileUserId = null;
            this.isTracking = false;
            this.heartbeatInterval = null;
            this.viewSource = this.detectViewSource();
        }

        init() {
            if (this.initialized) return;
            super.init();

            this.profileUserId = this.getProfileUserId();
            if (this.profileUserId) {
                this.startTracking();
                this.setupTrackingListeners();
            }
        }

        getProfileUserId() {
            const meta = document.querySelector('meta[name="profile-user-id"]');
            if (meta) return meta.getAttribute('content');

            const body = document.body.getAttribute('data-profile-user-id');
            if (body) return body;

            const match = window.location.pathname.match(/\/Profile\/Details\/([a-f0-9-]+)/i);
            if (match) return match[1];

            return null;
        }

        detectViewSource() {
            const referrer = document.referrer;
            if (!referrer) return 'direct';
            if (referrer.includes('google') || referrer.includes('bing')) return 'search';
            if (referrer.includes('facebook') || referrer.includes('twitter')) return 'social';
            return 'external';
        }

        async startTracking() {
            if (this.isTracking) return;
            try {
                const res = await fetch('/Profile/Views/Record', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': this.getToken()
                    },
                    body: JSON.stringify({
                        profileUserId: this.profileUserId,
                        viewSource: this.viewSource
                    })
                });
                if (res.ok) {
                    const data = await res.json();
                    if (data.success) {
                        this.isTracking = true;
                        this.startHeartbeat();
                    }
                }
            } catch (e) { this.startAnonymousTracking(); }
        }

        async startAnonymousTracking() {
            // Fallback logic
        }

        startHeartbeat() {
            this.heartbeatInterval = setInterval(() => this.updateViewDuration(), 30000);
        }

        async updateViewDuration() {
            if (!this.isTracking) return;
            const duration = Math.floor((Date.now() - this.viewStartTime) / 1000);
            try {
                await fetch('/Profile/Views/UpdateDuration', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': this.getToken() },
                    body: JSON.stringify({ durationSeconds: duration })
                });
            } catch (e) { console.warn(e); }
        }

        setupTrackingListeners() {
            window.addEventListener('beforeunload', () => this.stopTracking());
            document.addEventListener('visibilitychange', () => {
                document.hidden ? this.pauseTracking() : this.resumeTracking();
            });
        }

        pauseTracking() { if (this.heartbeatInterval) clearInterval(this.heartbeatInterval); }
        resumeTracking() { if (this.isTracking) this.startHeartbeat(); }

        stopTracking() {
            if (this.heartbeatInterval) clearInterval(this.heartbeatInterval);
            this.updateViewDuration();
            this.isTracking = false;
        }

        getToken() {
            return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
        }
    }

    CC.Modules = CC.Modules || {};
    CC.Modules.Profile = CC.Modules.Profile || {};
    CC.Modules.Profile.ViewTracker = new ViewTracker();

})(window.CommunityCar);
