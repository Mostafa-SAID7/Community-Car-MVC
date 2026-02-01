namespace CommunityCar.Application.Features.Community.Friends.ViewModels;

public class FriendshipStatusVM
{
    public bool AreFriends { get; set; }
    public bool HasPendingRequest { get; set; }
    public bool HasSentRequest { get; set; }
    public bool IsBlocked { get; set; }
    public Guid? FriendshipId { get; set; }
}