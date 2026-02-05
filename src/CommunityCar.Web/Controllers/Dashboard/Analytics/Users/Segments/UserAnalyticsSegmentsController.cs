using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Analytics.Users.Segments;
using CommunityCar.Application.Features.Dashboard.Analytics.Users.Segments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Analytics.Users.Segments;

[Route("{culture=en-US}/dashboard/analytics/users/segments")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class UserAnalyticsSegmentsController : Controller
{
    private readonly IUserSegmentService _userSegmentService;
    private readonly ILogger<UserAnalyticsSegmentsController> _logger;

    public UserAnalyticsSegmentsController(
        IUserSegmentService userSegmentService,
        ILogger<UserAnalyticsSegmentsController> logger)
    {
        _userSegmentService = userSegmentService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var segments = await _userSegmentService.GetUserSegmentsAsync();
            return View(segments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user segments");
            TempData["ErrorMessage"] = "Failed to load user segments. Please try again.";
            return View(new List<UserSegmentVM>());
        }
    }

    [HttpGet("{segmentId:int}")]
    public async Task<IActionResult> Details(int segmentId)
    {
        try
        {
            var segment = await _userSegmentService.GetUserSegmentByIdAsync(segmentId);
            if (segment == null)
            {
                return NotFound();
            }

            var analytics = await _userSegmentService.GetSegmentAnalyticsAsync(segmentId);
            ViewBag.Analytics = analytics;

            return View(segment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading segment details for ID: {SegmentId}", segmentId);
            return NotFound();
        }
    }

    [HttpGet("analytics/{segmentId:int}")]
    public async Task<IActionResult> GetSegmentAnalytics(int segmentId)
    {
        try
        {
            var analytics = await _userSegmentService.GetSegmentAnalyticsAsync(segmentId);
            return Json(new { success = true, data = analytics });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading segment analytics for ID: {SegmentId}", segmentId);
            return Json(new { success = false, message = "Failed to load segment analytics" });
        }
    }

    [HttpGet("trends")]
    public async Task<IActionResult> GetSegmentTrends(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var trends = await _userSegmentService.GetSegmentTrendsAsync(startDate, endDate);
            return Json(new { success = true, data = trends });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading segment trends");
            return Json(new { success = false, message = "Failed to load segment trends" });
        }
    }
}