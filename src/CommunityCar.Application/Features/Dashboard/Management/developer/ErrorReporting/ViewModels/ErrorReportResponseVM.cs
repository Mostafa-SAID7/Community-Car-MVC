namespace CommunityCar.Application.Features.Dashboard.Management.developer.ErrorReporting.ViewModels;

public class ErrorReportResponseVM
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string TicketId { get; set; } = string.Empty;
}