using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Features.News.DTOs;

public class NewsSearchRequest
{
    public string? SearchTerm { get; set; }
    public NewsCategory? Category { get; set; }
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public List<string> Tags { get; set; } = new();
    public Guid? AuthorId { get; set; }
    public bool? IsPublished { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsPinned { get; set; }
    public DateTime? PublishedAfter { get; set; }
    public DateTime? PublishedBefore { get; set; }
    public int? MinViews { get; set; }
    public int? MaxViews { get; set; }
    public int? MinLikes { get; set; }
    public int? MaxLikes { get; set; }
    public int? MinComments { get; set; }
    public int? MaxComments { get; set; }
    public NewsSortBy SortBy { get; set; } = NewsSortBy.Default;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public enum NewsSortBy
{
    Default = 0,
    Newest = 1,
    Oldest = 2,
    MostViews = 3,
    LeastViews = 4,
    MostLikes = 5,
    LeastLikes = 6,
    MostComments = 7,
    LeastComments = 8,
    MostShares = 9,
    Relevance = 10
}