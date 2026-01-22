using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Features.Dashboard.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Management;

[Route("dashboard/management")]
[Authorize(Roles = "Admin")]
public class ManagementController : Controller
{
    private readonly IDashboardManagementService _managementService;
    private readonly ILogger<ManagementController> _logger;

    public ManagementController(
        IDashboardManagementService managementService,
        ILogger<ManagementController> logger)
    {
        _managementService = managementService;
        _logger = logger;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Views/Dashboard/Management/Index.cshtml");
    }

    [HttpGet("system-health")]
    public async Task<IActionResult> GetSystemHealth()
    {
        try
        {
            var health = await _managementService.GetSystemHealthAsync();
            return Json(new { success = true, data = health });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system health");
            return Json(new { success = false, message = "Failed to load system health" });
        }
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] UserSearchRequest request)
    {
        try
        {
            var users = await _managementService.GetUsersAsync(request.Page, request.PageSize, request.Search);
            return Json(new { success = true, data = users });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return Json(new { success = false, message = "Failed to load users" });
        }
    }

    [HttpGet("users/{userId:guid}")]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        try
        {
            var user = await _managementService.GetUserAsync(userId);
            if (user == null)
                return Json(new { success = false, message = "User not found" });

            return Json(new { success = true, data = user });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", userId);
            return Json(new { success = false, message = "Failed to load user" });
        }
    }

    [HttpPost("users/{userId:guid}/block")]
    public async Task<IActionResult> BlockUser(Guid userId, [FromBody] BlockUserRequest request)
    {
        try
        {
            var success = await _managementService.BlockUserAsync(userId, request.Reason);
            if (success)
                return Json(new { success = true, message = "User blocked successfully" });

            return Json(new { success = false, message = "Failed to block user" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error blocking user {UserId}", userId);
            return Json(new { success = false, message = "Failed to block user" });
        }
    }

    [HttpPost("users/{userId:guid}/unblock")]
    public async Task<IActionResult> UnblockUser(Guid userId)
    {
        try
        {
            var success = await _managementService.UnblockUserAsync(userId);
            if (success)
                return Json(new { success = true, message = "User unblocked successfully" });

            return Json(new { success = false, message = "Failed to unblock user" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unblocking user {UserId}", userId);
            return Json(new { success = false, message = "Failed to unblock user" });
        }
    }

    [HttpPost("users/{userId:guid}/role")]
    public async Task<IActionResult> UpdateUserRole(Guid userId, [FromBody] UpdateUserRoleRequest request)
    {
        try
        {
            var success = await _managementService.UpdateUserRoleAsync(userId, request.Role);
            if (success)
                return Json(new { success = true, message = "User role updated successfully" });

            return Json(new { success = false, message = "Failed to update user role" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId} role", userId);
            return Json(new { success = false, message = "Failed to update user role" });
        }
    }

    [HttpGet("content-moderation")]
    public async Task<IActionResult> GetContentModeration(int page = 1, int pageSize = 20)
    {
        try
        {
            var moderation = await _managementService.GetContentModerationAsync(page, pageSize);
            return Json(new { success = true, data = moderation });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting content moderation");
            return Json(new { success = false, message = "Failed to load content moderation" });
        }
    }

    [HttpPost("moderate-content")]
    public async Task<IActionResult> ModerateContent([FromBody] ModerateContentRequest request)
    {
        try
        {
            var success = await _managementService.ModerateContentAsync(request);
            if (success)
                return Json(new { success = true, message = "Content moderated successfully" });

            return Json(new { success = false, message = "Failed to moderate content" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moderating content");
            return Json(new { success = false, message = "Failed to moderate content" });
        }
    }

    [HttpGet("alerts")]
    public async Task<IActionResult> GetSystemAlerts()
    {
        try
        {
            var alerts = await _managementService.GetSystemAlertsAsync();
            return Json(new { success = true, data = alerts });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system alerts");
            return Json(new { success = false, message = "Failed to load system alerts" });
        }
    }
}

public class BlockUserRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class UpdateUserRoleRequest
{
    public string Role { get; set; } = string.Empty;
}