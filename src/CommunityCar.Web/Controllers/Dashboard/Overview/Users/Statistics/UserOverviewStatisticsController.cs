using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Overview.Users.Statistics;
using CommunityCar.Application.Features.Dashboard.Overview.Users.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Overview.Users.Statistics;

[Route("{culture=en-US}/dashboard/overview/users/statistics")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class UserOverviewStatisticsController : Controller
{
    private readonly IUserOverviewStatisticsService _userStatisticsService;
    private readonly ILogger<UserOverviewStatisticsController> _logger;

    public UserOverviewStatisticsController(
        IUserOverviewStatisticsService userStatisticsService,
        ILogger<UserOverviewStatisticsController> logger)
    {
        _userStatisticsService = userStatisticsService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var stats = await _userStatisticsService.GetUserOverviewStatsAsync();
            return View(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user statistics");
            TempData["ErrorMessage"] = "Failed to load user statistics. Please try again.";
            return View(new UserOverviewStatsVM());
        }
    }

    [HttpGet("growth")]
    public async Task<IActionResult> GetUserGrowth(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var growth = await _userStatisticsService.GetUserGrowthAsync(startDate, endDate);
            return Json(new { success = true, data = growth });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user growth");
            return Json(new { success = false, message = "Failed to load user growth" });
        }
    }

    [HttpGet("engagement")]
    public async Task<IActionResult> GetEngagementStats()
    {
        try
        {
            var engagement = await _userStatisticsService.GetEngagementStatsAsync();
            return Json(new { success = true, data = engagement });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading engagement stats");
            return Json(new { success = false, message = "Failed to load engagement stats" });
        }
    }

    [HttpGet("activity-trends")]
    public async Task<IActionResult> GetActivityTrends(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var trends = await _userStatisticsService.GetActivityTrendsAsync(startDate, endDate);
            return Json(new { success = true, data = trends });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading activity trends");
            return Json(new { success = false, message = "Failed to load activity trends" });
        }
    }

    [HttpGet("demographics")]
    public async Task<IActionResult> GetUserDemographics()
    {
        try
        {
            var demographics = await _userStatisticsService.GetUserDemographicsAsync();
            return Json(new { success = true, data = demographics });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user demographics");
            return Json(new { success = false, message = "Failed to load user demographics" });
        }
    }
}