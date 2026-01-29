namespace CommunityCar.Application.Features.Account.DTOs.Media;

#region Gallery Analytics DTOs

public class GalleryAnalyticsDTO
{
    public Guid UserId { get; set; }
    public int TotalItems { get; set; }
    public int PublicItems { get; set; }
    public int PrivateItems { get; set; }
    public long TotalStorageSize { get; set; }
    public int TotalViews { get; set; }
    public List<string> PopularTags { get; set; } = new();
    public List<UserGalleryDTO> MostViewedItems { get; set; } = new();
    public List<UserGalleryDTO> RecentItems { get; set; } = new();
}

public class GalleryUsageStatsDTO
{
    public Guid UserId { get; set; }
    public long StorageUsed { get; set; }
    public long StorageLimit { get; set; }
    public double StorageUsagePercentage { get; set; }
    public int ItemsThisMonth { get; set; }
    public int ViewsThisMonth { get; set; }
    public Dictionary<string, int> ItemsByMimeType { get; set; } = new();
    public Dictionary<string, long> StorageByMimeType { get; set; } = new();
    public Dictionary<DateTime, int> UploadsByDate { get; set; } = new();
}

public class GalleryTagStatsDTO
{
    public Guid UserId { get; set; }
    public int TotalTags { get; set; }
    public Dictionary<string, int> TagUsageCount { get; set; } = new();
    public List<string> TrendingTags { get; set; } = new();
    public List<string> RecentTags { get; set; } = new();
    public Dictionary<string, DateTime> TagLastUsed { get; set; } = new();
}

#endregion