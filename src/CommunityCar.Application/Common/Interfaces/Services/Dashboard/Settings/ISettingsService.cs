using CommunityCar.Application.Features.Dashboard.Settings.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Settings;

public interface ISettingsService
{
    Task<DashboardSettingsVM> GetDashboardSettingsAsync();
    Task<bool> UpdateDashboardSettingsAsync(DashboardSettingsVM settings);
    Task<bool> ResetSettingsToDefaultAsync();
    Task<Dictionary<string, object>> GetSettingsByCategoryAsync(string category);
    Task<bool> UpdateSettingAsync(string key, object value);
    Task<object?> GetSettingAsync(string key);
    Task<bool> ValidateSettingsAsync(DashboardSettingsVM settings);
    Task<List<SettingsCategoryVM>> GetSettingsCategoriesAsync();
    Task<bool> ExportSettingsAsync(string format = "json");
    Task<bool> ImportSettingsAsync(string filePath);
}