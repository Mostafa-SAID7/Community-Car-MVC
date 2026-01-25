using CommunityCar.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Stories.DTOs;

public class UpdateStoryRequest
{
    [StringLength(500)]
    public string? Caption { get; set; }

    [StringLength(500)]
    public string? CaptionAr { get; set; }

    public string? ThumbnailUrl { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? LocationName { get; set; }

    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public string? EventType { get; set; }

    public StoryVisibility Visibility { get; set; } = StoryVisibility.Public;
    public bool AllowReplies { get; set; } = true;
    public bool AllowSharing { get; set; } = true;

    public List<string> Tags { get; set; } = new();
    public List<Guid> MentionedUsers { get; set; } = new();
    public List<string> AdditionalMediaUrls { get; set; } = new();

    public bool IsFeatured { get; set; } = false;
    public bool IsHighlighted { get; set; } = false;
}