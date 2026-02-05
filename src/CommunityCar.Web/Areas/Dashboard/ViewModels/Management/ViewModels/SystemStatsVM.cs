namespace CommunityCar.Application.Features.Dashboard.Management.ViewModels;

public class SystemStatsVM
{
    public decimal CpuUsage { get; set; }
    public decimal MemoryUsage { get; set; }
    public decimal DiskUsage { get; set; }
    public int ActiveConnections { get; set; }
    public string SystemHealth { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}