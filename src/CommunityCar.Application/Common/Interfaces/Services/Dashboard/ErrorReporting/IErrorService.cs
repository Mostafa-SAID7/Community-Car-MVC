using CommunityCar.Application.Features.Dashboard.ErrorReporting.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.ErrorReporting;

public interface IErrorService
{
    Task<ErrorDetailsViewModel?> GetErrorByIdAsync(Guid errorId);
    Task<List<ErrorLogVM>> GetErrorsAsync(int page = 1, int pageSize = 20);
    Task<ErrorStatsVM> GetErrorStatsAsync();
    Task<bool> LogErrorAsync(Exception exception, string? context = null);
    Task<bool> MarkErrorAsResolvedAsync(Guid errorId);
    Task<bool> DeleteErrorAsync(Guid errorId);
    Task<List<ErrorBoundaryVM>> GetErrorBoundariesAsync();
    Task<bool> CreateErrorBoundaryAsync(string name);
    Task<bool> RecoverErrorBoundaryAsync(Guid boundaryId);
}