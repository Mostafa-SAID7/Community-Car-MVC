using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Features.Dashboard.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Reports;

[Route("dashboard/reports")]
[Authorize(Roles = "Admin,Moderator")]
public class ReportsController : Controller
{
    private readonly IDashboardReportsService _reportsService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(
        IDashboardReportsService reportsService,
        ILogger<ReportsController> logger)
    {
        _reportsService = reportsService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
    {
        try
        {
            var reports = await _reportsService.GetReportsAsync(page, pageSize);
            return View("~/Views/Dashboard/Reports/Index.cshtml", reports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading reports");
            TempData["ErrorMessage"] = "Failed to load reports. Please try again.";
            return View("~/Views/Dashboard/Reports/Index.cshtml", new List<object>());
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var report = await _reportsService.GetReportByIdAsync(id);
            if (report == null)
            {
                TempData["ErrorMessage"] = "Report not found.";
                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Dashboard/Reports/Details.cshtml", report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading report details for {ReportId}", id);
            TempData["ErrorMessage"] = "Failed to load report details.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] ReportGenerationRequest request)
    {
        try
        {
            var success = await _reportsService.GenerateReportAsync(request);
            if (success)
            {
                return Json(new { success = true, message = "Report generation started successfully" });
            }

            return Json(new { success = false, message = "Failed to start report generation" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating report");
            return Json(new { success = false, message = "An error occurred while generating the report" });
        }
    }

    [HttpGet("download/{id:guid}")]
    public async Task<IActionResult> Download(Guid id)
    {
        try
        {
            var report = await _reportsService.GetReportByIdAsync(id);
            if (report == null || report.Status != "Completed")
            {
                TempData["ErrorMessage"] = "Report not found or not ready for download.";
                return RedirectToAction(nameof(Index));
            }

            var fileBytes = await _reportsService.DownloadReportAsync(id);
            var fileName = $"{report.Name}.pdf";
            
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading report {ReportId}", id);
            TempData["ErrorMessage"] = "Failed to download report.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var success = await _reportsService.DeleteReportAsync(id);
            if (success)
            {
                return Json(new { success = true, message = "Report deleted successfully" });
            }

            return Json(new { success = false, message = "Failed to delete report" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting report {ReportId}", id);
            return Json(new { success = false, message = "An error occurred while deleting the report" });
        }
    }

    [HttpPost("schedule")]
    public async Task<IActionResult> Schedule([FromBody] ReportScheduleRequest request)
    {
        try
        {
            var success = await _reportsService.ScheduleReportAsync(request);
            if (success)
            {
                return Json(new { success = true, message = "Report scheduled successfully" });
            }

            return Json(new { success = false, message = "Failed to schedule report" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling report");
            return Json(new { success = false, message = "An error occurred while scheduling the report" });
        }
    }

    [HttpGet("scheduled")]
    public async Task<IActionResult> Scheduled()
    {
        try
        {
            var scheduledReports = await _reportsService.GetScheduledReportsAsync();
            return Json(new { success = true, data = scheduledReports });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading scheduled reports");
            return Json(new { success = false, message = "Failed to load scheduled reports" });
        }
    }
}