using System;
using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Chats;

public class Message : BaseEntity
{
    public string Content { get; private set; }
    public Guid ConversationId { get; private set; }
    public Guid SenderId { get; private set; }
    public bool IsRead { get; private set; }

    public Message(string content, Guid conversationId, Guid senderId)
    {
        Content = content;
        ConversationId = conversationId;
        SenderId = senderId;
        IsRead = false;
    }

    public void MarkAsRead()
    {
        IsRead = true;
        Audit(UpdatedBy);
    }
}
