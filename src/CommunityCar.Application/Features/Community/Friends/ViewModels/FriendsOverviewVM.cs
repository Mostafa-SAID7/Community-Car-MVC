namespace CommunityCar.Application.Features.Community.Friends.ViewModels;

public class FriendsOverviewVM
{
    public int TotalFriends { get; set; }
    public int PendingRequests { get; set; }
    public int SentRequests { get; set; }
    public IEnumerable<FriendVM> RecentFriends { get; set; } = new List<FriendVM>();
    public IEnumerable<FriendRequestVM> RecentRequests { get; set; } = new List<FriendRequestVM>();
    public IEnumerable<FriendSuggestionVM> Suggestions { get; set; } = new List<FriendSuggestionVM>();
}