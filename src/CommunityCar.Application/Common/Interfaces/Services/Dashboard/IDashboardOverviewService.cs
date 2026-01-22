using CommunityCar.Application.Features.Dashboard.DTOs;
using CommunityCar.Application.Features.Dashboard.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard;

public interface IDashboardOverviewService
{
    Task<DashboardOverviewVM> GetOverviewAsync(DashboardOverviewRequest? request = null);
    Task<List<DashboardStatsVM>> GetQuickStatsAsync();
    Task<List<ChartDataVM>> GetUserGrowthChartAsync(DateTime startDate, DateTime endDate);
    Task<List<ChartDataVM>> GetContentChartAsync(DateTime startDate, DateTime endDate);
    Task<List<ChartDataVM>> GetEngagementChartAsync(DateTime startDate, DateTime endDate);
    Task RefreshOverviewDataAsync();

    // Added to match OverviewController expectations
    Task<DashboardOverviewVM> GetOverviewAsync();
    Task<DashboardStatsVM> GetStatsAsync(DateTime? startDate, DateTime? endDate);
    Task<List<RecentActivityVM>> GetRecentActivityAsync(int count);
    Task RefreshMetricsAsync();
}