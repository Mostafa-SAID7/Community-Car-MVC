using CommunityCar.Web.Areas.Dashboard.ViewModels.Analytics.Content;

namespace CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Analytics.Content;

public interface IContentAnalyticsRepository
{
    Task<ContentAnalyticsVM> GetContentAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<ContentAnalyticsVM>> GetContentAnalyticsListAsync(DateTime startDate, DateTime endDate);
    Task<ContentAnalyticsVM?> GetContentAnalyticsByIdAsync(string contentId, string contentType, DateTime date);
    Task<List<TopContentAnalyticsVM>> GetTopContentAsync(int count = 10);
    Task<TrafficAnalyticsVM> GetTrafficAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task UpdateContentAnalyticsAsync(string contentId, string contentType, string action);
}




