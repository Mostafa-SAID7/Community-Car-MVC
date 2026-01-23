using CommunityCar.Application.Features.Dashboard.DTOs;
using CommunityCar.Application.Features.Dashboard.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard;

public interface ISettingsService
{
    Task<List<SettingsVM>> GetSettingsAsync();
    Task<List<SettingsVM>> GetSettingsByCategoryAsync(string category);
    Task<SettingsVM?> GetSettingAsync(string key);
    Task<bool> UpdateSettingAsync(string key, string value);
    Task<bool> UpdateSettingsAsync(Dictionary<string, string> settings);
    Task<bool> ResetSettingAsync(string key);
    Task<bool> ResetCategoryAsync(string category);
    Task<List<string>> GetCategoriesAsync();

    // Added to match DashboardSettingsController expectations
    Task<List<SettingsVM>> GetSettingsAsync(Guid userId);
    Task<List<SettingsVM>> GetSettingsByCategoryAsync(Guid userId, string category);
    Task<SettingsVM?> GetSettingAsync(Guid userId, string key);
    Task<bool> UpdateSettingAsync(Guid userId, SettingsRequest request);
    Task<bool> ResetToDefaultAsync(Guid userId, string key);
    Task<bool> ResetAllToDefaultAsync(Guid userId);
}