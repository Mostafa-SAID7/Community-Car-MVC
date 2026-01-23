namespace CommunityCar.Application.Features.Friends.DTOs;

public class FriendshipResultDTO
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? FriendshipId { get; set; }
}

public class FriendshipStatusDTO
{
    public bool AreFriends { get; set; }
    public bool HasPendingRequest { get; set; }
    public bool HasSentRequest { get; set; }
    public bool IsBlocked { get; set; }
    public Guid? FriendshipId { get; set; }
}