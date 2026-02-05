namespace CommunityCar.Application.Features.Dashboard.Analytics.Content;

/// <summary>
/// Content analytics view model
/// </summary>
public class ContentAnalyticsVM
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public double EngagementRate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime AnalyzedAt { get; set; }
    
    // Missing properties that services expect
    public int TotalPosts { get; set; }
    public int TotalComments { get; set; }
    public int TotalViews { get; set; }
    public int TotalContent { get; set; }
    public int TotalEngagements { get; set; }
    public int TotalShares { get; set; }
    public DateTime Date { get; set; }
    
    // Growth metrics
    public double ContentGrowth { get; set; }
    public double ViewsGrowth { get; set; }
    public double EngagementGrowth { get; set; }
    public double SharesGrowth { get; set; }
}