using CommunityCar.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard;

[Route("dashboard/error-reports")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class ErrorReportsController : Controller
{
    private readonly IErrorReportingService _errorReportingService;
    private readonly ILogger<ErrorReportsController> _logger;

    public ErrorReportsController(
        IErrorReportingService errorReportingService,
        ILogger<ErrorReportsController> logger)
    {
        _errorReportingService = errorReportingService;
        _logger = logger;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        try
        {
            // In a real app, you would load error reports from the service
            // var reports = await _errorReportingService.GetAllErrorReportsAsync();
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load error reports");
            TempData["ErrorMessage"] = "Failed to load error reports. Please try again.";
            return View();
        }
    }

    [HttpGet("reports-list")]
    public IActionResult GetReports([FromQuery] string? status = null, [FromQuery] string? priority = null)
    {
        try
        {
            // Mock data for demonstration
            var reports = new[]
            {
                new
                {
                    TicketId = "ERR-20260125-ABC123",
                    UserName = "John Doe",
                    UserEmail = "john@example.com",
                    Priority = "High",
                    Status = "New",
                    Description = "Profile page not loading correctly after login",
                    CreatedAt = DateTime.UtcNow.AddHours(-2),
                    ErrorId = "error-123"
                },
                new
                {
                    TicketId = "ERR-20260125-DEF456",
                    UserName = "Jane Smith",
                    UserEmail = "jane@example.com",
                    Priority = "Critical",
                    Status = "In Progress",
                    Description = "Unable to upload images to gallery",
                    CreatedAt = DateTime.UtcNow.AddHours(-5),
                    ErrorId = "error-456"
                }
            };

            // Apply filters
            var filteredReports = reports.AsEnumerable();
            
            if (!string.IsNullOrEmpty(status))
            {
                filteredReports = filteredReports.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }
            
            if (!string.IsNullOrEmpty(priority))
            {
                filteredReports = filteredReports.Where(r => r.Priority.Equals(priority, StringComparison.OrdinalIgnoreCase));
            }

            return Json(new { success = true, data = filteredReports.ToList() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get error reports");
            return Json(new { success = false, message = "Failed to load error reports" });
        }
    }

    [HttpPost("status")]
    public IActionResult UpdateStatus([FromBody] UpdateStatusRequest request)
    {
        try
        {
            // In a real app, you would update the status in the database
            _logger.LogInformation("Updating status for ticket {TicketId} to {Status}", request.TicketId, request.Status);
            
            return Json(new { success = true, message = "Status updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update status for ticket {TicketId}", request.TicketId);
            return Json(new { success = false, message = "Failed to update status" });
        }
    }

    [HttpDelete("delete/{ticketId}")]
    public IActionResult DeleteReport(string ticketId)
    {
        try
        {
            // In a real app, you would delete the report from the database
            _logger.LogInformation("Deleting error report {TicketId}", ticketId);
            
            return Json(new { success = true, message = "Report deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete error report {TicketId}", ticketId);
            return Json(new { success = false, message = "Failed to delete report" });
        }
    }
}

public class UpdateStatusRequest
{
    public string TicketId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}


