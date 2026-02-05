using CommunityCar.Application.Features.Shared.ViewModels;
using CommunityCar.Web.Areas.Dashboard.ViewModels.Management.developer.System.ViewModels;
using CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.Users.Activity;

namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.ViewModels;

public class OverviewVM
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime LastUpdated { get; set; }
    public string TimeRange { get; set; } = "7d";
    
    // Missing properties that services expect
    public int TotalUsers { get; set; }
    public int NewUsersToday { get; set; }
    public decimal SecurityScore { get; set; }
    
    // Dashboard specific properties
    public int ActiveUsers { get; set; }
    public double ActiveUsersPercentage { get; set; }
    public int TotalPosts { get; set; }
    public int NewPostsToday { get; set; }
    public double EngagementRate { get; set; }
    public string EngagementTrend { get; set; } = "up";
    public int TotalComments { get; set; }
    public int TotalLikes { get; set; }
    public int TotalShares { get; set; }
    public int TotalViews { get; set; }
    public int TotalStories { get; set; }
    public int TotalReviews { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public string DatabaseStatus { get; set; } = "Healthy";
    public string CacheStatus { get; set; } = "Healthy";
    public string QueueStatus { get; set; } = "Healthy";
    
    public StatsVM Stats { get; set; } = new();
    public StatsVM QuickStats { get; set; } = new();
    public List<RecentActivityVM> RecentActivity { get; set; } = new();
    public List<RecentActivityVM> RecentActivities { get; set; } = new();
    public List<TopContentVM> TopContent { get; set; } = new();
    public List<ActiveUserVM> ActiveUserList { get; set; } = new();
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
    public bool IsHealthy { get; set; }
    public DateTime LastCheck { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public int ActiveConnections { get; set; }
    public int CriticalAlerts { get; set; }
    public int WarningAlerts { get; set; }
    public TimeSpan Uptime { get; set; }
}




