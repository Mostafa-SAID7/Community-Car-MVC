using CommunityCar.Application.Features.Account.ViewModels.Core;
using CommunityCar.Application.Features.Shared.ViewModels.Users;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Analytics.Users.Behavior;

public interface IUserBehaviorAnalyticsService
{
    Task<UserEngagementStatsVM> GetUserBehaviorAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<UserEngagementStatsVM>> GetUserEngagementAsync(int count = 100);
    Task<List<CommunityCar.Application.Features.Shared.ViewModels.Users.UserActivityVM>> GetActivityHeatmapAsync(DateTime? startDate = null, DateTime? endDate = null);
}