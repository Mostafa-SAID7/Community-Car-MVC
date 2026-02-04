namespace CommunityCar.Application.Features.Dashboard.Cache.ViewModels;

public class CacheKeyDetailVM
{
    public string Key { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public long Size { get; set; }
    public string SizeFormatted { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public TimeSpan? TimeToLive { get; set; }
    public int AccessCount { get; set; }
    public DateTime LastAccessed { get; set; }
    public string Type { get; set; } = string.Empty;
}