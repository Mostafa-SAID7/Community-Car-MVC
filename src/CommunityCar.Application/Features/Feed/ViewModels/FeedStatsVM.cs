namespace CommunityCar.Application.Features.Feed.ViewModels;

public class FeedStatsVM
{
    public int TotalItems { get; set; }
    public int UnseenItems { get; set; }
    public int TrendingItems { get; set; }
    public int FriendsItems { get; set; }
    public int StoriesCount { get; set; }
    public int ActiveStoriesCount { get; set; }
    public DateTime LastRefreshAt { get; set; }
    public string LastRefreshAgo { get; set; } = string.Empty;
    
    // Content breakdown
    public int NewsCount { get; set; }
    public int ReviewsCount { get; set; }
    public int QACount { get; set; }
    public int PostsCount { get; set; }
    
    // Engagement stats
    public int TotalLikes { get; set; }
    public int TotalComments { get; set; }
    public int TotalShares { get; set; }
    public int TotalViews { get; set; }
}


