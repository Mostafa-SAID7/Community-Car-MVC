namespace CommunityCar.Application.Features.Community.News.ViewModels;

public class NewsListVM
{
    public List<NewsVM> News { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
    public List<string> Categories { get; set; } = new();
    public List<string> PopularTags { get; set; } = new();
}