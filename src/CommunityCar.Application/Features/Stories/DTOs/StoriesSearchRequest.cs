using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Features.Stories.DTOs;

public class StoriesSearchRequest
{
    public string? SearchTerm { get; set; }
    public Guid? AuthorId { get; set; }
    public StoryType? Type { get; set; }
    public StoryVisibility? Visibility { get; set; }
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? EventType { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsArchived { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsHighlighted { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public DateTime? ExpiresAfter { get; set; }
    public DateTime? ExpiresBefore { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? RadiusKm { get; set; }
    public int? MinViews { get; set; }
    public int? MaxViews { get; set; }
    public int? MinLikes { get; set; }
    public int? MaxLikes { get; set; }
    public StoriesSortBy SortBy { get; set; } = StoriesSortBy.Default;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public enum StoriesSortBy
{
    Default = 0,
    Newest = 1,
    Oldest = 2,
    MostViews = 3,
    LeastViews = 4,
    MostLikes = 5,
    LeastLikes = 6,
    ExpiringFirst = 7,
    ExpiringLast = 8,
    Relevance = 9
}