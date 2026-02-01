using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.Feed.ViewModels;


public class FeedVM
{
    public Guid? UserId { get; set; }
    public FeedSortBy SortBy { get; set; } = FeedSortBy.Newest;
    public List<FeedItemVM> Items { get; set; } = new();
    public List<FeedItemVM> FeedItems { get; set; } = new();
    public List<StoryItemVM> Stories { get; set; } = new();
    public List<TrendingTopicVM> TrendingTopics { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public List<FriendSuggestionVM> SuggestedFriends { get; set; } = new();
    public CommunityCar.Application.Common.Models.PaginationInfo Pagination { get; set; } = new();
    public bool HasMoreContent { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
    public FeedStatsVM Stats { get; set; } = new();
    public string FeedType { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public List<string> ContentTypes { get; set; } = new();
}