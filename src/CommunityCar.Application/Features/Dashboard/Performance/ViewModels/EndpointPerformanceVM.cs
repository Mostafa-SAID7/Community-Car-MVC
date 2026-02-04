namespace CommunityCar.Application.Features.Dashboard.Performance.ViewModels;

public class EndpointPerformanceVM
{
    public string Endpoint { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public double AverageResponseTime { get; set; }
    public double MinResponseTime { get; set; }
    public double MaxResponseTime { get; set; }
    public int TotalRequests { get; set; }
    public int SuccessfulRequests { get; set; }
    public int FailedRequests { get; set; }
    public double ErrorRate { get; set; }
    public double RequestsPerSecond { get; set; }
    public DateTime LastAccessed { get; set; }
}