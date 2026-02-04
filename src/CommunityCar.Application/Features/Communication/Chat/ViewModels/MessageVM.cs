namespace CommunityCar.Application.Features.Communication.Chat.ViewModels;

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