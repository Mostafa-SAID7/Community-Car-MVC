namespace CommunityCar.Application.Features.Dashboard.SEO.ViewModels;

public class SEOAnalysisVM
{
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int TitleLength { get; set; }
    public string MetaDescription { get; set; } = string.Empty;
    public int MetaDescriptionLength { get; set; }
    public bool HasH1Tag { get; set; }
    public int H1Count { get; set; }
    public string H1Text { get; set; } = string.Empty;
    public bool HasMetaKeywords { get; set; }
    public string MetaKeywords { get; set; } = string.Empty;
    public bool HasCanonicalUrl { get; set; }
    public string CanonicalUrl { get; set; } = string.Empty;
    public bool HasOpenGraphTags { get; set; }
    public bool HasTwitterCardTags { get; set; }
    public bool HasStructuredData { get; set; }
    public int ImageCount { get; set; }
    public int ImagesWithoutAlt { get; set; }
    public int InternalLinksCount { get; set; }
    public int ExternalLinksCount { get; set; }
    public double PageLoadTime { get; set; }
    public long PageSize { get; set; }
    public bool IsMobileFriendly { get; set; }
    public bool HasSitemap { get; set; }
    public bool HasRobotsTxt { get; set; }
    public bool SslEnabled { get; set; }
    public List<string> Keywords { get; set; } = new();
    public int OverallScore { get; set; }
    public List<SEOIssueVM> Issues { get; set; } = new();
    public List<SEORecommendationVM> Recommendations { get; set; } = new();
    public DateTime AnalyzedAt { get; set; }
    public SEOMetricsVM Metrics { get; set; } = new();
}

public class SEOKeywordVM
{
    public string Keyword { get; set; } = string.Empty;
    public int Ranking { get; set; }
    public int Position { get; set; }
    public int PreviousPosition { get; set; }
    public int SearchVolume { get; set; }
    public decimal Difficulty { get; set; }
    public decimal Cpc { get; set; }
    public decimal Competition { get; set; }
    public string Trend { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime LastChecked { get; set; }
    public DateTime LastUpdated { get; set; }
    public int ChangeFromLastWeek { get; set; }
}

public class SEOReportVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public int OverallScore { get; set; }
    public SEOAnalysisVM Analysis { get; set; } = new();
    public List<SEOKeywordVM> Keywords { get; set; } = new();
    public List<SEOKeywordVM> TopKeywords { get; set; } = new();
    public List<SEOKeywordVM> ImprovingKeywords { get; set; } = new();
    public List<SEOKeywordVM> DecliningKeywords { get; set; } = new();
    public List<SEORecommendationVM> Recommendations { get; set; } = new();
    public List<CompetitorAnalysisVM> CompetitorAnalysis { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public List<SEOAnalysisVM> Analyses { get; set; } = new();
    public SEOSummaryVM Summary { get; set; } = new();
}

public class SEOSettingsVM
{
    public string DefaultTitle { get; set; } = string.Empty;
    public string DefaultDescription { get; set; } = string.Empty;
    public string DefaultMetaDescription { get; set; } = string.Empty;
    public List<string> DefaultKeywords { get; set; } = new();
    public string DefaultMetaKeywords { get; set; } = string.Empty;
    public string SiteName { get; set; } = string.Empty;
    public string SiteUrl { get; set; } = string.Empty;
    public string DefaultOgImage { get; set; } = string.Empty;
    public string TwitterHandle { get; set; } = string.Empty;
    public bool EnableSitemap { get; set; }
    public string SitemapUrl { get; set; } = string.Empty;
    public bool EnableRobotsTxt { get; set; }
    public string RobotsTxtContent { get; set; } = string.Empty;
    public bool EnableCanonicalUrls { get; set; }
    public bool EnableOpenGraph { get; set; }
    public bool EnableTwitterCards { get; set; }
    public bool EnableStructuredData { get; set; }
    public string DefaultLanguage { get; set; } = string.Empty;
    public bool EnableHreflang { get; set; }
    public bool EnableAmpPages { get; set; }
    public Dictionary<string, string> TrackingCodes { get; set; } = new();
    public string GoogleAnalyticsId { get; set; } = string.Empty;
    public string GoogleSearchConsoleId { get; set; } = string.Empty;
    public string BingWebmasterToolsId { get; set; } = string.Empty;
}

public class SEOIssueVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Impact { get; set; } = string.Empty;
    public bool IsFixed { get; set; }
    public DateTime? FixedAt { get; set; }
    public DateTime DetectedAt { get; set; }
}

public class SEORecommendationVM
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int ImpactScore { get; set; }
    public int EffortScore { get; set; }
    public string EstimatedImpact { get; set; } = string.Empty;
    public string Effort { get; set; } = string.Empty;
}

public class CompetitorAnalysisVM
{
    public string CompetitorName { get; set; } = string.Empty;
    public string CompetitorUrl { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public int SEOScore { get; set; }
    public int OrganicTraffic { get; set; }
    public int KeywordCount { get; set; }
    public int DomainAuthority { get; set; }
    public int PageAuthority { get; set; }
    public int BacklinkCount { get; set; }
    public string TrafficTrend { get; set; } = string.Empty;
    public List<string> TopKeywords { get; set; } = new();
    public DateTime LastAnalyzed { get; set; }
}

public class SEOMetricsVM
{
    public int PageSpeed { get; set; }
    public int MobileUsability { get; set; }
    public int CoreWebVitals { get; set; }
    public int BacklinkCount { get; set; }
    public int DomainAuthority { get; set; }
    public int PageAuthority { get; set; }
    public int IndexedPages { get; set; }
}

public class SEOSummaryVM
{
    public int TotalPages { get; set; }
    public int OptimizedPages { get; set; }
    public int IssuesFound { get; set; }
    public int CriticalIssues { get; set; }
    public decimal AverageScore { get; set; }
    public int TotalKeywords { get; set; }
    public int RankingKeywords { get; set; }
}

public class SitemapGenerationVM
{
    public List<string> IncludedUrls { get; set; } = new();
    public List<string> ExcludedUrls { get; set; } = new();
    public string ChangeFrequency { get; set; } = string.Empty;
    public decimal Priority { get; set; }
    public DateTime LastModified { get; set; }
}

public class RSSItemVM
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public string Author { get; set; } = string.Empty;
}