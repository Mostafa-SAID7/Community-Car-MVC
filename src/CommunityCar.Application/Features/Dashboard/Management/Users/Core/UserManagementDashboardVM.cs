using CommunityCar.Application.Features.Dashboard.Management.Users.Security;
using CommunityCar.Application.Features.Dashboard.Reports.Users.Security;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Dashboard.Management.Users.Core;

/// <summary>
/// Admin user management dashboard view model
/// </summary>
public class UserManagementDashboardVM
{
    public AdminUserStatsVM UserStats { get; set; } = new();
    public SecurityStatsVM SecurityStats { get; set; } = new();
    public UserManagementSummaryVM Summary { get; set; } = new();
    public List<QuickActionVM> QuickActions { get; set; } = new();
    public List<RecentUserActivityVM> RecentUserActivity { get; set; } = new();
    public List<UserSecurityLogVM> RecentSecurityLogs { get; set; } = new();
    public List<ChartDataVM> UserGrowthChart { get; set; } = new();
    public List<ChartDataVM> LoginActivityChart { get; set; } = new();
}

/// <summary>
/// User management summary view model
/// </summary>
public class UserManagementSummaryVM
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsersToday { get; set; }
    public int PendingApprovals { get; set; }
    public int SecurityAlerts { get; set; }
    public int LockedAccounts { get; set; }
    public decimal UserGrowthRate { get; set; }
    public decimal UserRetentionRate { get; set; }
    public string TrendDirection { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public DateTime LastUserRegistration { get; set; }
    public DateTime LastSecurityIncident { get; set; }
    public int ActiveSessions { get; set; }
    public int FailedLoginAttempts { get; set; }
    public int BlockedUsers { get; set; }
}

/// <summary>
/// Quick action view model
/// </summary>
public class QuickActionVM
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Count { get; set; }
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Recent user activity view model
/// </summary>
public class RecentUserActivityVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime LastActivity { get; set; }
    public string LastLocation { get; set; } = string.Empty;
    public bool IsCurrentlyOnline { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}