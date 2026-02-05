using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Analytics.Content;
using CommunityCar.Application.Features.Dashboard.Analytics.Content;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Analytics.Content;

[Route("{culture=en-US}/dashboard/analytics/content")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class ContentAnalyticsController : Controller
{
    private readonly IContentAnalyticsService _contentAnalyticsService;
    private readonly ILogger<ContentAnalyticsController> _logger;

    public ContentAnalyticsController(
        IContentAnalyticsService contentAnalyticsService,
        ILogger<ContentAnalyticsController> logger)
    {
        _contentAnalyticsService = contentAnalyticsService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var analytics = await _contentAnalyticsService.GetContentAnalyticsAsync(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);
            return View(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading content analytics");
            TempData["ErrorMessage"] = "Failed to load content analytics. Please try again.";
            return View(new ContentAnalyticsVM());
        }
    }

    [HttpGet("traffic")]
    public async Task<IActionResult> GetTrafficAnalytics(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var traffic = await _contentAnalyticsService.GetTrafficAnalyticsAsync(
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

    [HttpGet("popular")]
    public async Task<IActionResult> GetPopularContent(int count = 10)
    {
        try
        {
            var popular = await _contentAnalyticsService.GetPopularContentAsync(count);
            return Json(new { success = true, data = popular });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading popular content");
            return Json(new { success = false, message = "Failed to load popular content" });
        }
    }

    [HttpGet("trending")]
    public async Task<IActionResult> GetTrendingTopics(int count = 10)
    {
        try
        {
            var trending = await _contentAnalyticsService.GetTrendingTopicsAsync(count);
            return Json(new { success = true, data = trending });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading trending topics");
            return Json(new { success = false, message = "Failed to load trending topics" });
        }
    }
}