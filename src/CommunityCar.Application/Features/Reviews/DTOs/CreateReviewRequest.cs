using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Reviews.DTOs;

public class CreateReviewRequest
{
    [Required]
    public Guid TargetId { get; set; }
    
    [Required]
    public string TargetType { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(2000)]
    public string Comment { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? TitleAr { get; set; }
    
    [StringLength(2000)]
    public string? CommentAr { get; set; }
    
    [Required]
    public Guid ReviewerId { get; set; }
    
    // Enhanced properties
    public bool IsVerifiedPurchase { get; set; }
    public bool IsRecommended { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    
    // Automotive specific
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public int? Mileage { get; set; }
    public string? OwnershipDuration { get; set; }
    
    // Detailed ratings (1-5 each)
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
    
    // Media and content
    public List<string> ImageUrls { get; set; } = new();
    public List<string> Pros { get; set; } = new();
    public List<string> Cons { get; set; } = new();
}