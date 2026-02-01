namespace CommunityCar.Application.Features.Community.Reviews.ViewModels;

/// <summary>
/// Create review request view model
/// </summary>
public class CreateReviewVM
{
    public Guid TargetId { get; set; }
    public string TargetType { get; set; } = string.Empty;
    public Guid ReviewerId { get; set; }
    public int Rating { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? CommentAr { get; set; }
    public bool IsVerifiedPurchase { get; set; }
    public bool IsRecommended { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public int? Mileage { get; set; }
    public int? OwnershipDuration { get; set; }
    public int? QualityRating { get; set; }
    public int? ValueRating { get; set; }
    public int? ReliabilityRating { get; set; }
    public int? PerformanceRating { get; set; }
    public int? ComfortRating { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<string> Pros { get; set; } = new();
    public List<string> Cons { get; set; } = new();
}