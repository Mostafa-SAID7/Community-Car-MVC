namespace CommunityCar.Web.Areas.Identity.ViewModels.Profile;

/// <summary>
/// ViewModel for profile statistics
/// </summary>
public class ProfileStatsVM
{
    public int PostsCount { get; set; }
    public int CommentsCount { get; set; }
    public int LikesReceived { get; set; }
    public int SharesReceived { get; set; }
    public int ViewsReceived { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int AchievementsCount { get; set; }
    public int BadgesCount { get; set; }
    public int TotalPoints { get; set; }
    public int Level { get; set; }
    public double EngagementRate { get; set; }
    public DateTime JoinedDate { get; set; }
    public DateTime LastActiveDate { get; set; }
    public int DaysActive { get; set; }
    public Dictionary<string, int> MonthlyActivity { get; set; } = new();
}
