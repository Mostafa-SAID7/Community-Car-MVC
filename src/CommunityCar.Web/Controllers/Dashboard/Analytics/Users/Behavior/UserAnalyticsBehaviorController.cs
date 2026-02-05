using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Analytics.Users.Behavior;
using CommunityCar.Application.Features.Dashboard.Analytics.Users.Behavior;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Analytics.Users.Behavior;

[Route("{culture=en-US}/dashboard/analytics/users/behavior")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class UserAnalyticsBehaviorController : Controller
{
    private readonly IUserBehaviorAnalyticsService _userBehaviorService;
    private readonly ILogger<UserAnalyticsBehaviorController> _logger;

    public UserAnalyticsBehaviorController(
        IUserBehaviorAnalyticsService userBehaviorService,
        ILogger<UserAnalyticsBehaviorController> logger)
    {
        _userBehaviorService = userBehaviorService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var analytics = await _userBehaviorService.GetUserBehaviorAnalyticsAsync(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);
            return View(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user behavior analytics");
            TempData["ErrorMessage"] = "Failed to load user behavior analytics. Please try again.";
            return View(new UserBehaviorAnalyticsVM());
        }
    }

    [HttpGet("engagement")]
    public async Task<IActionResult> GetUserEngagement(int count = 100)
    {
        try
        {
            var engagement = await _userBehaviorService.GetUserEngagementAsync(count);
            return Json(new { success = true, data = engagement });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user engagement");
            return Json(new { success = false, message = "Failed to load user engagement" });
        }
    }

    [HttpGet("heatmap")]
    public async Task<IActionResult> GetActivityHeatmap(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var heatmap = await _userBehaviorService.GetActivityHeatmapAsync(startDate, endDate);
            return Json(new { success = true, data = heatmap });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading activity heatmap");
            return Json(new { success = false, message = "Failed to load activity heatmap" });
        }
    }
}