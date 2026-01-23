using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Chats;

public class ConversationParticipant : BaseEntity
{
    public Guid ConversationId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime JoinedAt { get; private set; }
    public DateTime? LastReadAt { get; private set; }

    // Parameterless constructor for EF
    private ConversationParticipant() { }

    public ConversationParticipant(Guid conversationId, Guid userId)
    {
        ConversationId = conversationId;
        UserId = userId;
        JoinedAt = DateTime.UtcNow;
    }

    public void UpdateLastRead()
    {
        LastReadAt = DateTime.UtcNow;
        Audit(UpdatedBy);
    }
}