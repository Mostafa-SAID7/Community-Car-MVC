namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class PerformanceMetricsVM
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int AverageResponseTime { get; set; } // milliseconds
    public int TotalRequests { get; set; }
    public decimal ErrorRate { get; set; } // percentage
    public int Throughput { get; set; } // requests per second
    public decimal CpuUsage { get; set; } // percentage
    public decimal MemoryUsage { get; set; } // percentage
    public decimal DiskUsage { get; set; } // percentage
    public int NetworkLatency { get; set; } // milliseconds
    public int DatabaseConnections { get; set; }
    public decimal CacheHitRate { get; set; } // percentage
}

public class DatabasePerformanceVM
{
    public int ConnectionCount { get; set; }
    public int AverageQueryTime { get; set; } // milliseconds
    public int SlowQueryCount { get; set; }
    public int DeadlockCount { get; set; }
    public decimal CacheHitRatio { get; set; } // percentage
    public int DatabaseSize { get; set; } // MB
    public decimal IndexFragmentation { get; set; } // percentage
    public decimal BufferPoolUsage { get; set; } // percentage
}

public class SlowQueryVM
{
    public string Query { get; set; } = string.Empty;
    public int ExecutionTime { get; set; } // milliseconds
    public int ExecutionCount { get; set; }
    public int AverageTime { get; set; } // milliseconds
    public DateTime LastExecuted { get; set; }
}

public class CachePerformanceVM
{
    public decimal HitRate { get; set; } // percentage
    public decimal MissRate { get; set; } // percentage
    public int TotalRequests { get; set; }
    public int CacheSize { get; set; } // MB
    public int EvictionCount { get; set; }
    public int AverageGetTime { get; set; } // milliseconds
    public int AverageSetTime { get; set; } // milliseconds
}

public class EndpointPerformanceVM
{
    public string Endpoint { get; set; } = string.Empty;
    public int AverageResponseTime { get; set; } // milliseconds
    public int RequestCount { get; set; }
    public int ErrorCount { get; set; }
    public decimal ErrorRate { get; set; } // percentage
    public int ThroughputPerSecond { get; set; }
}

public class SystemResourcesVM
{
    public decimal CpuUsage { get; set; } // percentage
    public decimal MemoryUsage { get; set; } // percentage
    public decimal DiskUsage { get; set; } // percentage
    public int NetworkIn { get; set; } // MB/s
    public int NetworkOut { get; set; } // MB/s
    public int AvailableMemory { get; set; } // MB
    public int TotalMemory { get; set; } // MB
    public int DiskReadSpeed { get; set; } // MB/s
    public int DiskWriteSpeed { get; set; } // MB/s
    public int ActiveConnections { get; set; }
}