namespace CommunityCar.Application.Features.Reviews.ViewModels;

public class ReviewVM
{
    public Guid Id { get; set; }
    public Guid TargetId { get; set; }
    public string TargetType { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? CommentAr { get; set; }
    public Guid ReviewerId { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    
    // Enhanced properties
    public bool IsVerifiedPurchase { get; set; }
    public bool IsRecommended { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    
    // Engagement metrics
    public int HelpfulCount { get; set; }
    public int NotHelpfulCount { get; set; }
    public int ReplyCount { get; set; }
    public int ViewCount { get; set; }
    
    // Review status
    public bool IsApproved { get; set; }
    public bool IsFlagged { get; set; }
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    
    // Automotive specific
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public int? Mileage { get; set; }
    public string? OwnershipDuration { get; set; }
    public string CarDisplayName { get; set; } = string.Empty;
    
    // Detailed ratings
    public int? QualityRating { get; set; }
    public int? ValueRating { get; set; }
    public int? ReliabilityRating { get; set; }
    public int? PerformanceRating { get; set; }
    public int? ComfortRating { get; set; }
    public double AverageDetailedRating { get; set; }
    
    // Media and content
    public IEnumerable<string> ImageUrls { get; set; } = new List<string>();
    public IEnumerable<string> Pros { get; set; } = new List<string>();
    public IEnumerable<string> Cons { get; set; } = new List<string>();
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Calculated properties
    public string TimeAgo { get; set; } = string.Empty;
    public int HelpfulnessScore { get; set; }
    public bool HasImages { get; set; }
    public bool HasDetailedRatings { get; set; }
}