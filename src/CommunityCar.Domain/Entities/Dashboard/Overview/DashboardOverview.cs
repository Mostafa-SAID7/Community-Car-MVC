using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Dashboard.Overview;

public class DashboardOverview : BaseEntity
{
    public Guid UserId { get; set; }
    public int TotalUsers { get; set; }
    public int TotalPosts { get; set; }
    public int TotalComments { get; set; }
    public int TotalQuestions { get; set; }
    public int TotalAnswers { get; set; }
    public int TotalReviews { get; set; }
    public int TotalStories { get; set; }
    public int TotalNews { get; set; }
    public int ActiveUsersToday { get; set; }
    public int ActiveUsersThisWeek { get; set; }
    public int ActiveUsersThisMonth { get; set; }
    public decimal GrowthRate { get; set; }
    public decimal EngagementRate { get; set; }
    public DateTime LastUpdated { get; set; }

    public DashboardOverview()
    {
        LastUpdated = DateTime.UtcNow;
    }

    public void UpdateMetrics(
        int totalUsers, int totalPosts, int totalComments, int totalQuestions,
        int totalAnswers, int totalReviews, int totalStories, int totalNews,
        int activeUsersToday, int activeUsersThisWeek, int activeUsersThisMonth,
        decimal growthRate, decimal engagementRate)
    {
        TotalUsers = totalUsers;
        TotalPosts = totalPosts;
        TotalComments = totalComments;
        TotalQuestions = totalQuestions;
        TotalAnswers = totalAnswers;
        TotalReviews = totalReviews;
        TotalStories = totalStories;
        TotalNews = totalNews;
        ActiveUsersToday = activeUsersToday;
        ActiveUsersThisWeek = activeUsersThisWeek;
        ActiveUsersThisMonth = activeUsersThisMonth;
        GrowthRate = growthRate;
        EngagementRate = engagementRate;
        LastUpdated = DateTime.UtcNow;
        Audit(UpdatedBy);
    }
}