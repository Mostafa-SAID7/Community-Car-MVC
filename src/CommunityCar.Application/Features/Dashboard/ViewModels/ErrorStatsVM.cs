namespace CommunityCar.Application.Features.Dashboard.ViewModels;

// Error Management ViewModels (converted from DTOs)
public class ErrorStatsVM
{
    public DateTime Date { get; set; }
    public int TotalErrors { get; set; }
    public int CriticalErrors { get; set; }
    public int WarningErrors { get; set; }
    public int InfoErrors { get; set; }
    public int ResolvedErrors { get; set; }
    public int UnresolvedErrors { get; set; }
    public string? MostCommonError { get; set; }
    public int MostCommonErrorCount { get; set; }
    public Dictionary<string, int> ErrorsByCategory { get; set; } = new();
    public Dictionary<string, int> ErrorsBySeverity { get; set; } = new();
}