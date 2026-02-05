using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Reports;
using CommunityCar.Application.Features.Dashboard.Reports.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Reports;

[Route("{culture=en-US}/dashboard/reports")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class ReportsController : Controller
{
    private readonly IReportsService _reportsService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(
        IReportsService reportsService,
        ILogger<ReportsController> logger)
    {
        _reportsService = reportsService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var reports = await _reportsService.GetReportsAsync();
            return View(reports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading reports");
            TempData["ErrorMessage"] = "Failed to load reports. Please try again.";
            return View(new List<SystemReportVM>());
        }
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View(new CreateReportVM());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateReportVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var reportId = await _reportsService.CreateReportAsync(model);
            TempData["SuccessMessage"] = "Report created successfully.";
            return RedirectToAction(nameof(Details), new { id = reportId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating report");
            ModelState.AddModelError("", "Failed to create report. Please try again.");
            return View(model);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
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
            return NotFound();
        }
    }

    [HttpGet("generate/{id:int}")]
    public async Task<IActionResult> Generate(int id)
    {
        try
        {
            var result = await _reportsService.GenerateReportAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Report generated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to generate report.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating report: {ReportId}", id);
            TempData["ErrorMessage"] = "Failed to generate report. Please try again.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    [HttpGet("download/{id:int}")]
    public async Task<IActionResult> Download(int id, string format = "pdf")
    {
        try
        {
            var fileData = await _reportsService.ExportReportAsync(id, format);
            if (fileData == null || fileData.Length == 0)
            {
                return NotFound();
            }

            var contentType = format.ToLower() switch
            {
                "pdf" => "application/pdf",
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "csv" => "text/csv",
                _ => "application/octet-stream"
            };

            var fileName = $"report_{id}_{DateTime.UtcNow:yyyyMMdd}.{format}";
            return File(fileData, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading report: {ReportId}", id);
            return NotFound();
        }
    }

    [HttpGet("templates")]
    public async Task<IActionResult> GetReportTemplates()
    {
        try
        {
            var templates = await _reportsService.GetReportTemplatesAsync();
            return Json(new { success = true, data = templates });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading report templates");
            return Json(new { success = false, message = "Failed to load report templates" });
        }
    }

    [HttpGet("scheduled")]
    public async Task<IActionResult> GetScheduledReports()
    {
        try
        {
            var scheduled = await _reportsService.GetScheduledReportsAsync();
            return Json(new { success = true, data = scheduled });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading scheduled reports");
            return Json(new { success = false, message = "Failed to load scheduled reports" });
        }
    }
}