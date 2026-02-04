namespace CommunityCar.Application.Features.Dashboard.Performance.ViewModels;

public class DatabasePerformanceVM
{
    public double AverageQueryTime { get; set; }
    public double SlowQueryThreshold { get; set; }
    public int SlowQueryCount { get; set; }
    public int TotalQueries { get; set; }
    public double ConnectionPoolUsage { get; set; }
    public int ActiveConnections { get; set; }
    public int MaxConnections { get; set; }
    public double CacheHitRatio { get; set; }
    public long DatabaseSize { get; set; }
    public double IndexUsage { get; set; }
    public DateTime LastUpdated { get; set; }
}