using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.News.DTOs;

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
    public string SortBy { get; set; } = "newest";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}

public enum NewsSortBy
{
    Newest,
    Oldest,
    MostViews,
    LeastViews,
    MostLikes,
    LeastLikes,
    MostComments,
    LeastComments,
    MostShares,
    Relevance
}


