namespace CommunityCar.Application.Features.Communication.Chat.ViewModels;

public class UserOnlineStatusVM
{
    public Guid UserId { get; set; }
    public bool IsOnline { get; set; }
    public DateTime? LastSeen { get; set; }
}