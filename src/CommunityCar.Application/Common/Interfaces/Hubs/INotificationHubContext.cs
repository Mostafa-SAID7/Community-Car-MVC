namespace CommunityCar.Application.Common.Interfaces.Hubs;

/// <summary>
/// Interface for notification hub context operations
/// </summary>
public interface INotificationHubContext
{
    Task SendNotificationToUserAsync(string userId, string method, object data);
    Task SendNotificationToGroupAsync(string groupName, string method, object data);
    Task SendNotificationToAllAsync(string method, object data);
    Task AddToGroupAsync(string connectionId, string groupName);
    Task RemoveFromGroupAsync(string connectionId, string groupName);
}