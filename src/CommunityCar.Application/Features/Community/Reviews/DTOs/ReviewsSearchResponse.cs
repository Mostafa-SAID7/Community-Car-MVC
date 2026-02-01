using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Community.Reviews.ViewModels;

namespace CommunityCar.Application.Features.Community.Reviews.DTOs;

public class ReviewsSearchResponse
{
    public IEnumerable<ReviewVM> Reviews { get; set; } = new List<ReviewVM>();
    public PaginationInfo Pagination { get; set; } = new();
    public ReviewsStatsVM Stats { get; set; } = new();
    public IEnumerable<string> AvailableCarMakes { get; set; } = new List<string>();
    
    // Computed properties for UI
    public int TotalCount => Pagination.TotalItems;
    public int CurrentPage => Pagination.CurrentPage;
    public int PageSize => Pagination.PageSize;
    public int TotalPages => Pagination.TotalPages;
    public bool HasPreviousPage => Pagination.HasPreviousPage;
    public bool HasNextPage => Pagination.HasNextPage;
}


