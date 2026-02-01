using System.ComponentModel.DataAnnotations;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Reviews.ViewModels;

/// <summary>
/// Review view model
/// </summary>
public class ReviewVM
{
    public Guid Id { get; set; }
    public Guid ReviewerId { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public string ReviewerAvatar { get; set; } = string.Empty;
    public string ReviewerSlug { get; set; } = string.Empty;
    public Guid TargetId { get; set; }
    public string TargetType { get; set; } = string.Empty;
    public string TargetName { get; set; } = string.Empty;
    
    [Range(1, 5)]
    public int Rating { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(2000)]
    public string Comment { get; set; } = string.Empty;
    
    public string? TitleAr { get; set; }
    public string? CommentAr { get; set; }
    
    public bool IsVerifiedPurchase { get; set; }
    public bool IsRecommended { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    
    // Car-specific fields
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public int? Mileage { get; set; }
    public int? OwnershipDuration { get; set; } // in months
    
    // Detailed ratings
    [Range(1, 5)]
    public int? QualityRating { get; set; }
    
    [Range(1, 5)]
    public int? ValueRating { get; set; }
    
    [Range(1, 5)]
    public int? ReliabilityRating { get; set; }
    
    [Range(1, 5)]
    public int? PerformanceRating { get; set; }
    
    [Range(1, 5)]
    public int? ComfortRating { get; set; }
    
    public List<string> ImageUrls { get; set; } = new();
    public List<string> Pros { get; set; } = new();
    public List<string> Cons { get; set; } = new();
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public int HelpfulCount { get; set; }
    public int NotHelpfulCount { get; set; }
    public int ViewsCount { get; set; }
    public int ViewCount { get; set; }
    public int FlagsCount { get; set; }
    
    public bool IsApproved { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsHelpful { get; set; } // For current user
    public bool IsFlagged { get; set; } // For current user
    
    public ContentStatus Status { get; set; }
    public string Slug { get; set; } = string.Empty;
    
    // Calculated fields
    public double AverageRating => new[] { QualityRating, ValueRating, ReliabilityRating, PerformanceRating, ComfortRating }
        .Where(r => r.HasValue)
        .Select(r => r!.Value)
        .DefaultIfEmpty(Rating)
        .Average();
    
    public int HelpfulnessScore { get; set; }
    public double HelpfulnessPercentage => HelpfulCount + NotHelpfulCount > 0 
        ? (double)HelpfulCount / (HelpfulCount + NotHelpfulCount) * 100 
        : 0;
    
    // Additional computed properties
    public string TimeAgo { get; set; } = string.Empty;
    public bool HasImages { get; set; }
    public bool HasDetailedRatings { get; set; }
    public double AverageDetailedRating { get; set; }
    public string CarDisplayName { get; set; } = string.Empty;
}