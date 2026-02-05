namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.ViewModels;

public class TopContentVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int Views { get; set; }
    public int LikeCount { get; set; }
    public int Likes { get; set; }
    public int CommentCount { get; set; }
    public int Comments { get; set; }
    public int ShareCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Url { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public double EngagementScore { get; set; }
}




