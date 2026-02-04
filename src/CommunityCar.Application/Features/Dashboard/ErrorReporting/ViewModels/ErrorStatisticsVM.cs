using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Dashboard.ErrorReporting.ViewModels;

public class ErrorStatisticsVM
{
    public int TotalErrors { get; set; }
    public int ErrorsToday { get; set; }
    public int ErrorsThisWeek { get; set; }
    public int ErrorsThisMonth { get; set; }
    public int CriticalErrors { get; set; }
    public int HighSeverityErrors { get; set; }
    public int MediumSeverityErrors { get; set; }
    public int LowSeverityErrors { get; set; }
    public int WarningErrors { get; set; }
    public int InfoErrors { get; set; }
    public int ResolvedErrors { get; set; }
    public int UnresolvedErrors { get; set; }
    public double ErrorRate { get; set; }
    public double ResolutionRate { get; set; }
    public TimeSpan AverageResolutionTime { get; set; }
    public string MostCommonErrorType { get; set; } = string.Empty;
    public string MostAffectedEndpoint { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public List<ChartDataVM> ErrorTrend { get; set; } = new();
    public List<ChartDataVM> ErrorsByDay { get; set; } = new();
    public Dictionary<string, int> ErrorsByCategory { get; set; } = new();
    public Dictionary<string, int> ErrorsByLevel { get; set; } = new();
    public Dictionary<string, int> ErrorsBySeverity { get; set; } = new();
    public Dictionary<string, int> ErrorsByType { get; set; } = new();
    public List<string> TopErrorSources { get; set; } = new();
    public List<ErrorSummaryVM> TopErrors { get; set; } = new();
}