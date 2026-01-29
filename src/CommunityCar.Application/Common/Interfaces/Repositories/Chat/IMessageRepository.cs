using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Chats;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Chat;

public interface IMessageRepository : IBaseRepository<Message>
{
    Task<IEnumerable<Message>> GetConversationMessagesAsync(Guid conversationId, int page = 1, int pageSize = 50);
    Task<IEnumerable<Message>> GetUnreadMessagesAsync(Guid userId);
    Task<int> GetUnreadMessageCountAsync(Guid userId);
    Task<int> GetConversationUnreadCountAsync(Guid conversationId, Guid userId);
    Task<Message?> GetLastMessageAsync(Guid conversationId);
    Task<bool> MarkMessageAsReadAsync(Guid messageId, Guid userId);
    Task<bool> MarkConversationAsReadAsync(Guid conversationId, Guid userId);
    Task<bool> DeleteMessageAsync(Guid messageId, Guid userId);
    Task<bool> EditMessageAsync(Guid messageId, string newContent);
    Task<IEnumerable<Message>> SearchMessagesAsync(Guid userId, string searchTerm, int page = 1, int pageSize = 20);
}