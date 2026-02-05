using CommunityCar.Web.Areas.Dashboard.ViewModels.Analytics.Content;

namespace CommunityCar.Web.Areas.Dashboard.Interfaces.Services.Analytics.Content;

public interface IContentAnalyticsService
{
    Task<ContentAnalyticsVM> GetContentAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<TopContentAnalyticsVM>> GetTopContentAsync(int count = 10);
    Task<TrafficAnalyticsVM> GetTrafficAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
}




