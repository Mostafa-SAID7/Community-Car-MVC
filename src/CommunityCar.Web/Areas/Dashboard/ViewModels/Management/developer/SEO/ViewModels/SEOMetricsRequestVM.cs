namespace CommunityCar.Application.Features.SEO.ViewModels;

public class SEOMetricsRequestVM
{
    public string Url { get; set; } = string.Empty;
    public bool IncludePerformance { get; set; } = true;
    public bool IncludeSEO { get; set; } = true;
    public bool IncludeAccessibility { get; set; } = false;
    public string Device { get; set; } = "desktop"; // "desktop" or "mobile"
}