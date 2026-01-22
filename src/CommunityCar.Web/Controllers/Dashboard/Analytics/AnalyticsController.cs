using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Features.Dashboard.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Analytics;

[Route("dashboard/analytics")]
[Authorize(Roles = "Admin,Moderator")]
public class AnalyticsController : Controller
{
    private readonly IDashboardAnalyticsService _analyticsService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(
        IDashboardAnalyticsService analyticsService,
        ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Views/Dashboard/Analytics/Index.cshtml");
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUserAnalytics([FromQuery] AnalyticsRequest request)
    {
        try
        {
            var analytics = await _analyticsService.GetUserAnalyticsAsync(request.StartDate, request.EndDate);
            return Json(new { success = true, data = analytics });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user analytics");
            return Json(new { success = false, message = "Failed to load user analytics" });
        }
    }

    [HttpGet("content")]
    public async Task<IActionResult> GetContentAnalytics([FromQuery] AnalyticsRequest request)
    {
        try
        {
            var analytics = await _analyticsService.GetContentAnalyticsAsync(request.StartDate, request.EndDate);
            return Json(new { success = true, data = analytics });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting content analytics");
            return Json(new { success = false, message = "Failed to load content analytics" });
        }
    }

    [HttpGet("traffic")]
    public async Task<IActionResult> GetTrafficAnalytics([FromQuery] AnalyticsRequest request)
    {
        try
        {
            var analytics = await _analyticsService.GetTrafficAnalyticsAsync(request.StartDate, request.EndDate);
            return Json(new { success = true, data = analytics });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting traffic analytics");
            return Json(new { success = false, message = "Failed to load traffic analytics" });
        }
    }

    [HttpGet("user-growth")]
    public async Task<IActionResult> GetUserGrowthChart(int days = 30)
    {
        try
        {
            var chartData = await _analyticsService.GetUserGrowthChartAsync(days);
            return Json(new { success = true, data = chartData });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user growth chart");
            return Json(new { success = false, message = "Failed to load chart data" });
        }
    }

    [HttpGet("engagement")]
    public async Task<IActionResult> GetEngagementChart(int days = 30)
    {
        try
        {
            var chartData = await _analyticsService.GetEngagementChartAsync(days);
            return Json(new { success = true, data = chartData });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting engagement chart");
            return Json(new { success = false, message = "Failed to load chart data" });
        }
    }

    [HttpGet("content-creation")]
    public async Task<IActionResult> GetContentCreationChart(int days = 30)
    {
        try
        {
            var chartData = await _analyticsService.GetContentCreationChartAsync(days);
            return Json(new { success = true, data = chartData });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting content creation chart");
            return Json(new { success = false, message = "Failed to load chart data" });
        }
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateAnalytics()
    {
        try
        {
            await _analyticsService.UpdateAnalyticsAsync();
            return Json(new { success = true, message = "Analytics updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating analytics");
            return Json(new { success = false, message = "Failed to update analytics" });
        }
    }
}