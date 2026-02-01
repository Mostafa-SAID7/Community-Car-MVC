namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class ContentAnalyticsVM
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string ContentTitle { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public int Bookmarks { get; set; }
    public decimal EngagementRate { get; set; }
    public TimeSpan AverageViewTime { get; set; }
    public int UniqueViewers { get; set; }
    public string TopReferrer { get; set; } = string.Empty;
    public string TopKeyword { get; set; } = string.Empty;

    public int PostsCreated { get; set; }
    public int CommentsCreated { get; set; }
    public int QuestionsCreated { get; set; }
    public int AnswersCreated { get; set; }
    public int ReviewsCreated { get; set; }
    public int StoriesCreated { get; set; }
    public int NewsCreated { get; set; }
    public int TotalLikes { get; set; }
    public int TotalShares { get; set; }
    public int TotalViews { get; set; }
    public List<ChartDataVM> ContentCreationData { get; set; } = new();
    public List<ChartDataVM> EngagementData { get; set; } = new();
}