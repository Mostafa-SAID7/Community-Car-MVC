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

            // Premium Red Design System Colors
            this.colors = {
                primary: '#fb2c36',
                primarySubtle: 'rgba(251, 44, 54, 0.1)',
                secondary: '#6366f1',
                secondarySubtle: 'rgba(99, 102, 241, 0.1)',
                success: '#10b981',
                warning: '#f59e0b',
                danger: '#ef4444',
                info: '#0ea5e9',
                grid: 'rgba(0, 0, 0, 0.05)',
                text: this.rootStyles.getPropertyValue('--muted-foreground').trim() || '#64748b'
            };
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

        initUserGrowthChart() {
            const canvas = document.getElementById('userGrowthChart');
            if (!canvas) return;

            const ctx = canvas.getContext('2d');
            const labels = JSON.parse(canvas.getAttribute('data-labels') || '[]');
            const datasets = JSON.parse(canvas.getAttribute('data-datasets') || '[]');

            // Map datasets to include premium styles
            const formattedDatasets = datasets.map((ds, index) => {
                const color = index === 0 ? this.colors.primary : this.colors.secondary;
                const bgColor = index === 0 ? this.colors.primarySubtle : this.colors.secondarySubtle;

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
                            labels: {
                                font: { weight: '800', size: 10 },
                                usePointStyle: true,
                                padding: 20,
                                color: this.colors.text
                            }
                        },
                        tooltip: {
                            backgroundColor: '#ffffff',
                            titleColor: '#000000',
                            bodyColor: '#64748b',
                            borderColor: 'rgba(0, 0, 0, 0.1)',
                            borderWidth: 1,
                            padding: 12,
                            cornerRadius: 12,
                            usePointStyle: true
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            grid: { color: this.colors.grid, drawBorder: false },
                            ticks: { font: { weight: '800', size: 10 }, color: this.colors.text, padding: 8 }
                        },
                        x: {
                            grid: { display: false },
                            ticks: { font: { weight: '800', size: 10 }, color: this.colors.text, padding: 8 }
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

            this.charts.activity = new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: labels,
                    datasets: [{
                        data: data,
                        backgroundColor: [
                            this.colors.primary,
                            this.colors.secondary,
                            this.colors.warning,
                            this.colors.success,
                            this.colors.info
                        ],
                        borderWidth: 0,
                        hoverOffset: 15
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    cutout: '75%',
                    plugins: {
                        legend: {
                            position: 'bottom',
                            labels: {
                                font: { weight: '800', size: 9 },
                                usePointStyle: true,
                                padding: 25,
                                color: this.colors.text
                            }
                        },
                        tooltip: {
                            backgroundColor: '#ffffff',
                            titleColor: '#000000',
                            bodyColor: '#64748b',
                            borderColor: 'rgba(0, 0, 0, 0.1)',
                            borderWidth: 1,
                            padding: 12,
                            cornerRadius: 12
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

