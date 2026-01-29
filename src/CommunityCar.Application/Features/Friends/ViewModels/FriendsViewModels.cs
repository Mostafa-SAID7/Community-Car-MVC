namespace CommunityCar.Application.Features.Friends.ViewModels;

public class FriendsOverviewVM
{
    public int TotalFriends { get; set; }
    public int PendingRequests { get; set; }
    public int SentRequests { get; set; }
    public IEnumerable<FriendVM> RecentFriends { get; set; } = new List<FriendVM>();
    public IEnumerable<FriendRequestVM> RecentRequests { get; set; } = new List<FriendRequestVM>();
    public IEnumerable<FriendSuggestionVM> Suggestions { get; set; } = new List<FriendSuggestionVM>();
}

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

public class FriendSuggestionVM
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Bio { get; set; }
    public int MutualFriendsCount { get; set; }
    public IEnumerable<string> MutualFriendsNames { get; set; } = new List<string>();
    public string SuggestionReason { get; set; } = string.Empty;
}

public class FriendshipResultVM
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? FriendshipId { get; set; }
}

public class FriendshipStatusVM
{
    public bool AreFriends { get; set; }
    public bool HasPendingRequest { get; set; }
    public bool HasSentRequest { get; set; }
    public bool IsBlocked { get; set; }
    public Guid? FriendshipId { get; set; }
}

