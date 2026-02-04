using CommunityCar.Application.Common.Interfaces.Services.Dashboard.ErrorReporting;
using CommunityCar.Application.Features.Dashboard.ErrorReporting.ViewModels;
using CommunityCar.Application.Features.Shared.ViewModels;
using CommunityCar.Application.Features.Dashboard.ViewModels;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Application.Common.Models;

namespace CommunityCar.Application.Services.Dashboard.ErrorReporting;

public class ErrorService : IErrorService
{
    public async Task<(IEnumerable<ErrorLog> Errors, PaginationInfo Pagination)> GetErrorsAsync(int page = 1, int pageSize = 50, string? category = null, string? severity = null, bool? isResolved = null)
    {
        var errors = new List<ErrorLog>();
        var random = new Random();
        
        for (int i = 0; i < pageSize; i++)
        {
            errors.Add(new ErrorLog
            {
                Message = $"Sample error message #{i + 1}",
                Exception = "System.Exception: Sample exception",
                StackTrace = "Sample stack trace",
                Timestamp = DateTime.UtcNow.AddMinutes(-random.Next(1, 1440)),
                Level = severity ?? "Error",
                Source = "Application",
                UserId = Guid.NewGuid().ToString(),
                RequestPath = "/api/sample",
                AdditionalData = "{}",
                IsResolved = isResolved ?? false
            });
        }

        var pagination = new PaginationInfo
        {
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = 1000,
            TotalPages = (int)Math.Ceiling(1000.0 / pageSize)
        };

        return await Task.FromResult((errors.AsEnumerable(), pagination));
    }

    public async Task<ErrorLog?> GetErrorAsync(string errorId)
    {
        return new ErrorLog
        {
            Message = "Sample error message",
            Exception = "System.Exception: Sample exception",
            StackTrace = "Sample stack trace",
            Timestamp = DateTime.UtcNow.AddHours(-1),
            Level = "Error",
            Source = "Application",
            UserId = Guid.NewGuid().ToString(),
            RequestPath = "/api/sample",
            AdditionalData = "{}",
            IsResolved = false
        };
    }

    public async Task<string> LogErrorAsync(string message, Exception? exception = null, string? userId = null, string? requestPath = null, string? additionalData = null)
    {
        await Task.Delay(50);
        return Guid.NewGuid().ToString();
    }

    public async Task<bool> ResolveErrorAsync(string errorId, string resolvedBy, string? resolution = null)
    {
        await Task.Delay(100);
        return true;
    }

    public async Task<bool> DeleteErrorAsync(string errorId)
    {
        await Task.Delay(100);
        return true;
    }

    public async Task<Features.Dashboard.ErrorReporting.ViewModels.ErrorStatsVM> GetErrorStatsAsync(DateTime? date = null)
    {
        var random = new Random();
        return new Features.Dashboard.ErrorReporting.ViewModels.ErrorStatsVM
        {
            TotalErrors = random.Next(100, 1000),
            CriticalErrors = random.Next(5, 50),
            ErrorsToday = random.Next(10, 100),
            ErrorsThisWeek = random.Next(50, 500),
            ErrorsThisMonth = random.Next(200, 2000),
            AverageErrorsPerDay = random.Next(20, 200),
            ErrorRate = (double)(random.NextDouble() * 0.1),
            MostCommonError = "NullReferenceException",
            MostActiveErrorSource = "Web Application",
            ErrorTrend = random.Next(2) == 0 ? "Increasing" : "Decreasing",
            ErrorTrendPercentage = (decimal)(random.NextDouble() * 20 + 5),
            LastErrorTime = DateTime.UtcNow.AddMinutes(-random.Next(1, 60)),
            ResolvedErrorsCount = random.Next(80, 800),
            UnresolvedErrorsCount = random.Next(20, 200),
            AverageResolutionTime = TimeSpan.FromHours(random.Next(1, 24)),
            TopErrorCategories = new List<string>
            {
                "System", "Business Logic", "Validation", "Security", "Performance"
            },
            ErrorsByLevel = new Dictionary<string, int>
            {
                { "Error", random.Next(100, 500) },
                { "Warning", random.Next(50, 300) },
                { "Critical", random.Next(10, 100) },
                { "Fatal", random.Next(1, 20) }
            },
            ErrorsBySource = new Dictionary<string, int>
            {
                { "Application", random.Next(100, 400) },
                { "Database", random.Next(20, 100) },
                { "API", random.Next(50, 200) },
                { "Background Job", random.Next(10, 80) },
                { "External Service", random.Next(5, 50) }
            }
        };
    }

    public async Task<IEnumerable<Features.Dashboard.ErrorReporting.ViewModels.ErrorStatsVM>> GetErrorStatsRangeAsync(DateTime startDate, DateTime endDate, string? category = null)
    {
        var stats = new List<Features.Dashboard.ErrorReporting.ViewModels.ErrorStatsVM>();
        var random = new Random();
        
        var current = startDate;
        while (current <= endDate)
        {
            stats.Add(new Features.Dashboard.ErrorReporting.ViewModels.ErrorStatsVM
            {
                TotalErrors = random.Next(10, 100),
                CriticalErrors = random.Next(1, 10),
                ErrorsToday = random.Next(5, 50),
                ErrorsThisWeek = random.Next(20, 200),
                ErrorsThisMonth = random.Next(100, 1000),
                AverageErrorsPerDay = random.Next(10, 100),
                ErrorRate = (double)(random.NextDouble() * 0.05),
                MostCommonError = "NullReferenceException",
                MostActiveErrorSource = "Web Application",
                ErrorTrend = random.Next(2) == 0 ? "Increasing" : "Decreasing",
                ErrorTrendPercentage = (decimal)(random.NextDouble() * 10 + 2),
                LastErrorTime = current.AddMinutes(-random.Next(1, 60)),
                ResolvedErrorsCount = random.Next(40, 400),
                UnresolvedErrorsCount = random.Next(10, 100),
                AverageResolutionTime = TimeSpan.FromHours(random.Next(1, 12)),
                TopErrorCategories = new List<string>
                {
                    "System", "Business Logic", "Validation", "Security", "Performance"
                },
                ErrorsByLevel = new Dictionary<string, int>
                {
                    { "Error", random.Next(50, 250) },
                    { "Warning", random.Next(25, 150) },
                    { "Critical", random.Next(5, 50) },
                    { "Fatal", random.Next(1, 10) }
                },
                ErrorsBySource = new Dictionary<string, int>
                {
                    { "Application", random.Next(50, 200) },
                    { "Database", random.Next(10, 50) },
                    { "API", random.Next(25, 100) },
                    { "Background Job", random.Next(5, 40) },
                    { "External Service", random.Next(2, 25) }
                }
            });
            current = current.AddDays(1);
        }

        return await Task.FromResult(stats);
    }

    public async Task<(IEnumerable<Features.Dashboard.Error.ViewModels.ErrorBoundaryVM> Boundaries, PaginationInfo Pagination)> GetBoundaryErrorsAsync(int page = 1, int pageSize = 50, string? boundaryName = null, bool? isRecovered = null)
    {
        var boundaries = new List<Features.Dashboard.Error.ViewModels.ErrorBoundaryVM>();
        var random = new Random();
        
        for (int i = 0; i < pageSize; i++)
        {
            boundaries.Add(new Features.Dashboard.Error.ViewModels.ErrorBoundaryVM
            {
                Id = Guid.NewGuid(),
                BoundaryName = boundaryName ?? $"Boundary_{i + 1}",
                ErrorMessage = $"Boundary error #{i + 1}",
                OccurredAt = DateTime.UtcNow.AddMinutes(-random.Next(1, 1440)),
                IsRecovered = isRecovered ?? random.Next(2) == 0,
                RecoveryAction = "Automatic recovery",
                ComponentName = $"Component_{i + 1}",
                Severity = new[] { "Low", "Medium", "High", "Critical" }[random.Next(4)]
            });
        }

        var pagination = new PaginationInfo
        {
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = 500,
            TotalPages = (int)Math.Ceiling(500.0 / pageSize)
        };

        return await Task.FromResult((boundaries.AsEnumerable(), pagination));
    }

    public async Task<bool> RecoverBoundaryAsync(string boundaryId, string recoveryAction)
    {
        await Task.Delay(100);
        return true;
    }

    public async Task UpdateErrorStatsAsync()
    {
        await Task.Delay(500);
    }

    public async Task CleanupOldErrorsAsync(int daysToKeep = 90)
    {
        await Task.Delay(1000);
    }
}