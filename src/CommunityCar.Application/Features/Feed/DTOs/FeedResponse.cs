using CommunityCar.Application.Features.Feed.ViewModels;
using CommunityCar.Application.Common.Models;

namespace CommunityCar.Application.Features.Feed.DTOs;

public class FeedResponse
{
    public IEnumerable<FeedItemVM> FeedItems { get; set; } = new List<FeedItemVM>();
    public IEnumerable<StoryFeedVM> Stories { get; set; } = new List<StoryFeedVM>();
    public IEnumerable<TrendingTopicVM> TrendingTopics { get; set; } = new List<TrendingTopicVM>();
    public IEnumerable<SuggestedFriendVM> SuggestedFriends { get; set; } = new List<SuggestedFriendVM>();
    public PaginationInfo Pagination { get; set; } = new();
    public FeedStatsVM Stats { get; set; } = new();
    public bool HasMoreContent { get; set; }
    public string? NextPageToken { get; set; }
}