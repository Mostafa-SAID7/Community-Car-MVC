using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Dashboard.DTOs;
using CommunityCar.Application.Features.Dashboard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Analytics;

[Route("dashboard/analytics")]
[Authorize]
public class AnalyticsController : Controller
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(
        IAnalyticsService analyticsService,
        ICurrentUserService currentUserService,
        ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var request = new AnalyticsRequest
            {
                StartDate = startDate ?? DateTime.UtcNow.AddDays(-30),
                EndDate = endDate ?? DateTime.UtcNow
            };

            var analytics = await _analyticsService.GetUserAnalyticsAsync(request);
            return View(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading analytics for index");
            return View(new UserAnalyticsVM());
        }
    }

    [HttpGet("users")]
    public async Task<IActionResult> Users(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var request = new AnalyticsRequest
            {
                StartDate = startDate ?? DateTime.UtcNow.AddDays(-30),
                EndDate = endDate ?? DateTime.UtcNow
            };

            var analytics = await _analyticsService.GetUserAnalyticsAsync(request);
            return View(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user analytics");
            TempData["ErrorMessage"] = "Failed to load user analytics. Please try again.";
            return View();
        }
    }

    [HttpGet("content")]
    public async Task<IActionResult> Content(DateTime? startDate = null, DateTime? endDate = null, string? contentType = null)
    {
        try
        {
            var request = new AnalyticsRequest
            {
                StartDate = startDate ?? DateTime.UtcNow.AddDays(-30),
                EndDate = endDate ?? DateTime.UtcNow,
                ContentType = contentType
            };

            var analytics = await _analyticsService.GetContentAnalyticsAsync(request);
            ViewBag.ContentType = contentType;
            return View(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading content analytics");
            TempData["ErrorMessage"] = "Failed to load content analytics. Please try again.";
            return View();
        }
    }

    [HttpGet("api/users")]
    public async Task<IActionResult> GetUserAnalytics([FromQuery] AnalyticsRequest request)
    {
        try
        {
            var analytics = await _analyticsService.GetUserAnalyticsAsync(request);
            return Json(new { success = true, data = analytics });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user analytics API");
            return Json(new { success = false, message = "Failed to load user analytics" });
        }
    }

    [HttpGet("api/content")]
    public async Task<IActionResult> GetContentAnalytics([FromQuery] AnalyticsRequest request)
    {
        try
        {
            var analytics = await _analyticsService.GetContentAnalyticsAsync(request);
            return Json(new { success = true, data = analytics });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading content analytics API");
            return Json(new { success = false, message = "Failed to load content analytics" });
        }
    }

    [HttpGet("api/chart")]
    public async Task<IActionResult> GetAnalyticsChart([FromQuery] AnalyticsRequest request)
    {
        try
        {
            var chartData = await _analyticsService.GetAnalyticsChartAsync(request);
            return Json(new { success = true, data = chartData });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading analytics chart");
            return Json(new { success = false, message = "Failed to load chart data" });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> UserDetails(Guid userId, DateTime? date = null)
    {
        try
        {
            var analyticsDate = date ?? DateTime.UtcNow.Date;
            var analytics = await _analyticsService.GetUserAnalyticsByIdAsync(userId, analyticsDate);

            if (analytics == null)
            {
                return NotFound();
            }

            return View(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user analytics details for user: {UserId}", userId);
            TempData["ErrorMessage"] = "Failed to load user analytics details.";
            return RedirectToAction(nameof(Users));
        }
    }

    [HttpGet("content/{contentId}")]
    public async Task<IActionResult> ContentDetails(Guid contentId, string contentType, DateTime? date = null)
    {
        try
        {
            var analyticsDate = date ?? DateTime.UtcNow.Date;
            var analytics = await _analyticsService.GetContentAnalyticsByIdAsync(contentId, contentType, analyticsDate);

            if (analytics == null)
            {
                return NotFound();
            }

            return View(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading content analytics details for content: {ContentId}", contentId);
            TempData["ErrorMessage"] = "Failed to load content analytics details.";
            return RedirectToAction(nameof(Content));
        }
    }
}