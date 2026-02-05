using CommunityCar.Application.Features.Dashboard.Analytics.Content;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Analytics.Content;

public interface IContentAnalyticsRepository
{
    Task<ContentAnalyticsVM> GetContentAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<ContentAnalyticsVM>> GetContentAnalyticsListAsync(DateTime startDate, DateTime endDate);
    Task<ContentAnalyticsVM?> GetContentAnalyticsByIdAsync(string contentId, string contentType, DateTime date);
    Task<List<TopContentAnalyticsVM>> GetTopContentAsync(int count = 10);
    Task<TrafficAnalyticsVM> GetTrafficAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task UpdateContentAnalyticsAsync(string contentId, string contentType, string action);
}