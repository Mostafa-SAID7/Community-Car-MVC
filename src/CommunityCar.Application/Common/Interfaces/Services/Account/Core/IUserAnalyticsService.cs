using CommunityCar.Application.Features.Account.ViewModels.Activity;
using CommunityCar.Application.Features.Account.ViewModels.Core;
using CommunityCar.Application.Features.Account.ViewModels.Social;

namespace CommunityCar.Application.Common.Interfaces.Services.Account.Core;

public interface IUserAnalyticsService
{
    Task<ActivityAnalyticsVM> GetUserActivityAnalyticsAsync(Guid userId, DateTime? fromDate = null);
    Task<UserStatisticsVM> GetUserStatisticsAsync(Guid userId);
    Task<FollowingAnalyticsVM> GetFollowingAnalyticsAsync(Guid userId);
    Task<InterestAnalyticsVM> GetInterestAnalyticsAsync(Guid userId);
    Task<ProfileViewStatsVM> GetProfileViewAnalyticsAsync(Guid userId, DateTime? fromDate = null);
    Task<bool> LogUserActivityAsync(Guid userId, string activityType, string description, Dictionary<string, object>? metadata = null);
    Task<bool> CleanupOldDataAsync(DateTime cutoffDate);
    Task<int> GetTotalUsersCountAsync();
    Task<int> GetActiveUsersCountAsync();
    Task<int> GetNewUsersCountAsync(DateTime fromDate);
}