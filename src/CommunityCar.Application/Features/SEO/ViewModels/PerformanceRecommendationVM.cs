namespace CommunityCar.Application.Features.SEO.ViewModels;

public class PerformanceRecommendationVM
{
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PotentialSavings { get; set; }
    public string Priority { get; set; } = string.Empty;
}