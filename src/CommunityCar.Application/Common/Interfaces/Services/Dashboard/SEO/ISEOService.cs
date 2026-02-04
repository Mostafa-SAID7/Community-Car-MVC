using CommunityCar.Application.Features.Dashboard.SEO.ViewModels;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.SEO;

public interface ISEOService
{
    Task<SEOAnalysisVM> AnalyzeSEOAsync(string url);
    Task<List<SEOKeywordVM>> GetKeywordRankingsAsync(string domain);
    Task<SEOReportVM> GenerateSEOReportAsync(string domain);
    Task<bool> UpdateSEOSettingsAsync(SEOSettingsVM settings);
    Task<SEOSettingsVM> GetSEOSettingsAsync();
    Task<List<SEOIssueVM>> GetSEOIssuesAsync(string? url = null);
    Task<List<CompetitorAnalysisVM>> GetCompetitorAnalysisAsync(string domain);
    Task<bool> SubmitSitemapAsync(string sitemapUrl);
    Task<List<ChartDataVM>> GetSEOMetricsChartAsync(string metricType, int days = 30);
}