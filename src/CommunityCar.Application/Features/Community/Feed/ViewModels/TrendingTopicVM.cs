namespace CommunityCar.Application.Features.Community.Feed.ViewModels;


public class TrendingTopicVM
{
    public string Topic { get; set; } = string.Empty;
    public int PostCount { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public string TrendingReason { get; set; } = string.Empty;
    public string? TopicAr { get; set; }
    public string? Category { get; set; }
    public int EngagementCount { get; set; }
    public double TrendingScore { get; set; }
    public DateTime LastActivityAt { get; set; }
}