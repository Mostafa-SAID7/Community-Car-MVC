using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.Stories.ViewModels;

public class StoryVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? ContentAr { get; set; }
    public string? Caption { get; set; }
    public string? CaptionAr { get; set; }
    public StoryType Type { get; set; }
    public StoryVisibility Visibility { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorAvatar { get; set; }
    public string? MediaUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Location { get; set; }
    public string? LocationName { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
    public DateTime? ExpiresAt { get; set; }
    public bool AllowComments { get; set; } = true;
    public bool AllowLikes { get; set; } = true;
    public bool AllowShares { get; set; } = true;
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int ShareCount { get; set; }
    public bool IsLikedByUser { get; set; }
    public bool IsHighlighted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Additional properties needed by services
    public string TypeName { get; set; } = string.Empty;
    public string VisibilityName { get; set; } = string.Empty;
    public string? CarDisplayName { get; set; }
    public string? TimeRemaining { get; set; }
    public bool IsMultiMedia { get; set; }
    public int TotalMediaCount { get; set; }
    public bool HasLocation { get; set; }
    public List<string> MentionedUserNames { get; set; } = new();
    
    // Helper properties
    public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;
    public bool IsActive => !IsExpired;
    public string TimeAgo
    {
        get
        {
            var timeAgo = DateTime.UtcNow - CreatedAt;
            return timeAgo.TotalDays > 7 ? CreatedAt.ToString("MMM dd, yyyy") :
                   timeAgo.TotalDays >= 1 ? $"{(int)timeAgo.TotalDays} days ago" :
                   timeAgo.TotalHours >= 1 ? $"{(int)timeAgo.TotalHours} hours ago" :
                   "Just now";
        }
    }
}