using System;
using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Chats;

public class Conversation : AggregateRoot
{
    public string Title { get; private set; } = string.Empty; // For group chats
    public bool IsGroupChat { get; private set; }
    
    // Navigation properties
    public virtual ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    
    // Parameterless constructor for EF
    private Conversation() { }
    
    public Conversation(string title, bool isGroupChat)
    {
        Title = title;
        IsGroupChat = isGroupChat;
    }
}
