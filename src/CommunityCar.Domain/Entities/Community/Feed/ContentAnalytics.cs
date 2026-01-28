using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Community.Feed;

public class ContentAnalytics : BaseEntity
{
    public string ContentType { get; set; } = string.Empty; // Post, Question, Answer, Review, Story, News
    public Guid ContentId { get; set; }
    public DateTime Date { get; set; }
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public int Bookmarks { get; set; }
    public decimal EngagementRate { get; set; }
    public TimeSpan AverageViewTime { get; set; }
    public int UniqueViewers { get; set; }
    public string? TopReferrer { get; set; }
    public string? TopKeyword { get; set; }

    public ContentAnalytics()
    {
        Date = DateTime.UtcNow.Date;
    }

    public void UpdateMetrics(int views, int likes, int comments, int shares, int bookmarks, 
        decimal engagementRate, TimeSpan avgViewTime, int uniqueViewers)
    {
        Views = views;
        Likes = likes;
        Comments = comments;
        Shares = shares;
        Bookmarks = bookmarks;
        EngagementRate = engagementRate;
        AverageViewTime = avgViewTime;
        UniqueViewers = uniqueViewers;
        Audit(UpdatedBy);
    }
}