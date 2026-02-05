using CommunityCar.Application.Features.Dashboard.Overview.ViewModels;
using CommunityCar.Application.Features.Dashboard.Management.developer.System.ViewModels;
using CommunityCar.Application.Features.Dashboard.Reports.Users.General;
using CommunityCar.Application.Features.Dashboard.Reports.Users.Security;

namespace CommunityCar.Application.Features.Dashboard.Management.ViewModels;

public class DashboardOverviewVM
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    
    // Missing properties that services expect
    public int NewUsersToday { get; set; }
    public DateTime LastUpdated { get; set; }
    
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
    public UserStatisticsReportVM UserStats { get; set; } = new();
    public ContentStatsVM ContentStats { get; set; } = new();
    public SecurityStatsVM SecurityStats { get; set; } = new();
    public List<RecentActivityVM> RecentActivities { get; set; } = new();
    public List<SystemAlertVM> SystemAlerts { get; set; } = new();
    public List<QuickActionVM> QuickActions { get; set; } = new();
}