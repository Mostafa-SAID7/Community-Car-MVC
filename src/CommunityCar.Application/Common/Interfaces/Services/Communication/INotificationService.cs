using CommunityCar.Application.Common.Models.Notifications;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Common.Interfaces.Services.Communication;

public interface INotificationService
{
    Task SendNotificationAsync(NotificationRequest request);
    Task SendBulkNotificationAsync(BulkNotificationRequest request);
    Task SendToUserAsync(Guid userId, string title, string message, NotificationType type, string? actionUrl = null);
    Task SendToUsersAsync(List<Guid> userIds, string title, string message, NotificationType type, string? actionUrl = null);
    Task NotifyNewMessageAsync(Guid userId, string senderName, string conversationId);
    Task NotifyNewAnswerAsync(Guid userId, string questionTitle, Guid questionId);
    Task NotifyQuestionSolvedAsync(Guid userId, string questionTitle, Guid questionId);
    Task NotifyVoteReceivedAsync(Guid userId, string itemTitle, bool isUpvote);
    Task NotifyCommentReceivedAsync(Guid userId, string itemTitle, string commenterName);
    Task NotifyFriendRequestAsync(Guid userId, string requesterName, Guid requesterId);
    Task NotifyFriendRequestAcceptedAsync(Guid userId, string accepterName, Guid accepterId);
    Task NotifyFriendRequestDeclinedAsync(Guid userId, string declinerName);
    Task NotifyFriendRemovedAsync(Guid userId, string removerName);
    
    // Additional methods for notification management
    Task<IEnumerable<object>> GetUserNotificationsAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<object?> GetNotificationByIdAsync(Guid notificationId);
    Task MarkAsReadAsync(Guid notificationId, Guid userId);
    Task MarkAllAsReadAsync(Guid userId);
    Task DeleteNotificationAsync(Guid notificationId, Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task<object> GetUserPreferencesAsync(Guid userId);
    Task UpdateUserPreferencesAsync(Guid userId, object preferences);
    Task SendTestNotificationAsync(Guid userId, string message);
}