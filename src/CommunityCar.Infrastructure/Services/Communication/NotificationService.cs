using Microsoft.AspNetCore.SignalR;
using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Common.Models.Notifications;
using CommunityCar.Infrastructure.Hubs;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Infrastructure.Services.Communication;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _notificationHub;

    public NotificationService(IHubContext<NotificationHub> notificationHub)
    {
        _notificationHub = notificationHub;
    }

    public async Task SendNotificationAsync(NotificationRequest request)
    {
        var notification = new
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Message = request.Message,
            Type = request.Type.ToString().ToLower(),
            ActionUrl = request.ActionUrl,
            IconClass = request.IconClass ?? GetDefaultIcon(request.Type),
            Data = request.Data,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await _notificationHub.Clients.Group($"user_{request.UserId}")
            .SendAsync("ReceiveNotification", notification);
    }

    public async Task SendBulkNotificationAsync(BulkNotificationRequest request)
    {
        var notification = new
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Message = request.Message,
            Type = request.Type.ToString().ToLower(),
            ActionUrl = request.ActionUrl,
            IconClass = request.IconClass ?? GetDefaultIcon(request.Type),
            Data = request.Data,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        var tasks = request.UserIds.Select(userId =>
            _notificationHub.Clients.Group($"user_{userId}")
                .SendAsync("ReceiveNotification", notification));

        await Task.WhenAll(tasks);
    }

    public async Task SendToUserAsync(Guid userId, string title, string message, NotificationType type, string? actionUrl = null)
    {
        await SendNotificationAsync(new NotificationRequest
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            ActionUrl = actionUrl
        });
    }

    public async Task SendToUsersAsync(List<Guid> userIds, string title, string message, NotificationType type, string? actionUrl = null)
    {
        await SendBulkNotificationAsync(new BulkNotificationRequest
        {
            UserIds = userIds,
            Title = title,
            Message = message,
            Type = type,
            ActionUrl = actionUrl
        });
    }

    public async Task NotifyNewMessageAsync(Guid userId, string senderName, string conversationId)
    {
        await SendNotificationAsync(new NotificationRequest
        {
            UserId = userId,
            Title = "New Message",
            Message = $"{senderName} sent you a message",
            Type = NotificationType.NewMessage,
            ActionUrl = $"/chats/{conversationId}",
            IconClass = "message-circle"
        });
    }

    public async Task NotifyNewAnswerAsync(Guid userId, string questionTitle, Guid questionId)
    {
        await SendNotificationAsync(new NotificationRequest
        {
            UserId = userId,
            Title = "New Answer",
            Message = $"Someone answered your question: {questionTitle}",
            Type = NotificationType.NewAnswer,
            ActionUrl = $"/qa/{questionId}",
            IconClass = "message-square"
        });
    }

    public async Task NotifyQuestionSolvedAsync(Guid userId, string questionTitle, Guid questionId)
    {
        await SendNotificationAsync(new NotificationRequest
        {
            UserId = userId,
            Title = "Question Solved",
            Message = $"Your question has been solved: {questionTitle}",
            Type = NotificationType.QuestionSolved,
            ActionUrl = $"/qa/{questionId}",
            IconClass = "check-circle"
        });
    }

    public async Task NotifyVoteReceivedAsync(Guid userId, string itemTitle, bool isUpvote)
    {
        await SendNotificationAsync(new NotificationRequest
        {
            UserId = userId,
            Title = isUpvote ? "Upvote Received" : "Downvote Received",
            Message = $"Your {itemTitle} received a {(isUpvote ? "upvote" : "downvote")}",
            Type = NotificationType.VoteReceived,
            IconClass = isUpvote ? "chevron-up" : "chevron-down"
        });
    }

    public async Task NotifyCommentReceivedAsync(Guid userId, string itemTitle, string commenterName)
    {
        await SendNotificationAsync(new NotificationRequest
        {
            UserId = userId,
            Title = "New Comment",
            Message = $"{commenterName} commented on your {itemTitle}",
            Type = NotificationType.CommentReceived,
            IconClass = "message-circle"
        });
    }

    public async Task NotifyFriendRequestAsync(Guid userId, string requesterName, Guid requesterId)
    {
        await SendNotificationAsync(new NotificationRequest
        {
            UserId = userId,
            Title = "Friend Request",
            Message = $"{requesterName} sent you a friend request",
            Type = NotificationType.FriendRequest,
            ActionUrl = $"/friends/requests",
            IconClass = "user-plus"
        });
    }

    public async Task NotifyFriendRequestAcceptedAsync(Guid userId, string accepterName, Guid accepterId)
    {
        await SendNotificationAsync(new NotificationRequest
        {
            UserId = userId,
            Title = "Friend Request Accepted",
            Message = $"{accepterName} accepted your friend request",
            Type = NotificationType.FriendRequestAccepted,
            ActionUrl = $"/profile/{accepterId}",
            IconClass = "user-check"
        });
    }

    public async Task NotifyFriendRequestDeclinedAsync(Guid userId, string declinerName)
    {
        await SendNotificationAsync(new NotificationRequest
        {
            UserId = userId,
            Title = "Friend Request Declined",
            Message = $"{declinerName} declined your friend request",
            Type = NotificationType.FriendRequestDeclined,
            IconClass = "user-x"
        });
    }

    public async Task NotifyFriendRemovedAsync(Guid userId, string removerName)
    {
        await SendNotificationAsync(new NotificationRequest
        {
            UserId = userId,
            Title = "Friend Removed",
            Message = $"{removerName} removed you from their friends",
            Type = NotificationType.FriendRemoved,
            IconClass = "user-minus"
        });
    }

    // Additional notification management methods
    public async Task<IEnumerable<object>> GetUserNotificationsAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        // TODO: Implement with actual database storage
        await Task.CompletedTask;
        return new List<object>();
    }

    public async Task<object?> GetNotificationByIdAsync(Guid notificationId)
    {
        // TODO: Implement with actual database storage
        await Task.CompletedTask;
        return null;
    }

    public async Task MarkAsReadAsync(Guid notificationId, Guid userId)
    {
        // TODO: Implement with actual database storage
        await Task.CompletedTask;
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        // TODO: Implement with actual database storage
        await Task.CompletedTask;
    }

    public async Task DeleteNotificationAsync(Guid notificationId, Guid userId)
    {
        // TODO: Implement with actual database storage
        await Task.CompletedTask;
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        // TODO: Implement with actual database storage
        await Task.CompletedTask;
        return 0;
    }

    public async Task<object> GetUserPreferencesAsync(Guid userId)
    {
        // TODO: Implement with actual database storage
        await Task.CompletedTask;
        return new { };
    }

    public async Task UpdateUserPreferencesAsync(Guid userId, object preferences)
    {
        // TODO: Implement with actual database storage
        await Task.CompletedTask;
    }

    public async Task SendTestNotificationAsync(Guid userId, string message)
    {
        await SendNotificationAsync(new NotificationRequest
        {
            UserId = userId,
            Title = "Test Notification",
            Message = message,
            Type = NotificationType.Info,
            IconClass = "bell"
        });
    }

    private static string GetDefaultIcon(NotificationType type)
    {
        return type switch
        {
            NotificationType.Info => "info",
            NotificationType.Success => "check-circle",
            NotificationType.Warning => "alert-triangle",
            NotificationType.Error => "alert-circle",
            NotificationType.NewMessage => "message-circle",
            NotificationType.NewAnswer => "message-square",
            NotificationType.QuestionSolved => "check-circle-2",
            NotificationType.VoteReceived => "thumbs-up",
            NotificationType.CommentReceived => "message-circle",
            NotificationType.FriendRequest => "user-plus",
            NotificationType.FriendRequestAccepted => "user-check",
            NotificationType.FriendRequestDeclined => "user-x",
            NotificationType.FriendRemoved => "user-minus",
            NotificationType.UserBlocked => "user-x",
            NotificationType.SystemUpdate => "bell",
            _ => "bell"
        };
    }
}