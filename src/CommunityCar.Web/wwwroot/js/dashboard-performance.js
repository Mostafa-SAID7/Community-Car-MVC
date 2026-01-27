class PerformanceDashboard {
    constructor() {
        this.init();
    }

    init() {
        this.setupEventHandlers();
    }

    setupEventHandlers() {
        document.body.addEventListener('click', (event) => {
            const target = event.target.closest('[data-action]');
            if (!target) return;

            const action = target.dataset.action;

            switch (action) {
                case 'analyze-performance':
                    this.analyzePerformance();
                    break;
                case 'generate-report':
                    this.generateReport();
                    break;
                case 'get-critical-resources':
                    this.getCriticalResources();
                    break;
                case 'optimize-image':
                    this.optimizeImage();
                    break;
            }
        });
    }

    async analyzePerformance() {
        const urlInput = document.getElementById('analyzeUrl');
        const url = urlInput ? urlInput.value : '/';

        // Show loading state or toast
        if (window.showToast) window.showToast(`Analyzing performance for ${url}...`, 'info');

        // Mock API call simulation
        setTimeout(() => {
            const mockResults = {
                url: url,
                timestamp: new Date().toISOString(),
                metrics: {
                    lcp: Math.floor(Math.random() * 2000) + 500,
                    fid: Math.floor(Math.random() * 100),
                    cls: Math.random() * 0.1
                },
                status: 'Complete'
            };
            this.displayResults(mockResults);
            if (window.showToast) window.showToast('Analysis complete', 'success');
        }, 1500);
    }

    async generateReport() {
        if (window.showToast) window.showToast('Generating audit report...', 'info');
        setTimeout(() => {
            const mockReport = {
                reportId: 'RPT-' + Date.now(),
                generatedAt: new Date().toISOString(),
                itemsAudited: 42,
                score: 95
            };
            this.displayResults(mockReport);
            if (window.showToast) window.showToast('Audit report generated', 'success');
        }, 1200);
    }

    async getCriticalResources() {
        if (window.showToast) window.showToast('Identifying critical chain...', 'info');
        setTimeout(() => {
            const mockChain = {
                resources: [
                    'app.css',
                    'dashboard-performance.js',
                    'font-inter.woff2'
                ],
                totalSize: '1.2MB'
            };
            this.displayResults(mockChain);
        }, 1000);
    }

    async optimizeImage() {
        const form = document.getElementById('imageOptimizationForm');
        if (!form) return;

        const fileInput = document.getElementById('imageFile');
        if (!fileInput || !fileInput.files.length) {
            if (window.showToast) window.showToast('Please select an image first', 'warning');
            return;
        }

        if (window.showToast) window.showToast('Optimizing image asset...', 'info');

        setTimeout(() => {
            const mockOptimization = {
                originalName: fileInput.files[0].name,
                originalSize: '2.5MB',
                optimizedSize: '850KB',
                reduction: '66%',
                format: document.getElementById('format')?.value || 'webp'
            };
            this.displayResults(mockOptimization);
            if (window.showToast) window.showToast('Optimization complete', 'success');
        }, 2000);
    }

    displayResults(data) {
        const container = document.getElementById('resultsContainer');
        const content = document.getElementById('resultsContent');
        if (container && content) {
            content.textContent = JSON.stringify(data, null, 2);
            container.classList.remove('hidden');
            container.scrollIntoView({ behavior: 'smooth' });
        }
    }
}

// Initialize on DOMContentLoaded
document.addEventListener('DOMContentLoaded', () => {
    window.performanceDashboard = new PerformanceDashboard();
});
