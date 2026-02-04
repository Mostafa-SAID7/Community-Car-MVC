namespace CommunityCar.Application.Features.SEO.ViewModels;

public class PerformanceReportVM
{
    public string Url { get; set; } = string.Empty;
    public int OverallScore { get; set; }
    public CoreWebVitalsVM CoreWebVitals { get; set; } = new();
    public ResourceAnalysisVM Resources { get; set; } = new();
    public List<PerformanceIssueVM> Issues { get; set; } = new();
    public List<PerformanceRecommendationVM> Recommendations { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
    
    // Legacy support
    public DateTime RunAt { get; set; }
    public PerformanceMetricsVM Metrics { get; set; } = new();
    public List<string> OptimizationTips { get; set; } = new();
}