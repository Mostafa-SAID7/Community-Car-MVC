using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Community.Reviews.ViewModels;

/// <summary>
/// Create review view model
/// </summary>
public class ReviewCreateVM
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
    public int? OwnershipDuration { get; set; }
    
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
    
    public string? ImageUrls { get; set; } // Comma-separated
    public string? Pros { get; set; } // Comma-separated
    public string? Cons { get; set; } // Comma-separated
    
    // Helper methods
    public List<string> GetImageUrlsList() => 
        string.IsNullOrEmpty(ImageUrls) ? new List<string>() : 
        ImageUrls.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
    
    public List<string> GetProsList() => 
        string.IsNullOrEmpty(Pros) ? new List<string>() : 
        Pros.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
    
    public List<string> GetConsList() => 
        string.IsNullOrEmpty(Cons) ? new List<string>() : 
        Cons.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
}