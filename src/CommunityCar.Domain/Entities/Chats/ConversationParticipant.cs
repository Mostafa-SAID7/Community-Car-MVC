using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Chats;

public class ConversationParticipant : BaseEntity
{
    public Guid ConversationId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime JoinedAt { get; private set; }
    public DateTime? LastReadAt { get; private set; }
    public bool IsArchived { get; private set; }
    public bool IsMuted { get; private set; }
    public DateTime? MutedUntil { get; private set; }
    public string Role { get; private set; } = "Member";

    // Parameterless constructor for EF
    private ConversationParticipant() { }

    public ConversationParticipant(Guid conversationId, Guid userId)
    {
        ConversationId = conversationId;
        UserId = userId;
        JoinedAt = DateTime.UtcNow;
    }

    public static ConversationParticipant Create(Guid conversationId, Guid userId)
    {
        return new ConversationParticipant(conversationId, userId);
    }

    public void UpdateLastRead()
    {
        LastReadAt = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public void UpdateLastRead(DateTime lastReadAt)
    {
        LastReadAt = lastReadAt;
        Audit(UpdatedBy);
    }

    public void Archive()
    {
        IsArchived = true;
        Audit(UpdatedBy);
    }

    public void Unarchive()
    {
        IsArchived = false;
        Audit(UpdatedBy);
    }

    public void Mute(DateTime? until = null)
    {
        IsMuted = true;
        MutedUntil = until;
        Audit(UpdatedBy);
    }

    public void Unmute()
    {
        IsMuted = false;
        MutedUntil = null;
        Audit(UpdatedBy);
    }

    public void SetRole(string role)
    {
        Role = role;
        Audit(UpdatedBy);
    }
}