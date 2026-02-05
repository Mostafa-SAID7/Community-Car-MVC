namespace CommunityCar.Application.Features.Dashboard.Management.Users.Core;

/// <summary>
/// ViewModel for dashboard user statistics
/// </summary>
public class DashboardUserStatisticsVM
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int NewUsersToday { get; set; }
    public int NewUsersThisWeek { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int OnlineUsers { get; set; }
    public int VerifiedUsers { get; set; }
    public int UnverifiedUsers { get; set; }
    public int LockedUsers { get; set; }
    public int BannedUsers { get; set; }
    public decimal GrowthRate { get; set; }
    public decimal RetentionRate { get; set; }
    public decimal VerificationRate { get; set; }
}