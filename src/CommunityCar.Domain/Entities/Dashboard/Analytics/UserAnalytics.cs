using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Dashboard.Analytics;

public class UserAnalytics : BaseEntity
{
    public DateTime Date { get; set; }
    public int NewUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int ReturnUsers { get; set; }
    public decimal RetentionRate { get; set; }
    public decimal ChurnRate { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public int PageViews { get; set; }
    public int UniquePageViews { get; set; }
    public decimal BounceRate { get; set; }

    public UserAnalytics()
    {
        Date = DateTime.UtcNow.Date;
    }

    public void UpdateMetrics(int newUsers, int activeUsers, int returnUsers,
        decimal retentionRate, decimal churnRate, TimeSpan avgSessionDuration,
        int pageViews, int uniquePageViews, decimal bounceRate)
    {
        NewUsers = newUsers;
        ActiveUsers = activeUsers;
        ReturnUsers = returnUsers;
        RetentionRate = retentionRate;
        ChurnRate = churnRate;
        AverageSessionDuration = avgSessionDuration;
        PageViews = pageViews;
        UniquePageViews = uniquePageViews;
        BounceRate = bounceRate;
        Audit(UpdatedBy);
    }
}

public class DailyContentAnalytics : BaseEntity
{
    public DateTime Date { get; set; }
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
    public decimal EngagementRate { get; set; }

    public DailyContentAnalytics()
    {
        Date = DateTime.UtcNow.Date;
    }

    public void UpdateMetrics(int postsCreated, int commentsCreated, int questionsCreated,
        int answersCreated, int reviewsCreated, int storiesCreated, int newsCreated,
        int totalLikes, int totalShares, int totalViews, decimal engagementRate)
    {
        PostsCreated = postsCreated;
        CommentsCreated = commentsCreated;
        QuestionsCreated = questionsCreated;
        AnswersCreated = answersCreated;
        ReviewsCreated = reviewsCreated;
        StoriesCreated = storiesCreated;
        NewsCreated = newsCreated;
        TotalLikes = totalLikes;
        TotalShares = totalShares;
        TotalViews = totalViews;
        EngagementRate = engagementRate;
        Audit(UpdatedBy);
    }
}