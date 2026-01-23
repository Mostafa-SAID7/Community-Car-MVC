using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.SEO;
using CommunityCar.Application.Features.SEO.DTOs;

namespace CommunityCar.Web.Controllers.Dashboard.Performance;

[Authorize(Roles = "Admin")]
[Route("dashboard/performance")]
public class PerformanceController : Controller
{
    private readonly IPerformanceService _performanceService;
    private readonly ILogger<PerformanceController> _logger;

    public PerformanceController(IPerformanceService performanceService, ILogger<PerformanceController> logger)
    {
        _performanceService = performanceService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var metrics = await _performanceService.GetCoreWebVitalsAsync("/");
            return View(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading performance dashboard");
            return View();
        }
    }

    [HttpPost("vitals")]
    public async Task<IActionResult> GetCoreWebVitals(string url)
    {
        try
        {
            var metrics = await _performanceService.GetCoreWebVitalsAsync(url);
            return Json(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Core Web Vitals for URL: {Url}", url);
            return Json(new { error = "Failed to get Core Web Vitals" });
        }
    }

    [HttpPost("report")]
    public async Task<IActionResult> GenerateReport(string url)
    {
        try
        {
            var report = await _performanceService.GeneratePerformanceReportAsync(url);
            return Json(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating performance report for URL: {Url}", url);
            return Json(new { error = "Failed to generate performance report" });
        }
    }

    [HttpPost("optimize-image")]
    public async Task<IActionResult> OptimizeImage(IFormFile image, [FromForm] ImageOptimizationOptions options)
    {
        try
        {
            if (image == null || image.Length == 0)
            {
                return Json(new { error = "No image file provided" });
            }

            // Save uploaded file temporarily
            var tempPath = Path.GetTempFileName();
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            var result = await _performanceService.OptimizeImageAsync(tempPath, options);

            // Clean up temp file
            if (System.IO.File.Exists(tempPath))
            {
                System.IO.File.Delete(tempPath);
            }

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing image");
            return Json(new { error = "Failed to optimize image" });
        }
    }

    [HttpGet("resources/critical")]
    public async Task<IActionResult> GetCriticalResources(string url)
    {
        try
        {
            var resources = await _performanceService.GetCriticalResourcesAsync(url);
            return Json(resources);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting critical resources for URL: {Url}", url);
            return Json(new { error = "Failed to get critical resources" });
        }
    }

    [HttpGet("resources/blocking")]
    public async Task<IActionResult> GetRenderBlockingResources(string url)
    {
        try
        {
            var resources = await _performanceService.GetRenderBlockingResourcesAsync(url);
            return Json(resources);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting render blocking resources for URL: {Url}", url);
            return Json(new { error = "Failed to get render blocking resources" });
        }
    }

    [HttpGet("resources/analyze")]
    public async Task<IActionResult> AnalyzeResources(string url)
    {
        try
        {
            var analysis = await _performanceService.AnalyzeResourcesAsync(url);
            return Json(analysis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing resources for URL: {Url}", url);
            return Json(new { error = "Failed to analyze resources" });
        }
    }
}
