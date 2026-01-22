using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Features.Dashboard.DTOs;
using CommunityCar.Application.Features.Dashboard.ViewModels;

namespace CommunityCar.Application.Services.Dashboard;

public class DashboardSettingsService : IDashboardSettingsService
{
    public async Task<List<DashboardSettingsVM>> GetSettingsAsync()
    {
        return new List<DashboardSettingsVM>
        {
            new() { Key = "Theme", Value = "Dark", Category = "Display", Description = "System theme preference" },
            new() { Key = "Language", Value = "English", Category = "General", Description = "Display language" }
        };
    }

    public async Task<List<DashboardSettingsVM>> GetSettingsByCategoryAsync(string category)
    {
        var all = await GetSettingsAsync();
        return all.Where(s => s.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task<DashboardSettingsVM?> GetSettingAsync(string key)
    {
        var all = await GetSettingsAsync();
        return all.FirstOrDefault(s => s.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> UpdateSettingAsync(string key, string value)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> UpdateSettingsAsync(Dictionary<string, string> settings)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> ResetSettingAsync(string key)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> ResetCategoryAsync(string category)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        return await Task.FromResult(new List<string> { "Display", "Notifications", "Security", "Performance", "General" });
    }

    public async Task<List<DashboardSettingsVM>> GetSettingsAsync(Guid userId)
    {
        return await GetSettingsAsync();
    }

    public async Task<List<DashboardSettingsVM>> GetSettingsByCategoryAsync(Guid userId, string category)
    {
        return await GetSettingsByCategoryAsync(category);
    }

    public async Task<DashboardSettingsVM?> GetSettingAsync(Guid userId, string key)
    {
        return await GetSettingAsync(key);
    }

    public async Task<bool> UpdateSettingAsync(Guid userId, DashboardSettingsRequest request)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> ResetToDefaultAsync(Guid userId, string key)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> ResetAllToDefaultAsync(Guid userId)
    {
        await Task.CompletedTask;
        return true;
    }
}