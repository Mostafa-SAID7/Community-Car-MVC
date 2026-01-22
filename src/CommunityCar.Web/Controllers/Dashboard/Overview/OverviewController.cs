using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Overview;

[Route("dashboard")]
[Authorize(Roles = "Admin,Moderator")]
public class OverviewController : Controller
{
    private readonly IDashboardOverviewService _dashboardService;
    private readonly ILogger<OverviewController> _logger;

    public OverviewController(
        IDashboardOverviewService dashboardService,
        ILogger<OverviewController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    [HttpGet("")]
    [HttpGet("overview")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var overview = await _dashboardService.GetOverviewAsync();
            return View("~/Views/Dashboard/Overview/Index.cshtml", overview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard overview");
            TempData["ErrorMessage"] = "Failed to load dashboard data. Please try again.";
            return View("~/Views/Dashboard/Overview/Index.cshtml");
        }
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var stats = await _dashboardService.GetStatsAsync(startDate, endDate);
            return Json(new { success = true, data = stats });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard stats");
            return Json(new { success = false, message = "Failed to load stats" });
        }
    }

    [HttpGet("recent-activity")]
    public async Task<IActionResult> GetRecentActivity(int count = 10)
    {
        try
        {
            var activity = await _dashboardService.GetRecentActivityAsync(count);
            return Json(new { success = true, data = activity });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent activity");
            return Json(new { success = false, message = "Failed to load recent activity" });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshMetrics()
    {
        try
        {
            await _dashboardService.RefreshMetricsAsync();
            return Json(new { success = true, message = "Metrics refreshed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing metrics");
            return Json(new { success = false, message = "Failed to refresh metrics" });
        }
    }
}
