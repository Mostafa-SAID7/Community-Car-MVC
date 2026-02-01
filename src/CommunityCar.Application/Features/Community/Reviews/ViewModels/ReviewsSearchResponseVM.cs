namespace CommunityCar.Application.Features.Community.Reviews.ViewModels;

/// <summary>
/// Review search response view model
/// </summary>
public class ReviewsSearchResponseVM
{
    public List<ReviewVM> Reviews { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
    public ReviewsStatsVM Stats { get; set; } = new();
    public ReviewsFiltersVM AvailableFilters { get; set; } = new();
}