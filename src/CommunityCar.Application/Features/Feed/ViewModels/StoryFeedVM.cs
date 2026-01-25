using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Features.Feed.ViewModels;

public class StoryFeedVM
{
    public Guid Id { get; set; }
    public string MediaUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public StoryType Type { get; set; }
    public string? Caption { get; set; }
    public string? CaptionAr { get; set; }
    
    // Author information
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorAvatar { get; set; }
    
    // Story metadata
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string TimeRemaining { get; set; } = string.Empty;
    public bool IsExpired { get; set; }
    public bool IsViewed { get; set; }
    
    // Engagement
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public bool IsLikedByUser { get; set; }
    
    // Content
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public string CarDisplayName { get; set; } = string.Empty;
    public string? Location { get; set; }
    public List<string> Tags { get; set; } = new();
    
    // Additional media for multi-part stories
    public List<string> AdditionalMediaUrls { get; set; } = new();
    public bool IsMultiMedia { get; set; }
    public int TotalMediaCount { get; set; }
}