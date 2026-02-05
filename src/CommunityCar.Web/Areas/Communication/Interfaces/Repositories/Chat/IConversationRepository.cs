using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Chats;

namespace CommunityCar.Web.Areas.Communication.Interfaces.Repositories.Chat;

public interface IConversationRepository : IBaseRepository<Conversation>
{
    Task<IEnumerable<Conversation>> GetUserConversationsAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<Conversation?> GetConversationWithParticipantsAsync(Guid conversationId);
    Task<Conversation?> GetDirectConversationAsync(Guid userId1, Guid userId2);
    Task<bool> IsUserParticipantAsync(Guid conversationId, Guid userId);
    Task<bool> AddParticipantAsync(Guid conversationId, Guid userId);
    Task<bool> RemoveParticipantAsync(Guid conversationId, Guid userId);
    Task<int> GetParticipantCountAsync(Guid conversationId);
    Task<IEnumerable<Guid>> GetParticipantIdsAsync(Guid conversationId);
    Task<bool> ArchiveConversationAsync(Guid conversationId, Guid userId);
    Task<bool> UnarchiveConversationAsync(Guid conversationId, Guid userId);
    Task<IEnumerable<Conversation>> GetArchivedConversationsAsync(Guid userId);
}
