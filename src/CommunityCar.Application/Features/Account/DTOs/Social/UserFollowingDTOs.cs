namespace CommunityCar.Application.Features.Account.DTOs.Social;

#region User Following DTOs

public class UserFollowingDTO
{
    public Guid Id { get; set; }
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
    public string FollowerName { get; set; } = string.Empty;
    public string FollowingName { get; set; } = string.Empty;
    public string? FollowerProfilePicture { get; set; }
    public string? FollowingProfilePicture { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsMutual { get; set; }
}

public class FollowUserRequest
{
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
}

public class UnfollowUserRequest
{
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
}

public class FollowingListRequest
{
    public Guid UserId { get; set; }
    public string ListType { get; set; } = "following"; // "following" or "followers"
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchTerm { get; set; }
}

#endregion