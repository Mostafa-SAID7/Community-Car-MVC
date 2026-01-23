using CommunityCar.Application.Features.SEO.ViewModels;
using CommunityCar.Application.Features.SEO.DTOs;

namespace CommunityCar.Application.Common.Interfaces.Services.SEO;

public interface ISEOService
{
    Task<SEOMetaDataVM> GenerateMetaDataAsync(string pageType, Guid? entityId = null);
    Task<string> GenerateStructuredDataAsync(string pageType, Guid? entityId = null);
    Task<SitemapVM> GenerateSitemapAsync(SitemapGenerationRequest? request = null);
    Task<RSSFeedVM> GenerateRSSFeedAsync(RSSFeedRequest? request = null);
    Task<SEOAnalysisVM> AnalyzePageSEOAsync(string url);
    Task UpdateSEOMetricsAsync(string url, SEOMetricsRequest metrics);
    Task<string> GenerateSitemapXmlAsync(SitemapGenerationRequest? request = null);
    Task<string> GenerateRSSXmlAsync(RSSFeedRequest? request = null);
}

public interface IPerformanceService
{
    Task<PerformanceMetricsVM> GetCoreWebVitalsAsync(string url);
    Task<ImageOptimizationVM> OptimizeImageAsync(string imagePath, ImageOptimizationOptions options);
    Task<List<string>> GetCriticalResourcesAsync(string url);
    Task<PerformanceReportVM> GeneratePerformanceReportAsync(string url);
    Task UpdatePerformanceMetricsAsync(string url, CoreWebVitalsRequest metrics);
    Task<List<string>> GetRenderBlockingResourcesAsync(string url);
    Task<ResourceAnalysisVM> AnalyzeResourcesAsync(string url);
    Task OptimizeImagesInDirectoryAsync(string directoryPath, ImageOptimizationOptions options);
}