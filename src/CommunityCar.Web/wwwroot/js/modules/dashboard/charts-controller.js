/**
 * Dashboard Charts Controller
 * Handles initialization and updates for Chart.js instances with the Premium Red Design System.
 */
(function (CC) {
    class ChartsController extends CC.Utils.BaseComponent {
        constructor() {
            super('DashboardCharts');
            this.rootStyles = getComputedStyle(document.documentElement);
            this.charts = {};

            // Premium Red Design System Palette
            this.colors = {
                primary: '#fb2c36',
                primaryGradientStart: 'rgba(251, 44, 54, 0.3)',
                primaryGradientEnd: 'rgba(251, 44, 54, 0)',
                secondary: '#2563eb', // Executive Blue
                secondaryGradientStart: 'rgba(37, 99, 235, 0.2)',
                secondaryGradientEnd: 'rgba(37, 99, 235, 0)',
                text: 'rgba(255, 255, 255, 0.6)',
                textStrong: '#ffffff',
                grid: 'rgba(255, 255, 255, 0.05)',
                tooltipBg: '#1e1e1e',
                success: '#10b981',
                warning: '#f59e0b',
                info: '#0ea5e9',
                danger: '#ef4444'
            };

            // Global Defaults
            if (typeof Chart !== 'undefined') {
                Chart.defaults.color = this.colors.text;
                Chart.defaults.font.family = "'Inter', system-ui, -apple-system, sans-serif";
                Chart.defaults.font.weight = '700';
            }
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

            // Create Gradients
            const primaryGradient = ctx.createLinearGradient(0, 0, 0, 300);
            primaryGradient.addColorStop(0, this.colors.primaryGradientStart);
            primaryGradient.addColorStop(1, this.colors.primaryGradientEnd);

            const secondaryGradient = ctx.createLinearGradient(0, 0, 0, 300);
            secondaryGradient.addColorStop(0, this.colors.secondaryGradientStart);
            secondaryGradient.addColorStop(1, this.colors.secondaryGradientEnd);

            const formattedDatasets = datasets.map((ds, index) => {
                const isPrimary = index === 0;
                return {
                    ...ds,
                    borderColor: isPrimary ? this.colors.primary : this.colors.secondary,
                    backgroundColor: isPrimary ? primaryGradient : secondaryGradient,
                    borderWidth: 2.5,
                    tension: 0.45,
                    fill: true,
                    pointRadius: 0,
                    pointHoverRadius: 6,
                    pointHoverBackgroundColor: isPrimary ? this.colors.primary : this.colors.secondary,
                    pointHoverBorderColor: '#fff',
                    pointHoverBorderWidth: 2
                };
            });

            this.charts.userGrowth = new Chart(ctx, {
                type: 'line',
                data: { labels, datasets: formattedDatasets },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    interaction: { mode: 'index', intersect: false },
                    plugins: {
                        legend: {
                            display: false // Handled by custom header
                        },
                        tooltip: {
                            backgroundColor: this.colors.tooltipBg,
                            titleColor: this.colors.textStrong,
                            bodyColor: this.colors.text,
                            padding: 12,
                            cornerRadius: 8,
                            usePointStyle: true,
                            boxPadding: 6,
                            borderWidth: 1,
                            borderColor: 'rgba(255, 255, 255, 0.1)'
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            grid: { color: this.colors.grid, drawBorder: false },
                            ticks: {
                                color: this.colors.text,
                                font: { size: 10, weight: '700' },
                                padding: 10,
                                callback: (value) => value >= 1000 ? (value / 1000) + 'k' : value
                            }
                        },
                        x: {
                            grid: { display: false },
                            ticks: {
                                color: this.colors.text,
                                font: { size: 10, weight: '700' },
                                padding: 10
                            }
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
                        hoverOffset: 12,
                        borderRadius: 4
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    cutout: '82%',
                    plugins: {
                        legend: {
                            display: false // Handled by manual legend in view if needed
                        },
                        tooltip: {
                            backgroundColor: this.colors.tooltipBg,
                            padding: 12,
                            cornerRadius: 8,
                            usePointStyle: true,
                            boxPadding: 6
                        }
                    }
                }
            });
        }
    }

    // Initialize module namespace
    CC.Modules = CC.Modules || {};
    CC.Modules.Dashboard = CC.Modules.Dashboard || {};
    CC.Modules.Dashboard.Charts = new ChartsController();

})(window.CommunityCar);

