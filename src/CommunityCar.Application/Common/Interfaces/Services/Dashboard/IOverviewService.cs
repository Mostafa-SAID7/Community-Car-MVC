using CommunityCar.Application.Features.Dashboard.DTOs;
using CommunityCar.Application.Features.Dashboard.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard;

public interface IOverviewService
{
    Task<OverviewVM> GetOverviewAsync(OverviewRequest? request = null);
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


