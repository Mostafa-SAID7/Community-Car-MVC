using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Dashboard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunityCar.Web.Controllers.Dashboard;

[Route("{culture=en-US}/dashboard")]
[Authorize(Roles = "Admin,SuperAdmin")]
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

    [HttpGet("test")]
    public IActionResult Test()
    {
        ViewBag.Message = "Dashboard controller is working!";
        ViewBag.UserRoles = string.Join(", ", User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value));
        ViewBag.IsAuthenticated = User.Identity?.IsAuthenticated ?? false;
        return View("~/Views/Dashboard/Test.cshtml");
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            // Get overview data for the last 30 days
            var overviewRequest = new OverviewVM
            {
                TimeRange = "month"
            };

            var overview = await _overviewService.GetOverviewAsync(overviewRequest);
            var quickStats = await _overviewService.GetQuickStatsAsync();
            var systemHealthList = await _monitoringService.GetSystemHealthAsync();
            var isSystemHealthy = await _monitoringService.IsSystemHealthyAsync();

            // Get the first system health item or create a default one
            var systemHealth = systemHealthList.FirstOrDefault() ?? new SystemHealthVM
            {
                CheckTime = DateTime.UtcNow,
                ServiceName = "System",
                Status = "Unknown",
                IsHealthy = false,
                CpuUsage = 0,
                MemoryUsage = 0,
                DiskUsage = 0,
                LastCheck = DateTime.UtcNow
            };

            // Update the overview with system health data
            overview.SystemHealth = systemHealth;

            ViewBag.QuickStats = quickStats;
            ViewBag.SystemHealth = systemHealthList;
            ViewBag.IsSystemHealthy = isSystemHealthy;

            return View(overview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading main dashboard");
            TempData["ErrorMessage"] = "Failed to load dashboard. Please try again.";
            
            // Return a default overview model to prevent view errors
            var defaultOverview = new OverviewVM
            {
                Stats = new StatsVM(),
                SystemHealth = new SystemHealthVM
                {
                    CheckTime = DateTime.UtcNow,
                    ServiceName = "System",
                    Status = "Error",
                    IsHealthy = false,
                    LastCheck = DateTime.UtcNow
                }
            };
            
            return View(defaultOverview);
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
            new { Title = "Overview", Icon = "layout-dashboard", Url = Url.Action("Index", "Overview", new { area = "" }), Description = "Dashboard overview and statistics" },
            new { Title = "Analytics", Icon = "bar-chart-3", Url = Url.Action("Index", "Analytics", new { area = "" }), Description = "User and content analytics" },
            new { Title = "Reports", Icon = "file-text", Url = Url.Action("Index", "Reports", new { area = "" }), Description = "Generate and manage reports" },
            new { Title = "Monitoring", Icon = "activity", Url = Url.Action("Index", "Monitoring", new { area = "" }), Description = "System health monitoring" },
            new { Title = "Management", Icon = "users", Url = Url.Action("Index", "Management", new { area = "" }), Description = "User management tools" },
            new { Title = "Settings", Icon = "settings", Url = Url.Action("Index", "Settings", new { area = "" }), Description = "Dashboard configuration" }
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


