using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Dashboard.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Reports;

[Route("dashboard/reports")]
[Authorize]
public class ReportsController : Controller
{
    private readonly IReportsService _reportsService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(
        IReportsService reportsService,
        ICurrentUserService currentUserService,
        ILogger<ReportsController> logger)
    {
        _reportsService = reportsService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
    {
        try
        {
            var reports = await _reportsService.GetReportsAsync(page, pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            return View(reports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading reports");
            TempData["ErrorMessage"] = "Failed to load reports. Please try again.";
            return View();
        }
    }

    [HttpGet("generate")]
    public IActionResult Generate()
    {
        return View();
    }

    [HttpPost("generate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(ReportGenerationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        try
        {
            var reportId = await _reportsService.GenerateReportAsync(request);
            TempData["SuccessMessage"] = "Report generation started successfully! You will be notified when it's ready.";
            return RedirectToAction(nameof(Details), new { id = reportId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating report");
            ModelState.AddModelError("", "Failed to generate report. Please try again.");
            return View(request);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var report = await _reportsService.GetReportByIdAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading report details for ID: {ReportId}", id);
            TempData["ErrorMessage"] = "Failed to load report details.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        try
        {
            var report = await _reportsService.GetReportByIdAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            if (report.Status != "Completed")
            {
                TempData["ErrorMessage"] = "Report is not ready for download yet.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var fileBytes = await _reportsService.DownloadReportAsync(id);
            if (fileBytes == null)
            {
                TempData["ErrorMessage"] = "Report file not found.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var fileName = $"{report.Title}_{DateTime.UtcNow:yyyyMMdd}.{report.Format?.ToLower()}";
            var contentType = report.Format?.ToLower() switch
            {
                "pdf" => "application/pdf",
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "csv" => "text/csv",
                _ => "application/octet-stream"
            };

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading report: {ReportId}", id);
            TempData["ErrorMessage"] = "Failed to download report.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    [HttpPost("{id}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var success = await _reportsService.DeleteReportAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Report deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete report.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting report: {ReportId}", id);
            TempData["ErrorMessage"] = "Failed to delete report.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet("api")]
    public async Task<IActionResult> GetReports(int page = 1, int pageSize = 20)
    {
        try
        {
            var reports = await _reportsService.GetReportsAsync(page, pageSize);
            return Json(new { success = true, data = reports, page, pageSize });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading reports API");
            return Json(new { success = false, message = "Failed to load reports" });
        }
    }
}