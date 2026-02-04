namespace CommunityCar.Application.Common.Interfaces.Services.Community;

/// <summary>
/// Interface for broadcast services
/// </summary>
public interface IBroadcastService
{
    Task BroadcastToAllAsync(string message, object? data = null, CancellationToken cancellationToken = default);
    Task BroadcastToUserAsync(Guid userId, string message, object? data = null, CancellationToken cancellationToken = default);
    Task BroadcastToGroupAsync(string groupName, string message, object? data = null, CancellationToken cancellationToken = default);
    Task BroadcastToRoleAsync(string role, string message, object? data = null, CancellationToken cancellationToken = default);
    Task NotifyUserAsync(Guid userId, string title, string message, string? url = null, CancellationToken cancellationToken = default);
    Task NotifyFollowersAsync(Guid userId, string title, string message, string? url = null, CancellationToken cancellationToken = default);
}