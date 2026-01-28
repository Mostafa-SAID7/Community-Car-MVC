using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Dashboard.System;

public class SystemHealth : BaseEntity
{
    public DateTime CheckTime { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Healthy, Warning, Critical, Down
    public double ResponseTime { get; set; } // in milliseconds
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public int ActiveConnections { get; set; }
    public int ErrorCount { get; set; }
    public int WarningCount { get; set; }
    public string? ErrorDetails { get; set; }
    public string? Issues { get; set; }
    public double Uptime { get; set; } // in hours
    public string? Version { get; set; }
    public string? Environment { get; set; }
    public bool IsHealthy { get; set; }

    public SystemHealth()
    {
        CheckTime = DateTime.UtcNow;
        Status = "Healthy";
    }

    public void UpdateHealth(double cpuUsage, double memoryUsage, double diskUsage,
        int activeConnections, double responseTime, int errorCount, int warningCount,
        string? issues = null)
    {
        CpuUsage = cpuUsage;
        MemoryUsage = memoryUsage;
        DiskUsage = diskUsage;
        ActiveConnections = activeConnections;
        ResponseTime = responseTime;
        ErrorCount = errorCount;
        WarningCount = warningCount;
        Issues = issues;
        IsHealthy = cpuUsage < 80 && memoryUsage < 80 && diskUsage < 90 && errorCount < 10;
        Status = IsHealthy ? "Healthy" : "Warning";
        CheckTime = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public bool IsSystemHealthy()
    {
        return Status == "Healthy" && ResponseTime < 1000 && CpuUsage < 80 && MemoryUsage < 80;
    }
}