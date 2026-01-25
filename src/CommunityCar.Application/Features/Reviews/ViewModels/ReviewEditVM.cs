using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Reviews.ViewModels;

public class ReviewEditVM
{
    public Guid Id { get; set; }
    
    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }
    
    [Required]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Arabic title cannot exceed 200 characters")]
    public string? TitleAr { get; set; }
    
    [Required]
    [StringLength(2000, ErrorMessage = "Comment cannot exceed 2000 characters")]
    public string Comment { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Arabic comment cannot exceed 2000 characters")]
    public string? CommentAr { get; set; }
    
    // Enhanced properties
    public bool IsRecommended { get; set; }
    
    // Automotive specific
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    
    [Range(1900, 2030, ErrorMessage = "Please enter a valid year")]
    public int? CarYear { get; set; }
    
    [Range(0, 999999, ErrorMessage = "Please enter a valid mileage")]
    public int? Mileage { get; set; }
    
    public string? OwnershipDuration { get; set; }
    
    // Detailed ratings (1-5 each)
    [Range(1, 5, ErrorMessage = "Quality rating must be between 1 and 5")]
    public int? QualityRating { get; set; }
    
    [Range(1, 5, ErrorMessage = "Value rating must be between 1 and 5")]
    public int? ValueRating { get; set; }
    
    [Range(1, 5, ErrorMessage = "Reliability rating must be between 1 and 5")]
    public int? ReliabilityRating { get; set; }
    
    [Range(1, 5, ErrorMessage = "Performance rating must be between 1 and 5")]
    public int? PerformanceRating { get; set; }
    
    [Range(1, 5, ErrorMessage = "Comfort rating must be between 1 and 5")]
    public int? ComfortRating { get; set; }
    
    // Media and content
    public string ImageUrls { get; set; } = string.Empty; // Comma-separated URLs
    public string Pros { get; set; } = string.Empty; // Comma-separated pros
    public string Cons { get; set; } = string.Empty; // Comma-separated cons
    
    // Helper methods
    public List<string> GetImageUrlsList() => 
        string.IsNullOrWhiteSpace(ImageUrls) 
            ? new List<string>() 
            : ImageUrls.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(url => url.Trim())
                      .Where(url => !string.IsNullOrWhiteSpace(url))
                      .ToList();
    
    public List<string> GetProsList() => 
        string.IsNullOrWhiteSpace(Pros) 
            ? new List<string>() 
            : Pros.Split(',', StringSplitOptions.RemoveEmptyEntries)
                 .Select(pro => pro.Trim())
                 .Where(pro => !string.IsNullOrWhiteSpace(pro))
                 .ToList();
    
    public List<string> GetConsList() => 
        string.IsNullOrWhiteSpace(Cons) 
            ? new List<string>() 
            : Cons.Split(',', StringSplitOptions.RemoveEmptyEntries)
                 .Select(con => con.Trim())
                 .Where(con => !string.IsNullOrWhiteSpace(con))
                 .ToList();
}