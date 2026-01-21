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
}