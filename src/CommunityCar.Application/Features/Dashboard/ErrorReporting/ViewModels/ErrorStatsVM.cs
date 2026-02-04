namespace CommunityCar.Application.Features.Dashboard.ErrorReporting.ViewModels;

/// <summary>
/// ViewModel for error statistics
/// </summary>
public class ErrorStatsVM
{
    public int TotalErrors { get; set; }
    public int CriticalErrors { get; set; }
    public int WarningErrors { get; set; }
    public int InfoErrors { get; set; }
    public int ResolvedErrors { get; set; }
    public int UnresolvedErrors { get; set; }
    public DateTime LastErrorDate { get; set; }
    public double ErrorRate { get; set; } // Errors per hour/day
    public Dictionary<string, int> ErrorsByCategory { get; set; } = new();
    public Dictionary<string, int> ErrorsBySource { get; set; } = new();
    public List<string> TopErrorMessages { get; set; } = new();
    public int ErrorsToday { get; set; }
    public int ErrorsThisWeek { get; set; }
    public int ErrorsThisMonth { get; set; }
}