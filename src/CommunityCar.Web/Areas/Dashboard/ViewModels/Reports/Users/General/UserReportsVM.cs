using CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.Activity;
using CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.Security;

namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.General;

/// <summary>
/// ViewModel for comprehensive user reports
/// </summary>
public class UserReportsVM
{
    public UserStatisticsReportVM Statistics { get; set; } = new();
    public UserActivityReportVM Activity { get; set; } = new();
    public UserSecurityReportVM Security { get; set; } = new();
    public UserEngagementReportVM Engagement { get; set; } = new();
    public UserDemographicsReportVM Demographics { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
    public string ReportPeriod { get; set; } = string.Empty;
    public string GeneratedBy { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for user statistics report
/// </summary>
public class UserStatisticsReportVM
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int NewUsers { get; set; }
    public int VerifiedUsers { get; set; }
    public int UnverifiedUsers { get; set; }
    public int BannedUsers { get; set; }
    public int SuspendedUsers { get; set; }
    public decimal UserRetentionRate { get; set; }
    public decimal UserChurnRate { get; set; }
    public decimal VerificationRate { get; set; }
    public List<UserRegistrationTrendReportVM> RegistrationTrends { get; set; } = new();
    public List<UserStatusDistributionVM> StatusDistribution { get; set; } = new();
}

/// <summary>
/// ViewModel for user registration trend in reports
/// </summary>
public class UserRegistrationTrendReportVM
{
    public DateTime Date { get; set; }
    public int NewRegistrations { get; set; }
    public int Verifications { get; set; }
    public int Activations { get; set; }
    public string Period { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for user status distribution
/// </summary>
public class UserStatusDistributionVM
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
    public string Color { get; set; } = string.Empty;
}




