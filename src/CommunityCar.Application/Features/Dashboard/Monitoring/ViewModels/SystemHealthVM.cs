namespace CommunityCar.Application.Features.Dashboard.Monitoring.ViewModels;

public class SystemHealthVM
{
    public string OverallStatus { get; set; } = string.Empty;
    public decimal HealthScore { get; set; }
    public DateTime LastChecked { get; set; }
    public List<ServiceHealthVM> Services { get; set; } = new();
    public SystemResourcesVM Resources { get; set; } = new();
    public List<HealthCheckVM> HealthChecks { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}

public class ServiceHealthVM
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime LastChecked { get; set; }
    public TimeSpan ResponseTime { get; set; }
    public string Version { get; set; } = string.Empty;
    public Dictionary<string, object> Metrics { get; set; } = new();
}

public class HealthCheckVM
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public DateTime CheckedAt { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
}

public class SystemResourcesVM
{
    public decimal CpuUsage { get; set; }
    public decimal MemoryUsage { get; set; }
    public decimal DiskUsage { get; set; }
    public decimal NetworkIn { get; set; }
    public decimal NetworkOut { get; set; }
    public long AvailableMemory { get; set; }
    public long TotalMemory { get; set; }
    public decimal DiskReadSpeed { get; set; }
    public decimal DiskWriteSpeed { get; set; }
    public int ActiveConnections { get; set; }
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