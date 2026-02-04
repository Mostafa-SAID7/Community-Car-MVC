namespace CommunityCar.Application.Features.Communication.Chat.ViewModels;

public class SendMessageVM
{
    public Guid ConversationId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<string>? Attachments { get; set; }
    public Guid SenderId { get; set; }
}