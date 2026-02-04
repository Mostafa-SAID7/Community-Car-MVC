using CommunityCar.Application.Features.Dashboard.SEO.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.SEO;

public interface ISEOService
{
    Task<SEOMetaDataVM> GenerateMetaDataAsync(string pageType, Guid? entityId = null);
    Task<string> GenerateStructuredDataAsync(string pageType, Guid? entityId = null);
    Task<SitemapVM> GenerateSitemapAsync(SitemapGenerationVM? request = null);
    Task<RSSFeedVM> GenerateRSSFeedAsync(RSSFeedVM? request = null);
    Task<SEOAnalysisVM> AnalyzePageSEOAsync(string url);
    Task UpdateSEOMetricsAsync(string url, SEOMetricsVM metrics);
    Task<string> GenerateSitemapXmlAsync(SitemapGenerationVM? request = null);
    Task<string> GenerateRSSXmlAsync(RSSFeedVM? request = null);
}