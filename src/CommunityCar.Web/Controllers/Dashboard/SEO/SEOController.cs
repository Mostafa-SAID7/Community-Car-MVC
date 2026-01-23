using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.SEO;
using CommunityCar.Application.Features.SEO.DTOs;

namespace CommunityCar.Web.Controllers.Dashboard.SEO;

[Authorize(Roles = "Admin")]
[Route("dashboard/seo")]
public class SEOController : Controller
{
    private readonly ISEOService _seoService;
    private readonly ILogger<SEOController> _logger;

    public SEOController(ISEOService seoService, ILogger<SEOController> logger)
    {
        _seoService = seoService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var homeAnalysis = await _seoService.AnalyzePageSEOAsync("/");
            return View("~/Views/Dashboard/SEO/Index.cshtml", homeAnalysis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading SEO dashboard");
            return View();
        }
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzePage(string url)
    {
        try
        {
            var analysis = await _seoService.AnalyzePageSEOAsync(url);
            return Json(analysis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing page SEO for URL: {Url}", url);
            return Json(new { error = "Failed to analyze page SEO" });
        }
    }

    [HttpGet("sitemap")]
    public async Task<IActionResult> Sitemap()
    {
        try
        {
            var xml = await _seoService.GenerateSitemapXmlAsync();
            return Content(xml, "application/xml");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sitemap");
            return StatusCode(500);
        }
    }

    [HttpGet("rss")]
    public async Task<IActionResult> RSS(string? feedType = "all")
    {
        try
        {
            var request = new RSSFeedRequest { FeedType = feedType ?? "all" };
            var xml = await _seoService.GenerateRSSXmlAsync(request);
            return Content(xml, "application/rss+xml");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating RSS feed");
            return StatusCode(500);
        }
    }

    [HttpGet("meta-data")]
    public async Task<IActionResult> GetMetaData(string pageType, Guid? entityId = null)
    {
        try
        {
            var metaData = await _seoService.GenerateMetaDataAsync(pageType, entityId);
            return Json(metaData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating meta data for page type: {PageType}", pageType);
            return Json(new { error = "Failed to generate meta data" });
        }
    }

    [HttpGet("structured-data")]
    public async Task<IActionResult> GetStructuredData(string pageType, Guid? entityId = null)
    {
        try
        {
            var structuredData = await _seoService.GenerateStructuredDataAsync(pageType, entityId);
            return Content(structuredData, "application/ld+json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating structured data for page type: {PageType}", pageType);
            return StatusCode(500);
        }
    }
}
