using CommunityCar.Domain.Enums.Community;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Community.Reviews.ViewModels;

/// <summary>
/// Review search request view model
/// </summary>
public class ReviewsSearchVM
{
    public string? SearchTerm { get; set; }
    public Guid? ReviewerId { get; set; }
    public Guid? TargetId { get; set; }
    public string? TargetType { get; set; }
    public int? Rating { get; set; }
    public int? MinRating { get; set; }
    public int? MaxRating { get; set; }
    public bool? IsVerifiedPurchase { get; set; }
    public bool? IsRecommended { get; set; }
    public bool? IsApproved { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsFlagged { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public int? MinHelpfulCount { get; set; }
    public int? MaxHelpfulCount { get; set; }
    
    // Car-specific filters
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public int? MinCarYear { get; set; }
    public int? MaxCarYear { get; set; }
    
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    
    public ReviewsSortBy SortBy { get; set; } = ReviewsSortBy.Newest;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    
    // Results
    public List<ReviewVM> Reviews { get; set; } = new();
    public PaginationVM Pagination { get; set; } = new();
    public ReviewsStatsVM Stats { get; set; } = new();
    public List<string> AvailableCarMakes { get; set; } = new();
}