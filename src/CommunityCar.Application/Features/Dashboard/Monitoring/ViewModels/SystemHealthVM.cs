namespace CommunityCar.Application.Features.Dashboard.Monitoring.ViewModels;

public class SystemHealthVM
{
    public DateTime CheckTime { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public double ResponseTime { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public int ActiveConnections { get; set; }
    public int ErrorCount { get; set; }
    public double Uptime { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public bool IsHealthy { get; set; }
    public string OverallStatus { get; set; } = string.Empty;
}