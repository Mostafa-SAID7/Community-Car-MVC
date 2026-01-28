using CommunityCar.Application.Common.Interfaces.Data;
using CommunityCar.Application.Common.Interfaces.Services;
using CommunityCar.Domain.Entities.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Dashboard;

public class ErrorService : IErrorService
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ErrorService> _logger;

    public ErrorService(IApplicationDbContext context, ILogger<ErrorService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ErrorLog>> GetErrorsAsync(int page = 1, int pageSize = 50, string? category = null, string? severity = null, bool? isResolved = null)
    {
        var query = _context.ErrorLogs.AsQueryable();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(e => e.Category == category);

        if (!string.IsNullOrEmpty(severity))
            query = query.Where(e => e.Severity == severity);

        if (isResolved.HasValue)
            query = query.Where(e => e.IsResolved == isResolved.Value);

        return await query
            .OrderByDescending(e => e.LastOccurrence)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<ErrorLog?> GetErrorAsync(string errorId)
    {
        return await _context.ErrorLogs
            .Include(e => e.Occurrences)
            .FirstOrDefaultAsync(e => e.ErrorId == errorId);
    }

    public async Task<string> LogErrorAsync(string message, Exception? exception = null, string? userId = null, string? requestPath = null, string? additionalData = null)
    {
        try
        {
            // Check if similar error exists (same message and stack trace)
            var existingError = await _context.ErrorLogs
                .FirstOrDefaultAsync(e => e.Message == message && 
                                        e.StackTrace == (exception != null ? exception.StackTrace : null) &&
                                        !e.IsResolved);

            if (existingError != null)
            {
                // Update existing error
                existingError.OccurrenceCount++;
                existingError.LastOccurrence = DateTime.UtcNow;
                existingError.Audit(userId ?? "System");

                // Add new occurrence
                var occurrence = new ErrorOccurrence
                {
                    ErrorLogId = existingError.Id,
                    UserId = userId,
                    RequestPath = requestPath,
                    AdditionalContext = additionalData,
                    OccurredAt = DateTime.UtcNow
                };
                // occurrence.Audit(userId ?? "System");

                // _context.ErrorOccurrences.Add(occurrence);
                await _context.SaveChangesAsync();

                return existingError.ErrorId;
            }

            // Create new error log
            var errorLog = new ErrorLog
            {
                ErrorId = Guid.NewGuid().ToString(),
                Message = message,
                StackTrace = exception?.StackTrace,
                InnerException = exception?.InnerException?.ToString(),
                Source = exception?.Source ?? "Application",
                Severity = DetermineSeverity(exception),
                Category = DetermineCategory(exception, requestPath),
                UserId = userId,
                RequestPath = requestPath,
                AdditionalData = additionalData,
                LastOccurrence = DateTime.UtcNow
            };
            errorLog.Audit(userId ?? "System");

            _context.ErrorLogs.Add(errorLog);

            // Add initial occurrence
            var initialOccurrence = new ErrorOccurrence
            {
                ErrorLogId = errorLog.Id,
                UserId = userId,
                RequestPath = requestPath,
                AdditionalContext = additionalData,
                OccurredAt = DateTime.UtcNow
            };
            // initialOccurrence.Audit(userId ?? "System");

            // _context.ErrorOccurrences.Add(initialOccurrence);
            await _context.SaveChangesAsync();

            return errorLog.ErrorId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log error: {Message}", message);
            return string.Empty;
        }
    }

    public async Task<bool> ResolveErrorAsync(string errorId, string resolvedBy, string? resolution = null)
    {
        try
        {
            var error = await _context.ErrorLogs.FirstOrDefaultAsync(e => e.ErrorId == errorId);
            if (error == null) return false;

            error.IsResolved = true;
            error.ResolvedAt = DateTime.UtcNow;
            error.ResolvedBy = resolvedBy;
            error.Resolution = resolution;
            error.Audit(resolvedBy);

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resolve error {ErrorId}", errorId);
            return false;
        }
    }

    public async Task<bool> DeleteErrorAsync(string errorId)
    {
        try
        {
            var error = await _context.ErrorLogs
                .Include(e => e.Occurrences)
                .FirstOrDefaultAsync(e => e.ErrorId == errorId);
            
            if (error == null) return false;

            // _context.ErrorOccurrences.RemoveRange(error.Occurrences);
            _context.ErrorLogs.Remove(error);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete error {ErrorId}", errorId);
            return false;
        }
    }

    public async Task<ErrorStatsDto> GetErrorStatsAsync(DateTime? date = null)
    {
        var targetDate = date ?? DateTime.UtcNow.Date;
        var nextDay = targetDate.AddDays(1);

        var errors = await _context.ErrorLogs
            .Where(e => e.CreatedAt >= targetDate && e.CreatedAt < nextDay)
            .ToListAsync();

        var stats = new ErrorStatsDto
        {
            Date = targetDate,
            TotalErrors = errors.Sum(e => e.OccurrenceCount),
            CriticalErrors = errors.Where(e => e.Severity == "Critical").Sum(e => e.OccurrenceCount),
            WarningErrors = errors.Where(e => e.Severity == "Warning").Sum(e => e.OccurrenceCount),
            InfoErrors = errors.Where(e => e.Severity == "Info").Sum(e => e.OccurrenceCount),
            ResolvedErrors = errors.Where(e => e.IsResolved).Sum(e => e.OccurrenceCount),
            UnresolvedErrors = errors.Where(e => !e.IsResolved).Sum(e => e.OccurrenceCount)
        };

        // Most common error
        var mostCommon = errors
            .GroupBy(e => e.Message)
            .OrderByDescending(g => g.Sum(e => e.OccurrenceCount))
            .FirstOrDefault();

        if (mostCommon != null)
        {
            stats.MostCommonError = mostCommon.Key;
            stats.MostCommonErrorCount = mostCommon.Sum(e => e.OccurrenceCount);
        }

        // Errors by category
        stats.ErrorsByCategory = errors
            .GroupBy(e => e.Category)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.OccurrenceCount));

        // Errors by severity
        stats.ErrorsBySeverity = errors
            .GroupBy(e => e.Severity)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.OccurrenceCount));

        return stats;
    }

    public async Task<IEnumerable<ErrorStatsDto>> GetErrorStatsRangeAsync(DateTime startDate, DateTime endDate, string? category = null)
    {
        var stats = new List<ErrorStatsDto>();
        var currentDate = startDate.Date;

        while (currentDate <= endDate.Date)
        {
            var dayStats = await GetErrorStatsAsync(currentDate);
            stats.Add(dayStats);
            currentDate = currentDate.AddDays(1);
        }

        return stats;
    }

    public async Task<IEnumerable<ErrorBoundaryDto>> GetBoundaryErrorsAsync(string? boundaryName = null, bool? isRecovered = null)
    {
        // This is a placeholder implementation
        // In a real application, you would have a separate ErrorBoundary entity
        var boundaries = new List<ErrorBoundaryDto>();

        var errors = await _context.ErrorLogs
            .Where(e => e.Severity == "Critical")
            .OrderByDescending(e => e.LastOccurrence)
            .Take(50)
            .ToListAsync();

        foreach (var error in errors)
        {
            boundaries.Add(new ErrorBoundaryDto
            {
                Id = error.Id.ToString(),
                BoundaryName = error.Category,
                ErrorMessage = error.Message,
                OccurredAt = error.CreatedAt,
                IsRecovered = error.IsResolved,
                RecoveryAction = error.Resolution,
                RecoveredAt = error.ResolvedAt,
                FailureCount = error.OccurrenceCount
            });
        }

        if (!string.IsNullOrEmpty(boundaryName))
            boundaries = boundaries.Where(b => b.BoundaryName.Contains(boundaryName, StringComparison.OrdinalIgnoreCase)).ToList();

        if (isRecovered.HasValue)
            boundaries = boundaries.Where(b => b.IsRecovered == isRecovered.Value).ToList();

        return boundaries;
    }

    public async Task<bool> RecoverBoundaryAsync(string boundaryId, string recoveryAction)
    {
        return await ResolveErrorAsync(boundaryId, "System", recoveryAction);
    }

    public async Task UpdateErrorStatsAsync()
    {
        // This could be used to update cached statistics or perform maintenance
        _logger.LogInformation("Error statistics updated at {Timestamp}", DateTime.UtcNow);
        await Task.CompletedTask;
    }

    public async Task CleanupOldErrorsAsync(int daysToKeep = 90)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
            
            var oldErrors = await _context.ErrorLogs
                .Where(e => e.CreatedAt < cutoffDate && e.IsResolved)
                .Include(e => e.Occurrences)
                .ToListAsync();

            foreach (var error in oldErrors)
            {
                // _context.ErrorOccurrences.RemoveRange(error.Occurrences);
                _context.ErrorLogs.Remove(error);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Cleaned up {Count} old error logs", oldErrors.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup old errors");
        }
    }

    private static string DetermineSeverity(Exception? exception)
    {
        if (exception == null) return "Info";

        return exception switch
        {
            ArgumentNullException => "Error",
            ArgumentException => "Warning",
            InvalidOperationException => "Error",
            NotImplementedException => "Warning",
            UnauthorizedAccessException => "Error",
            TimeoutException => "Warning",
            OutOfMemoryException => "Critical",
            StackOverflowException => "Critical",
            _ => "Error"
        };
    }

    private static string DetermineCategory(Exception? exception, string? requestPath)
    {
        if (!string.IsNullOrEmpty(requestPath))
        {
            if (requestPath.Contains("/api/", StringComparison.OrdinalIgnoreCase))
                return "API";
            if (requestPath.Contains("/dashboard/", StringComparison.OrdinalIgnoreCase))
                return "Dashboard";
            if (requestPath.Contains("/community/", StringComparison.OrdinalIgnoreCase))
                return "Community";
        }

        if (exception != null)
        {
            var exceptionType = exception.GetType().Name;
            if (exceptionType.Contains("Sql") || exceptionType.Contains("Database"))
                return "Database";
            if (exceptionType.Contains("Network") || exceptionType.Contains("Http"))
                return "Network";
            if (exceptionType.Contains("Security") || exceptionType.Contains("Unauthorized"))
                return "Security";
            if (exceptionType.Contains("Validation"))
                return "Validation";
        }

        return "General";
    }
}


