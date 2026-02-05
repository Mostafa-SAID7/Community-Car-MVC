using CommunityCar.Application.Features.Dashboard.Overview.Users.Statistics;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Overview.Users.Statistics;

public interface IUserOverviewStatisticsRepository
{
    Task<UserOverviewStatsVM> GetUserOverviewStatsAsync();
    Task<List<UserRegistrationTrendVM>> GetRegistrationTrendsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<CommunityCar.Application.Features.Dashboard.Overview.Users.Statistics.UserActivityStatsVM> GetActivityStatsAsync();
    Task<List<UserDemographicsVM>> GetUserDemographicsAsync();
}