using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Monitoring;

[Route("dashboard/monitoring")]
public class MonitoringController : Controller
{
    private readonly IMonitoringService _monitoringService;
    private readonly ILogger<MonitoringController> _logger;

    public MonitoringController(
        IMonitoringService monitoringService,
        ILogger<MonitoringController> logger)
    {
        _monitoringService = monitoringService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var systemHealth = await _monitoringService.GetSystemHealthAsync();
            var isHealthy = await _monitoringService.IsSystemHealthyAsync();

            ViewBag.IsSystemHealthy = isHealthy;
            return View("~/Views/Dashboard/Monitoring/Index.cshtml", systemHealth);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading system monitoring");
            TempData["ErrorMessage"] = "Failed to load system monitoring data. Please try again.";
            return View("~/Views/Dashboard/Monitoring/Index.cshtml");
        }
    }

    [HttpGet("service/{serviceName}")]
    public async Task<IActionResult> ServiceDetails(string serviceName)
    {
        try
        {
            var serviceHealth = await _monitoringService.GetServiceHealthAsync(serviceName);
            if (serviceHealth == null)
            {
                return NotFound();
            }

            // Get performance chart for the last 24 hours
            var startDate = DateTime.UtcNow.AddHours(-24);
            var endDate = DateTime.UtcNow;
            var performanceChart = await _monitoringService.GetPerformanceChartAsync(serviceName, startDate, endDate);

            ViewBag.PerformanceChart = performanceChart;
            ViewBag.ServiceName = serviceName;

            return View(serviceHealth);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading service details for: {ServiceName}", serviceName);
            TempData["ErrorMessage"] = "Failed to load service details.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet("api/health")]
    public async Task<IActionResult> GetSystemHealth()
    {
        try
        {
            var systemHealth = await _monitoringService.GetSystemHealthAsync();
            var isHealthy = await _monitoringService.IsSystemHealthyAsync();

            return Json(new
            {
                success = true,
                data = systemHealth,
                isHealthy = isHealthy,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading system health API");
            return Json(new { success = false, message = "Failed to load system health" });
        }
    }

    [HttpGet("api/service/{serviceName}")]
    public async Task<IActionResult> GetServiceHealth(string serviceName)
    {
        try
        {
            var serviceHealth = await _monitoringService.GetServiceHealthAsync(serviceName);
            if (serviceHealth == null)
            {
                return Json(new { success = false, message = "Service not found" });
            }

            return Json(new { success = true, data = serviceHealth });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading service health API for: {ServiceName}", serviceName);
            return Json(new { success = false, message = "Failed to load service health" });
        }
    }

    [HttpGet("api/performance/{serviceName}")]
    public async Task<IActionResult> GetPerformanceChart(string serviceName, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddHours(-24);
            var end = endDate ?? DateTime.UtcNow;

            var performanceData = await _monitoringService.GetPerformanceChartAsync(serviceName, start, end);
            return Json(new { success = true, data = performanceData });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading performance chart for: {ServiceName}", serviceName);
            return Json(new { success = false, message = "Failed to load performance data" });
        }
    }

    [HttpPost("api/health/{serviceName}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateServiceHealth(string serviceName, [FromBody] UpdateHealthRequest request)
    {
        try
        {
            var success = await _monitoringService.UpdateSystemHealthAsync(
                serviceName,
                request.Status,
                request.ResponseTime,
                request.CpuUsage,
                request.MemoryUsage,
                request.DiskUsage,
                request.ActiveConnections,
                request.ErrorCount);

            if (success)
            {
                return Json(new { success = true, message = "Health status updated successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to update health status" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating service health for: {ServiceName}", serviceName);
            return Json(new { success = false, message = "Failed to update health status" });
        }
    }
}

public class UpdateHealthRequest
{
    public string Status { get; set; } = string.Empty;
    public double ResponseTime { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public int ActiveConnections { get; set; }
    public int ErrorCount { get; set; }
}