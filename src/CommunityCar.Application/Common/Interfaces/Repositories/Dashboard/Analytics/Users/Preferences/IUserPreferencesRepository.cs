using CommunityCar.Application.Features.Dashboard.Analytics.Users.Preferences;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Analytics.Users.Preferences;

public interface IUserPreferencesRepository
{
    Task<UserPreferencesAnalyticsVM> GetPreferencesAnalyticsAsync();
    Task<List<PreferenceTrendVM>> GetPreferenceTrendsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<UserPreferenceInsightsVM> GetPreferenceInsightsAsync();
    Task<Dictionary<string, int>> GetTopCategoriesAsync(int count = 10);
    Task<Dictionary<string, int>> GetTopTagsAsync(int count = 10);
    Task<Dictionary<string, double>> GetPreferenceDistributionAsync();
}