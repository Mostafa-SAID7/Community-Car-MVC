using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Models.Notifications;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUserService;

    public NotificationsController(
        INotificationService notificationService,
        ICurrentUserService currentUserService)
    {
        _notificationService = notificationService;
        _currentUserService = currentUserService;
    }

    [HttpPost("test")]
    public async Task<IActionResult> SendTestNotification()
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        await _notificationService.SendToUserAsync(
            Guid.Parse(userId),
            "Test Notification",
            "This is a test notification to verify SignalR is working correctly!",
            NotificationType.Info
        );

        return Ok(new { message = "Test notification sent successfully" });
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        // Set the user ID from the current user
        request.UserId = Guid.Parse(userId);

        await _notificationService.SendNotificationAsync(request);

        return Ok(new { message = "Notification sent successfully" });
    }

    [HttpPost("send-bulk")]
    public async Task<IActionResult> SendBulkNotification([FromBody] BulkNotificationRequest request)
    {
        await _notificationService.SendBulkNotificationAsync(request);
        return Ok(new { message = "Bulk notification sent successfully" });
    }

    [HttpPost("new-message")]
    public async Task<IActionResult> NotifyNewMessage([FromBody] NewMessageNotificationRequest request)
    {
        await _notificationService.NotifyNewMessageAsync(
            request.UserId,
            request.SenderName,
            request.ConversationId
        );

        return Ok(new { message = "New message notification sent" });
    }

    [HttpPost("new-answer")]
    public async Task<IActionResult> NotifyNewAnswer([FromBody] NewAnswerNotificationRequest request)
    {
        await _notificationService.NotifyNewAnswerAsync(
            request.UserId,
            request.QuestionTitle,
            request.QuestionId
        );

        return Ok(new { message = "New answer notification sent" });
    }

    [HttpPost("question-solved")]
    public async Task<IActionResult> NotifyQuestionSolved([FromBody] QuestionSolvedNotificationRequest request)
    {
        await _notificationService.NotifyQuestionSolvedAsync(
            request.UserId,
            request.QuestionTitle,
            request.QuestionId
        );

        return Ok(new { message = "Question solved notification sent" });
    }

    [HttpPost("vote-received")]
    public async Task<IActionResult> NotifyVoteReceived([FromBody] VoteReceivedNotificationRequest request)
    {
        await _notificationService.NotifyVoteReceivedAsync(
            request.UserId,
            request.ItemTitle,
            request.IsUpvote
        );

        return Ok(new { message = "Vote received notification sent" });
    }

    [HttpPost("friend-request")]
    public async Task<IActionResult> NotifyFriendRequest([FromBody] FriendRequestNotificationRequest request)
    {
        await _notificationService.NotifyFriendRequestAsync(
            request.UserId,
            request.RequesterName,
            request.RequesterId
        );

        return Ok(new { message = "Friend request notification sent" });
    }
}

// Request models for specific notification types
public class NewMessageNotificationRequest
{
    public Guid UserId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string ConversationId { get; set; } = string.Empty;
}

public class NewAnswerNotificationRequest
{
    public Guid UserId { get; set; }
    public string QuestionTitle { get; set; } = string.Empty;
    public Guid QuestionId { get; set; }
}

public class QuestionSolvedNotificationRequest
{
    public Guid UserId { get; set; }
    public string QuestionTitle { get; set; } = string.Empty;
    public Guid QuestionId { get; set; }
}

public class VoteReceivedNotificationRequest
{
    public Guid UserId { get; set; }
    public string ItemTitle { get; set; } = string.Empty;
    public bool IsUpvote { get; set; }
}

public class FriendRequestNotificationRequest
{
    public Guid UserId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public Guid RequesterId { get; set; }
}