using CommunityCar.Application.Common.Interfaces.Services.Dashboard.SEO;
using CommunityCar.Application.Features.Dashboard.SEO.ViewModels;

namespace CommunityCar.Application.Services.SEO;

public class SEOService : ISEOService
{
    public Task<SEOAnalysisVM> AnalyzePageSEOAsync(string url)
    {
        throw new NotImplementedException();
    }

    public Task<SEOMetaDataVM> GenerateMetaDataAsync(string pageType, Guid? entityId = null)
    {
        throw new NotImplementedException();
    }

    public Task<RSSFeedVM> GenerateRSSFeedAsync(RSSFeedVM? request = null)
    {
        throw new NotImplementedException();
    }

    public Task<string> GenerateRSSXmlAsync(RSSFeedVM? request = null)
    {
        throw new NotImplementedException();
    }

    public Task<SitemapVM> GenerateSitemapAsync(SitemapGenerationVM? request = null)
    {
        throw new NotImplementedException();
    }

    public Task<string> GenerateSitemapXmlAsync(SitemapGenerationVM? request = null)
    {
        throw new NotImplementedException();
    }

    public Task<string> GenerateStructuredDataAsync(string pageType, Guid? entityId = null)
    {
        throw new NotImplementedException();
    }

    public Task UpdateSEOMetricsAsync(string url, SEOMetricsVM metrics)
    {
        throw new NotImplementedException();
    }
}