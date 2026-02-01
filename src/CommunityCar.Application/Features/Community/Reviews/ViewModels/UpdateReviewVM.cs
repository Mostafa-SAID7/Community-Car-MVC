namespace CommunityCar.Application.Features.Community.Reviews.ViewModels;

/// <summary>
/// Update review request view model
/// </summary>
public class UpdateReviewVM
{
    public int Rating { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? CommentAr { get; set; }
    public bool IsRecommended { get; set; }
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