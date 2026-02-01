namespace CommunityCar.Application.Features.Community.Feed.ViewModels;

public class StoryFeedVM
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorAvatar { get; set; } = string.Empty;
    public string MediaUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsViewed { get; set; }
    public int ViewCount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public string? CaptionAr { get; set; }
    public string? TimeRemaining { get; set; }
    public bool IsExpired { get; set; }
    public int LikeCount { get; set; }
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public string? CarDisplayName { get; set; }
    public string? Location { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> AdditionalMediaUrls { get; set; } = new();
    public bool IsMultiMedia { get; set; }
    public int TotalMediaCount { get; set; }
    public bool IsLikedByUser { get; set; }
}