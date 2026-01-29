namespace CommunityCar.Application.Features.Account.ViewModels.Media;

public class GalleryDashboardVM
{
    public Guid UserId { get; set; }
    public int TotalItems { get; set; }
    public int PublicItems { get; set; }
    public int PrivateItems { get; set; }
    public long TotalStorageSize { get; set; }
    public string TotalStorageSizeFormatted { get; set; } = string.Empty;
    public int TotalViews { get; set; }
    public List<string> PopularTags { get; set; } = new();
    public List<UserGalleryVM> RecentItems { get; set; } = new();
    public List<UserGalleryVM> MostViewedItems { get; set; } = new();
    public List<UserGalleryVM> FeaturedItems { get; set; } = new();
}

public class GalleryTagsVM
{
    public Guid UserId { get; set; }
    public List<TagUsageVM> Tags { get; set; } = new();
    public int TotalTags { get; set; }
}

public class TagUsageVM
{
    public string Tag { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public DateTime LastUsed { get; set; }
    public string Color { get; set; } = string.Empty;
}