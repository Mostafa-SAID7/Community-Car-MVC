using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Account.Analytics;

public class UserContentAnalytics : BaseEntity
{
    public Guid UserId { get; private set; }
    public DateTime Date { get; private set; }
    public int PostsCreated { get; private set; }
    public int CommentsCreated { get; private set; }
    public int QuestionsCreated { get; private set; }
    public int AnswersCreated { get; private set; }
    public int ReviewsCreated { get; private set; }
    public int StoriesCreated { get; private set; }
    public int NewsCreated { get; private set; }
    public int TotalLikes { get; private set; }
    public int TotalShares { get; private set; }
    public int TotalViews { get; private set; }
    public decimal EngagementRate { get; private set; }

    public UserContentAnalytics(Guid userId, DateTime? date = null)
    {
        UserId = userId;
        Date = date?.Date ?? DateTime.UtcNow.Date;
    }

    private UserContentAnalytics() { }

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

    public int TotalContentCreated => PostsCreated + CommentsCreated + QuestionsCreated + 
                                     AnswersCreated + ReviewsCreated + StoriesCreated + NewsCreated;

    public int TotalEngagements => TotalLikes + TotalShares + TotalViews;
}