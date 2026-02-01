namespace CommunityCar.Application.Features.Community.Feed.ViewModels;

public class FeedResponseVM
{
    public List<FeedItemVM> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
    public FeedStatsVM Stats { get; set; } = new();
    public List<TrendingTopicVM> TrendingTopics { get; set; } = new();
    public List<FriendSuggestionVM> SuggestedFriends { get; set; } = new();
}