using CommunityCar.Web.Areas.Dashboard.ViewModels.Management.ViewModels;
using CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.ViewModels;
using CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.Users.Trends;
using CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.Users.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Areas.Dashboard.Controllers;

[Area("Dashboard")]
[Route("{culture=en-US}/dashboard/management")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class ManagementController : Controller
{
    private readonly IManagementService _managementService;
    private readonly ILogger<ManagementController> _logger;

    public ManagementController(
        IManagementService managementService,
        ILogger<ManagementController> logger)
    {
        _managementService = managementService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var overview = await _managementService.GetDashboardOverviewAsync();
            return View(overview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading management dashboard");
            TempData["ErrorMessage"] = "Failed to load management dashboard. Please try again.";
            return View(new DashboardOverviewVM());
        }
    }

    [HttpGet("system-tasks")]
    public async Task<IActionResult> GetSystemTasks()
    {
        try
        {
            var tasks = await _managementService.GetSystemTasksAsync();
            return Json(new { success = true, data = tasks });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading system tasks");
            return Json(new { success = false, message = "Failed to load system tasks" });
        }
    }

    [HttpPost("execute-task")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExecuteSystemTask(string taskType, [FromBody] Dictionary<string, object>? parameters = null)
    {
        try
        {
            var result = await _managementService.ExecuteSystemTaskAsync(taskType, parameters);
            if (result)
            {
                return Json(new { success = true, message = "Task executed successfully" });
            }
            return Json(new { success = false, message = "Failed to execute task" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing system task: {TaskType}", taskType);
            return Json(new { success = false, message = "Failed to execute task" });
        }
    }

    [HttpGet("system-resources")]
    public async Task<IActionResult> GetSystemResources()
    {
        try
        {
            var resources = await _managementService.GetSystemResourcesAsync();
            return Json(new { success = true, data = resources });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading system resources");
            return Json(new { success = false, message = "Failed to load system resources" });
        }
    }

    [HttpGet("system-logs")]
    public async Task<IActionResult> GetSystemLogs(int page = 1, int pageSize = 50, string? level = null)
    {
        try
        {
            var logs = await _managementService.GetSystemLogsAsync(page, pageSize, level);
            return Json(new { success = true, data = logs });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading system logs");
            return Json(new { success = false, message = "Failed to load system logs" });
        }
    }
}




