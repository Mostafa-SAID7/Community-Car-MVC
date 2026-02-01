using CommunityCar.Application.Common.Models;

namespace CommunityCar.Application.Features.Community.News.ViewModels;

public class NewsSearchResponse
{
    public List<NewsItemVM> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
    public PaginationInfo? Pagination { get; set; }
    public NewsStatsVM? Stats { get; set; }
    public IEnumerable<string> AvailableTags { get; set; } = new List<string>();
    public IEnumerable<string> AvailableCarMakes { get; set; } = new List<string>();
}


