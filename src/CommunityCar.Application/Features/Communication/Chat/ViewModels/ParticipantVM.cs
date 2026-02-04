namespace CommunityCar.Application.Features.Communication.Chat.ViewModels;

public class ParticipantVM
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public bool IsOnline { get; set; }
    public DateTime? LastSeen { get; set; }
}