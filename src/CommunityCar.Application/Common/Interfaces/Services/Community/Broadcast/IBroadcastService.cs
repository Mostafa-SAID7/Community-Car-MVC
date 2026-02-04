namespace CommunityCar.Application.Common.Interfaces.Services.Community.Broadcast;

public interface IBroadcastService
{
    Task BroadcastNewPostAsync(Guid postId, Guid? groupId = null);
    Task BroadcastPostInteractionAsync(Guid postId, string interactionType, object data);
    Task BroadcastNewsInteractionAsync(Guid newsId, string interactionType, bool newValue, int count);
    Task BroadcastEventInteractionAsync(Guid eventId, string interactionType, object data);
    Task BroadcastEventJoinAsync(Guid eventId, Guid userId);
    Task BroadcastEventLeaveAsync(Guid eventId, Guid userId);
    Task BroadcastGroupJoinAsync(Guid groupId, Guid userId);
    Task BroadcastGroupLeaveAsync(Guid groupId, Guid userId);
    Task NotifyGroupAdminsAsync(Guid groupId, string message, object? data = null);
}