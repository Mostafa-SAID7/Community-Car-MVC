namespace CommunityCar.Application.Features.Dashboard.Error.ViewModels;

public class ErrorStatsVM
{
    public int TotalErrors { get; set; }
    public int ResolvedErrors { get; set; }
    public int UnresolvedErrors { get; set; }
    public int CriticalErrors { get; set; }
    public int WarningErrors { get; set; }
    public int InfoErrors { get; set; }
    public double ErrorRate { get; set; }
    public List<ChartDataVM> ErrorTrendChart { get; set; } = new();
    public List<ChartDataVM> ErrorTypeChart { get; set; } = new();
    public List<ChartDataVM> ErrorSeverityChart { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}