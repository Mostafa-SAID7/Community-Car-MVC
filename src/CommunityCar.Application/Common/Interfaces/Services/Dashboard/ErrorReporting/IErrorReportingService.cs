using CommunityCar.Application.Features.Dashboard.ErrorReporting.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.ErrorReporting;

public interface IErrorReportingService
{
    Task<ErrorReportResponseVM> SubmitErrorReportAsync(ErrorReportVM request);
    Task<bool> SendErrorReportEmailAsync(ErrorReportVM request, string ticketId);
    Task<List<ErrorReportVM>> GetUserErrorReportsAsync(string userId);
    Task<ErrorReportVM> GetErrorReportByTicketIdAsync(string ticketId);
}