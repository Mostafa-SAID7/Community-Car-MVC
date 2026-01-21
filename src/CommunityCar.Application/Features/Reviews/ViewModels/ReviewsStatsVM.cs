namespace CommunityCar.Application.Features.Reviews.ViewModels;

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
}