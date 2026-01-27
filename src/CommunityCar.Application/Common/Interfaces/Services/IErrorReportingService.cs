using CommunityCar.Application.Features.ErrorReporting.DTOs;

namespace CommunityCar.Application.Common.Interfaces.Services;

public interface IErrorReportingService
{
    Task<ErrorReportResponse> SubmitErrorReportAsync(ErrorReportRequest request);
    Task<bool> SendErrorReportEmailAsync(ErrorReportRequest request, string ticketId);
    Task<List<ErrorReportResponse>> GetUserErrorReportsAsync(string userId);
    Task<ErrorReportResponse> GetErrorReportByTicketIdAsync(string ticketId);
}


