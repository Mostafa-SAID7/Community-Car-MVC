using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Features.Dashboard.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Monitoring;

[Route("dashboard/monitoring")]
[Authorize(Roles = "Admin,Moderator")]
public class MonitoringController : Controller
{
    private readonly IDashboardMonitoringService _monitoringService;
    private readonly ILogger<MonitoringController> _logger;

    public MonitoringController(
        IDashboardMonitoringService monitoringService,
        ILogger<MonitoringController> logger)
    {
        _monitoringService = monitoringService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var systemHealth = await _monitoringService.GetSystemHealthAsync();
            var alerts = await _monitoringService.GetSystemAlertsAsync(1, 10);
            var contentModeration = await _monitoringService.GetContentModerationAsync(1, 10);

            var model = new
            {
                SystemHealth = systemHealth,
                RecentAlerts = alerts,
                ContentModeration = contentModeration
            };

            return View("~/Views/Dashboard/Monitoring/Index.cshtml", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading monitoring dashboard");
            TempData["ErrorMessage"] = "Failed to load monitoring data. Please try again.";
            return View("~/Views/Dashboard/Monitoring/Index.cshtml");
        }
    }

    [HttpGet("health")]
    public async Task<IActionResult> GetSystemHealth()
    {
        try
        {
            var health = await _monitoringService.GetSystemHealthAsync();
            return Json(new { success = true, data = health });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system health");
            return Json(new { success = false, message = "Failed to get system health" });
        }
    }

    [HttpGet("alerts")]
    public async Task<IActionResult> GetAlerts(int page = 1, int pageSize = 20)
    {
        try
        {
            var alerts = await _monitoringService.GetSystemAlertsAsync(page, pageSize);
            return Json(new { success = true, data = alerts });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system alerts");
            return Json(new { success = false, message = "Failed to get system alerts" });
        }
    }

    [HttpGet("alerts/unread")]
    public async Task<IActionResult> GetUnreadAlerts()
    {
        try
        {
            var alerts = await _monitoringService.GetUnreadAlertsAsync();
            return Json(new { success = true, data = alerts, count = alerts.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread alerts");
            return Json(new { success = false, message = "Failed to get unread alerts" });
        }
    }

    [HttpPost("alerts/{id:guid}/read")]
    public async Task<IActionResult> MarkAlertAsRead(Guid id)
    {
        try
        {
            var success = await _monitoringService.MarkAlertAsReadAsync(id);
            if (success)
            {
                return Json(new { success = true, message = "Alert marked as read" });
            }

            return Json(new { success = false, message = "Failed to mark alert as read" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking alert as read {AlertId}", id);
            return Json(new { success = false, message = "An error occurred while marking alert as read" });
        }
    }

    [HttpPost("alerts/read-all")]
    public async Task<IActionResult> MarkAllAlertsAsRead()
    {
        try
        {
            var success = await _monitoringService.MarkAllAlertsAsReadAsync();
            if (success)
            {
                return Json(new { success = true, message = "All alerts marked as read" });
            }

            return Json(new { success = false, message = "Failed to mark all alerts as read" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all alerts as read");
            return Json(new { success = false, message = "An error occurred while marking alerts as read" });
        }
    }

    [HttpDelete("alerts/{id:guid}")]
    public async Task<IActionResult> DeleteAlert(Guid id)
    {
        try
        {
            var success = await _monitoringService.DeleteAlertAsync(id);
            if (success)
            {
                return Json(new { success = true, message = "Alert deleted successfully" });
            }

            return Json(new { success = false, message = "Failed to delete alert" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting alert {AlertId}", id);
            return Json(new { success = false, message = "An error occurred while deleting alert" });
        }
    }

    [HttpGet("moderation")]
    public async Task<IActionResult> GetContentModeration(int page = 1, int pageSize = 20)
    {
        try
        {
            var moderation = await _monitoringService.GetContentModerationAsync(page, pageSize);
            return Json(new { success = true, data = moderation });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting content moderation data");
            return Json(new { success = false, message = "Failed to get content moderation data" });
        }
    }

    [HttpPost("moderation/{id:guid}")]
    public async Task<IActionResult> ModerateContent(Guid id, [FromBody] ModerateContentRequest request)
    {
        try
        {
            var success = await _monitoringService.ModeratContentAsync(id, request.Action, request.Reason);
            if (success)
            {
                return Json(new { success = true, message = $"Content {request.Action.ToLower()}d successfully" });
            }

            return Json(new { success = false, message = "Failed to moderate content" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moderating content {ContentId}", id);
            return Json(new { success = false, message = "An error occurred while moderating content" });
        }
    }
}