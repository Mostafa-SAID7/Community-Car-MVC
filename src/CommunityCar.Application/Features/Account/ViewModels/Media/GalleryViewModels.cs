namespace CommunityCar.Application.Features.Account.ViewModels.Media;

using Microsoft.AspNetCore.Http;

public class UserGalleryItemVM
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

    // View Compatibility
    public string MediaUrl => ImageUrl;
    public string Title { get; set; } = string.Empty; // Missing in original VM
    public string Description => Caption ?? string.Empty; 
    public CommunityCar.Domain.Enums.Account.MediaType MediaType { get; set; } // Needs domain enum reference
    public int LikeCount { get; set; }
    public bool IsFeatured { get; set; }
    public string TimeAgo => CreatedAt != default ? GetTimeAgo(CreatedAt) : "Just now";

    private static string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;
        if (timeSpan.TotalMinutes < 1) return "Just now";
        if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes}m ago";
        if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours}h ago";
        if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays}d ago";
        return dateTime.ToString("MMM dd, yyyy");
    }
}

public class UploadImageRequest
{
    public Guid UserId { get; set; }
    public IFormFile? ImageFile { get; set; }
    public string? ImageData { get; set; } // Base64
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public string? Caption { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsPublic { get; set; } = true;
    public string? Category { get; set; }
}

public class AddGalleryItemRequest : UploadImageRequest { }

public class UpdateGalleryItemRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Caption { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsPublic { get; set; }
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
    public List<UserGalleryItemVM> RecentItems { get; set; } = new();
    public List<UserGalleryItemVM> MostViewedItems { get; set; } = new();
    public List<UserGalleryItemVM> FeaturedItems { get; set; } = new();
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

public class GalleryCollectionVM
{
    public Guid UserId { get; set; }
    public List<UserGalleryItemVM> Items { get; set; } = new();
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