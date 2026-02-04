namespace CommunityCar.Application.Features.Dashboard.Cache.ViewModels;

public class CacheStatsVM
{
    public int TotalKeys { get; set; }
    public long TotalMemoryUsage { get; set; }
    public string TotalMemoryUsageFormatted { get; set; } = string.Empty;
    public int HitCount { get; set; }
    public int MissCount { get; set; }
    public double HitRatio { get; set; }
    public List<CacheKeyVM> TopKeys { get; set; } = new();
    public List<ChartDataVM> UsageChart { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

public class CacheKeyVM
{
    public string Key { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public long Size { get; set; }
    public string SizeFormatted { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int AccessCount { get; set; }
    public DateTime LastAccessed { get; set; }
}

public class ChartDataVM
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Color { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Category { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}