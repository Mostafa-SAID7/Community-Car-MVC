using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Analytics.Users.Preferences;

public interface IUserPreferencesAnalyticsService
{
    Task<SearchAnalyticsVM> GetPreferencesAnalyticsAsync();
    Task<List<TrendingItemVM>> GetPreferenceTrendsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<SearchAnalyticsVM> GetPreferenceInsightsAsync();
}