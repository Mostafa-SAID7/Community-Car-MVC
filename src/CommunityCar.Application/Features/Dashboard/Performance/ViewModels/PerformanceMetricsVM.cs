using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Dashboard.Performance.ViewModels;

public class PerformanceMetricsVM
{
    public double AverageResponseTime { get; set; }
    public double MinResponseTime { get; set; }
    public double MaxResponseTime { get; set; }
    public int TotalRequests { get; set; }
    public int SuccessfulRequests { get; set; }
    public int FailedRequests { get; set; }
    public double ErrorRate { get; set; }
    public double Throughput { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public int ActiveConnections { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<ChartDataVM> ResponseTimeChart { get; set; } = new();
    public List<ChartDataVM> ThroughputChart { get; set; } = new();
    public List<ChartDataVM> ErrorRateChart { get; set; } = new();
}