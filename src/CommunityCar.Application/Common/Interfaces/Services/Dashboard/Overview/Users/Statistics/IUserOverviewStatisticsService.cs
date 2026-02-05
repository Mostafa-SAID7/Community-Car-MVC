using CommunityCar.Application.Features.Dashboard.Overview.Users.Statistics;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Overview.Users.Statistics;

public interface IUserOverviewStatisticsService
{
    Task<UserOverviewStatsVM> GetUserOverviewStatsAsync();
    Task<List<TrendingItemVM>> GetRegistrationTrendsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<UserOverviewStatsVM> GetActivityStatsAsync();
    Task<StatsBoxVM> GetUserDemographicsAsync();
}