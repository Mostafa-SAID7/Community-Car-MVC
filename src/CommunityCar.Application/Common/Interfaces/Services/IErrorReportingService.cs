using CommunityCar.Application.Features.ErrorReporting.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services;

public interface IErrorReportingService
{
    Task<ErrorReportResponseVM> SubmitErrorReportAsync(ErrorReportVM request);
    Task<bool> SendErrorReportEmailAsync(ErrorReportVM request, string ticketId);
    Task<List<ErrorReportVM>> GetUserErrorReportsAsync(string userId);
    Task<ErrorReportVM> GetErrorReportByTicketIdAsync(string ticketId);
}


