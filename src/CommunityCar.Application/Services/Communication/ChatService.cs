using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Features.Chat.ViewModels;
using CommunityCar.Application.Features.Chat.DTOs;
using CommunityCar.Domain.Entities.Chats;
using CommunityCar.Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Application.Services.Communication;

public class ChatService : IChatService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;

    public ChatService(IUnitOfWork unitOfWork, UserManager<User> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ConversationVM?> GetConversationAsync(Guid conversationId, Guid userId)
    {
        var conversation = await _unitOfWork.Conversations.GetConversationWithParticipantsAsync(conversationId);
        if (conversation == null) return null;

        // Check if user is participant
        var isParticipant = await _unitOfWork.Conversations.IsUserParticipantAsync(conversationId, userId);
        if (!isParticipant) return null;

        var lastMessage = await _unitOfWork.Messages.GetLastMessageAsync(conversationId);
        var unreadCount = await _unitOfWork.Messages.GetConversationUnreadCountAsync(conversationId, userId);

        return new ConversationVM
        {
            Id = conversation.Id,
            Title = conversation.Title,
            IsGroupChat = conversation.IsGroupChat,
            LastMessage = lastMessage != null ? new MessageVM
            {
                Id = lastMessage.Id,
                Content = lastMessage.Content,
                SenderId = lastMessage.SenderId,
                CreatedAt = lastMessage.CreatedAt,
                TimeAgo = GetTimeAgo(lastMessage.CreatedAt)
            } : null,
            UnreadCount = unreadCount,
            LastActivity = lastMessage?.CreatedAt ?? conversation.CreatedAt,
            CreatedAt = conversation.CreatedAt
        };
    }

    public async Task<List<ConversationVM>> GetUserConversationsAsync(Guid userId)
    {
        var conversations = await _unitOfWork.Conversations.GetUserConversationsAsync(userId);
        var result = new List<ConversationVM>();

        foreach (var conversation in conversations)
        {
            var lastMessage = await _unitOfWork.Messages.GetLastMessageAsync(conversation.Id);
            var unreadCount = await _unitOfWork.Messages.GetConversationUnreadCountAsync(conversation.Id, userId);

            result.Add(new ConversationVM
            {
                Id = conversation.Id,
                Title = conversation.Title,
                IsGroupChat = conversation.IsGroupChat,
                LastMessage = lastMessage != null ? new MessageVM
                {
                    Id = lastMessage.Id,
                    Content = lastMessage.Content,
                    SenderId = lastMessage.SenderId,
                    CreatedAt = lastMessage.CreatedAt,
                    TimeAgo = GetTimeAgo(lastMessage.CreatedAt)
                } : null,
                UnreadCount = unreadCount,
                LastActivity = lastMessage?.CreatedAt ?? conversation.CreatedAt,
                CreatedAt = conversation.CreatedAt
            });
        }

        return result.OrderByDescending(c => c.LastActivity).ToList();
    }

    public async Task<ConversationVM> CreateConversationAsync(CreateConversationRequest request)
    {
        var conversation = new Conversation(request.Title, request.IsGroupChat);
        
        await _unitOfWork.Conversations.AddAsync(conversation);
        
        // Add creator as participant
        await _unitOfWork.Conversations.AddParticipantAsync(conversation.Id, request.CreatedBy);
        
        // Add other participants
        foreach (var participantId in request.ParticipantIds)
        {
            if (participantId != request.CreatedBy)
            {
                await _unitOfWork.Conversations.AddParticipantAsync(conversation.Id, participantId);
            }
        }
        
        await _unitOfWork.SaveChangesAsync();
        
        return new ConversationVM
        {
            Id = conversation.Id,
            Title = conversation.Title,
            IsGroupChat = conversation.IsGroupChat,
            CreatedAt = conversation.CreatedAt,
            LastActivity = conversation.CreatedAt
        };
    }

    public async Task<MessageVM> SendMessageAsync(SendMessageRequest request)
    {
        var message = new Message(request.Content, request.ConversationId, request.SenderId);
        
        await _unitOfWork.Messages.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();
        
        var sender = await _userManager.FindByIdAsync(request.SenderId.ToString());
        
        return new MessageVM
        {
            Id = message.Id,
            Content = message.Content,
            SenderId = message.SenderId,
            SenderName = sender?.FullName ?? "Unknown User",
            SenderAvatar = sender?.ProfilePictureUrl,
            ConversationId = message.ConversationId,
            IsRead = message.IsRead,
            CreatedAt = message.CreatedAt,
            TimeAgo = GetTimeAgo(message.CreatedAt),
            IsOwnMessage = true
        };
    }

    public async Task<bool> MarkMessageAsReadAsync(Guid messageId, Guid userId)
    {
        var message = await _unitOfWork.Messages.GetByIdAsync(messageId);
        if (message == null) return false;

        message.MarkAsRead();
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkConversationAsReadAsync(Guid conversationId, Guid userId)
    {
        await _unitOfWork.Messages.MarkConversationAsReadAsync(conversationId, userId);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<List<MessageVM>> GetConversationMessagesAsync(Guid conversationId, Guid userId, int page = 1, int pageSize = 50)
    {
        // Check if user is participant
        var isParticipant = await _unitOfWork.Conversations.IsUserParticipantAsync(conversationId, userId);
        if (!isParticipant) return new List<MessageVM>();

        var messages = await _unitOfWork.Messages.GetConversationMessagesAsync(conversationId, page, pageSize);
        var result = new List<MessageVM>();

        foreach (var message in messages)
        {
            var sender = await _userManager.FindByIdAsync(message.SenderId.ToString());
            result.Add(new MessageVM
            {
                Id = message.Id,
                Content = message.Content,
                SenderId = message.SenderId,
                SenderName = sender?.FullName ?? "Unknown User",
                SenderAvatar = sender?.ProfilePictureUrl,
                ConversationId = message.ConversationId,
                IsRead = message.IsRead,
                CreatedAt = message.CreatedAt,
                TimeAgo = GetTimeAgo(message.CreatedAt),
                IsOwnMessage = message.SenderId == userId
            });
        }

        return result;
    }

    public async Task<int> GetUnreadMessageCountAsync(Guid userId)
    {
        return await _unitOfWork.Messages.GetUnreadMessageCountAsync(userId);
    }

    public async Task<List<UserOnlineStatusVM>> GetOnlineUsersAsync(List<Guid> userIds)
    {
        var users = await _userManager.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync();

        return users.Select(u => new UserOnlineStatusVM
        {
            UserId = u.Id,
            IsOnline = IsUserOnline(u.Id),
            LastSeen = u.LastLoginAt
        }).ToList();
    }

    private bool IsUserOnline(Guid userId)
    {
        // Check if user was active in the last 15 minutes
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
        return user?.LastLoginAt.HasValue == true && 
               user.LastLoginAt > DateTime.UtcNow.AddMinutes(-15);
    }

    private string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalMinutes < 1)
            return "Just now";
        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes}m ago";
        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours}h ago";
        if (timeSpan.TotalDays < 7)
            return $"{(int)timeSpan.TotalDays}d ago";
        
        return dateTime.ToString("MMM dd");
    }
}


