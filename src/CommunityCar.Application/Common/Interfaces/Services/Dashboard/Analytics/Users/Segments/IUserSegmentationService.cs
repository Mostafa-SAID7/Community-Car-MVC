using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Analytics.Users.Segments;

public interface IUserSegmentationService
{
    Task<List<TrendingItemVM>> GetUserSegmentsAsync();
    Task<TrendingItemVM> GetSegmentAnalyticsAsync(int segmentId);
    Task<List<TrendingItemVM>> GetSegmentTrendsAsync(DateTime? startDate = null, DateTime? endDate = null);
}