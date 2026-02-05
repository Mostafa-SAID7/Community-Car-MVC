using CommunityCar.Application.Features.Dashboard.Overview.Users.Trends;

namespace CommunityCar.Application.Features.Dashboard.Overview.Users.Trends;

/// <summary>
/// User overview view model
/// </summary>
public class UserOverviewVM
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsersToday { get; set; }
    public int NewUsersThisWeek { get; set; }
    public int OnlineUsers { get; set; }
    public decimal GrowthRate { get; set; }
    public decimal ActiveUserRate { get; set; }
    public decimal VerificationRate { get; set; }
    public List<UserRegistrationTrendVM> RegistrationTrend { get; set; } = new();
    public List<ActiveUserTrendVM> ActivityTrend { get; set; } = new();
}