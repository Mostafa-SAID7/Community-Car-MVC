using CommunityCar.Application.Features.Dashboard.Overview.Users.Activity;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Overview.Users.Activity;

public interface IUserOverviewActivityService
{
    Task<List<UserActivityOverviewVM>> GetRecentActivityAsync(int count = 50);
    Task<UserActivitySummaryVM> GetActivitySummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<UserActivityTrendVM>> GetActivityTrendsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<UserEngagementOverviewVM> GetEngagementOverviewAsync();
}