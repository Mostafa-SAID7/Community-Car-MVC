/**
 * Dashboard Charts Controller
 * Handles initialization and updates for Chart.js instances.
 */
(function (CC) {
    class ChartsController extends CC.Utils.BaseComponent {
        constructor() {
            super('DashboardCharts');
            this.rootStyles = getComputedStyle(document.documentElement);
            this.charts = {};
        }

        init() {
            if (this.initialized) return;
            super.init();

            if (typeof Chart !== 'undefined') {
                this.initUserGrowthChart();
                this.initActivityChart();
            } else {
                console.warn('Chart.js not loaded');
            }
        }

        getStyle(name, opacity = 1) {
            const val = this.rootStyles.getPropertyValue(name).trim();
            if (opacity < 1 && (val.startsWith('hsl') || !isNaN(parseFloat(val)))) {
                return `hsla(${val}, ${opacity})`;
            }
            return `hsl(${val})`;
        }

        initUserGrowthChart() {
            const canvas = document.getElementById('userGrowthChart');
            if (!canvas) return;

            const ctx = canvas.getContext('2d');
            const labels = JSON.parse(canvas.getAttribute('data-labels') || '[]');
            const datasets = JSON.parse(canvas.getAttribute('data-datasets') || '[]');

            const textColor = this.getStyle('--muted-foreground');
            const gridColor = this.getStyle('--chart-grid', 0.5);

            // Map datasets to include styles
            const formattedDatasets = datasets.map((ds, index) => {
                const colorVar = index === 0 ? '--chart-primary' : '--chart-secondary';
                const color = this.getStyle(colorVar);
                const bgColor = this.getStyle(colorVar, 0.1);

                return {
                    ...ds,
                    borderColor: color,
                    backgroundColor: bgColor,
                    borderWidth: 3,
                    tension: 0.4,
                    fill: true,
                    pointRadius: canvas.hasAttribute('data-show-points') ? 4 : 0,
                    pointHoverRadius: 6,
                    pointBackgroundColor: '#fff',
                    pointBorderColor: color,
                    pointBorderWidth: 2
                };
            });

            this.charts.userGrowth = new Chart(ctx, {
                type: 'line',
                data: { labels, datasets: formattedDatasets },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            display: datasets.length > 1,
                            position: 'top',
                            labels: { font: { weight: 'bold', size: 10 } }
                        },
                        tooltip: {
                            backgroundColor: 'rgba(255, 255, 255, 0.95)',
                            titleColor: '#0f172a',
                            bodyColor: '#334155',
                            borderColor: '#e2e8f0',
                            borderWidth: 1,
                            padding: 12,
                            cornerRadius: 8,
                            usePointStyle: true
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            grid: { color: gridColor, drawBorder: false },
                            ticks: { font: { weight: 'bold', size: 10 }, color: textColor, padding: 8 }
                        },
                        x: {
                            grid: { display: false },
                            ticks: { font: { weight: 'bold', size: 10 }, color: textColor, padding: 8 }
                        }
                    }
                }
            });
        }

        initActivityChart() {
            const canvas = document.getElementById('activityChart');
            if (!canvas) return;

            const ctx = canvas.getContext('2d');
            const labels = JSON.parse(canvas.getAttribute('data-labels') || '[]');
            const data = JSON.parse(canvas.getAttribute('data-values') || '[]');

            const primaryColor = this.getStyle('--chart-primary');
            const secondaryColor = this.getStyle('--chart-secondary');
            const accentVariant = this.getStyle('--chart-accent');

            this.charts.activity = new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: labels,
                    datasets: [{
                        data: data,
                        backgroundColor: [primaryColor, secondaryColor, '#f59e0b', '#ef4444', accentVariant],
                        borderWidth: 0,
                        weight: 1
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    cutout: '70%',
                    plugins: {
                        legend: {
                            position: 'bottom',
                            labels: {
                                font: { weight: 'bold', size: 9 },
                                usePointStyle: true,
                                padding: 20
                            }
                        }
                    }
                }
            });
        }
    }

    // Initialize module namespace if needed
    CC.Modules = CC.Modules || {};
    CC.Modules.Dashboard = CC.Modules.Dashboard || {};

    CC.Modules.Dashboard.Charts = new ChartsController();

})(window.CommunityCar);
