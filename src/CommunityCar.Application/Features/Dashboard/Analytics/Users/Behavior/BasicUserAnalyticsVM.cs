using CommunityCar.Domain.Enums.Account;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Dashboard.Analytics.Users.Behavior;

/// <summary>
/// ViewModel for basic user analytics data
/// </summary>
public class BasicUserAnalyticsVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int NewUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int ReturnUsers { get; set; }
    public double RetentionRate { get; set; }
    public List<ChartDataVM> UserGrowthData { get; set; } = new();
    public List<ChartDataVM> ActivityData { get; set; } = new();
    public int QuestionsAsked { get; set; }
    public int AnswersGiven { get; set; }
    public int ReviewsWritten { get; set; }
    public int StoriesShared { get; set; }
    public int TotalSessions { get; set; }
    public int PageViews { get; set; }
    public TimeSpan TotalTimeSpent { get; set; }
    public double AverageSessionDuration { get; set; }
    public double BounceRate { get; set; }
    public int PostsCreated { get; set; }
    public int CommentsCreated { get; set; }
    public int LikesGiven { get; set; }
    public int LikesReceived { get; set; }
    public int SharesGiven { get; set; }
    public int SharesReceived { get; set; }
    public int ProfileViews { get; set; }
    public int FollowersGained { get; set; }
    public int FollowingAdded { get; set; }
    public DateTime LastActivity { get; set; }
    public string? LastActivityType { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    
    // Additional properties used by AnalyticsService
    public int LoginCount { get; set; }
    public int InteractionsCount { get; set; }
    public TimeSpan TimeSpentOnSite { get; set; }
    public string MostVisitedSection { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string BrowserType { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}