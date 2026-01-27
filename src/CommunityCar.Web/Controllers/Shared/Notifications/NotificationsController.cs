using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Domain.Enums.Users;

namespace CommunityCar.Web.Controllers.Shared.Notifications;

[Route("shared/[controller]")]
[Controller]
[Authorize]
public class NotificationsController : Controller
{
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        ICurrentUserService currentUserService,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Get user notifications
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] bool unreadOnly = false)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            var notifications = await _notificationService.GetUserNotificationsAsync(userId, page, pageSize);
            return Ok(new { success = true, data = notifications });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user notifications");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get notification by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotification(Guid id)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
                return NotFound(new { success = false, message = "Notification not found" });

            return Ok(new { success = true, data = notification });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification {NotificationId}", id);
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            await _notificationService.MarkAsReadAsync(id, userId);
            return Ok(new { success = true, message = "Notification marked as read" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as read {NotificationId}", id);
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(new { success = true, message = "All notifications marked as read" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Delete notification
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotification(Guid id)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            await _notificationService.DeleteNotificationAsync(id, userId);
            return Ok(new { success = true, message = "Notification deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting notification {NotificationId}", id);
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get unread notification count
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(new { success = true, data = count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread notification count");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get notification preferences
    /// </summary>
    [HttpGet("preferences")]
    public async Task<IActionResult> GetPreferences()
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            var preferences = await _notificationService.GetUserPreferencesAsync(userId);
            return Ok(new { success = true, data = preferences });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification preferences");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Update notification preferences
    /// </summary>
    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferences([FromBody] object preferences)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            await _notificationService.UpdateUserPreferencesAsync(userId, preferences);
            return Ok(new { success = true, message = "Preferences updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating notification preferences");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Send test notification
    /// </summary>
    [HttpPost("test")]
    public async Task<IActionResult> SendTestNotification([FromBody] NotificationType type)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            await _notificationService.SendTestNotificationAsync(userId, $"Test notification of type: {type}");
            return Ok(new { success = true, message = "Test notification sent" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending test notification");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}


