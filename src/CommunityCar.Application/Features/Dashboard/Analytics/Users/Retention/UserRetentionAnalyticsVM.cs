namespace CommunityCar.Application.Features.Dashboard.Analytics.Users.Retention;

/// <summary>
/// User retention analytics view model
/// </summary>
public class UserRetentionAnalyticsVM
{
    public string Period { get; set; } = string.Empty; // Daily, Weekly, Monthly
    public double RetentionRate { get; set; }
    public int CohortSize { get; set; }
    public int RetainedUsers { get; set; }
    public int ChurnedUsers { get; set; }
    public double ChurnRate { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public List<RetentionCohortVM> Cohorts { get; set; } = new();
    public Dictionary<string, double> RetentionBySegment { get; set; } = new();
}