namespace CommunityCar.Application.Features.Account.ViewModels.Media;

public class UserGalleryVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsPublic { get; set; }
    public int DisplayOrder { get; set; }
    public long FileSize { get; set; }
    public string FileSizeFormatted { get; set; } = string.Empty;
    public string? MimeType { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string UploadedTimeAgo { get; set; } = string.Empty;
    public string PrivacyIcon { get; set; } = string.Empty;
    public string PrivacyText { get; set; } = string.Empty;
}

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

public class GalleryCollectionVM
{
    public Guid UserId { get; set; }
    public List<UserGalleryVM> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasMore { get; set; }
    public string? CurrentTag { get; set; }
    public bool? IsPublicFilter { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}