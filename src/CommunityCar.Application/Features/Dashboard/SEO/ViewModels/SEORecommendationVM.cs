namespace CommunityCar.Application.Features.Dashboard.SEO.ViewModels;

public class SEORecommendationVM
{
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public int Impact { get; set; }
    public string ActionRequired { get; set; } = string.Empty;
}