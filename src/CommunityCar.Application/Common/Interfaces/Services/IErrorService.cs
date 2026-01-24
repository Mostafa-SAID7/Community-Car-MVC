using CommunityCar.Domain.Entities.Shared;

namespace CommunityCar.Application.Common.Interfaces.Services;

public interface IErrorService
{
    Task<IEnumerable<ErrorLog>> GetErrorsAsync(int page = 1, int pageSize = 50, string? category = null, string? severity = null, bool? isResolved = null);
    Task<ErrorLog?> GetErrorAsync(string errorId);
    Task<string> LogErrorAsync(string message, Exception? exception = null, string? userId = null, string? requestPath = null, string? additionalData = null);
    Task<bool> ResolveErrorAsync(string errorId, string resolvedBy, string? resolution = null);
    Task<bool> DeleteErrorAsync(string errorId);
    Task<ErrorStatsDto> GetErrorStatsAsync(DateTime? date = null);
    Task<IEnumerable<ErrorStatsDto>> GetErrorStatsRangeAsync(DateTime startDate, DateTime endDate, string? category = null);
    Task<IEnumerable<ErrorBoundaryDto>> GetBoundaryErrorsAsync(string? boundaryName = null, bool? isRecovered = null);
    Task<bool> RecoverBoundaryAsync(string boundaryId, string recoveryAction);
    Task UpdateErrorStatsAsync();
    Task CleanupOldErrorsAsync(int daysToKeep = 90);
}

public class ErrorStatsDto
{
    public DateTime Date { get; set; }
    public int TotalErrors { get; set; }
    public int CriticalErrors { get; set; }
    public int WarningErrors { get; set; }
    public int InfoErrors { get; set; }
    public int ResolvedErrors { get; set; }
    public int UnresolvedErrors { get; set; }
    public string? MostCommonError { get; set; }
    public int MostCommonErrorCount { get; set; }
    public Dictionary<string, int> ErrorsByCategory { get; set; } = new();
    public Dictionary<string, int> ErrorsBySeverity { get; set; } = new();
}

public class ErrorBoundaryDto
{
    public string Id { get; set; } = string.Empty;
    public string BoundaryName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public bool IsRecovered { get; set; }
    public string? RecoveryAction { get; set; }
    public DateTime? RecoveredAt { get; set; }
    public int FailureCount { get; set; }
}