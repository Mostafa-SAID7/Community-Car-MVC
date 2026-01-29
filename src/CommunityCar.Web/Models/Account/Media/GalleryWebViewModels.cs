namespace CommunityCar.Web.Models.Account.Media;

public class UserGalleryWebVM
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

public class GalleryDashboardWebVM
{
    public Guid UserId { get; set; }
    public int TotalItems { get; set; }
    public int PublicItems { get; set; }
    public int PrivateItems { get; set; }
    public long TotalStorageSize { get; set; }
    public string TotalStorageSizeFormatted { get; set; } = string.Empty;
    public int TotalViews { get; set; }
    public List<string> PopularTags { get; set; } = new();
    public List<UserGalleryWebVM> RecentItems { get; set; } = new();
    public List<UserGalleryWebVM> MostViewedItems { get; set; } = new();
    public List<UserGalleryWebVM> FeaturedItems { get; set; } = new();
}

public class UploadGalleryItemWebVM
{
    public Guid UserId { get; set; }
    public string? Caption { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsPublic { get; set; } = true;
    public IFormFile? ImageFile { get; set; }
    public string? ImageUrl { get; set; }
    public long MaxFileSize { get; set; } = 5 * 1024 * 1024; // 5MB
    public List<string> AllowedFileTypes { get; set; } = new() { "image/jpeg", "image/png", "image/gif", "image/webp" };
}