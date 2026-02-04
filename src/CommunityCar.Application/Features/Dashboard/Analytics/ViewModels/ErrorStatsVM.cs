using System.ComponentModel.DataAnnotations;

using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Dashboard.Analytics.ViewModels;

public class ErrorStatsVM
{
    public int TotalErrors { get; set; }
    public int CriticalErrors { get; set; }
    public int WarningErrors { get; set; }
    public int InfoErrors { get; set; }
    public int ResolvedErrors { get; set; }
    public int UnresolvedErrors { get; set; }
    public double ErrorRate { get; set; }
    public double ResolutionRate { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<ChartDataVM> ErrorTrendData { get; set; } = new();
    public List<ChartDataVM> ErrorTypeDistribution { get; set; } = new();
    public List<string> TopErrorMessages { get; set; } = new();
    public Dictionary<string, int> ErrorsByCategory { get; set; } = new();
}