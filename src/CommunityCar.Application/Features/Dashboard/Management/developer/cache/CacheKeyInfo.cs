namespace CommunityCar.Application.Features.Dashboard.Management.developer.cache;

public class CacheKeyInfo
{
    public string Key { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public int AccessCount { get; set; }
}