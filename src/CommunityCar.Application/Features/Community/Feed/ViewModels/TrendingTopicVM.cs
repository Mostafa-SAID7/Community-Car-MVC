namespace CommunityCar.Application.Features.Community.Feed.ViewModels;

public class TrendingTopicVM
{
    public string Topic { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? TopicAr { get; set; }
    public string? CategoryAr { get; set; }
    public int PostCount { get; set; }
    public int EngagementCount { get; set; }
    public double TrendingScore { get; set; }
    public string TrendingReason { get; set; } = string.Empty; // "Viral", "Breaking News", "Community Interest"
    public List<string> RelatedTags { get; set; } = new();
    public string? ImageUrl { get; set; }
    public DateTime LastActivityAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
}


