using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Features.Stories.ViewModels;

public class StoryVM
{
    public Guid Id { get; set; }
    public string MediaUrl { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorAvatar { get; set; }
    public DateTime? ExpiresAt { get; set; }
    
    // Enhanced properties
    public string? Title { get; set; }
    public string? Caption { get; set; }
    public string? CaptionAr { get; set; }
    public StoryType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public int Duration { get; set; }
    
    // Engagement metrics
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int ReplyCount { get; set; }
    public int ShareCount { get; set; }
    public int CommentCount { get; set; }
    
    // Story status
    public bool IsActive { get; set; }
    public bool IsArchived { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsHighlighted { get; set; }
    public bool IsExpired { get; set; }
    public bool IsViewed { get; set; }
    public bool IsLikedByUser { get; set; }
    
    // Location and context
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? LocationName { get; set; }
    
    // Automotive context
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public string CarDisplayName { get; set; } = string.Empty;
    public string? EventType { get; set; }
    
    // Privacy and visibility
    public StoryVisibility Visibility { get; set; }
    public string VisibilityName { get; set; } = string.Empty;
    public bool AllowReplies { get; set; }
    public bool AllowSharing { get; set; }
    public bool AllowComments { get; set; }
    public bool AllowLikes { get; set; }
    public bool AllowShares { get; set; }
    
    // Tags and mentions
    public IEnumerable<string> Tags { get; set; } = new List<string>();
    public IEnumerable<Guid> MentionedUsers { get; set; } = new List<Guid>();
    public IEnumerable<string> MentionedUserNames { get; set; } = new List<string>();
    
    // Additional media
    public IEnumerable<string> AdditionalMediaUrls { get; set; } = new List<string>();
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Calculated properties
    public string TimeAgo { get; set; } = string.Empty;
    public string TimeRemaining { get; set; } = string.Empty;
    public bool IsMultiMedia { get; set; }
    public int TotalMediaCount { get; set; }
    public bool HasLocation { get; set; }
}