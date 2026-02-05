using CommunityCar.Application.Features.Dashboard.Analytics.Users;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Analytics.Users.Behavior;

public interface IUserBehaviorAnalyticsRepository
{
    Task<List<UserAnalyticsVM>> GetUserAnalyticsAsync(DateTime startDate, DateTime endDate);
    Task<UserAnalyticsVM?> GetUserAnalyticsByIdAsync(string userId, DateTime date);
    Task<List<CommunityCar.Application.Features.Account.ViewModels.Core.UserEngagementStatsVM>> GetUserEngagementAsync(DateTime? startDate = null, DateTime? endDate = null, int count = 100);
    Task<List<CommunityCar.Application.Features.Shared.ViewModels.Users.UserActivityVM>> GetActivityHeatmapAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<CommunityCar.Application.Features.Account.ViewModels.Core.UserEngagementStatsVM> GetUserBehaviorAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task UpdateUserAnalyticsAsync(string userId, string action);
}