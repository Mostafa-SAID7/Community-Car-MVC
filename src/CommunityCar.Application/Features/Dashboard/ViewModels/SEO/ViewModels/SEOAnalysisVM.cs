namespace CommunityCar.Application.Features.SEO.ViewModels;

public class SEOAnalysisVM
{
    public string Url { get; set; } = string.Empty;
    public int Score { get; set; }
    public DateTime AnalyzedAt { get; set; }
    public SEOMetricsVM Metrics { get; set; } = new();
    public List<SEOIssueVM> Issues { get; set; } = new();
    public List<SEORecommendationVM> Recommendations { get; set; } = new();
}