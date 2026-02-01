namespace CommunityCar.Application.Features.Community.Friends.ViewModels;

public class FriendVM
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Bio { get; set; }
    public DateTime FriendsSince { get; set; }
    public bool IsOnline { get; set; }
    public DateTime? LastSeen { get; set; }
    public int MutualFriendsCount { get; set; }
}