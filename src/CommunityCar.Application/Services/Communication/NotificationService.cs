using Microsoft.AspNetCore.SignalR;
using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Common.Interfaces.Hubs;
using CommunityCar.Application.Features.Notifications.ViewModels;
using CommunityCar.Domain.Enums.Account;
using System.Collections.Concurrent;

namespace CommunityCar.Application.Services.Communication;

public class NotificationService : INotificationService
{
    private readonly INotificationHubContext _notificationHub;
    private static readonly ConcurrentDictionary<Guid, List<object>> _userNotifications = new();

    public NotificationService(INotificationHubContext notificationHub)
    {
        _notificationHub = notificationHub;
    }

    private void AddToHistory(Guid userId, object notification)
    {
        var notifications = _userNotifications.GetOrAdd(userId, _ => new List<object>());
        lock (notifications)
        {
            notifications.Insert(0, notification);
            if (notifications.Count > 50) notifications.RemoveAt(50);
        }
    }

    public async Task SendNotificationAsync(NotificationVM request)
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

        await _notificationHub.SendNotificationToGroupAsync($"user_{request.UserId.ToString().ToLower()}", "ReceiveNotification", notification);
            
        AddToHistory(request.UserId, notification);
    }

    public async Task SendBulkNotificationAsync(BulkNotificationVM request)
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
            _notificationHub.SendNotificationToGroupAsync($"user_{userId.ToString().ToLower()}", "ReceiveNotification", notification));

        foreach (var userId in request.UserIds)
        {
            AddToHistory(userId, notification);
        }

        await Task.WhenAll(tasks);
    }

    public async Task SendToUserAsync(Guid userId, string title, string message, NotificationType type, string? actionUrl = null)
    {
        await SendNotificationAsync(new NotificationVM
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
        await SendBulkNotificationAsync(new BulkNotificationVM
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
        await SendNotificationAsync(new NotificationVM
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
        await SendNotificationAsync(new NotificationVM
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
        await SendNotificationAsync(new NotificationVM
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
        await SendNotificationAsync(new NotificationVM
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
        await SendNotificationAsync(new NotificationVM
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
        await SendNotificationAsync(new NotificationVM
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
        await SendNotificationAsync(new NotificationVM
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
        await SendNotificationAsync(new NotificationVM
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
        await SendNotificationAsync(new NotificationVM
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
        if (_userNotifications.TryGetValue(userId, out var notifications))
        {
            lock (notifications)
            {
                return notifications.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }
        }
        return new List<object>();
    }

    public async Task<object?> GetNotificationByIdAsync(Guid notificationId)
    {
        foreach (var userNotifs in _userNotifications.Values)
        {
            lock (userNotifs)
            {
                var notif = userNotifs.FirstOrDefault(n => (n as dynamic).Id == notificationId);
                if (notif != null) return notif;
            }
        }
        return null;
    }

    public async Task MarkAsReadAsync(Guid notificationId, Guid userId)
    {
        if (_userNotifications.TryGetValue(userId, out var notifications))
        {
            lock (notifications)
            {
                var notif = notifications.FirstOrDefault(n => (n as dynamic).Id == notificationId);
                if (notif != null) (notif as dynamic).IsRead = true;
            }
        }
        await Task.CompletedTask;
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        if (_userNotifications.TryGetValue(userId, out var notifications))
        {
            lock (notifications)
            {
                foreach (var notif in notifications)
                {
                    (notif as dynamic).IsRead = true;
                }
            }
        }
        await Task.CompletedTask;
    }

    public async Task DeleteNotificationAsync(Guid notificationId, Guid userId)
    {
        if (_userNotifications.TryGetValue(userId, out var notifications))
        {
            lock (notifications)
            {
                notifications.RemoveAll(n => (n as dynamic).Id == notificationId);
            }
        }
        await Task.CompletedTask;
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        if (_userNotifications.TryGetValue(userId, out var notifications))
        {
            lock (notifications)
            {
                return notifications.Count(n => !(n as dynamic).IsRead);
            }
        }
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
        await SendNotificationAsync(new NotificationVM
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