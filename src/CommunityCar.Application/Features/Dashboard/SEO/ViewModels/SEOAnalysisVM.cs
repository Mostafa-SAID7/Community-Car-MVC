namespace CommunityCar.Application.Features.Dashboard.SEO.ViewModels;

public class SEOAnalysisVM
{
    public string Url { get; set; } = string.Empty;
    public int Score { get; set; }
    public string Grade { get; set; } = string.Empty;
    public List<SEOIssueVM> Issues { get; set; } = new();
    public List<SEORecommendationVM> Recommendations { get; set; } = new();
    public SEOMetricsVM Metrics { get; set; } = new();
    public DateTime AnalyzedAt { get; set; }
}