using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Features.Dashboard.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Settings;

[Route("dashboard/settings")]
[Authorize(Roles = "Admin")]
public class SettingsController : Controller
{
    private readonly IDashboardSettingsService _settingsService;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(
        IDashboardSettingsService settingsService,
        ILogger<SettingsController> logger)
    {
        _settingsService = settingsService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? category = null)
    {
        try
        {
            var settings = string.IsNullOrEmpty(category) 
                ? await _settingsService.GetSettingsAsync()
                : await _settingsService.GetSettingsByCategoryAsync(category);
            
            var categories = await _settingsService.GetCategoriesAsync();

            var model = new
            {
                Settings = settings,
                Categories = categories,
                SelectedCategory = category
            };

            return View("~/Views/Dashboard/Settings/Index.cshtml", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading settings");
            TempData["ErrorMessage"] = "Failed to load settings. Please try again.";
            return View("~/Views/Dashboard/Settings/Index.cshtml");
        }
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            var categories = await _settingsService.GetCategoriesAsync();
            return Json(new { success = true, data = categories });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return Json(new { success = false, message = "Failed to get categories" });
        }
    }

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(string category)
    {
        try
        {
            var settings = await _settingsService.GetSettingsByCategoryAsync(category);
            return Json(new { success = true, data = settings });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting settings for category {Category}", category);
            return Json(new { success = false, message = "Failed to get settings for category" });
        }
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetSetting(string key)
    {
        try
        {
            var setting = await _settingsService.GetSettingAsync(key);
            if (setting == null)
            {
                return Json(new { success = false, message = "Setting not found" });
            }

            return Json(new { success = true, data = setting });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting setting {Key}", key);
            return Json(new { success = false, message = "Failed to get setting" });
        }
    }

    [HttpPost("{key}")]
    public async Task<IActionResult> UpdateSetting(string key, [FromBody] DashboardSettingsRequest request)
    {
        try
        {
            var success = await _settingsService.UpdateSettingAsync(key, request.Value);
            if (success)
            {
                return Json(new { success = true, message = "Setting updated successfully" });
            }

            return Json(new { success = false, message = "Failed to update setting" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating setting {Key}", key);
            return Json(new { success = false, message = "An error occurred while updating setting" });
        }
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> UpdateSettings([FromBody] Dictionary<string, string> settings)
    {
        try
        {
            var success = await _settingsService.UpdateSettingsAsync(settings);
            if (success)
            {
                return Json(new { success = true, message = "Settings updated successfully" });
            }

            return Json(new { success = false, message = "Failed to update settings" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating multiple settings");
            return Json(new { success = false, message = "An error occurred while updating settings" });
        }
    }

    [HttpPost("{key}/reset")]
    public async Task<IActionResult> ResetSetting(string key)
    {
        try
        {
            var success = await _settingsService.ResetSettingAsync(key);
            if (success)
            {
                return Json(new { success = true, message = "Setting reset to default value" });
            }

            return Json(new { success = false, message = "Failed to reset setting" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting setting {Key}", key);
            return Json(new { success = false, message = "An error occurred while resetting setting" });
        }
    }

    [HttpPost("category/{category}/reset")]
    public async Task<IActionResult> ResetCategory(string category)
    {
        try
        {
            var success = await _settingsService.ResetCategoryAsync(category);
            if (success)
            {
                return Json(new { success = true, message = "Category settings reset to default values" });
            }

            return Json(new { success = false, message = "Failed to reset category settings" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting category {Category}", category);
            return Json(new { success = false, message = "An error occurred while resetting category" });
        }
    }
}