namespace CommunityCar.Application.Features.Dashboard.System.ViewModels;

public class SystemResourcesVM
{
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public long MemoryTotal { get; set; }
    public long MemoryUsed { get; set; }
    public long MemoryFree { get; set; }
    public double DiskUsage { get; set; }
    public long DiskTotal { get; set; }
    public long DiskUsed { get; set; }
    public long DiskFree { get; set; }
    public double NetworkUsage { get; set; }
    public long NetworkIn { get; set; }
    public long NetworkOut { get; set; }
    public int ActiveConnections { get; set; }
    public int ThreadCount { get; set; }
    public int ProcessCount { get; set; }
    public TimeSpan Uptime { get; set; }
    public DateTime LastUpdated { get; set; }
}