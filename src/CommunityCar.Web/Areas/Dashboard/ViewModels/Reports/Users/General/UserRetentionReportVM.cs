namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.General;

/// <summary>
/// User retention report view model
/// </summary>
public class UserRetentionReportVM
{
    public DateTime ReportDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double RetentionRate { get; set; }
    public double ChurnRate { get; set; }
    public Dictionary<string, double> RetentionByPeriod { get; set; } = new(); // 1-day, 7-day, 30-day
    public Dictionary<string, double> RetentionByCohort { get; set; } = new();
    public List<string> RetentionFactors { get; set; } = new();
    public List<string> ChurnReasons { get; set; } = new();
}




