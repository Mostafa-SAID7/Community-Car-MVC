using CommunityCar.Application.Features.Communication.Chat.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Communication;

public interface IChatService
{
    Task<ConversationVM?> GetConversationAsync(Guid conversationId, Guid userId);
    Task<List<ConversationVM>> GetUserConversationsAsync(Guid userId);
    Task<ConversationVM> CreateConversationAsync(CreateConversationVM request);
    Task<MessageVM> SendMessageAsync(SendMessageVM request);
    Task<bool> MarkMessageAsReadAsync(Guid messageId, Guid userId);
    Task<bool> MarkConversationAsReadAsync(Guid conversationId, Guid userId);
    Task<List<MessageVM>> GetConversationMessagesAsync(Guid conversationId, Guid userId, int page = 1, int pageSize = 50);
    Task<int> GetUnreadMessageCountAsync(Guid userId);
    Task<List<UserOnlineStatusVM>> GetOnlineUsersAsync(List<Guid> userIds);
}


