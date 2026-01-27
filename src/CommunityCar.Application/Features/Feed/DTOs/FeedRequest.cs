using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Features.Feed.DTOs;

public class FeedRequest
{
    public Guid? UserId { get; set; }
    public FeedType FeedType { get; set; } = FeedType.Personalized;
    public List<string> ContentTypes { get; set; } = new(); // Stories, News, Reviews, QA, Posts
    public List<string> CarMakes { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public DateTime? Since { get; set; }
    public DateTime? Until { get; set; }
    public bool IncludeSeenContent { get; set; } = false;
    public FeedSortBy SortBy { get; set; } = FeedSortBy.Relevance;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public enum FeedType
{
    Personalized = 0,
    Trending = 1,
    Friends = 2,
    Following = 3,
    Discover = 4
}

public enum FeedSortBy
{
    Relevance = 0,
    Newest = 1,
    Popular = 2,
    Trending = 3,
    Engagement = 4
}


