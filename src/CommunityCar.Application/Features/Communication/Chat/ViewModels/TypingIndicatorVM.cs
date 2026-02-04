namespace CommunityCar.Application.Features.Communication.Chat.ViewModels;

public class TypingIndicatorVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid ConversationId { get; set; }
    public bool IsTyping { get; set; }
}