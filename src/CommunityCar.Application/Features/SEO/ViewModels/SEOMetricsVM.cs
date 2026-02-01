namespace CommunityCar.Application.Features.SEO.ViewModels;

public class SEOMetricsVM
{
    public int TitleLength { get; set; }
    public int DescriptionLength { get; set; }
    public int H1Count { get; set; }
    public int H2Count { get; set; }
    public int ImageCount { get; set; }
    public int ImagesWithoutAlt { get; set; }
    public int InternalLinks { get; set; }
    public int ExternalLinks { get; set; }
    public int WordCount { get; set; }
    public decimal ReadabilityScore { get; set; }
    public bool HasCanonical { get; set; }
    public bool HasMetaDescription { get; set; }
    public bool HasOgTags { get; set; }
    public bool HasTwitterCards { get; set; }
    public bool HasStructuredData { get; set; }
}