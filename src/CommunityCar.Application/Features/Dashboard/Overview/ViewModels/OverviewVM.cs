using CommunityCar.Application.Features.Shared.ViewModels;
using CommunityCar.Application.Features.Dashboard.System.ViewModels;

namespace CommunityCar.Application.Features.Dashboard.Overview.ViewModels;

public class OverviewVM
{
    public DateTime LastUpdated { get; set; }
    public StatsVM QuickStats { get; set; } = new();
    public List<RecentActivityVM> RecentActivities { get; set; } = new();
    public List<ChartDataVM> UserGrowthChart { get; set; } = new();
    public List<ChartDataVM> ContentChart { get; set; } = new();
    public List<ChartDataVM> EngagementChart { get; set; } = new();
    public List<SystemAlertVM> SystemAlerts { get; set; } = new();
    public SystemHealthSummaryVM SystemHealth { get; set; } = new();
    public Dictionary<string, double> KeyPerformanceIndicators { get; set; } = new();
}

public class SystemHealthSummaryVM
{
    public string Status { get; set; } = string.Empty;
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public int ActiveConnections { get; set; }
    public int CriticalAlerts { get; set; }
    public int WarningAlerts { get; set; }
    public TimeSpan Uptime { get; set; }
}