using System.ComponentModel.DataAnnotations;
using CommunityCar.Domain.Enums.Users;

namespace CommunityCar.Application.Features.Analytics.DTOs;

public class TrackActivityRequest
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public ActivityType ActivityType { get; set; }

    [Required]
    [StringLength(50)]
    public string EntityType { get; set; } = string.Empty; // Post, Story, Guide, Review, etc.

    public Guid? EntityId { get; set; }

    [StringLength(200)]
    public string? EntityTitle { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public string? Metadata { get; set; } // JSON string for additional data

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? Location { get; set; }

    public int Duration { get; set; } = 0; // in seconds

    public DateTime? ActivityDate { get; set; }
}

public class UpdateAnalyticsPrivacySettingsRequest
{
    public bool AllowActivityTracking { get; set; } = true;
    public bool AllowInterestTracking { get; set; } = true;
    public bool AllowLocationTracking { get; set; } = false;
    public bool AllowPersonalizedRecommendations { get; set; } = true;
    public bool AllowDataSharing { get; set; } = false;
    public bool AllowAnalytics { get; set; } = true;
}


