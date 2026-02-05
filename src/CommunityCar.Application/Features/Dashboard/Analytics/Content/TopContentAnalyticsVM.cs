namespace CommunityCar.Application.Features.Dashboard.Analytics.Content;

/// <summary>
/// Top content analytics view model
/// </summary>
public class TopContentAnalyticsVM
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int Views { get; set; }
    public int UniqueViews { get; set; }
    public decimal BounceRate { get; set; }
    public double AverageTimeOnPage { get; set; }
    public double ExitRate { get; set; }
    public double EngagementRate { get; set; }
    public int Rank { get; set; }
    public DateTime CreatedAt { get; set; }
}