namespace CommunityCar.Web.Areas.Identity.ViewModels.Social;

public class FollowStatsVM
{
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public bool IsFollowing { get; set; }
    public bool IsFollowedBy { get; set; }
    public bool CanFollow { get; set; } = true;
}
