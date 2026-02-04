using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Monitoring;
using CommunityCar.Application.Features.Dashboard.Monitoring.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Monitoring;

[Route("{culture=en-US}/dashboard/monitoring")]
[Authorize(Roles = "Admin,SuperAdmin")]
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
            var systemHealthList = await _monitoringService.GetSystemHealthAsync();
            var isHealthy = await _monitoringService.IsSystemHealthyAsync();

            // If we have multiple health items, we need to create a summary or use the first one
            // For now, let's create a summary SystemHealthVM or use the first available
            SystemHealthVM systemHealth;
            
            if (systemHealthList?.Any() == true)
            {
                // Create a summary SystemHealthVM from all services
                systemHealth = new SystemHealthVM
                {
                    CheckTime = DateTime.UtcNow,
                    ServiceName = "System Overview",
                    Status = isHealthy ? "Healthy" : "Issues Detected",
                    ResponseTime = systemHealthList.Average(x => x.ResponseTime),
                    CpuUsage = systemHealthList.Average(x => x.CpuUsage),
                    MemoryUsage = systemHealthList.Average(x => x.MemoryUsage),
                    DiskUsage = systemHealthList.Average(x => x.DiskUsage),
                    ActiveConnections = systemHealthList.Sum(x => x.ActiveConnections),
                    Uptime = systemHealthList.Average(x => x.Uptime),
                    Version = "System",
                    Environment = systemHealthList.FirstOrDefault()?.Environment ?? "Production",
                    IsHealthy = isHealthy,
                    ErrorCount = systemHealthList.Sum(x => x.ErrorCount),
                    WarningCount = systemHealthList.Sum(x => x.WarningCount),
                    Issues = systemHealthList.SelectMany(x => x.Issues).ToList(),
                    LastCheck = systemHealthList.Any() ? systemHealthList.Max(x => x.LastCheck) : DateTime.UtcNow
                };
            }
            else
            {
                // Create a default health status if no data is available
                systemHealth = new SystemHealthVM
                {
                    CheckTime = DateTime.UtcNow,
                    ServiceName = "System",
                    Status = "Unknown",
                    ResponseTime = 0,
                    CpuUsage = 0,
                    MemoryUsage = 0,
                    DiskUsage = 0,
                    ActiveConnections = 0,
                    Uptime = 0,
                    Version = "1.0.0",
                    Environment = "Production",
                    IsHealthy = false,
                    ErrorCount = 0,
                    WarningCount = 0,
                    Issues = new List<string> { "No health data available" },
                    LastCheck = DateTime.UtcNow
                };
            }

            ViewBag.IsSystemHealthy = isHealthy;
            ViewBag.SystemHealthList = systemHealthList; // Pass the full list for detailed views
            return View("~/Views/Dashboard/Monitoring/Index.cshtml", systemHealth);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading system monitoring");
            TempData["ErrorMessage"] = "Failed to load system monitoring data. Please try again.";
            
            // Return a default model to prevent view errors
            var defaultHealth = new SystemHealthVM
            {
                CheckTime = DateTime.UtcNow,
                ServiceName = "System",
                Status = "Error",
                ResponseTime = 0,
                CpuUsage = 0,
                MemoryUsage = 0,
                DiskUsage = 0,
                ActiveConnections = 0,
                Uptime = 0,
                Version = "1.0.0",
                Environment = "Production",
                IsHealthy = false,
                ErrorCount = 1,
                WarningCount = 0,
                Issues = new List<string> { "Failed to load monitoring data" },
                LastCheck = DateTime.UtcNow
            };
            
            return View("~/Views/Dashboard/Monitoring/Index.cshtml", defaultHealth);
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

    [HttpGet("health")]
    public async Task<IActionResult> GetSystemHealth()
    {
        try
        {
            var systemHealthList = await _monitoringService.GetSystemHealthAsync();
            var isHealthy = await _monitoringService.IsSystemHealthyAsync();

            return Json(new
            {
                success = true,
                data = systemHealthList,
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

    [HttpGet("service-api/{serviceName}")]
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

    [HttpGet("performance-api/{serviceName}")]
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

    [HttpPost("health-api/{serviceName}")]
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


