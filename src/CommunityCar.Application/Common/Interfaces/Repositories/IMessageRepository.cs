using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Chats;

namespace CommunityCar.Application.Common.Interfaces.Repositories;

public interface IMessageRepository : IBaseRepository<Message>
{
    Task<List<Message>> GetConversationMessagesAsync(Guid conversationId, int page = 1, int pageSize = 50);
    Task<int> GetUnreadMessageCountAsync(Guid userId);
    Task<int> GetConversationUnreadCountAsync(Guid conversationId, Guid userId);
    Task MarkConversationAsReadAsync(Guid conversationId, Guid userId);
    Task<Message?> GetLastMessageAsync(Guid conversationId);
}