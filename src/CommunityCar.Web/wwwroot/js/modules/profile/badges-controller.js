/**
 * Profile Badges Controller
 * Handles badge data loading and display
 */
(function (CC) {
    class BadgesController extends CC.Utils.BaseComponent {
        constructor() {
            super('ProfileBadges');
        }

        init() {
            if (this.initialized) return;
            super.init();

            if (window.Skeleton) window.Skeleton.initPageLoad('badges-skeleton', 'earnedBadges');
            if (typeof lucide !== 'undefined') lucide.createIcons();

            this.loadBadgeData();
        }

        loadBadgeData() {
            const isRtl = document.documentElement.dir === 'rtl' || document.body.dir === 'rtl';

            // Mock Data
            this.updateStats({
                total: '3',
                rare: '1',
                recent: '1',
                progress: '12%'
            });

            // Mock Display
            const earnedContainer = document.getElementById('earnedBadges');
            if (earnedContainer) {
                const badgeTitle = isRtl ? "الخطوات الأولى" : "First Steps";
                const badgeDesc = isRtl ? "قمت بنشر محتواك الأول" : "Posted your first content";

                earnedContainer.innerHTML = `
                <div class="flex items-center gap-4 p-4 bg-muted/20 rounded-xl border border-border">
                    <div class="w-12 h-12 rounded-full bg-primary/10 flex items-center justify-center text-primary">
                        <i data-lucide="award" class="w-6 h-6"></i>
                    </div>
                    <div>
                        <h4 class="font-bold text-foreground">${badgeTitle}</h4>
                        <p class="text-sm text-muted-foreground">${badgeDesc}</p>
                    </div>
                </div>`;

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

    CC.Modules = CC.Modules || {};
    CC.Modules.Profile = CC.Modules.Profile || {};
    CC.Modules.Profile.Badges = new BadgesController();

})(window.CommunityCar);
