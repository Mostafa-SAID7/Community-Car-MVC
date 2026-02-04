namespace CommunityCar.Application.Features.Dashboard.Cache.ViewModels;

public class CacheManagementViewModel
{
    public bool IsRedisConnected { get; set; }
    public string CacheType { get; set; } = "Unknown";
    public Dictionary<string, object> Statistics { get; set; } = new();
}

public class CacheStatistics
{
    public long TotalKeys { get; set; }
    public long UsedMemory { get; set; }
    public long MaxMemory { get; set; }
    public double HitRate { get; set; }
    public double MissRate { get; set; }
    public int ConnectedClients { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class CacheKeyInfo
{
    public string Key { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public int AccessCount { get; set; }
}

public class CacheVM
{
    public CacheStatistics Statistics { get; set; } = new();
    public List<CacheKeyInfo> Keys { get; set; } = new();
    public bool IsRedisConnected { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
}