using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Analytics;
using CommunityCar.Application.Features.Dashboard.Analytics.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Areas.Dashboard.Controllers;

[Area("Dashboard")]
[Route("{culture=en-US}/dashboard/analytics")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class AnalyticsController : Controller
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(
        IAnalyticsService analyticsService,
        ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var analytics = await _analyticsService.GetAnalyticsAsync(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);
            return View(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading analytics dashboard");
            TempData["ErrorMessage"] = "Failed to load analytics. Please try again.";
            return View(new AnalyticsVM());
        }
    }

    [HttpGet("data")]
    public async Task<IActionResult> GetAnalyticsData(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var analytics = await _analyticsService.GetAnalyticsAsync(
                startDate ?? DateTime.UtcNow.AddDays(-30), 
                endDate ?? DateTime.UtcNow);
            
            return Json(new { success = true, data = analytics });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading analytics data");
            return Json(new { success = false, message = "Failed to load analytics data" });
        }
    }

    [HttpGet("traffic")]
    public async Task<IActionResult> GetTrafficAnalytics(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var traffic = await _analyticsService.GetTrafficAnalyticsAsync(
                startDate ?? DateTime.UtcNow.AddDays(-7), 
                endDate ?? DateTime.UtcNow);
            
            return Json(new { success = true, data = traffic });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading traffic analytics");
            return Json(new { success = false, message = "Failed to load traffic analytics" });
        }
    }

    [HttpGet("top-pages")]
    public async Task<IActionResult> GetTopPages(DateTime? startDate = null, DateTime? endDate = null, int count = 10)
    {
        try
        {
            var topPages = await _analyticsService.GetTopPagesAsync(
                startDate ?? DateTime.UtcNow.AddDays(-7), 
                endDate ?? DateTime.UtcNow, 
                count);
            
            return Json(new { success = true, data = topPages });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading top pages");
            return Json(new { success = false, message = "Failed to load top pages" });
        }
    }
}
