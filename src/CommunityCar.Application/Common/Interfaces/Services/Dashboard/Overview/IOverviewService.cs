using CommunityCar.Application.Features.Dashboard.Overview.ViewModels;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Overview;

public interface IOverviewService
{
    Task<OverviewVM> GetOverviewAsync(OverviewVM? request = null);
    Task<List<StatsVM>> GetQuickStatsAsync();
    Task<List<ChartDataVM>> GetUserGrowthChartAsync(DateTime startDate, DateTime endDate);
    Task<List<ChartDataVM>> GetContentChartAsync(DateTime startDate, DateTime endDate);
    Task<List<ChartDataVM>> GetEngagementChartAsync(DateTime startDate, DateTime endDate);
    Task RefreshOverviewDataAsync();

    // Added to match OverviewController expectations
    Task<OverviewVM> GetOverviewAsync();
    Task<StatsVM> GetStatsAsync(DateTime? startDate, DateTime? endDate);
    Task<List<RecentActivityVM>> GetRecentActivityAsync(int count);
    Task RefreshMetricsAsync();
}