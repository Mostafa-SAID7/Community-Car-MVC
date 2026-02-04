namespace CommunityCar.Application.Features.Dashboard.Management.ViewModels;

/// <summary>
/// ViewModel for system resources in management context
/// </summary>
public class SystemResourcesVM
{
    public decimal CpuUsage { get; set; }
    public decimal MemoryUsage { get; set; }
    public decimal DiskUsage { get; set; }
    public int NetworkIn { get; set; }
    public int NetworkOut { get; set; }
    public int AvailableMemory { get; set; }
    public int TotalMemory { get; set; }
    public int DiskReadSpeed { get; set; }
    public int DiskWriteSpeed { get; set; }
    public int ActiveConnections { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}