namespace CommunityCar.Application.Features.Community.Friends.ViewModels;

public class FriendRequestVM
{
    public Guid FriendshipId { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Bio { get; set; }
    public DateTime RequestDate { get; set; }
    public int MutualFriendsCount { get; set; }
    public bool IsIncoming { get; set; }
}