using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Overview;
using CommunityCar.Application.Features.Dashboard.Overview.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Areas.Dashboard.Controllers;

[Area("Dashboard")]
[Route("{culture=en-US}/dashboard/overview")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class OverviewController : Controller
{
    private readonly IOverviewService _overviewService;
    private readonly ILogger<OverviewController> _logger;

    public OverviewController(
        IOverviewService overviewService,
        ILogger<OverviewController> logger)
    {
        _overviewService = overviewService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var overview = await _overviewService.GetOverviewAsync();
            return View(overview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading overview dashboard");
            TempData["ErrorMessage"] = "Failed to load overview. Please try again.";
            return View(new OverviewVM());
        }
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var stats = await _overviewService.GetStatsAsync(startDate, endDate);
            return Json(new { success = true, data = stats });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading stats");
            return Json(new { success = false, message = "Failed to load stats" });
        }
    }

    [HttpGet("recent-activity")]
    public async Task<IActionResult> GetRecentActivity(int count = 10)
    {
        try
        {
            var activity = await _overviewService.GetRecentActivityAsync(count);
            return Json(new { success = true, data = activity });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading recent activity");
            return Json(new { success = false, message = "Failed to load recent activity" });
        }
    }

    [HttpGet("user-growth")]
    public async Task<IActionResult> GetUserGrowthChart(DateTime startDate, DateTime endDate)
    {
        try
        {
            var chartData = await _overviewService.GetUserGrowthChartAsync(startDate, endDate);
            return Json(new { success = true, data = chartData });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user growth chart");
            return Json(new { success = false, message = "Failed to load user growth chart" });
        }
    }

    [HttpGet("content-chart")]
    public async Task<IActionResult> GetContentChart(DateTime startDate, DateTime endDate)
    {
        try
        {
            var chartData = await _overviewService.GetContentChartAsync(startDate, endDate);
            return Json(new { success = true, data = chartData });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading content chart");
            return Json(new { success = false, message = "Failed to load content chart" });
        }
    }

    [HttpGet("engagement-chart")]
    public async Task<IActionResult> GetEngagementChart(DateTime startDate, DateTime endDate)
    {
        try
        {
            var chartData = await _overviewService.GetEngagementChartAsync(startDate, endDate);
            return Json(new { success = true, data = chartData });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading engagement chart");
            return Json(new { success = false, message = "Failed to load engagement chart" });
        }
    }

    [HttpPost("refresh")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RefreshMetrics()
    {
        try
        {
            await _overviewService.RefreshMetricsAsync();
            return Json(new { success = true, message = "Metrics refreshed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing metrics");
            return Json(new { success = false, message = "Failed to refresh metrics" });
        }
    }
}
