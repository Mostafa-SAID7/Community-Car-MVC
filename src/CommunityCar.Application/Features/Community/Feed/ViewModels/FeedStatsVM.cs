namespace CommunityCar.Application.Features.Community.Feed.ViewModels;


public class FeedStatsVM
{
    public int TotalItems { get; set; }
    public int TotalLikes { get; set; }
    public int TotalComments { get; set; }
    public int TotalShares { get; set; }
    public int TotalViews { get; set; }
    public int ActiveStoriesCount { get; set; }
    public int TrendingItemsCount { get; set; }
    public DateTime LastRefresh { get; set; }
    public string LastRefreshAgo { get; set; } = string.Empty;
    public double EngagementRate { get; set; }
    public List<CategoryStatsVM> CategoryStats { get; set; } = new();
    public List<TagStatsVM> TagStats { get; set; } = new();
    public int UnseenItems { get; set; }
    public int TrendingItems { get; set; }
    public int FriendsItems { get; set; }
    public int StoriesCount { get; set; }
    public DateTime LastRefreshAt { get; set; }
    public int NewsCount { get; set; }
    public int ReviewsCount { get; set; }
    public int QACount { get; set; }
    public int PostsCount { get; set; }
}