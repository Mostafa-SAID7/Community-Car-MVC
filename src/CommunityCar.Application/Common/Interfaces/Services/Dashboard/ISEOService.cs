using CommunityCar.Application.Features.SEO.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.SEO;

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

public interface IPerformanceService
{
    Task<PerformanceMetricsVM> GetCoreWebVitalsAsync(string url);
    Task<ImageOptimizationVM> OptimizeImageAsync(string imagePath, ImageOptimizationOptionsVM options);
    Task<List<string>> GetCriticalResourcesAsync(string url);
    Task<PerformanceReportVM> GeneratePerformanceReportAsync(string url);
    Task UpdatePerformanceMetricsAsync(string url, CoreWebVitalsVM metrics);
    Task<List<string>> GetRenderBlockingResourcesAsync(string url);
    Task<ResourceAnalysisVM> AnalyzeResourcesAsync(string url);
    Task OptimizeImagesInDirectoryAsync(string directoryPath, ImageOptimizationOptionsVM options);
}


