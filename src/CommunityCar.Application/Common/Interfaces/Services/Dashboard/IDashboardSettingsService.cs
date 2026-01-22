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
}