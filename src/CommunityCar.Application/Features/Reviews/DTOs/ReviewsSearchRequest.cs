namespace CommunityCar.Application.Features.Reviews.DTOs;

public class ReviewsSearchRequest
{
    public string? SearchTerm { get; set; }
    public Guid? TargetId { get; set; }
    public string? TargetType { get; set; }
    public Guid? ReviewerId { get; set; }
    public int? Rating { get; set; }
    public int? MinRating { get; set; }
    public int? MaxRating { get; set; }
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public bool? IsVerifiedPurchase { get; set; }
    public bool? IsRecommended { get; set; }
    public bool? IsApproved { get; set; }
    public bool? IsFlagged { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public int? MinHelpfulCount { get; set; }
    public int? MaxHelpfulCount { get; set; }
    public ReviewsSortBy SortBy { get; set; } = ReviewsSortBy.Default;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public enum ReviewsSortBy
{
    Default = 0,
    Newest = 1,
    Oldest = 2,
    HighestRating = 3,
    LowestRating = 4,
    MostHelpful = 5,
    LeastHelpful = 6,
    MostViews = 7,
    Relevance = 8
}