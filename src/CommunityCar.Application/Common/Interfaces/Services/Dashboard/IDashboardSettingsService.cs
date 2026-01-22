using CommunityCar.Application.Features.Dashboard.DTOs;
using CommunityCar.Application.Features.Dashboard.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard;

public interface IDashboardSettingsService
{
    Task<List<DashboardSettingsVM>> GetSettingsAsync();
    Task<List<DashboardSettingsVM>> GetSettingsByCategoryAsync(string category);
    Task<DashboardSettingsVM?> GetSettingAsync(string key);
    Task<bool> UpdateSettingAsync(string key, string value);
    Task<bool> UpdateSettingsAsync(Dictionary<string, string> settings);
    Task<bool> ResetSettingAsync(string key);
    Task<bool> ResetCategoryAsync(string category);
    Task<List<string>> GetCategoriesAsync();

    // Added to match DashboardSettingsController expectations
    Task<List<DashboardSettingsVM>> GetSettingsAsync(Guid userId);
    Task<List<DashboardSettingsVM>> GetSettingsByCategoryAsync(Guid userId, string category);
    Task<DashboardSettingsVM?> GetSettingAsync(Guid userId, string key);
    Task<bool> UpdateSettingAsync(Guid userId, DashboardSettingsRequest request);
    Task<bool> ResetToDefaultAsync(Guid userId, string key);
    Task<bool> ResetAllToDefaultAsync(Guid userId);
}