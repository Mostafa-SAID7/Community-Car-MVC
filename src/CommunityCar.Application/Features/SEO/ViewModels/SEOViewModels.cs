namespace CommunityCar.Application.Features.SEO.ViewModels;

public class SEOMetaDataVM
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public string CanonicalUrl { get; set; } = string.Empty;
    public string OgTitle { get; set; } = string.Empty;
    public string OgDescription { get; set; } = string.Empty;
    public string OgType { get; set; } = string.Empty;
    public string TwitterCard { get; set; } = string.Empty;
    public string SiteName { get; set; } = string.Empty;
    public Dictionary<string, string> MetaTags { get; set; } = new();
    public Dictionary<string, string> OpenGraphTags { get; set; } = new();
}

public class SitemapVM
{
    public DateTime GeneratedAt { get; set; }
    public List<SitemapUrlVM> Urls { get; set; } = new();
    public int TotalUrls { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime LastModified { get; set; }
    public string ChangeFrequency { get; set; } = "monthly";
    public double Priority { get; set; } = 0.5;
}

public class SitemapUrlVM
{
    public string Url { get; set; } = string.Empty;
    public DateTime LastModified { get; set; }
    public string ChangeFrequency { get; set; } = string.Empty;
    public decimal Priority { get; set; }
}

public class RSSFeedVM
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public DateTime LastBuildDate { get; set; }
    public List<RSSItemVM> Items { get; set; } = new();
}

public class RSSItemVM
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Guid { get; set; } = string.Empty;
}

public class SEOAnalysisVM
{
    public string Url { get; set; } = string.Empty;
    public int Score { get; set; }
    public DateTime AnalyzedAt { get; set; }
    public SEOMetricsVM Metrics { get; set; } = new();
    public List<SEOIssueVM> Issues { get; set; } = new();
    public List<SEORecommendationVM> Recommendations { get; set; } = new();
}

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

public class SEOIssueVM
{
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Element { get; set; } = string.Empty;
    public string Fix { get; set; } = string.Empty;
}

public class SEORecommendationVM
{
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Impact { get; set; } = string.Empty;
}

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

public class CoreWebVitalsVM
{
    public decimal LCP { get; set; }
    public decimal FID { get; set; }
    public decimal CLS { get; set; }
    public decimal FCP { get; set; }
    public decimal TTI { get; set; }
    public decimal TBT { get; set; }
    public decimal SI { get; set; }
    public string LCPGrade { get; set; } = string.Empty;
    public string FIDGrade { get; set; } = string.Empty;
    public string CLSGrade { get; set; } = string.Empty;
}

public class PerformanceIssueVM
{
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public decimal Impact { get; set; }
    public string Fix { get; set; } = string.Empty;
}

public class PerformanceRecommendationVM
{
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PotentialSavings { get; set; }
    public string Priority { get; set; } = string.Empty;
}

public class ResourceAnalysisVM
{
    public int TotalRequests { get; set; }
    public long TotalSize { get; set; }
    public int JavaScriptFiles { get; set; }
    public long JavaScriptSize { get; set; }
    public int CSSFiles { get; set; }
    public long CSSSize { get; set; }
    public int ImageFiles { get; set; }
    public long ImageSize { get; set; }
    public int FontFiles { get; set; }
    public long FontSize { get; set; }
    public List<string> RenderBlockingResources { get; set; } = new();
    public List<string> UnusedResources { get; set; } = new();
}

public class ImageOptimizationVM
{
    public string OriginalPath { get; set; } = string.Empty;
    public string OptimizedPath { get; set; } = string.Empty;
    public long OriginalSize { get; set; }
    public long OptimizedSize { get; set; }
    public decimal CompressionRatio { get; set; }
    public string Format { get; set; } = string.Empty;
    public int? Width { get; set; }
    public int? Height { get; set; }
    public bool HasWebPVersion { get; set; }
    public bool HasAvifVersion { get; set; }
    public List<string> GeneratedSizes { get; set; } = new();
    public double SavingsPercentage { get; set; }
}

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