using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Core;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Domain.Entities.Chats;
using System.Collections.Concurrent;

namespace CommunityCar.Infrastructure.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private static readonly ConcurrentDictionary<string, string> _userConnections = new();
    private static readonly ConcurrentDictionary<string, HashSet<string>> _conversationGroups = new();

    public ChatHub(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = _currentUserService.UserId;
        if (!string.IsNullOrEmpty(userId))
        {
            _userConnections[userId] = Context.ConnectionId;
            
            // Join user to their personal notification group
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            
            // Notify others that user is online
            await Clients.Others.SendAsync("UserOnline", userId);
        }
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = _currentUserService.UserId;
        if (!string.IsNullOrEmpty(userId))
        {
            _userConnections.TryRemove(userId, out _);
            
            // Remove from all conversation groups
            foreach (var group in _conversationGroups.Values)
            {
                group.Remove(Context.ConnectionId);
            }
            
            // Notify others that user is offline
            await Clients.Others.SendAsync("UserOffline", userId);
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinConversation(string conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        
        if (!_conversationGroups.ContainsKey(conversationId))
        {
            _conversationGroups[conversationId] = new HashSet<string>();
        }
        _conversationGroups[conversationId].Add(Context.ConnectionId);
    }

    public async Task LeaveConversation(string conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        
        if (_conversationGroups.ContainsKey(conversationId))
        {
            _conversationGroups[conversationId].Remove(Context.ConnectionId);
        }
    }

    public async Task SendMessage(string conversationId, string content)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId) || string.IsNullOrWhiteSpace(content))
            return;

        var userGuid = Guid.Parse(userId);
        var conversationGuid = Guid.Parse(conversationId);

        // Create and save message
        var message = new Message(content.Trim(), conversationGuid, userGuid);
        
        await _unitOfWork.Messages.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();

        // Get user info for the message
        var user = await _unitOfWork.Users.GetByIdAsync(userGuid);
        
        var messageData = new
        {
            Id = message.Id,
            Content = message.Content,
            SenderId = message.SenderId,
            SenderName = user?.Profile.FullName ?? "Unknown User",
            SenderAvatar = user?.Profile.ProfilePictureUrl,
            ConversationId = message.ConversationId,
            CreatedAt = message.CreatedAt,
            IsRead = message.IsRead
        };

        // Send to conversation group
        await Clients.Group($"conversation_{conversationId}").SendAsync("ReceiveMessage", messageData);
    }

    public async Task MarkMessageAsRead(string messageId)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
            return;

        var message = await _unitOfWork.Messages.GetByIdAsync(Guid.Parse(messageId));
        if (message != null)
        {
            message.MarkAsRead();
            await _unitOfWork.SaveChangesAsync();
        }

        await Clients.Caller.SendAsync("MessageMarkedAsRead", messageId);
    }

    public async Task StartTyping(string conversationId)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
            return;

        await Clients.OthersInGroup($"conversation_{conversationId}")
            .SendAsync("UserTyping", userId, conversationId);
    }

    public async Task StopTyping(string conversationId)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
            return;

        await Clients.OthersInGroup($"conversation_{conversationId}")
            .SendAsync("UserStoppedTyping", userId, conversationId);
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

