using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Dashboard.UserManagement.ViewModels;

public class UserStatisticsVM
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int NewUsersToday { get; set; }
    public int NewUsersThisWeek { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int VerifiedUsers { get; set; }
    public int UnverifiedUsers { get; set; }
    public int LockedUsers { get; set; }
    public int BannedUsers { get; set; }
    public double GrowthRate { get; set; }
    public double RetentionRate { get; set; }
    public double EngagementRate { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<ChartDataVM> UserGrowthTrend { get; set; } = new();
    public List<ChartDataVM> UserActivityTrend { get; set; } = new();
    public Dictionary<string, int> UsersByRole { get; set; } = new();
    public Dictionary<string, int> UsersByStatus { get; set; } = new();
}