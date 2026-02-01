namespace CommunityCar.Application.Features.SEO.ViewModels;

public class PerformanceMetricsVM
{
    public string Url { get; set; } = string.Empty;
    public DateTime MeasuredAt { get; set; }
    public int OverallScore { get; set; }
    public CoreWebVitalsVM CoreWebVitals { get; set; } = new();
    public List<PerformanceIssueVM> Issues { get; set; } = new();
    public List<PerformanceRecommendationVM> Recommendations { get; set; } = new();
    
    // Legacy support
    public double LCP { get; set; }
    public double FID { get; set; }
    public double CLS { get; set; }
    public double FCP { get; set; }
    public double TTFB { get; set; }
}