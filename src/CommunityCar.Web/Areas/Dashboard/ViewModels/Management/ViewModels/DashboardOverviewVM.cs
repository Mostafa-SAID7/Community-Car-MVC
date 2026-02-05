using CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.ViewModels;
using CommunityCar.Web.Areas.Dashboard.ViewModels.Management.developer.System.ViewModels;
using CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.General;
using CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.Security;
using CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.Users.Trends;
using CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.Users.Security;

namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.ViewModels;

public class DashboardOverviewVM
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    
    // Missing properties that services expect
    public int NewUsersToday { get; set; }
    public DateTime LastUpdated { get; set; }
    
    // System health and performance
    public double SystemHealthScore { get; set; }
    public int ActiveServices { get; set; }
    public int TotalServices { get; set; }
    public int PendingTasks { get; set; }
    public int PendingModeration { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public TimeSpan SystemUptime { get; set; }
    
    public int TotalPosts { get; set; }
    public int TotalComments { get; set; }
    public string SystemHealth { get; set; } = string.Empty;
    public TimeSpan ServerUptime { get; set; }
    public long DatabaseSize { get; set; }
    public long StorageUsed { get; set; }
    public double BandwidthUsage { get; set; }
    public double ErrorRate { get; set; }
    public double ResponseTime { get; set; }
    public int ActiveSessions { get; set; }
    public int QueuedJobs { get; set; }
    public int FailedJobs { get; set; }
    public double CacheHitRate { get; set; }
    public DateTime? LastBackup { get; set; }
    public int SecurityAlerts { get; set; }
    public int PendingUpdates { get; set; }
    public DateTime? LicenseExpiry { get; set; }
    public bool MaintenanceMode { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public SystemStatsVM SystemStats { get; set; } = new();
    public DashboardUserOverviewVM UserStats { get; set; } = new();
    public ContentStatsVM ContentStats { get; set; } = new();
    public DashboardSecurityOverviewVM SecurityStats { get; set; } = new();
    public List<RecentActivityVM> RecentActivities { get; set; } = new();
    public List<SystemAlertVM> SystemAlerts { get; set; } = new();
    public List<QuickActionVM> QuickActions { get; set; } = new();
}




