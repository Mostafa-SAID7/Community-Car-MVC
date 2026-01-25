using CommunityCar.Application.Common.Interfaces.Services;
using CommunityCar.Application.Features.ErrorReporting.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Api;

[ApiController]
[Route("api/error-reporting")]
public class ErrorReportingApiController : ControllerBase
{
    private readonly IErrorReportingService _errorReportingService;
    private readonly ILogger<ErrorReportingApiController> _logger;

    public ErrorReportingApiController(
        IErrorReportingService errorReportingService,
        ILogger<ErrorReportingApiController> logger)
    {
        _errorReportingService = errorReportingService;
        _logger = logger;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitErrorReport([FromBody] ErrorReportRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Add browser and device info from request headers
            request.BrowserInfo = Request.Headers.UserAgent.ToString();
            request.DeviceInfo = GetDeviceInfo();

            var result = await _errorReportingService.SubmitErrorReportAsync(request);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to submit error report");
            return StatusCode(500, new ErrorReportResponse
            {
                Success = false,
                Message = "An error occurred while submitting the report. Please try again later."
            });
        }
    }

    [HttpGet("user-reports")]
    public async Task<IActionResult> GetUserReports()
    {
        try
        {
            var userId = User?.Identity?.Name ?? "anonymous";
            var reports = await _errorReportingService.GetUserErrorReportsAsync(userId);
            return Ok(reports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user error reports");
            return StatusCode(500, "Failed to retrieve error reports");
        }
    }

    [HttpGet("ticket/{ticketId}")]
    public async Task<IActionResult> GetErrorReportByTicket(string ticketId)
    {
        try
        {
            var report = await _errorReportingService.GetErrorReportByTicketIdAsync(ticketId);
            if (report.Success)
            {
                return Ok(report);
            }
            
            return NotFound("Error report not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get error report by ticket ID: {TicketId}", ticketId);
            return StatusCode(500, "Failed to retrieve error report");
        }
    }

    private string GetDeviceInfo()
    {
        var userAgent = Request.Headers.UserAgent.ToString();
        var deviceInfo = "Unknown Device";

        if (userAgent.Contains("Mobile"))
            deviceInfo = "Mobile Device";
        else if (userAgent.Contains("Tablet"))
            deviceInfo = "Tablet";
        else if (userAgent.Contains("Windows"))
            deviceInfo = "Windows Desktop";
        else if (userAgent.Contains("Mac"))
            deviceInfo = "Mac Desktop";
        else if (userAgent.Contains("Linux"))
            deviceInfo = "Linux Desktop";

        return deviceInfo;
    }
}