using CommunityCar.Domain.Enums.Account;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Dashboard.Analytics.ViewModels;

/// <summary>
/// ViewModel for user analytics data
/// </summary>
public class UserAnalyticsVM
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

/// <summary>
/// User Analytics ViewModels
/// 
/// This file has been reorganized for better maintainability. Each ViewModel class
/// is now in its own separate file within this namespace. This improves code organization,
/// reduces merge conflicts, and makes the codebase easier to navigate.
/// 
/// Individual files:
/// - AnalyticsActivityVM.cs - User activity tracking with duration and metadata
/// - UserActivityStatsVM.cs - Comprehensive activity statistics and breakdowns
/// - AnalyticsInterestVM.cs - User interest tracking with scoring
/// - AnalyticsFollowingVM.cs - Following relationship analytics
/// - AnalyticsSuggestionVM.cs - User suggestion analytics with relevance scoring
/// - ContentRecommendationVM.cs - Content recommendation with relevance metrics
/// - UserEngagementStatsVM.cs - User engagement statistics and trends
/// - TrendingTopicVM.cs - Trending topic analytics with growth metrics
/// - PopularContentVM.cs - Popular content analytics with popularity scoring
/// - UserPrivacySettingsVM.cs - User privacy settings for analytics tracking
/// 
/// All classes maintain their original functionality and public API.
/// </summary>

// The individual classes are now in separate files within this namespace.
// This file is kept for documentation and backward compatibility.