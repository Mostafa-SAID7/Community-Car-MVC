using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Dashboard.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Settings;

[Route("dashboard/settings")]
public class SettingsController : Controller
{
    private readonly ISettingsService _settingsService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(
        ISettingsService settingsService,
        ICurrentUserService currentUserService,
        ILogger<SettingsController> logger)
    {
        _settingsService = settingsService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? category = null)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var settings = string.IsNullOrEmpty(category)
                ? await _settingsService.GetSettingsAsync(userId)
                : await _settingsService.GetSettingsByCategoryAsync(userId, category);

            ViewBag.Category = category;
            ViewBag.Categories = new[] { "Display", "Notifications", "Performance", "Security" };

            return View("~/Views/Dashboard/Settings/Index.cshtml", settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard settings");
            TempData["ErrorMessage"] = "Failed to load dashboard settings. Please try again.";
            return View("~/Views/Dashboard/Settings/Index.cshtml");
        }
    }

    [HttpGet("category/{category}")]
    public async Task<IActionResult> Category(string category)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var settings = await _settingsService.GetSettingsByCategoryAsync(userId, category);
            ViewBag.Category = category;

            return View("~/Views/Dashboard/Settings/Index.cshtml", settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard settings for category: {Category}", category);
            TempData["ErrorMessage"] = "Failed to load settings for the specified category.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost("update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSetting(SettingsRequest request)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid setting data provided.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var success = await _settingsService.UpdateSettingAsync(userId, request);
            if (success)
            {
                TempData["SuccessMessage"] = "Setting updated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update setting.";
            }

            return RedirectToAction(nameof(Index), new { category = request.Category });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating dashboard setting: {SettingKey}", request.SettingKey);
            TempData["ErrorMessage"] = "An error occurred while updating the setting.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost("reset/{settingKey}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetSetting(string settingKey)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var success = await _settingsService.ResetToDefaultAsync(userId, settingKey);
            if (success)
            {
                TempData["SuccessMessage"] = "Setting reset to default successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to reset setting to default.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting dashboard setting: {SettingKey}", settingKey);
            TempData["ErrorMessage"] = "An error occurred while resetting the setting.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost("reset-all")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetAllSettings()
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var success = await _settingsService.ResetAllToDefaultAsync(userId);
            if (success)
            {
                TempData["SuccessMessage"] = "All settings reset to default successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to reset all settings to default.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting all dashboard settings");
            TempData["ErrorMessage"] = "An error occurred while resetting all settings.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet("api")]
    public async Task<IActionResult> GetSettings(string? category = null)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            var settings = string.IsNullOrEmpty(category)
                ? await _settingsService.GetSettingsAsync(userId)
                : await _settingsService.GetSettingsByCategoryAsync(userId, category);

            return Json(new { success = true, data = settings });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard settings API");
            return Json(new { success = false, message = "Failed to load settings" });
        }
    }

    [HttpGet("api/{settingKey}")]
    public async Task<IActionResult> GetSetting(string settingKey)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            var setting = await _settingsService.GetSettingAsync(userId, settingKey);
            if (setting == null)
            {
                return Json(new { success = false, message = "Setting not found" });
            }

            return Json(new { success = true, data = setting });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard setting API: {SettingKey}", settingKey);
            return Json(new { success = false, message = "Failed to load setting" });
        }
    }

    [HttpPost("api/update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSettingApi([FromBody] SettingsRequest request)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Invalid setting data", errors = ModelState });
        }

        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            var success = await _settingsService.UpdateSettingAsync(userId, request);
            if (success)
            {
                return Json(new { success = true, message = "Setting updated successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to update setting" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating dashboard setting API: {SettingKey}", request.SettingKey);
            return Json(new { success = false, message = "An error occurred while updating the setting" });
        }
    }

    [HttpPost("api/reset/{settingKey}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetSettingApi(string settingKey)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            var success = await _settingsService.ResetToDefaultAsync(userId, settingKey);
            if (success)
            {
                return Json(new { success = true, message = "Setting reset to default successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to reset setting to default" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting dashboard setting API: {SettingKey}", settingKey);
            return Json(new { success = false, message = "An error occurred while resetting the setting" });
        }
    }
}