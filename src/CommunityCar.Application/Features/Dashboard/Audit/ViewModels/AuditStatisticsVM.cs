namespace CommunityCar.Application.Features.Dashboard.Audit.ViewModels;

public class AuditStatisticsVM
{
    public int TotalActions { get; set; }
    public int UniqueUsers { get; set; }
    public int SuccessfulActions { get; set; }
    public int FailedActions { get; set; }
    public string MostActiveUser { get; set; } = string.Empty;
    public string MostCommonAction { get; set; } = string.Empty;
    public string MostAffectedEntityType { get; set; } = string.Empty;
    public int AverageActionsPerDay { get; set; }
    public int PeakActivityHour { get; set; }
    public Dictionary<string, int> TopActions { get; set; } = new();
}