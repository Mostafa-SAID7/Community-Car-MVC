namespace CommunityCar.Application.Features.Dashboard.Reports.Users.Audit;

/// <summary>
/// Audit statistics for reports
/// </summary>
public class AuditStatisticsVM
{
    public int TotalAuditLogs { get; set; }
    public int TotalActions { get; set; }
    public int UniqueUsers { get; set; }
    public int SuccessfulActions { get; set; }
    public int FailedActions { get; set; }
    public int SecurityEvents { get; set; }
    public int UserActions { get; set; }
    public int SystemActions { get; set; }
    public DateTime LastAuditEntry { get; set; }
    public string MostActiveUser { get; set; } = string.Empty;
    public string MostCommonAction { get; set; } = string.Empty;
    public string MostAffectedEntityType { get; set; } = string.Empty;
    public double AverageActionsPerDay { get; set; }
    public int PeakActivityHour { get; set; }
    public Dictionary<string, int> ActionsByType { get; set; } = new();
    public Dictionary<string, int> ActionsByUser { get; set; } = new();
    public List<string> MostCommonActions { get; set; } = new();
    public List<string> TopActions { get; set; } = new();
}