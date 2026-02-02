using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Dashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Analytics;

[Route("dashboard/analytics")]
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
            var request = new AnalyticsVM
            {
                StartDate = startDate ?? DateTime.UtcNow.AddDays(-30),
                EndDate = endDate ?? DateTime.UtcNow
            };

            var dailyAnalytics = await _analyticsService.GetUserAnalyticsAsync(request);
            
            // Aggregate daily data into a single UserAnalyticsVM
            var aggregateAnalytics = new UserAnalyticsVM
            {
                NewUsers = dailyAnalytics.Sum(x => x.NewUsers),
                ActiveUsers = dailyAnalytics.Any() ? dailyAnalytics.Max(x => x.ActiveUsers) : 0, // Using Max for ActiveUsers as a proxy
                ReturnUsers = dailyAnalytics.Sum(x => x.ReturnUsers),
                RetentionRate = dailyAnalytics.Any() ? dailyAnalytics.Average(x => x.RetentionRate) : 0,
                AverageSessionDuration = dailyAnalytics.Any() 
                    ? TimeSpan.FromTicks((long)dailyAnalytics.Average(x => x.TimeSpentOnSite.Ticks))
                    : TimeSpan.Zero,
                UserGrowthData = dailyAnalytics.Select(x => new ChartDataVM 
                { 
                    Label = x.Date.ToString("MMM dd"), 
                    Value = x.NewUsers,
                    Date = x.Date
                }).ToList(),
                ActivityData = dailyAnalytics.Select(x => new ChartDataVM 
                { 
                    Label = x.Date.ToString("MMM dd"), 
                    Value = x.ActiveUsers,
                    Date = x.Date
                }).ToList()
            };

            // Set some default/dummy data for properties not currently provided by the service
            aggregateAnalytics.PostsCreated = dailyAnalytics.Sum(x => x.PostsCreated);
            aggregateAnalytics.QuestionsAsked = dailyAnalytics.Sum(x => x.QuestionsAsked);
            aggregateAnalytics.AnswersGiven = dailyAnalytics.Sum(x => x.AnswersGiven);
            aggregateAnalytics.ReviewsWritten = dailyAnalytics.Sum(x => x.ReviewsWritten);
            aggregateAnalytics.StoriesShared = dailyAnalytics.Sum(x => x.StoriesShared);
            aggregateAnalytics.PageViews = dailyAnalytics.Sum(x => x.PageViews);
            aggregateAnalytics.DeviceType = "Desktop/Mobile";
            aggregateAnalytics.BrowserType = "Chrome/Safari";
            aggregateAnalytics.Location = "Global";
            aggregateAnalytics.MostVisitedSection = "Community Feed";

            return View("~/Views/Dashboard/Analytics/Index.cshtml", aggregateAnalytics);
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
            var request = new AnalyticsVM
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
            var request = new AnalyticsVM
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

    [HttpGet("users-data")]
    public async Task<IActionResult> GetUserAnalytics([FromQuery] AnalyticsVM request)
    {
        try
        {
            var analytics = await _analyticsService.GetUserAnalyticsAsync(request);
            return Json(new { success = true, data = analytics });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user analytics");
            return Json(new { success = false, message = "Failed to load user analytics" });
        }
    }

    [HttpGet("content-data")]
    public async Task<IActionResult> GetContentAnalytics([FromQuery] AnalyticsVM request)
    {
        try
        {
            var analytics = await _analyticsService.GetContentAnalyticsAsync(request);
            return Json(new { success = true, data = analytics });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading content analytics ");
            return Json(new { success = false, message = "Failed to load content analytics" });
        }
    }

    [HttpGet("chart-data")]
    public async Task<IActionResult> GetAnalyticsChart([FromQuery] AnalyticsVM request)
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


