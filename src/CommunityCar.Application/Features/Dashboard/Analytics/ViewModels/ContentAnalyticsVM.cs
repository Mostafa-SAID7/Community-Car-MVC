using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Dashboard.Analytics.ViewModels;

/// <summary>
/// ViewModel for content analytics data
/// </summary>
public class ContentAnalyticsVM
{
    public Guid ContentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty; // Post, Story, Guide, etc.
    public string AuthorName { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int ShareCount { get; set; }
    public int BookmarkCount { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastViewedDate { get; set; }
    public List<ChartDataVM> ViewsOverTime { get; set; } = new();
    public List<ChartDataVM> EngagementOverTime { get; set; } = new();
    public Dictionary<string, int> AudienceBreakdown { get; set; } = new();
    public double EngagementRate { get; set; }
    public int AverageTimeSpent { get; set; } // in seconds
}