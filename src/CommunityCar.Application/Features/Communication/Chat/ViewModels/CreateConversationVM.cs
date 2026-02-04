namespace CommunityCar.Application.Features.Communication.Chat.ViewModels;

public class CreateConversationVM
{
    public string Title { get; set; } = string.Empty;
    public bool IsGroupChat { get; set; }
    public List<Guid> ParticipantIds { get; set; } = new();
    public string? InitialMessage { get; set; }
    public Guid CreatedBy { get; set; }
}