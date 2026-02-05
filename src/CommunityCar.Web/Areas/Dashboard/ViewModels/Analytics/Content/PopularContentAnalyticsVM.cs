namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Analytics.Content;

public class PopularContentAnalyticsVM
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorAvatar { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int ShareCount { get; set; }
    public double PopularityScore { get; set; }
    public string PopularityReason { get; set; } = string.Empty;
    public IEnumerable<string> Tags { get; set; } = new List<string>();
}




