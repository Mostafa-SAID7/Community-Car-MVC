using System.ComponentModel.DataAnnotations;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Features.Stories.DTOs;

public class CreateStoryRequest
{
    [Required]
    public string MediaUrl { get; set; } = string.Empty;
    
    [Required]
    public Guid AuthorId { get; set; }
    
    [Required]
    public StoryType Type { get; set; }
    
    public int Duration { get; set; } = 24; // Default 24 hours
    
    [StringLength(500)]
    public string? Caption { get; set; }
    
    public string? Title { get; set; }
    
    public string? ThumbnailUrl { get; set; }
    
    // Location
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? LocationName { get; set; }
    
    // Car Information
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public string? EventType { get; set; }
    
    // Privacy and Interaction
    public StoryVisibility Visibility { get; set; } = StoryVisibility.Public;
    public bool AllowReplies { get; set; } = true;
    public bool AllowSharing { get; set; } = true;
    public bool AllowComments { get; set; } = true;
    public bool AllowLikes { get; set; } = true;
    public bool AllowShares { get; set; } = true;
    
    // Features
    public bool IsFeatured { get; set; } = false;
    public bool IsHighlighted { get; set; } = false;
    
    // Tags and Mentions
    public string Tags { get; set; } = string.Empty; // Comma-separated
    public List<Guid> MentionedUsers { get; set; } = new();
    public List<string> AdditionalMediaUrls { get; set; } = new();
}