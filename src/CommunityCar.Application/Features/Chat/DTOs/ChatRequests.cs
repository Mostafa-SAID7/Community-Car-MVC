namespace CommunityCar.Application.Features.Chat.DTOs;

public class CreateConversationRequest
{
    public string Title { get; set; } = string.Empty;
    public bool IsGroupChat { get; set; }
    public List<Guid> ParticipantIds { get; set; } = new();
    public Guid CreatedBy { get; set; }
}

public class SendMessageRequest
{
    public Guid ConversationId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid SenderId { get; set; }
}

public class MarkAsReadRequest
{
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
}

public class GetMessagesRequest
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}


