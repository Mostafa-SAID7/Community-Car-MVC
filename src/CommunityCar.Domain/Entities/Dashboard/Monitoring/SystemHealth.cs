using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Dashboard.Monitoring;

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
    public string? ErrorDetails { get; set; }
    public double Uptime { get; set; } // in hours
    public string? Version { get; set; }
    public string? Environment { get; set; }

    public SystemHealth()
    {
        CheckTime = DateTime.UtcNow;
        Status = "Healthy";
    }

    public void UpdateHealth(string status, double responseTime, double cpuUsage, 
        double memoryUsage, double diskUsage, int activeConnections, int errorCount)
    {
        Status = status;
        ResponseTime = responseTime;
        CpuUsage = cpuUsage;
        MemoryUsage = memoryUsage;
        DiskUsage = diskUsage;
        ActiveConnections = activeConnections;
        ErrorCount = errorCount;
        CheckTime = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public bool IsHealthy()
    {
        return Status == "Healthy" && ResponseTime < 1000 && CpuUsage < 80 && MemoryUsage < 80;
    }
}