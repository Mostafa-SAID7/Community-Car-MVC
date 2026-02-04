namespace CommunityCar.Application.Features.Dashboard.System.ViewModels;

public class SystemHealthVM
{
    public string OverallStatus { get; set; } = string.Empty;
    public decimal CpuUsage { get; set; }
    public decimal MemoryUsage { get; set; }
    public decimal DiskUsage { get; set; }
    public string DatabaseStatus { get; set; } = string.Empty;
    public string CacheStatus { get; set; } = string.Empty;
    public string QueueStatus { get; set; } = string.Empty;
    public DateTime LastHealthCheck { get; set; }
    public TimeSpan Uptime { get; set; }
    public int ActiveConnections { get; set; }
    public int RequestsPerMinute { get; set; }
    public decimal ErrorRate { get; set; }
    public int ResponseTime { get; set; }
    
    // Additional properties used by MonitoringService
    public DateTime CheckTime { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int ErrorCount { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public bool IsHealthy { get; set; }
}