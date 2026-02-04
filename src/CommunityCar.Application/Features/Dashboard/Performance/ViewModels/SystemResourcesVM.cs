namespace CommunityCar.Application.Features.Dashboard.Performance.ViewModels;

public class SystemResourcesVM
{
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public double NetworkUsage { get; set; }
    public long TotalMemory { get; set; }
    public long AvailableMemory { get; set; }
    public long TotalDiskSpace { get; set; }
    public long AvailableDiskSpace { get; set; }
    public int ProcessCount { get; set; }
    public int ThreadCount { get; set; }
    public TimeSpan Uptime { get; set; }
    public DateTime LastUpdated { get; set; }
}