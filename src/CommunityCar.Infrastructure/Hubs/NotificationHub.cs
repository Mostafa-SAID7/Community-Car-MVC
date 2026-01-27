using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using System.Collections.Concurrent;

namespace CommunityCar.Infrastructure.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    private readonly ICurrentUserService _currentUserService;
    private static readonly ConcurrentDictionary<string, string> _userConnections = new();

    public NotificationHub(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = _currentUserService.UserId;
        if (!string.IsNullOrEmpty(userId))
        {
            _userConnections[userId] = Context.ConnectionId;
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId.ToLower()}");
        }
        
        await base.OnConnectedAsync();
    }

    [HubMethodName("JoinEntityGroup")]
    public async Task JoinEntityGroup(string entityType, string entityId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"{entityType.ToLower()}_{entityId.ToLower()}");
    }

    [HubMethodName("LeaveEntityGroup")]
    public async Task LeaveEntityGroup(string entityType, string entityId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{entityType.ToLower()}_{entityId.ToLower()}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = _currentUserService.UserId;
        if (!string.IsNullOrEmpty(userId))
        {
            _userConnections.TryRemove(userId, out _);
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    public async Task MarkNotificationAsRead(string notificationId)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
            return;

        // TODO: Implement notification read status when repository is available
        await Clients.Caller.SendAsync("NotificationMarkedAsRead", notificationId);
    }

    public async Task MarkAllNotificationsAsRead()
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
            return;

        // TODO: Implement mark all notifications as read when repository is available
        await Clients.Caller.SendAsync("AllNotificationsMarkedAsRead");
    }

    public static bool IsUserOnline(string userId)
    {
        return _userConnections.ContainsKey(userId);
    }

    public static string? GetUserConnectionId(string userId)
    {
        _userConnections.TryGetValue(userId, out var connectionId);
        return connectionId;
    }
}
