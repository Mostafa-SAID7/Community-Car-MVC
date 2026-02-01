namespace CommunityCar.Application.Features.Chat.ViewModels;

public class UserOnlineStatusVM
{
    public Guid UserId { get; set; }
    public bool IsOnline { get; set; }
    public DateTime? LastSeen { get; set; }
}