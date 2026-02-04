namespace CommunityCar.Application.Features.Dashboard.Overview.ViewModels;

public class SystemHealthVM
{
    public string Status { get; set; } = "Healthy";
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public int ActiveConnections { get; set; }
    public int QueuedJobs { get; set; }
    public int FailedJobs { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<SystemAlertVM> Alerts { get; set; } = new();
    public bool IsHealthy => Status == "Healthy";
}