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

public class UploadGalleryItemVM
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

public class EditGalleryItemVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsPublic { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ViewCount { get; set; }
}