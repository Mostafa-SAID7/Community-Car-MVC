using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.News.ViewModels;

public class NewsSearchVM
{
    // Search parameters
    public string? SearchTerm { get; set; }
    public string? Query { get; set; }
    public NewsCategory? Category { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool? IsFeatured { get; set; }
    public bool? IsPublished { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public Guid? AuthorId { get; set; }
    public bool? IsPinned { get; set; }
    public DateTime? PublishedAfter { get; set; }
    public DateTime? PublishedBefore { get; set; }
    public int? MinViews { get; set; }
    public int? MaxViews { get; set; }
    public int? MinLikes { get; set; }
    public int? MaxLikes { get; set; }
    public int? MinComments { get; set; }
    public int? MaxComments { get; set; }
    
    // Pagination
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "PublishedAt";
    public string SortOrder { get; set; } = "desc";
    
    // Results
    public List<NewsVM> Results { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}