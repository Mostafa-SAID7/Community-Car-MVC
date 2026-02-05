using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Shared.ViewModels.Users;

/// <summary>
/// Unified user statistics view model that consolidates all user stats
/// </summary>
public class UnifiedUserStatsVM
{
    #region Basic User Counts
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int OnlineUsers { get; set; }
    public int OfflineUsers { get; set; }
    public int VerifiedUsers { get; set; }
    public int UnverifiedUsers { get; set; }
    public int LockedUsers { get; set; }
    #endregion

    #region Time-based Registrations
    public int NewUsersToday { get; set; }
    public int NewUsersThisWeek { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int NewUsersThisYear { get; set; }
    #endregion

    #region Growth Metrics
    public decimal GrowthRate { get; set; }
    public string TrendDirection { get; set; } = string.Empty; // "Increasing", "Decreasing", "Stable"
    public decimal GrowthPercentage { get; set; }
    public int AverageUsersPerDay { get; set; }
    #endregion

    #region Engagement Metrics
    public double OverallEngagementRate { get; set; }
    public int TotalInteractions { get; set; }
    public int ActiveUsersToday { get; set; }
    public int ActiveUsersThisWeek { get; set; }
    public int ActiveUsersThisMonth { get; set; }
    public double AverageSessionDuration { get; set; }
    #endregion

    #region Security Metrics
    public int UsersWithTwoFactorEnabled { get; set; }
    public int FailedLoginAttempts { get; set; }
    public int LockedAccountsToday { get; set; }
    public int SuspiciousActivities { get; set; }
    public DateTime LastSecurityIncident { get; set; }
    #endregion

    #region Chart Data
    public List<ChartDataVM> RegistrationTrend { get; set; } = new();
    public List<ChartDataVM> ActivityTrend { get; set; } = new();
    public List<ChartDataVM> EngagementTrend { get; set; } = new();
    #endregion

    #region Breakdown Data
    public Dictionary<string, int> UsersByRole { get; set; } = new();
    public Dictionary<string, int> UsersByStatus { get; set; } = new();
    public Dictionary<string, int> UsersByCountry { get; set; } = new();
    public Dictionary<string, int> UsersByAgeGroup { get; set; } = new();
    #endregion

    #region Top Lists
    public List<string> TopActiveUsers { get; set; } = new();
    public List<string> TopEngagingUsers { get; set; } = new();
    public List<string> RecentRegistrations { get; set; } = new();
    #endregion

    #region Admin-Only Data (populated only when adminView = true)
    public int PendingApprovals { get; set; }
    public int ReportedUsers { get; set; }
    public int BannedUsers { get; set; }
    public int UsersRequiringAttention { get; set; }
    public List<UserSecurityAlertVM> SecurityAlerts { get; set; } = new();
    #endregion
}

/// <summary>
/// User security alert for admin view
/// </summary>
public class UserSecurityAlertVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string AlertType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsResolved { get; set; }
}