using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Chats;

namespace CommunityCar.Web.Areas.Communication.Interfaces.Repositories.Chat;

public interface IConversationParticipantRepository : IBaseRepository<ConversationParticipant>
{
    Task<IEnumerable<ConversationParticipant>> GetConversationParticipantsAsync(Guid conversationId);
    Task<ConversationParticipant?> GetParticipantAsync(Guid conversationId, Guid userId);
    Task<bool> IsParticipantAsync(Guid conversationId, Guid userId);
    Task<bool> UpdateLastReadAsync(Guid conversationId, Guid userId, DateTime lastReadAt);
    Task<bool> MuteConversationAsync(Guid conversationId, Guid userId, bool isMuted);
    Task<bool> SetParticipantRoleAsync(Guid conversationId, Guid userId, string role);
    Task<IEnumerable<ConversationParticipant>> GetActiveParticipantsAsync(Guid conversationId);
    Task<int> GetActiveParticipantCountAsync(Guid conversationId);
}
