using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Settings;

[Route("dashboard/settings/maintenance")]
public class MaintenanceController : Controller
{
    private readonly IMaintenanceService _maintenanceService;
    private readonly ILogger<MaintenanceController> _logger;

    public MaintenanceController(IMaintenanceService maintenanceService, ILogger<MaintenanceController> logger)
    {
        _maintenanceService = maintenanceService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            ViewBag.IsMaintenanceEnabled = await _maintenanceService.IsMaintenanceModeEnabledAsync();
            ViewBag.MaintenanceMessage = await _maintenanceService.GetMaintenanceMessageAsync();
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading maintenance settings");
            TempData["ErrorMessage"] = "Failed to load maintenance settings.";
            return RedirectToAction("Index", "DashboardSettings");
        }
    }

    [HttpPost("toggle")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(bool isEnabled, string message)
    {
        try
        {
            await _maintenanceService.SetMaintenanceModeAsync(isEnabled);
            await _maintenanceService.SetMaintenanceMessageAsync(message);
            
            TempData["SuccessMessage"] = isEnabled 
                ? "Maintenance mode ENABLED. Only administrators can access the site." 
                : "Maintenance mode DISABLED. The site is now live for all users.";
            
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling maintenance mode");
            TempData["ErrorMessage"] = "Failed to update maintenance settings.";
            return RedirectToAction(nameof(Index));
        }
    }
}



