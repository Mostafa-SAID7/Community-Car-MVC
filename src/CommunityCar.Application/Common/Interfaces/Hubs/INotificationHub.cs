namespace CommunityCar.Application.Common.Interfaces.Hubs;

/// <summary>
/// Interface for SignalR notification hub
/// </summary>
public interface INotificationHub
{
    Task ReceiveNotification(object notification);
    Task NotificationMarkedAsRead(string notificationId);
    Task AllNotificationsMarkedAsRead();
}