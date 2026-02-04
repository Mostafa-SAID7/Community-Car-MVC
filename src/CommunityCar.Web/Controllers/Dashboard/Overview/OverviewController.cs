using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Overview;
using CommunityCar.Application.Common.Interfaces.Services.Account.Core;
using CommunityCar.Application.Features.Dashboard.Overview.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Overview;

[Route("dashboard/overview")]
public class OverviewController : Controller
{
    private readonly IOverviewService _overviewService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<OverviewController> _logger;

    public OverviewController(
        IOverviewService overviewService,
        ICurrentUserService currentUserService,
        ILogger<OverviewController> logger)
    {
        _overviewService = overviewService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? timeRange = "month")
    {
        try
        {
            var request = new OverviewVM
            {
                TimeRange = timeRange
            };

            var overview = await _overviewService.GetOverviewAsync(request);
            var quickStats = await _overviewService.GetQuickStatsAsync();

            ViewBag.QuickStats = quickStats;
            ViewBag.TimeRange = timeRange;

            return View("~/Views/Dashboard/Overview/Index.cshtml", overview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard overview");
            TempData["ErrorMessage"] = "Failed to load dashboard overview. Please try again.";
            return View();
        }
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var stats = await _overviewService.GetQuickStatsAsync();
            return Json(new { success = true, data = stats });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard stats");
            return Json(new { success = false, message = "Failed to load stats" });
        }
    }

    [HttpGet("chart/{type}")]
    public async Task<IActionResult> GetChart(string type, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(-30);
            var end = endDate ?? DateTime.UtcNow;

            var data = type.ToLower() switch
            {
                "users" => await _overviewService.GetUserGrowthChartAsync(start, end),
                "content" => await _overviewService.GetContentChartAsync(start, end),
                "engagement" => await _overviewService.GetEngagementChartAsync(start, end),
                _ => new List<Application.Features.Dashboard.ViewModels.ChartDataVM>()
            };

            return Json(new { success = true, data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading chart data for type: {ChartType}", type);
            return Json(new { success = false, message = "Failed to load chart data" });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshData()
    {
        try
        {
            await _overviewService.RefreshOverviewDataAsync();
            TempData["SuccessMessage"] = "Dashboard data refreshed successfully!";
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing dashboard data");
            return Json(new { success = false, message = "Failed to refresh data" });
        }
    }
}


