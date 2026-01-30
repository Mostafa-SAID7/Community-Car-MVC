using CommunityCar.Application.Features.Account.ViewModels.Activity;
using CommunityCar.Application.Features.Account.ViewModels.Core;
using CommunityCar.Application.Features.Account.ViewModels.Social;

namespace CommunityCar.Application.Common.Interfaces.Services.Account;

public interface IUserAnalyticsService
{
    // Activity Analytics
    Task<ActivityAnalyticsVM> GetUserActivityAnalyticsAsync(Guid userId, DateTime? fromDate = null);
    Task<UserStatisticsVM> GetUserStatisticsAsync(Guid userId);
    
    // Following Analytics
    Task<FollowingAnalyticsVM> GetFollowingAnalyticsAsync(Guid userId);
    
    // Interest Analytics
    Task<InterestAnalyticsVM> GetInterestAnalyticsAsync(Guid userId);
    
    // Profile View Analytics
    Task<ProfileViewStatsVM> GetProfileViewAnalyticsAsync(Guid userId, DateTime? fromDate = null);
    
    // Activity Tracking
    Task<bool> LogUserActivityAsync(Guid userId, string activityType, string description, Dictionary<string, object>? metadata = null);
    
    // Data Management
    Task<bool> CleanupOldDataAsync(DateTime cutoffDate);

    // User Statistics (extracted from IUserRepository)
    Task<int> GetTotalUsersCountAsync();
    Task<int> GetActiveUsersCountAsync();
    Task<int> GetNewUsersCountAsync(DateTime fromDate);
}
