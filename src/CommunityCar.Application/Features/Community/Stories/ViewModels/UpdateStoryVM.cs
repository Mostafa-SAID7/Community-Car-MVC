using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.Stories.ViewModels;

public class UpdateStoryVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public StoryType Type { get; set; }
    public StoryVisibility Visibility { get; set; }
    public string? Location { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
    public DateTime? ExpiresAt { get; set; }
    public bool AllowComments { get; set; }
    
    // Additional properties needed by services
    public string? Caption { get; set; }
    public string? CaptionAr { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? LocationName { get; set; }
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public string? EventType { get; set; }
    public bool AllowReplies { get; set; } = true;
    public bool AllowSharing { get; set; } = true;
    public bool IsFeatured { get; set; }
    public bool IsHighlighted { get; set; }
    public List<string> MentionedUsers { get; set; } = new();
    public List<string> AdditionalMediaUrls { get; set; } = new();
}