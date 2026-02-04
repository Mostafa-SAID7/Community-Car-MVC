namespace CommunityCar.Application.Features.Dashboard.SEO.ViewModels;

public class SEOMetricsVM
{
    public int PageLoadTime { get; set; }
    public int TitleLength { get; set; }
    public int DescriptionLength { get; set; }
    public int HeadingCount { get; set; }
    public int ImageCount { get; set; }
    public int LinkCount { get; set; }
    public int WordCount { get; set; }
    public double KeywordDensity { get; set; }
    public bool HasMetaDescription { get; set; }
    public bool HasMetaKeywords { get; set; }
    public bool HasCanonical { get; set; }
    public bool HasOgTags { get; set; }
    public bool HasTwitterCards { get; set; }
}