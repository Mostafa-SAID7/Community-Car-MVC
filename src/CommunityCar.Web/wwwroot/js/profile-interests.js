/**
 * Profile Interests Manager
 * Handles interest management and category filtering
 */
class ProfileInterestsManager {
    constructor() {
        this.userInterests = [];
        this.suggestedInterests = [];
        this.categoryFilter = null;

        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => this.init());
        } else {
            this.init();
        }
    }

    init() {
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }

        this.setupEventHandlers();
        this.loadUserInterests();
        this.loadSuggestedInterests();
    }

    setupEventHandlers() {
        // Event delegation for all data-action buttons
        document.body.addEventListener('click', (event) => {
            const target = event.target.closest('[data-action]');
            if (!target) return;

            const action = target.dataset.action;
            const category = target.dataset.category;
            const interestId = target.dataset.interestId;
            const interestName = target.dataset.interestName;

            switch (action) {
                case 'filter-category':
                    this.filterByCategory(category);
                    break;
                case 'add-interest':
                    this.addInterest(interestId, interestName);
                    break;
                case 'remove-interest':
                    this.removeInterest(interestId);
                    break;
            }
        });
    }

    async loadUserInterests() {
        try {
            const response = await fetch('/Profile/GetUserInterests');
            const result = await response.json();

            if (result.success) {
                this.userInterests = result.data;
                this.displayUserInterests(this.userInterests);
                this.updateStats(this.userInterests);
                this.updateCategoryCounts(this.userInterests);
            } else {
                this.showEmptyState('interests-list-container');
            }
        } catch (error) {
            console.error('Error loading interests:', error);
            this.showEmptyState('interests-list-container', 'Failed to load interests');
        }
    }

    async loadSuggestedInterests() {
        try {
            const response = await fetch('/Profile/GetSuggestedInterests');
            const result = await response.json();

            if (result.success) {
                this.suggestedInterests = result.data;
                this.displaySuggestedInterests(this.suggestedInterests);
            }
        } catch (error) {
            console.error('Error loading suggested interests:', error);
        }
    }

    displayUserInterests(interests) {
        const container = document.getElementById('interests-list-container');
        if (!container) return;

        container.innerHTML = '';

        if (!interests || interests.length === 0) {
            container.innerHTML = `
                <div class="text-center py-12">
                    <i data-lucide="heart-off" class="w-16 h-16 text-muted-foreground mx-auto mb-4"></i>
                    <h3 class="text-lg font-bold text-foreground mb-2">No Interests Yet</h3>
                    <p class="text-sm text-muted-foreground">Start adding interests to personalize your experience</p>
                </div>
            `;
            if (typeof lucide !== 'undefined') lucide.createIcons();
            return;
        }

        interests.forEach((interest, index) => {
            const interestEl = document.createElement('div');
            interestEl.className = 'flex items-center justify-between p-4 rounded-xl bg-muted/20 hover:bg-muted/30 transition-all group';
            interestEl.innerHTML = `
                <div class="flex items-center gap-4 flex-1">
                    <div class="w-12 h-12 rounded-full bg-primary/10 flex items-center justify-center text-primary font-black text-lg">
                        ${index + 1}
                    </div>
                    <div>
                        <div class="text-sm font-black text-foreground">${interest.displayName}</div>
                        <div class="text-xs text-muted-foreground">${interest.category || 'General'} • Score: ${Math.round(interest.score)}</div>
                    </div>
                </div>
                <div class="flex items-center gap-3">
                    <div class="eng-progress-container h-1.5">
                        <div class="eng-progress-bar bg-primary transition-all duration-500" style="width: ${Math.min(interest.score, 100)}%"></div>
                    </div>
                    <button class="opacity-0 group-hover:opacity-100 p-2 text-muted-foreground hover:text-destructive transition-all" data-action="remove-interest" data-interest-id="${interest.id}">
                        <i data-lucide="x" class="w-4 h-4"></i>
                    </button>
                </div>
            `;
            container.appendChild(interestEl);
        });

        if (typeof lucide !== 'undefined') lucide.createIcons();
    }

    displaySuggestedInterests(suggestions) {
        const container = document.getElementById('suggestedInterests');
        if (!container) return;

        container.innerHTML = '';

        if (!suggestions || suggestions.length === 0) {
            container.innerHTML = `
                <div class="text-center py-8">
                    <i data-lucide="check-circle" class="w-8 h-8 text-green-500 mx-auto mb-2"></i>
                    <p class="text-xs text-muted-foreground">No new suggestions</p>
                </div>
            `;
            if (typeof lucide !== 'undefined') lucide.createIcons();
            return;
        }

        suggestions.slice(0, 5).forEach(suggestion => {
            const suggestionEl = document.createElement('div');
            suggestionEl.className = 'flex items-center justify-between p-3 rounded-lg hover:bg-muted/30 transition-colors';
            suggestionEl.innerHTML = `
                <div>
                    <div class="text-sm font-bold text-foreground">${suggestion.name}</div>
                    <div class="text-xs text-muted-foreground">${suggestion.reason}</div>
                </div>
                <button class="p-1 text-primary hover:bg-primary/10 rounded transition-all" data-action="add-interest" data-interest-id="${suggestion.id}" data-interest-name="${suggestion.name}">
                    <i data-lucide="plus" class="w-4 h-4"></i>
                </button>
            `;
            container.appendChild(suggestionEl);
        });

        if (typeof lucide !== 'undefined') lucide.createIcons();
    }

    filterByCategory(category) {
        this.categoryFilter = this.categoryFilter === category ? null : category;

        let filteredInterests = this.userInterests;
        if (this.categoryFilter) {
            filteredInterests = this.userInterests.filter(i =>
                i.category && i.category.toLowerCase() === this.categoryFilter.toLowerCase()
            );
        }

        this.displayUserInterests(filteredInterests);
    }

    async addInterest(interestId, interestName) {
        try {
            const response = await fetch('/Profile/AddInterest', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ interestId, interestName })
            });

            const result = await response.json();

            if (result.success) {
                this.loadUserInterests();
                this.loadSuggestedInterests();

                if (window.AlertModal) {
                    window.AlertModal.success(`Added ${interestName} to your interests!`);
                }
            } else {
                if (window.AlertModal) {
                    window.AlertModal.error(result.message || 'Failed to add interest');
                } else {
                    alert(result.message || 'Failed to add interest');
                }
            }
        } catch (error) {
            console.error('Error adding interest:', error);
            if (window.AlertModal) {
                window.AlertModal.error('An error occurred. Please try again.');
            } else {
                alert('An error occurred. Please try again.');
            }
        }
    }

    async removeInterest(interestId) {
        const confirmDelete = window.AlertModal
            ? await new Promise(resolve => {
                window.AlertModal.confirm('Are you sure you want to remove this interest?', resolve);
            })
            : confirm('Are you sure you want to remove this interest?');

        if (!confirmDelete) return;

        try {
            const response = await fetch(`/Profile/RemoveInterest/${interestId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            const result = await response.json();

            if (result.success) {
                this.userInterests = this.userInterests.filter(i => i.id !== interestId);
                this.displayUserInterests(this.userInterests);
                this.updateStats(this.userInterests);
                this.updateCategoryCounts(this.userInterests);

                if (window.AlertModal) {
                    window.AlertModal.success('Interest removed successfully!');
                }
            } else {
                if (window.AlertModal) {
                    window.AlertModal.error(result.message || 'Failed to remove interest');
                } else {
                    alert(result.message || 'Failed to remove interest');
                }
            }
        } catch (error) {
            console.error('Error removing interest:', error);
            if (window.AlertModal) {
                window.AlertModal.error('An error occurred. Please try again.');
            } else {
                alert('An error occurred. Please try again.');
            }
        }
    }

    updateStats(interests) {
        document.getElementById('totalInterests').textContent = interests.length;

        // Calculate top category
        const categoryCounts = {};
        interests.forEach(i => {
            const cat = i.category || 'General';
            categoryCounts[cat] = (categoryCounts[cat] || 0) + 1;
        });

        const topCat = Object.entries(categoryCounts).sort((a, b) => b[1] - a[1])[0];
        document.getElementById('topCategory').textContent = topCat ? topCat[0] : '-';

        // Calculate engagement score
        const avgScore = interests.length > 0
            ? Math.round(interests.reduce((sum, i) => sum + i.score, 0) / interests.length)
            : 0;
        document.getElementById('engagementScore').textContent = `${avgScore}%`;

        // Last updated
        document.getElementById('lastUpdated').textContent = 'Today';
    }

    updateCategoryCounts(interests) {
        const counts = {
            automotive: 0,
            performance: 0,
            restoration: 0,
            community: 0
        };

        interests.forEach(i => {
            const cat = (i.category || '').toLowerCase();
            if (counts.hasOwnProperty(cat)) {
                counts[cat]++;
            }
        });

        Object.keys(counts).forEach(cat => {
            const elem = document.getElementById(`${cat}Count`);
            if (elem) elem.textContent = counts[cat];
        });
    }

    showEmptyState(containerId, message = null) {
        const container = document.getElementById(containerId);
        if (!container) return;

        container.innerHTML = `
            <div class="text-center py-12">
                <i data-lucide="alert-circle" class="w-16 h-16 text-muted-foreground mx-auto mb-4"></i>
                <p class="text-sm text-muted-foreground">${message || 'No data available'}</p>
            </div>
        `;

        if (typeof lucide !== 'undefined') lucide.createIcons();
    }
}

// Global instance
window.profileInterests = new ProfileInterestsManager();
