namespace CommunityCar.Application.Features.Community.Reviews.ViewModels;

/// <summary>
/// Reviews statistics view model
/// </summary>
public class ReviewsStatsVM
{
    public int TotalReviews { get; set; }
    public int ApprovedReviews { get; set; }
    public int PendingReviews { get; set; }
    public int FlaggedReviews { get; set; }
    public int VerifiedPurchaseReviews { get; set; }
    public int TotalViews { get; set; }
    public int TotalHelpfulVotes { get; set; }
    public int ReviewsThisMonth { get; set; }
    public int ReviewsThisWeek { get; set; }
    public int ReviewsToday { get; set; }
    public double AverageRating { get; set; }
    public double AverageHelpfulnessScore { get; set; }
    public double AverageViewsPerReview { get; set; }
    public int FiveStarCount { get; set; }
    public int FourStarCount { get; set; }
    public int ThreeStarCount { get; set; }
    public int TwoStarCount { get; set; }
    public int OneStarCount { get; set; }
    public int VerifiedPurchaseCount { get; set; }
    public int RecommendedCount { get; set; }
    public double RecommendationPercentage { get; set; }
    public List<CarMakeStatsVM> CarMakeStats { get; set; } = new();
    public List<CarModelStatsVM> CarModelStats { get; set; } = new();
}