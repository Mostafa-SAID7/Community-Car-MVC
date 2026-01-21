using CommunityCar.Application.Features.News.ViewModels;
using CommunityCar.Application.Common.Models;

namespace CommunityCar.Application.Features.News.DTOs;

public class NewsSearchResponse
{
    public IEnumerable<NewsItemVM> NewsItems { get; set; } = new List<NewsItemVM>();
    public PaginationInfo Pagination { get; set; } = new();
    public NewsStatsVM Stats { get; set; } = new();
    public IEnumerable<string> AvailableTags { get; set; } = new List<string>();
    public IEnumerable<string> AvailableCarMakes { get; set; } = new List<string>();
}