namespace CommunityCar.Application.Features.Dashboard.Management.ViewModels;

public class DashboardOverviewVM
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
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
    public UserStatsVM UserStats { get; set; } = new();
    public ContentStatsVM ContentStats { get; set; } = new();
    public SecurityStatsVM SecurityStats { get; set; } = new();
    public List<RecentActivityVM> RecentActivities { get; set; } = new();
    public List<SystemAlertVM> SystemAlerts { get; set; } = new();
    public List<QuickActionVM> QuickActions { get; set; } = new();
}

public class SystemStatsVM
{
    public decimal CpuUsage { get; set; }
    public decimal MemoryUsage { get; set; }
    public decimal DiskUsage { get; set; }
    public int ActiveConnections { get; set; }
    public string SystemHealth { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}

public class UserStatsVM
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsersToday { get; set; }
    public int OnlineUsers { get; set; }
    public decimal UserGrowthRate { get; set; }
}

public class ContentStatsVM
{
    public int TotalPosts { get; set; }
    public int TotalComments { get; set; }
    public int PostsToday { get; set; }
    public int CommentsToday { get; set; }
    public int PendingModeration { get; set; }
}

public class SecurityStatsVM
{
    public int FailedLogins { get; set; }
    public int BlockedIps { get; set; }
    public int SecurityThreats { get; set; }
    public int SuspiciousActivities { get; set; }
    public decimal SecurityScore { get; set; }
}

public class RecentActivityVM
{
    public Guid Id { get; set; }
    public string Activity { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

public class SystemAlertVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public string Type { get; set; } = string.Empty;
}

public class QuickActionVM
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}