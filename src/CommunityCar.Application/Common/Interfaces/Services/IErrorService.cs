using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Application.Features.Dashboard.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services;

public interface IErrorService
{
    Task<IEnumerable<ErrorLog>> GetErrorsAsync(int page = 1, int pageSize = 50, string? category = null, string? severity = null, bool? isResolved = null);
    Task<ErrorLog?> GetErrorAsync(string errorId);
    Task<string> LogErrorAsync(string message, Exception? exception = null, string? userId = null, string? requestPath = null, string? additionalData = null);
    Task<bool> ResolveErrorAsync(string errorId, string resolvedBy, string? resolution = null);
    Task<bool> DeleteErrorAsync(string errorId);
    Task<ErrorStatsVM> GetErrorStatsAsync(DateTime? date = null);
    Task<IEnumerable<ErrorStatsVM>> GetErrorStatsRangeAsync(DateTime startDate, DateTime endDate, string? category = null);
    Task<IEnumerable<ErrorBoundaryVM>> GetBoundaryErrorsAsync(string? boundaryName = null, bool? isRecovered = null);
    Task<bool> RecoverBoundaryAsync(string boundaryId, string recoveryAction);
    Task UpdateErrorStatsAsync();
    Task CleanupOldErrorsAsync(int daysToKeep = 90);
}




