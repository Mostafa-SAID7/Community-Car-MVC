using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Settings;
using CommunityCar.Application.Features.Dashboard.Settings.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Settings;

[Route("{culture=en-US}/dashboard/settings")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class SettingsController : Controller
{
    private readonly ISettingsService _settingsService;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(
        ISettingsService settingsService,
        ILogger<SettingsController> logger)
    {
        _settingsService = settingsService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var settings = await _settingsService.GetDashboardSettingsAsync();
            return View(settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard settings");
            TempData["ErrorMessage"] = "Failed to load settings. Please try again.";
            return View(new DashboardSettingsVM());
        }
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSettings(DashboardSettingsVM model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        try
        {
            var result = await _settingsService.UpdateDashboardSettingsAsync(model);
            if (result)
            {
                TempData["SuccessMessage"] = "Settings updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Failed to update settings.");
            return View("Index", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating dashboard settings");
            ModelState.AddModelError("", "Failed to update settings. Please try again.");
            return View("Index", model);
        }
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetSettingsCategories()
    {
        try
        {
            var categories = await _settingsService.GetSettingsCategoriesAsync();
            return Json(new { success = true, data = categories });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading settings categories");
            return Json(new { success = false, message = "Failed to load settings categories" });
        }
    }

    [HttpGet("category/{categoryName}")]
    public async Task<IActionResult> GetCategorySettings(string categoryName)
    {
        try
        {
            var settings = await _settingsService.GetCategorySettingsAsync(categoryName);
            return Json(new { success = true, data = settings });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading category settings: {CategoryName}", categoryName);
            return Json(new { success = false, message = "Failed to load category settings" });
        }
    }

    [HttpPost("category/{categoryName}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCategorySettings(string categoryName, [FromBody] List<SettingItemVM> settings)
    {
        try
        {
            var result = await _settingsService.UpdateCategorySettingsAsync(categoryName, settings);
            if (result)
            {
                return Json(new { success = true, message = "Category settings updated successfully" });
            }
            return Json(new { success = false, message = "Failed to update category settings" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category settings: {CategoryName}", categoryName);
            return Json(new { success = false, message = "Failed to update category settings" });
        }
    }

    [HttpGet("security")]
    public async Task<IActionResult> GetSecuritySettings()
    {
        try
        {
            var settings = await _settingsService.GetSecuritySettingsAsync();
            return Json(new { success = true, data = settings });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading security settings");
            return Json(new { success = false, message = "Failed to load security settings" });
        }
    }

    [HttpPost("security")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSecuritySettings([FromBody] DashboardSecuritySettingsVM settings)
    {
        try
        {
            var result = await _settingsService.UpdateSecuritySettingsAsync(settings);
            if (result)
            {
                return Json(new { success = true, message = "Security settings updated successfully" });
            }
            return Json(new { success = false, message = "Failed to update security settings" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating security settings");
            return Json(new { success = false, message = "Failed to update security settings" });
        }
    }

    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotificationSettings()
    {
        try
        {
            var settings = await _settingsService.GetNotificationSettingsAsync();
            return Json(new { success = true, data = settings });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading notification settings");
            return Json(new { success = false, message = "Failed to load notification settings" });
        }
    }

    [HttpPost("notifications")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateNotificationSettings([FromBody] DashboardNotificationSettingsVM settings)
    {
        try
        {
            var result = await _settingsService.UpdateNotificationSettingsAsync(settings);
            if (result)
            {
                return Json(new { success = true, message = "Notification settings updated successfully" });
            }
            return Json(new { success = false, message = "Failed to update notification settings" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating notification settings");
            return Json(new { success = false, message = "Failed to update notification settings" });
        }
    }

    [HttpPost("reset")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetToDefaults()
    {
        try
        {
            var result = await _settingsService.ResetToDefaultsAsync();
            if (result)
            {
                TempData["SuccessMessage"] = "Settings reset to defaults successfully.";
                return Json(new { success = true, message = "Settings reset to defaults successfully" });
            }
            return Json(new { success = false, message = "Failed to reset settings" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting settings to defaults");
            return Json(new { success = false, message = "Failed to reset settings" });
        }
    }
}