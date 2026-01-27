using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Dashboard.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard;

[Route("dashboard")]
public class DashboardController : Controller
{
    private readonly IOverviewService _overviewService;
    private readonly IMonitoringService _monitoringService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IOverviewService overviewService,
        IMonitoringService monitoringService,
        ICurrentUserService currentUserService,
        ILogger<DashboardController> logger)
    {
        _overviewService = overviewService;
        _monitoringService = monitoringService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            // Get overview data for the last 30 days
            var overviewRequest = new OverviewRequest
            {
                TimeRange = "month"
            };

            var overview = await _overviewService.GetOverviewAsync(overviewRequest);
            var quickStats = await _overviewService.GetQuickStatsAsync();
            var systemHealth = await _monitoringService.GetSystemHealthAsync();
            var isSystemHealthy = await _monitoringService.IsSystemHealthyAsync();

            ViewBag.QuickStats = quickStats;
            ViewBag.SystemHealth = systemHealth;
            ViewBag.IsSystemHealthy = isSystemHealthy;

            return View(overview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading main dashboard");
            TempData["ErrorMessage"] = "Failed to load dashboard. Please try again.";
            return View();
        }
    }

    [HttpGet("quick-stats")]
    public async Task<IActionResult> GetQuickStats()
    {
        try
        {
            var stats = await _overviewService.GetQuickStatsAsync();
            return Json(new { success = true, data = stats });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading quick stats");
            return Json(new { success = false, message = "Failed to load quick stats" });
        }
    }

    [HttpGet("system-status")]
    public async Task<IActionResult> GetSystemStatus()
    {
        try
        {
            var systemHealth = await _monitoringService.GetSystemHealthAsync();
            var isHealthy = await _monitoringService.IsSystemHealthyAsync();

            return Json(new
            {
                success = true,
                data = systemHealth,
                isHealthy = isHealthy,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading system status");
            return Json(new { success = false, message = "Failed to load system status" });
        }
    }

    [HttpGet("navigation")]
    public IActionResult Navigation()
    {
        var navigationItems = new[]
        {
            new { Title = "Overview", Icon = "fas fa-tachometer-alt", Url = Url.Action("Index", "Overview", new { area = "" }), Description = "Dashboard overview and statistics" },
            new { Title = "Analytics", Icon = "fas fa-chart-bar", Url = Url.Action("Index", "Analytics", new { area = "" }), Description = "User and content analytics" },
            new { Title = "Reports", Icon = "fas fa-file-alt", Url = Url.Action("Index", "Reports", new { area = "" }), Description = "Generate and manage reports" },
            new { Title = "Monitoring", Icon = "fas fa-heartbeat", Url = Url.Action("Index", "Monitoring", new { area = "" }), Description = "System health monitoring" },
            new { Title = "Management", Icon = "fas fa-users-cog", Url = Url.Action("Index", "Management", new { area = "" }), Description = "User management tools" },
            new { Title = "Settings", Icon = "fas fa-cog", Url = Url.Action("Index", "Settings", new { area = "" }), Description = "Dashboard configuration" }
        };

        return Json(new { success = true, data = navigationItems });
    }

    [HttpPost("refresh")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RefreshDashboard()
    {
        try
        {
            await _overviewService.RefreshOverviewDataAsync();
            TempData["SuccessMessage"] = "Dashboard refreshed successfully!";
            return Json(new { success = true, message = "Dashboard refreshed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing dashboard");
            return Json(new { success = false, message = "Failed to refresh dashboard" });
        }
    }
}


