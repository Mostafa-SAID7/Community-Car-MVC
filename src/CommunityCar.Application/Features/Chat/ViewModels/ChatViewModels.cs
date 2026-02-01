namespace CommunityCar.Application.Features.Chat.ViewModels;

public class ConversationVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsGroupChat { get; set; }
    public List<ParticipantVM> Participants { get; set; } = new();
    public MessageVM? LastMessage { get; set; }
    public int UnreadCount { get; set; }
    public DateTime LastActivity { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MessageVM
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string? SenderAvatar { get; set; }
    public Guid ConversationId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public bool IsOwnMessage { get; set; }
}

public class ParticipantVM
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public bool IsOnline { get; set; }
    public DateTime? LastSeen { get; set; }
}

public class UserOnlineStatusVM
{
    public Guid UserId { get; set; }
    public bool IsOnline { get; set; }
    public DateTime? LastSeen { get; set; }
}

public class TypingIndicatorVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid ConversationId { get; set; }
    public bool IsTyping { get; set; }
}

public class CreateConversationVM
{
    public string Title { get; set; } = string.Empty;
    public bool IsGroupChat { get; set; }
    public List<Guid> ParticipantIds { get; set; } = new();
    public string? InitialMessage { get; set; }
    public Guid CreatedBy { get; set; }
}

public class SendMessageVM
{
    public Guid ConversationId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<string>? Attachments { get; set; }
    public Guid SenderId { get; set; }
}


