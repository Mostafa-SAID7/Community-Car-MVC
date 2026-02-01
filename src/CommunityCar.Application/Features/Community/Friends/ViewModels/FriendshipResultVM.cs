namespace CommunityCar.Application.Features.Community.Friends.ViewModels;

public class FriendshipResultVM
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? FriendshipId { get; set; }
}