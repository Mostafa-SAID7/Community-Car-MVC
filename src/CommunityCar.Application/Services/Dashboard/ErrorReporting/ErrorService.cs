using CommunityCar.Application.Common.Interfaces.Services.Dashboard.ErrorReporting;
using CommunityCar.Application.Features.Dashboard.ErrorReporting.ViewModels;
using CommunityCar.Application.Features.Shared.ViewModels;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Application.Common.Models;

namespace CommunityCar.Application.Services.Dashboard.ErrorReporting;

public class ErrorService : IErrorService
{
    public async Task<ErrorDetailsViewModel?> GetErrorByIdAsync(Guid errorId)
    {
        return new ErrorDetailsViewModel
        {
            ErrorId = errorId,
            RequestId = Guid.NewGuid().ToString(),
            Message = "Sample error message",
            Exception = "System.Exception: Sample exception",
            StackTrace = "Sample stack trace",
            Timestamp = DateTime.UtcNow.AddHours(-1),
            Level = "Error",
            Source = "Application",
            UserId = Guid.NewGuid().ToString(),
            RequestPath = "/api/sample",
            AdditionalData = new Dictionary<string, object> { { "key", "value" } },
            IsResolved = false
        };
    }

    public async Task<List<ErrorLogVM>> GetErrorsAsync(int page = 1, int pageSize = 20)
    {
        var errors = new List<ErrorLogVM>();
        var random = new Random();
        
        for (int i = 0; i < pageSize; i++)
        {
            errors.Add(new ErrorLogVM
            {
                Id = Guid.NewGuid(),
                Message = $"Sample error message #{i + 1}",
                Exception = "System.Exception: Sample exception",
                StackTrace = "Sample stack trace",
                Timestamp = DateTime.UtcNow.AddMinutes(-random.Next(1, 1440)),
                Level = "Error",
                Source = "Application",
                UserId = Guid.NewGuid().ToString(),
                RequestPath = "/api/sample",
                AdditionalData = new Dictionary<string, object> { { "key", "value" } },
                IsResolved = false
            });
        }

        return errors;
    }

    public async Task<ErrorStatsVM> GetErrorStatsAsync()
    {
        var random = new Random();
        return new ErrorStatsVM
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

    public async Task<bool> LogErrorAsync(Exception exception, string? context = null)
    {
        await Task.Delay(50);
        return true;
    }

    public async Task<bool> MarkErrorAsResolvedAsync(Guid errorId)
    {
        await Task.Delay(100);
        return true;
    }

    public async Task<bool> DeleteErrorAsync(Guid errorId)
    {
        await Task.Delay(100);
        return true;
    }

    public async Task<List<ErrorBoundaryVM>> GetErrorBoundariesAsync()
    {
        var boundaries = new List<ErrorBoundaryVM>();
        var random = new Random();
        
        for (int i = 0; i < 10; i++)
        {
            boundaries.Add(new ErrorBoundaryVM
            {
                Id = Guid.NewGuid(),
                BoundaryName = $"Boundary_{i + 1}",
                ErrorMessage = $"Boundary error #{i + 1}",
                OccurredAt = DateTime.UtcNow.AddMinutes(-random.Next(1, 1440)),
                IsRecovered = random.Next(2) == 0,
                RecoveryAction = "Automatic recovery",
                ComponentName = $"Component_{i + 1}",
                Severity = new[] { "Low", "Medium", "High", "Critical" }[random.Next(4)]
            });
        }

        return boundaries;
    }

    public async Task<bool> CreateErrorBoundaryAsync(string name)
    {
        await Task.Delay(100);
        return true;
    }

    public async Task<bool> RecoverErrorBoundaryAsync(Guid boundaryId)
    {
        await Task.Delay(100);
        return true;
    }
}