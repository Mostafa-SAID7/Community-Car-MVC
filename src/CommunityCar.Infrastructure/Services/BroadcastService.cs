using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CommunityCar.Infrastructure.Services;

public class BroadcastService : IBroadcastService
{
    private readonly IHubContext<BroadcastHub> _hubContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public BroadcastService(
        IHubContext<BroadcastHub> hubContext,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _hubContext = hubContext;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task BroadcastNewPostAsync(Guid postId, Guid? groupId = null)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        if (post == null) return;

        var author = await _unitOfWork.Users.GetByIdAsync(post.AuthorId);
        
        var postData = new
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            AuthorId = post.AuthorId,
            AuthorName = author?.Profile.FullName ?? "Unknown User",
            AuthorAvatar = author?.Profile.ProfilePictureUrl,
            GroupId = post.GroupId,
            CreatedAt = post.CreatedAt,
            Type = post.Type.ToString(),
            LikeCount = 0, // New posts start with 0 likes
            CommentCount = 0 // New posts start with 0 comments
        };

        if (groupId.HasValue)
        {
            // Broadcast to specific group
            await _hubContext.Clients.Group($"group_broadcast_{groupId}")
                .SendAsync("NewPostBroadcast", postData);
        }
        else
        {
            // Broadcast to all users (public post)
            await _hubContext.Clients.All
                .SendAsync("NewPostBroadcast", postData);
        }
    }

    public async Task BroadcastPostInteractionAsync(Guid postId, string interactionType, object data)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        if (post == null) return;

        var interactionData = new
        {
            PostId = postId,
            InteractionType = interactionType,
            Data = data,
            UserId = _currentUserService.UserId,
            Timestamp = DateTime.UtcNow
        };

        if (post.GroupId.HasValue)
        {
            // Broadcast to group members
            await _hubContext.Clients.Group($"group_broadcast_{post.GroupId}")
                .SendAsync("PostInteractionBroadcast", interactionData);
        }
        else
        {
            // Broadcast to all users (public post)
            await _hubContext.Clients.All
                .SendAsync("PostInteractionBroadcast", interactionData);
        }
    }

    public async Task BroadcastNewsInteractionAsync(Guid newsId, string interactionType, bool newValue, int count)
    {
        var interactionData = new
        {
            NewsId = newsId,
            InteractionType = interactionType,
            NewValue = newValue,
            Count = count,
            UserId = _currentUserService.UserId,
            Timestamp = DateTime.UtcNow
        };

        // Broadcast to all users viewing this news article
        await _hubContext.Clients.Group($"News_{newsId}")
            .SendAsync("NewsInteractionUpdate", newsId, interactionType, newValue, count);
    }

    public async Task BroadcastEventInteractionAsync(Guid eventId, string interactionType, object data)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId);
        if (eventEntity == null) return;

        var interactionData = new
        {
            EventId = eventId,
            InteractionType = interactionType,
            Data = data,
            UserId = _currentUserService.UserId,
            Timestamp = DateTime.UtcNow
        };

        // Broadcast to all users viewing this event
        await _hubContext.Clients.Group($"Event_{eventId}")
            .SendAsync("EventInteractionUpdate", eventId, interactionType, data);
    }

    public async Task BroadcastEventJoinAsync(Guid eventId, Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId);
        
        if (user == null || eventEntity == null) return;

        var joinData = new
        {
            EventId = eventId,
            EventTitle = eventEntity.Title,
            UserId = userId,
            UserName = user.Profile.FullName,
            UserAvatar = user.Profile.ProfilePictureUrl,
            AttendeeCount = eventEntity.AttendeeCount,
            JoinedAt = DateTime.UtcNow
        };

        // Broadcast to all users viewing this event
        await _hubContext.Clients.Group($"Event_{eventId}")
            .SendAsync("EventJoinUpdate", joinData);

        // Notify the user they successfully joined
        var userConnectionId = BroadcastHub.GetUserConnectionId(userId.ToString());
        if (!string.IsNullOrEmpty(userConnectionId))
        {
            await _hubContext.Clients.Client(userConnectionId)
                .SendAsync("EventJoinConfirmed", joinData);
        }
    }

    public async Task BroadcastEventLeaveAsync(Guid eventId, Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId);
        
        if (user == null || eventEntity == null) return;

        var leaveData = new
        {
            EventId = eventId,
            EventTitle = eventEntity.Title,
            UserId = userId,
            UserName = user.Profile.FullName,
            AttendeeCount = eventEntity.AttendeeCount,
            LeftAt = DateTime.UtcNow
        };

        // Broadcast to all users viewing this event
        await _hubContext.Clients.Group($"Event_{eventId}")
            .SendAsync("EventLeaveUpdate", leaveData);
    }

    public async Task BroadcastGroupJoinAsync(Guid groupId, Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        var group = await _unitOfWork.Groups.GetByIdAsync(groupId);
        
        if (user == null || group == null) return;

        var joinData = new
        {
            GroupId = groupId,
            GroupName = group.Name,
            UserId = userId,
            UserName = user.Profile.FullName,
            UserAvatar = user.Profile.ProfilePictureUrl,
            JoinedAt = DateTime.UtcNow
        };

        // Notify group members
        await _hubContext.Clients.Group($"group_broadcast_{groupId}")
            .SendAsync("UserJoinedGroup", joinData);

        // Notify the user they successfully joined
        var userConnectionId = BroadcastHub.GetUserConnectionId(userId.ToString());
        if (!string.IsNullOrEmpty(userConnectionId))
        {
            await _hubContext.Clients.Client(userConnectionId)
                .SendAsync("GroupJoinConfirmed", joinData);
        }
    }

    public async Task BroadcastGroupLeaveAsync(Guid groupId, Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        var group = await _unitOfWork.Groups.GetByIdAsync(groupId);
        
        if (user == null || group == null) return;

        var leaveData = new
        {
            GroupId = groupId,
            GroupName = group.Name,
            UserId = userId,
            UserName = user.Profile.FullName,
            LeftAt = DateTime.UtcNow
        };

        // Notify remaining group members
        await _hubContext.Clients.Group($"group_broadcast_{groupId}")
            .SendAsync("UserLeftGroup", leaveData);
    }

    public async Task NotifyGroupAdminsAsync(Guid groupId, string message, object? data = null)
    {
        var admins = await _unitOfWork.Groups.GetAdminsAsync(groupId);
        
        foreach (var admin in admins)
        {
            var connectionId = BroadcastHub.GetUserConnectionId(admin.Id.ToString());
            if (!string.IsNullOrEmpty(connectionId))
            {
                await _hubContext.Clients.Client(connectionId)
                    .SendAsync("AdminNotification", new
                    {
                        GroupId = groupId,
                        Message = message,
                        Data = data,
                        Timestamp = DateTime.UtcNow
                    });
            }
        }
    }
}