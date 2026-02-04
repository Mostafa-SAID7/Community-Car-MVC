using CommunityCar.Application.Features.Dashboard.ViewModels;

using CommunityCar.Application.Features.Dashboard.Analytics.ViewModels;

using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Analytics;

public interface IAnalyticsService
{
    Task<List<UserAnalyticsVM>> GetUserAnalyticsAsync(AnalyticsVM request);
    Task<List<ContentAnalyticsVM>> GetContentAnalyticsAsync(AnalyticsVM request);
    Task<UserAnalyticsVM?> GetUserAnalyticsByIdAsync(Guid userId, DateTime date);
    Task<ContentAnalyticsVM?> GetContentAnalyticsByIdAsync(Guid contentId, string contentType, DateTime date);
    Task<List<ChartDataVM>> GetAnalyticsChartAsync(AnalyticsVM request);
    Task UpdateUserAnalyticsAsync(Guid userId, string action);
    Task UpdateContentAnalyticsAsync(Guid contentId, string contentType, string action);

    // Added to match AnalyticsController expectations
    Task<TrafficAnalyticsVM> GetTrafficAnalyticsAsync(DateTime startDate, DateTime endDate);
    Task<List<ChartDataVM>> GetUserGrowthChartAsync(int days);
    Task<List<ChartDataVM>> GetEngagementChartAsync(int days);
    Task<List<ChartDataVM>> GetContentCreationChartAsync(int days);
    Task<bool> UpdateAnalyticsAsync(AnalyticsVM request);
}