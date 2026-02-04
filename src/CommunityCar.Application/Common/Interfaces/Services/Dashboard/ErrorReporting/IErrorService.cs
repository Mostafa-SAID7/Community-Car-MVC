using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Application.Features.Dashboard.ViewModels;
using CommunityCar.Application.Common.Models;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.ErrorReporting;

public interface IErrorService
{
    Task<(IEnumerable<ErrorLog> Errors, PaginationInfo Pagination)> GetErrorsAsync(int page = 1, int pageSize = 50, string? category = null, string? severity = null, bool? isResolved = null);
    Task<ErrorLog?> GetErrorAsync(string errorId);
    Task<string> LogErrorAsync(string message, Exception? exception = null, string? userId = null, string? requestPath = null, string? additionalData = null);
    Task<bool> ResolveErrorAsync(string errorId, string resolvedBy, string? resolution = null);
    Task<bool> DeleteErrorAsync(string errorId);
    Task<Features.Dashboard.ErrorReporting.ViewModels.ErrorStatsVM> GetErrorStatsAsync(DateTime? date = null);
    Task<IEnumerable<Features.Dashboard.ErrorReporting.ViewModels.ErrorStatsVM>> GetErrorStatsRangeAsync(DateTime startDate, DateTime endDate, string? category = null);
    Task<(IEnumerable<Features.Dashboard.Error.ViewModels.ErrorBoundaryVM> Boundaries, PaginationInfo Pagination)> GetBoundaryErrorsAsync(int page = 1, int pageSize = 50, string? boundaryName = null, bool? isRecovered = null);
    Task<bool> RecoverBoundaryAsync(string boundaryId, string recoveryAction);
    Task UpdateErrorStatsAsync();
    Task CleanupOldErrorsAsync(int daysToKeep = 90);
}