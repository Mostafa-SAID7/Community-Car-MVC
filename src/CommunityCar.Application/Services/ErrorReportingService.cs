using CommunityCar.Application.Common.Interfaces.Services;
using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Features.ErrorReporting.ViewModels;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CommunityCar.Application.Services;

public class ErrorReportingService : IErrorReportingService
{
    private readonly ILogger<ErrorReportingService> _logger;
    private readonly IEmailService _emailService;
    private readonly List<ErrorReportVM> _reports = new(); // In-memory storage for demo

    public ErrorReportingService(
        ILogger<ErrorReportingService> logger,
        IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task<ErrorReportResponseVM> SubmitErrorReportAsync(ErrorReportVM request)
    {
        try
        {
            var ticketId = $"ERR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
            
            var response = new ErrorReportResponseVM
            {
                Success = true,
                Message = "Error report submitted successfully. You will receive updates via email.",
                TicketId = ticketId
            };

            // Store the report (in real app, this would go to database)
            var reportWithResponse = request;
            reportWithResponse.Success = true;
            reportWithResponse.Message = response.Message;
            reportWithResponse.TicketId = ticketId;
            _reports.Add(reportWithResponse);

            // Send email notification to admin
            await SendErrorReportEmailAsync(request, ticketId);

            _logger.LogInformation("Error report submitted successfully with ticket ID: {TicketId}", ticketId);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to submit error report for error ID: {ErrorId}", request.ErrorId);
            
            return new ErrorReportResponseVM
            {
                Success = false,
                Message = "Failed to submit error report. Please try again later.",
                TicketId = string.Empty
            };
        }
    }

    public async Task<bool> SendErrorReportEmailAsync(ErrorReportVM request, string ticketId)
    {
        try
        {
            var subject = $"Error Report - {ticketId}";
            var body = $@"
<html>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h2 style='color: #d32f2f; border-bottom: 2px solid #d32f2f; padding-bottom: 10px;'>
            🚨 New Error Report - {ticketId}
        </h2>
        
        <div style='background: #f5f5f5; padding: 15px; border-radius: 8px; margin: 20px 0;'>
            <h3 style='margin-top: 0; color: #1976d2;'>Error Information</h3>
            <p><strong>Error ID:</strong> {request.ErrorId}</p>
            <p><strong>Reported by:</strong> {request.UserName} ({request.UserEmail})</p>
            <p><strong>Priority:</strong> {request.Priority}</p>
            <p><strong>Timestamp:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
        </div>

        <div style='margin: 20px 0;'>
            <h3 style='color: #1976d2;'>Description</h3>
            <p style='background: #fff; padding: 15px; border-left: 4px solid #1976d2; margin: 10px 0;'>
                {request.Description}
            </p>
        </div>

        <div style='margin: 20px 0;'>
            <h3 style='color: #1976d2;'>Steps to Reproduce</h3>
            <p style='background: #fff; padding: 15px; border-left: 4px solid #ff9800; margin: 10px 0;'>
                {request.StepsToReproduce}
            </p>
        </div>

        <div style='margin: 20px 0;'>
            <h3 style='color: #1976d2;'>Expected vs Actual Behavior</h3>
            <div style='display: flex; gap: 20px;'>
                <div style='flex: 1;'>
                    <h4 style='color: #4caf50;'>Expected:</h4>
                    <p style='background: #e8f5e8; padding: 10px; border-radius: 4px;'>{request.ExpectedBehavior}</p>
                </div>
                <div style='flex: 1;'>
                    <h4 style='color: #f44336;'>Actual:</h4>
                    <p style='background: #ffeaea; padding: 10px; border-radius: 4px;'>{request.ActualBehavior}</p>
                </div>
            </div>
        </div>

        <div style='margin: 20px 0;'>
            <h3 style='color: #1976d2;'>Technical Information</h3>
            <p><strong>Browser:</strong> {request.BrowserInfo}</p>
            <p><strong>Device:</strong> {request.DeviceInfo}</p>
        </div>

        <div style='background: #e3f2fd; padding: 15px; border-radius: 8px; margin: 20px 0;'>
            <p style='margin: 0;'><strong>Action Required:</strong> Please investigate this error and update the ticket status in the admin dashboard.</p>
        </div>
    </div>
</body>
</html>";

            // In a real app, you would send this to admin email
            // await _emailService.SendEmailAsync("admin@communitycar.com", subject, body);
            
            _logger.LogInformation("Error report email would be sent to admin for ticket: {TicketId}", ticketId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send error report email for ticket: {TicketId}", ticketId);
            return false;
        }
    }

    public async Task<List<ErrorReportVM>> GetUserErrorReportsAsync(string userId)
    {
        // In real app, filter by userId from database
        return await Task.FromResult(_reports.ToList());
    }

    public async Task<ErrorReportVM> GetErrorReportByTicketIdAsync(string ticketId)
    {
        var report = _reports.FirstOrDefault(r => r.TicketId == ticketId);
        return await Task.FromResult(report ?? new ErrorReportVM());
    }
}


