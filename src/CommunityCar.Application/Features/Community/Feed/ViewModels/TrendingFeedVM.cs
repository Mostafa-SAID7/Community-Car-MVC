namespace CommunityCar.Application.Features.Community.Feed.ViewModels;


public class TrendingFeedVM
{
    public List<FeedItemVM> TrendingPosts { get; set; } = new();
    public List<StoryFeedVM> TrendingStories { get; set; } = new();
    public List<string> TrendingTags { get; set; } = new();
    public List<TrendingTopicVM> TrendingTopics { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan ValidFor { get; set; } = TimeSpan.FromHours(1);
}