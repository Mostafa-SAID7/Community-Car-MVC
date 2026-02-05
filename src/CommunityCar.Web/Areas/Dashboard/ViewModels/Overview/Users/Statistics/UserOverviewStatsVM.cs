namespace CommunityCar.Application.Features.Dashboard.Overview.Users.Statistics;

/// <summary>
/// User overview statistics view model
/// </summary>
public class UserOverviewStatsVM
{
    public Guid UserId { get; set; }
    public int TotalPosts { get; set; }
    public int TotalComments { get; set; }
    public int TotalLikes { get; set; }
    public int TotalShares { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int ProfileViews { get; set; }
    public DateTime LastActiveDate { get; set; }
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsersToday { get; set; }
    public int NewUsersThisWeek { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int OnlineUsers { get; set; }
    public int OfflineUsers { get; set; }
    public int VerifiedUsers { get; set; }
    public int UnverifiedUsers { get; set; }
    public int PremiumUsers { get; set; }
    public int FreeUsers { get; set; }
    public decimal GrowthRate { get; set; }
    
    // Missing property that services expect
    public decimal UserGrowthRate { get; set; }
    
    public string TrendDirection { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}