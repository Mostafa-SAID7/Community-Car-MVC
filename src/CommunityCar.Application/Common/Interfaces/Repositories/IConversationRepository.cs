using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Chats;

namespace CommunityCar.Application.Common.Interfaces.Repositories;

public interface IConversationRepository : IBaseRepository<Conversation>
{
    Task<List<Conversation>> GetUserConversationsAsync(Guid userId);
    Task<Conversation?> GetConversationWithParticipantsAsync(Guid conversationId);
    Task<bool> IsUserParticipantAsync(Guid conversationId, Guid userId);
    Task AddParticipantAsync(Guid conversationId, Guid userId);
    Task RemoveParticipantAsync(Guid conversationId, Guid userId);
}


