using CommunityCar.Application.Features.Dashboard.DTOs;
using CommunityCar.Application.Features.Dashboard.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard;

public interface IDashboardAnalyticsService
{
    Task<List<UserAnalyticsVM>> GetUserAnalyticsAsync(AnalyticsRequest request);
    Task<List<ContentAnalyticsVM>> GetContentAnalyticsAsync(AnalyticsRequest request);
    Task<UserAnalyticsVM?> GetUserAnalyticsByIdAsync(Guid userId, DateTime date);
    Task<ContentAnalyticsVM?> GetContentAnalyticsByIdAsync(Guid contentId, string contentType, DateTime date);
    Task<List<ChartDataVM>> GetAnalyticsChartAsync(AnalyticsRequest request);
    Task UpdateUserAnalyticsAsync(Guid userId, string action);
    Task UpdateContentAnalyticsAsync(Guid contentId, string contentType, string action);
}