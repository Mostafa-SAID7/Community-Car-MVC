namespace CommunityCar.Application.Features.Dashboard.Reports.Users.General;

/// <summary>
/// User activity report view model
/// </summary>
public class UserActivityReportVM
{
    public DateTime ReportDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalActiveUsers { get; set; }
    public int DailyActiveUsers { get; set; }
    public int WeeklyActiveUsers { get; set; }
    public int MonthlyActiveUsers { get; set; }
    public double AverageSessionDuration { get; set; }
    public int TotalSessions { get; set; }
    public int TotalPageViews { get; set; }
    public Dictionary<string, int> ActivityByHour { get; set; } = new();
    public Dictionary<string, int> ActivityByDay { get; set; } = new();
    public List<string> TopActivities { get; set; } = new();
}