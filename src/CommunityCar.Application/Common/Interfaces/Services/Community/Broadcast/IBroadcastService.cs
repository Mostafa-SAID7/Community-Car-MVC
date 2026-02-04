namespace CommunityCar.Application.Common.Interfaces.Services.Community.Broadcast;

public interface IBroadcastService
{
    Task BroadcastToAllAsync(string message, CancellationToken cancellationToken = default);
    Task BroadcastToGroupAsync(string groupName, string message, CancellationToken cancellationToken = default);
    Task BroadcastToUserAsync(string userId, string message, CancellationToken cancellationToken = default);
    Task BroadcastToRoleAsync(string role, string message, CancellationToken cancellationToken = default);
    Task NotifyUserAsync(string userId, string title, string message, CancellationToken cancellationToken = default);
    Task NotifyGroupAsync(string groupName, string title, string message, CancellationToken cancellationToken = default);
}