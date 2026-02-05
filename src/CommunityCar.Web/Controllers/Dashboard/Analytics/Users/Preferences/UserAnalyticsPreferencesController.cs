using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Analytics.Users.Preferences;
using CommunityCar.Application.Features.Dashboard.Analytics.Users.Preferences;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Analytics.Users.Preferences;

[Route("{culture=en-US}/dashboard/analytics/users/preferences")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class UserAnalyticsPreferencesController : Controller
{
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly ILogger<UserAnalyticsPreferencesController> _logger;

    public UserAnalyticsPreferencesController(
        IUserPreferencesService userPreferencesService,
        ILogger<UserAnalyticsPreferencesController> logger)
    {
        _userPreferencesService = userPreferencesService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var preferences = await _userPreferencesService.GetUserPreferencesAsync();
            return View(preferences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user preferences analytics");
            TempData["ErrorMessage"] = "Failed to load user preferences analytics. Please try again.";
            return View(new List<UserPreferencesVM>());
        }
    }

    [HttpGet("analytics")]
    public async Task<IActionResult> GetPreferencesAnalytics()
    {
        try
        {
            var analytics = await _userPreferencesService.GetPreferencesAnalyticsAsync();
            return Json(new { success = true, data = analytics });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading preferences analytics");
            return Json(new { success = false, message = "Failed to load preferences analytics" });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserPreferences(string userId)
    {
        try
        {
            var preferences = await _userPreferencesService.GetUserPreferencesByIdAsync(userId);
            if (preferences == null)
            {
                return NotFound();
            }

            return Json(new { success = true, data = preferences });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user preferences for ID: {UserId}", userId);
            return Json(new { success = false, message = "Failed to load user preferences" });
        }
    }
}