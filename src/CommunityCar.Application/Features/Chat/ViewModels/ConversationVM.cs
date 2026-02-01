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