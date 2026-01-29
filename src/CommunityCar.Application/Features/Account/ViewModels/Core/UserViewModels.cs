namespace CommunityCar.Application.Features.Account.ViewModels.Core;

public class UserDashboardVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int PostsCount { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int AchievementsCount { get; set; }
    public int BadgesCount { get; set; }
    public DateTime LastLoginAt { get; set; }
    public List<UserActivityVM> RecentActivities { get; set; } = new();
}