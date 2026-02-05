namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.developer.ErrorReporting.ViewModels;

public class ErrorSummaryVM
{
    public int TotalErrors { get; set; }
    public int CriticalErrors { get; set; }
    public int HighSeverityErrors { get; set; }
    public int MediumSeverityErrors { get; set; }
    public int LowSeverityErrors { get; set; }
    public int ResolvedErrors { get; set; }
    public int UnresolvedErrors { get; set; }
    public double ErrorRate { get; set; }
    public TimeSpan AverageResolutionTime { get; set; }
    public string MostCommonErrorType { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}




