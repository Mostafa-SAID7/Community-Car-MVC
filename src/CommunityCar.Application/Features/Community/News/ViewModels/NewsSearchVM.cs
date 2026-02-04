namespace CommunityCar.Application.Features.Community.News.ViewModels;

public class NewsSearchVM
{
    public string? Query { get; set; }
    public string? SearchTerm { get; set; }
    public string? Category { get; set; }
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public List<string> Tags { get; set; } = new();
    public Guid? AuthorId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool? IsPublished { get; set; }
    public bool? IsFeatured { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public string SortOrder { get; set; } = "Desc";
    public int Page { get; set; } = 1;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }
    public List<NewsVM> Items { get; set; } = new();
}