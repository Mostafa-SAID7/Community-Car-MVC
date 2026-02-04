using System.ComponentModel.DataAnnotations;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Dashboard.Monitoring.ViewModels;

public class MonitoringStatsVM
{
    public DateTime LastUpdated { get; set; }
    public SystemHealthVM SystemHealth { get; set; } = new();
    public PerformanceMetricsVM Performance { get; set; } = new();
    public SecurityMetricsVM Security { get; set; } = new();
    public UserActivityMetricsVM UserActivity { get; set; } = new();
    public List<ChartDataVM> SystemLoadTrend { get; set; } = new();
    public List<ChartDataVM> ErrorRateTrend { get; set; } = new();
    public List<SystemAlertVM> RecentAlerts { get; set; } = new();
    public Dictionary<string, double> KeyMetrics { get; set; } = new();
}

public class SystemHealthVM
{
    public string OverallStatus { get; set; } = string.Empty;
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public double NetworkUsage { get; set; }
    public int ActiveConnections { get; set; }
    public TimeSpan Uptime { get; set; }
    public List<ServiceStatusVM> Services { get; set; } = new();
}

public class ServiceStatusVM
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime LastChecked { get; set; }
    public string? ErrorMessage { get; set; }
}

public class PerformanceMetricsVM
{
    public double AverageResponseTime { get; set; }
    public double ThroughputPerSecond { get; set; }
    public double ErrorRate { get; set; }
    public int ActiveSessions { get; set; }
    public int QueuedJobs { get; set; }
    public int FailedJobs { get; set; }
    public double CacheHitRate { get; set; }
    public double DatabaseResponseTime { get; set; }
}

public class SecurityMetricsVM
{
    public int FailedLoginAttempts { get; set; }
    public int BlockedIPs { get; set; }
    public int SecurityAlerts { get; set; }
    public int SuspiciousActivities { get; set; }
    public DateTime LastSecurityScan { get; set; }
    public string SecurityStatus { get; set; } = string.Empty;
}

public class UserActivityMetricsVM
{
    public int OnlineUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewRegistrations { get; set; }
    public int TotalSessions { get; set; }
    public double AverageSessionDuration { get; set; }
    public int PageViews { get; set; }
    public int UniqueVisitors { get; set; }
}

public class SystemAlertVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public bool IsResolved { get; set; }
    public string? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
}