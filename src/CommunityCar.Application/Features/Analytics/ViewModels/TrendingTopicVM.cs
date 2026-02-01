namespace CommunityCar.Application.Features.Analytics.ViewModels;

public class TrendingTopicVM
{
    public string Topic { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int PostCount { get; set; }
    public int ViewCount { get; set; }
    public int EngagementCount { get; set; }
    public double TrendingScore { get; set; }
    public string TrendingReason { get; set; } = string.Empty;
    public DateTime FirstSeenAt { get; set; }
    public DateTime LastSeenAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public bool IsRising { get; set; }
    public double GrowthRate { get; set; }
}