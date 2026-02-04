using System.ComponentModel.DataAnnotations;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Dashboard.Performance.ViewModels;

public class PerformanceMetricsVM
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime LastUpdated { get; set; }
    public CoreWebVitalsVM CoreWebVitals { get; set; } = new();
    public List<PerformanceIssueVM> Issues { get; set; } = new();
    public double AverageResponseTime { get; set; }
    public double MedianResponseTime { get; set; }
    public double P95ResponseTime { get; set; }
    public double P99ResponseTime { get; set; }
    public double ThroughputPerSecond { get; set; }
    public decimal ErrorRate { get; set; }
    public int TotalRequests { get; set; }
    public int SuccessfulRequests { get; set; }
    public int FailedRequests { get; set; }
    public decimal CpuUsage { get; set; }
    public decimal MemoryUsage { get; set; }
    public decimal DiskUsage { get; set; }
    public double NetworkUsage { get; set; }
    public int ActiveConnections { get; set; }
    public int Throughput { get; set; }
    public int NetworkLatency { get; set; }
    public int DatabaseConnections { get; set; }
    public decimal CacheHitRate { get; set; }
    public List<ChartDataVM> ResponseTimeTrend { get; set; } = new();
    public List<ChartDataVM> ThroughputTrend { get; set; } = new();
    public List<ChartDataVM> ErrorRateTrend { get; set; } = new();
}

public class CoreWebVitalsVM
{
    public double LargestContentfulPaint { get; set; }
    public double FirstInputDelay { get; set; }
    public double CumulativeLayoutShift { get; set; }
    public double FirstContentfulPaint { get; set; }
    public double TimeToInteractive { get; set; }
    public double TotalBlockingTime { get; set; }
    public string Grade { get; set; } = "Unknown";
    public DateTime LastMeasured { get; set; }
    
    // Additional properties for view compatibility
    public double LCP => LargestContentfulPaint;
    public double FID => FirstInputDelay;
    public double CLS => CumulativeLayoutShift;
    public string LCPGrade => Grade;
    public string FIDGrade => Grade;
    public string CLSGrade => Grade;
}

public class PerformanceIssueVM
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = "Info";
    public string Recommendation { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
    
    // Additional properties for view compatibility
    public string Message => Description;
    public string Impact => Severity;
    public string Resource => Type;
    public string Fix => Recommendation;
}

public class PerformanceReportVM
{
    public DateTime GeneratedAt { get; set; }
    public string ReportId { get; set; } = string.Empty;
    public PerformanceMetricsVM Metrics { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public string Summary { get; set; } = string.Empty;
}

public class ResourceVM
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public long Size { get; set; }
    public double LoadTime { get; set; }
    public bool IsBlocking { get; set; }
    public bool IsCritical { get; set; }
}

public class ResourceAnalysisVM
{
    public List<ResourceVM> Resources { get; set; } = new();
    public int TotalResources { get; set; }
    public long TotalSize { get; set; }
    public double TotalLoadTime { get; set; }
    public List<string> Recommendations { get; set; } = new();
}

public class DatabasePerformanceVM
{
    public string Provider { get; set; } = string.Empty;
    public double AverageQueryTime { get; set; }
    public double SlowestQueryTime { get; set; }
    public int TotalQueries { get; set; }
    public int SlowQueries { get; set; }
    public double ConnectionPoolUsage { get; set; }
    public int ActiveConnections { get; set; }
    public int MaxConnections { get; set; }
    public long DatabaseSize { get; set; }
    public double IndexUsage { get; set; }
    public int ConnectionCount { get; set; }
    public int SlowQueryCount { get; set; }
    public int DeadlockCount { get; set; }
    public decimal CacheHitRatio { get; set; }
    public decimal IndexFragmentation { get; set; }
    public decimal BufferPoolUsage { get; set; }
    public List<SlowQueryVM> TopSlowQueries { get; set; } = new();
    public List<ChartDataVM> QueryTimeTrend { get; set; } = new();
}

public class SlowQueryVM
{
    public string Query { get; set; } = string.Empty;
    public double ExecutionTime { get; set; }
    public int ExecutionCount { get; set; }
    public double AverageTime { get; set; }
    public DateTime LastExecuted { get; set; }
    public string Database { get; set; } = string.Empty;
    public string Table { get; set; } = string.Empty;
}

public class EndpointPerformanceVM
{
    public string Endpoint { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public double AverageResponseTime { get; set; }
    public double MedianResponseTime { get; set; }
    public double P95ResponseTime { get; set; }
    public int RequestCount { get; set; }
    public decimal ErrorRate { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public int ThroughputPerSecond { get; set; }
    public DateTime LastAccessed { get; set; }
    public List<ChartDataVM> ResponseTimeTrend { get; set; } = new();
}

public class SystemResourcesVM
{
    public decimal CpuUsage { get; set; }
    public decimal MemoryUsage { get; set; }
    public long MemoryTotal { get; set; }
    public long MemoryUsed { get; set; }
    public long TotalMemory { get; set; }
    public int ActiveConnections { get; set; }
    public decimal DiskUsage { get; set; }
    public long DiskTotal { get; set; }
    public long DiskUsed { get; set; }
    public double NetworkUsage { get; set; }
    public decimal NetworkIn { get; set; }
    public decimal NetworkOut { get; set; }
    public int ProcessCount { get; set; }
    public int ThreadCount { get; set; }
    public TimeSpan Uptime { get; set; }
    public DateTime LastUpdated { get; set; }
    public long AvailableMemory { get; set; }
    public int DiskReadSpeed { get; set; }
    public int DiskWriteSpeed { get; set; }
    public List<ProcessInfoVM> TopProcesses { get; set; } = new();
}

public class ProcessInfoVM
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double CpuUsage { get; set; }
    public long MemoryUsage { get; set; }
    public int ThreadCount { get; set; }
    public DateTime StartTime { get; set; }
}