using CommunityCar.Application.Features.Dashboard.DTOs;
using CommunityCar.Application.Features.Dashboard.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard;

public interface IDashboardOverviewService
{
    Task<DashboardOverviewVM> GetOverviewAsync(DashboardOverviewRequest request);
    Task<List<DashboardStatsVM>> GetQuickStatsAsync();
    Task<List<ChartDataVM>> GetUserGrowthChartAsync(DateTime startDate, DateTime endDate);
    Task<List<ChartDataVM>> GetContentChartAsync(DateTime startDate, DateTime endDate);
    Task<List<ChartDataVM>> GetEngagementChartAsync(DateTime startDate, DateTime endDate);
    Task RefreshOverviewDataAsync();
}