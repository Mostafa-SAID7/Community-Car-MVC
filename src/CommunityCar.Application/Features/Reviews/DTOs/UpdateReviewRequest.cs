using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Reviews.DTOs;

public class UpdateReviewRequest
{
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Comment { get; set; } = string.Empty;

    public bool IsRecommended { get; set; } = false;
    public DateTime? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }

    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public int? Mileage { get; set; }
    public string? OwnershipDuration { get; set; }

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
}