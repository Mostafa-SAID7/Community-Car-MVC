using System;
using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Chats;

public class Conversation : AggregateRoot
{
    public string Title { get; private set; } // For group chats
    public bool IsGroupChat { get; private set; }
    
    // Participants would be a collection of User IDs or a link entity
    
    public Conversation(string title, bool isGroupChat)
    {
        Title = title;
        IsGroupChat = isGroupChat;
    }
}
