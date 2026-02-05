namespace CommunityCar.Application.Features.Dashboard.Reports.Users.General;

/// <summary>
/// General user report view model
/// </summary>
public class UserReportVM
{
    public Guid ReportId { get; set; }
    public string ReportName { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public UserActivityReportVM ActivityReport { get; set; } = new();
    public UserEngagementReportVM EngagementReport { get; set; } = new();
    public List<UserRetentionReportVM> RetentionReports { get; set; } = new();
    public Dictionary<string, object> Summary { get; set; } = new();
}