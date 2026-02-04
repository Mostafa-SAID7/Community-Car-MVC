namespace CommunityCar.Application.Features.Dashboard.Analytics.ViewModels;

public class ContentAnalyticsVM
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string ContentTitle { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int PostsCreated { get; set; }
    public int CommentsCreated { get; set; }
    public int TotalViews { get; set; }
    public decimal EngagementRate { get; set; }
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public int Bookmarks { get; set; }
    public TimeSpan AverageViewTime { get; set; }
    public int UniqueViewers { get; set; }
    public string TopReferrer { get; set; } = string.Empty;
    public string TopKeyword { get; set; } = string.Empty;
}