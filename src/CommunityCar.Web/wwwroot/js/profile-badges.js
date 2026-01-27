/**
 * Profile Badges Manager
 * Handles badge data loading and display
 */
class ProfileBadgesManager {
    constructor() {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => this.init());
        } else {
            this.init();
        }
    }

    init() {
        // Initialize skeleton loading
        if (window.Skeleton) {
            window.Skeleton.initPageLoad('badges-skeleton', 'earnedBadges');
        }

        // Initialize Lucide icons
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }

        this.loadBadgeData();
    }

    loadBadgeData() {
        // Get direction for localization
        const isRtl = document.documentElement.dir === 'rtl' || document.body.dir === 'rtl';

        // Mock data - in real app, this would come from API
        this.updateStats({
            total: '3',
            rare: '1',
            recent: '1',
            progress: '12%'
        });

        // Simple mock display logic for localized content
        const earnedContainer = document.getElementById('earnedBadges');
        if (earnedContainer) {
            const badgeTitle = isRtl ? "الخطوات الأولى" : "First Steps";
            const badgeDesc = isRtl ? "قمت بنشر محتواك الأول" : "Posted your first content";

            // Clear skeleton/loading state
            earnedContainer.innerHTML = '';

            earnedContainer.innerHTML = `
                <div class="flex items-center gap-4 p-4 bg-muted/20 rounded-xl border border-border">
                    <div class="w-12 h-12 rounded-full bg-primary/10 flex items-center justify-center text-primary">
                        <i data-lucide="award" class="w-6 h-6"></i>
                    </div>
                    <div>
                        <h4 class="font-bold text-foreground">${badgeTitle}</h4>
                        <p class="text-sm text-muted-foreground">${badgeDesc}</p>
                    </div>
                </div>
            `;

            if (typeof lucide !== 'undefined') lucide.createIcons();
        }
    }

    updateStats(stats) {
        const setText = (id, value) => {
            const el = document.getElementById(id);
            if (el) el.textContent = value;
        };

        setText('totalBadges', stats.total);
        setText('rareBadges', stats.rare);
        setText('recentBadges', stats.recent);
        setText('progressPercent', stats.progress);
    }
}

// Global instance
window.profileBadges = new ProfileBadgesManager();
