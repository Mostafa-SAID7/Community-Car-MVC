namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.developer.Performance.ViewModels;

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




