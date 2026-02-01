using Microsoft.AspNetCore.Http;

namespace CommunityCar.Application.Features.Account.ViewModels.Media;

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