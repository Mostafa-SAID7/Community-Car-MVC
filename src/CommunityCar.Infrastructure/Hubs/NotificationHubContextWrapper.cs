using CommunityCar.Application.Common.Interfaces.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CommunityCar.Infrastructure.Hubs;

/// <summary>
/// Wrapper implementation for INotificationHubContext that uses SignalR's IHubContext
/// </summary>
public class NotificationHubContextWrapper : INotificationHubContext
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationHubContextWrapper(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationToUserAsync(string userId, string method, object data)
    {
        await _hubContext.Clients.User(userId).SendAsync(method, data);
    }

    public async Task SendNotificationToGroupAsync(string groupName, string method, object data)
    {
        await _hubContext.Clients.Group(groupName).SendAsync(method, data);
    }

    public async Task SendNotificationToAllAsync(string method, object data)
    {
        await _hubContext.Clients.All.SendAsync(method, data);
    }

    public async Task AddToGroupAsync(string connectionId, string groupName)
    {
        await _hubContext.Groups.AddToGroupAsync(connectionId, groupName);
    }

    public async Task RemoveFromGroupAsync(string connectionId, string groupName)
    {
        await _hubContext.Groups.RemoveFromGroupAsync(connectionId, groupName);
    }
}