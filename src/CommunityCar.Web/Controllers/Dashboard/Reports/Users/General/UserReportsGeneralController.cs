using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Reports.Users.General;
using CommunityCar.Application.Features.Dashboard.Reports.Users.General;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Reports.Users.General;

[Route("{culture=en-US}/dashboard/reports/users/general")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class UserReportsGeneralController : Controller
{
    private readonly IUserReportsService _userReportsService;
    private readonly ILogger<UserReportsGeneralController> _logger;

    public UserReportsGeneralController(
        IUserReportsService userReportsService,
        ILogger<UserReportsGeneralController> logger)
    {
        _userReportsService = userReportsService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var reports = await _userReportsService.GetUserReportsAsync(startDate, endDate);
            
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            
            return View(reports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user reports");
            TempData["ErrorMessage"] = "Failed to load user reports. Please try again.";
            return View(new List<UserReportVM>());
        }
    }

    [HttpGet("{reportId:int}")]
    public async Task<IActionResult> Details(int reportId)
    {
        try
        {
            var report = await _userReportsService.GetUserReportByIdAsync(reportId);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user report details for ID: {ReportId}", reportId);
            return NotFound();
        }
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetReportSummary(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var summary = await _userReportsService.GetReportSummaryAsync(startDate, endDate);
            return Json(new { success = true, data = summary });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading report summary");
            return Json(new { success = false, message = "Failed to load report summary" });
        }
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateReport([FromBody] UserReportVM report)
    {
        try
        {
            var reportId = await _userReportsService.CreateUserReportAsync(report);
            return Json(new { success = true, reportId, message = "Report created successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user report");
            return Json(new { success = false, message = "Failed to create report" });
        }
    }

    [HttpGet("export/{reportId:int}")]
    public async Task<IActionResult> ExportReport(int reportId, string format = "pdf")
    {
        try
        {
            var fileData = await _userReportsService.ExportUserReportAsync(reportId, format);
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

            var fileName = $"user_report_{reportId}_{DateTime.UtcNow:yyyyMMdd}.{format}";
            return File(fileData, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting user report: {ReportId}", reportId);
            return NotFound();
        }
    }

    [HttpDelete("{reportId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteReport(int reportId)
    {
        try
        {
            var result = await _userReportsService.DeleteUserReportAsync(reportId);
            if (result)
            {
                return Json(new { success = true, message = "Report deleted successfully" });
            }
            return Json(new { success = false, message = "Failed to delete report" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user report: {ReportId}", reportId);
            return Json(new { success = false, message = "Failed to delete report" });
        }
    }
}