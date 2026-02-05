using CommunityCar.Application.Common.Interfaces.Services.Community.Broadcast;

namespace CommunityCar.Application.Services.Community.Broadcast;

public class BroadcastService : IBroadcastService
{
    private readonly CommunityCar.Application.Common.Interfaces.Services.Community.IBroadcastService _broadcastService;

    public BroadcastService(CommunityCar.Application.Common.Interfaces.Services.Community.IBroadcastService broadcastService)
    {
        _broadcastService = broadcastService;
    }

    public async Task BroadcastToAllAsync(string message, CancellationToken cancellationToken = default)
    {
        await _broadcastService.BroadcastToAllAsync(message, null, cancellationToken);
    }

    public async Task BroadcastToGroupAsync(string groupName, string message, CancellationToken cancellationToken = default)
    {
        await _broadcastService.BroadcastToGroupAsync(groupName, message, null, cancellationToken);
    }

    public async Task BroadcastToUserAsync(string userId, string message, CancellationToken cancellationToken = default)
    {
        if (Guid.TryParse(userId, out var userGuid))
        {
            await _broadcastService.BroadcastToUserAsync(userGuid, message, null, cancellationToken);
        }
    }

    public async Task BroadcastToRoleAsync(string role, string message, CancellationToken cancellationToken = default)
    {
        await _broadcastService.BroadcastToRoleAsync(role, message, null, cancellationToken);
    }

    public async Task NotifyUserAsync(string userId, string title, string message, CancellationToken cancellationToken = default)
    {
        if (Guid.TryParse(userId, out var userGuid))
        {
            await _broadcastService.NotifyUserAsync(userGuid, title, message, null, cancellationToken);
        }
    }

    public async Task NotifyGroupAsync(string groupName, string title, string message, CancellationToken cancellationToken = default)
    {
        // For group notifications, we can broadcast to the group with notification data
        var notificationData = new
        {
            Title = title,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        await _broadcastService.BroadcastToGroupAsync(groupName, "Notification", notificationData, cancellationToken);
    }
}