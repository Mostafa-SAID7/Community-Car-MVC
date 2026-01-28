namespace CommunityCar.Web.Models.Profile.Following;

/// <summary>
/// Follow statistics for a user
/// </summary>
public class FollowStatsVM
{
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public bool IsFollowing { get; set; }
    public bool IsFollowedBy { get; set; }
    public bool CanFollow { get; set; } = true;
}