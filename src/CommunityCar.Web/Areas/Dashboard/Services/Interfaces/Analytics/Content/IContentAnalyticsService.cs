using CommunityCar.Application.Features.Dashboard.Analytics.Content;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Analytics.Content;

public interface IContentAnalyticsService
{
    Task<ContentAnalyticsVM> GetContentAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<TopContentAnalyticsVM>> GetTopContentAsync(int count = 10);
    Task<TrafficAnalyticsVM> GetTrafficAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
}