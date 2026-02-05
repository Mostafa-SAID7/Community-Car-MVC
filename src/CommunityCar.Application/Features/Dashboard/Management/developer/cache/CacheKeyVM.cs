namespace CommunityCar.Application.Features.Dashboard.Management.developer.cache;

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