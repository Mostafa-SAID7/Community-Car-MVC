using CommunityCar.Application.Features.Dashboard.Analytics.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Analytics;

/// <summary>
/// Interface for analytics services
/// </summary>
public interface IAnalyticsService
{
    Task<AnalyticsVM> GetAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<UserAnalyticsVM?> GetUserAnalyticsByIdAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);
    Task<ContentAnalyticsVM?> GetContentAnalyticsByIdAsync(Guid contentId, string contentType, DateTime date, CancellationToken cancellationToken = default);
    Task<TrafficAnalyticsVM> GetTrafficAnalyticsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<List<TopPageVM>> GetTopPagesAsync(DateTime startDate, DateTime endDate, int count = 10, CancellationToken cancellationToken = default);
    Task<bool> TrackPageViewAsync(string url, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<bool> TrackEventAsync(string eventName, Guid? userId = null, Dictionary<string, object>? properties = null, CancellationToken cancellationToken = default);
    Task<AnalyticsVM> GetUserAnalyticsAsync(AnalyticsVM filter, CancellationToken cancellationToken = default);
    Task<AnalyticsVM> GetContentAnalyticsAsync(AnalyticsVM filter, CancellationToken cancellationToken = default);
    Task<AnalyticsVM> GetAnalyticsChartAsync(AnalyticsVM filter, CancellationToken cancellationToken = default);
    Task<bool> UpdateAnalyticsAsync(AnalyticsVM analytics, CancellationToken cancellationToken = default);
}