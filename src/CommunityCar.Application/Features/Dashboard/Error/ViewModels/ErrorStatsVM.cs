using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Dashboard.Error.ViewModels;

public class ErrorStatsVM
{
    public int TotalErrors { get; set; }
    public int ErrorsToday { get; set; }
    public int ErrorsThisWeek { get; set; }
    public int ErrorsThisMonth { get; set; }
    public int CriticalErrors { get; set; }
    public int WarningErrors { get; set; }
    public int InfoErrors { get; set; }
    public int ResolvedErrors { get; set; }
    public int UnresolvedErrors { get; set; }
    public double ErrorRate { get; set; }
    public double ResolutionRate { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<ChartDataVM> ErrorTrend { get; set; } = new();
    public Dictionary<string, int> ErrorsByCategory { get; set; } = new();
    public Dictionary<string, int> ErrorsByLevel { get; set; } = new();
}