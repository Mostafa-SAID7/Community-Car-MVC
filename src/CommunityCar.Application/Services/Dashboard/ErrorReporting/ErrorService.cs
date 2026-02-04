using CommunityCar.Application.Common.Interfaces.Services.Dashboard.ErrorReporting;
using CommunityCar.Application.Features.Dashboard.ViewModels;

namespace CommunityCar.Application.Services.Dashboard.ErrorReporting;

public class ErrorService : IErrorService
{
    public async Task<List<ErrorLogVM>> GetErrorLogsAsync(int page = 1, int pageSize = 20, string? level = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var errors = new List<ErrorLogVM>();
        var random = new Random();
        var levels = new[] { "Error", "Warning", "Critical", "Fatal" };
        var sources = new[] { "Application", "Database", "API", "Background Job", "External Service" };
        var categories = new[] { "System", "Business Logic", "Validation", "Security", "Performance" };

        var start = startDate ?? DateTime.UtcNow.AddDays(-7);
        var end = endDate ?? DateTime.UtcNow;

        for (int i = 0; i < pageSize; i++)
        {
            var errorLevel = level ?? levels[random.Next(levels.Length)];
            var timestamp = start.AddMinutes(random.Next(0, (int)(end - start).TotalMinutes));
            
            errors.Add(new ErrorLogVM
            {
                Id = Guid.NewGuid(),
                Timestamp = timestamp,
                Level = errorLevel,
                Source = sources[random.Next(sources.Length)],
                Category = categories[random.Next(categories.Length)],
                Message = $"Sample {errorLevel.ToLower()} message #{i + 1}: An error occurred during processing",
                Exception = errorLevel is "Error" or "Critical" or "Fatal" ? 
                    $"System.{categories[random.Next(categories.Length)].Replace(" ", "")}Exception: Detailed error information" : null,
                StackTrace = errorLevel is "Error" or "Critical" or "Fatal" ? 
                    $"   at CommunityCar.Services.SampleService.Method() in SampleService.cs:line {random.Next(10, 200)}\n   at CommunityCar.Controllers.SampleController.Action() in SampleController.cs:line {random.Next(10, 100)}" : null,
                UserId = random.Next(4) == 0 ? Guid.NewGuid().ToString() : null,
                UserName = random.Next(4) == 0 ? $"user{random.Next(1, 100)}" : null,
                IpAddress = $"{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                RequestId = Guid.NewGuid().ToString(),
                RequestPath = $"/api/{new[] { "users", "posts", "comments", "groups", "events" }[random.Next(5)]}",
                HttpMethod = new[] { "GET", "POST", "PUT", "DELETE", "PATCH" }[random.Next(5)],
                StatusCode = new[] { 400, 401, 403, 404, 422, 500, 502, 503 }[random.Next(8)],
                ResponseTime = random.Next(100, 5000),
                MemoryUsage = random.Next(50, 500), // MB
                CpuUsage = (decimal)(random.NextDouble() * 100),
                ThreadId = random.Next(1, 20),
                ProcessId = random.Next(1000, 9999),
                MachineName = Environment.MachineName,
                ApplicationVersion = "1.0.0",
                Environment = "Development",
                CorrelationId = Guid.NewGuid().ToString(),
                Properties = new Dictionary<string, object>
                {
                    { "RequestSize", random.Next(100, 10000) },
                    { "ResponseSize", random.Next(500, 50000) },
                    { "DatabaseQueryTime", random.Next(10, 1000) },
                    { "CacheHit", random.Next(2) == 0 }
                },
                Tags = new List<string> { errorLevel.ToLower(), sources[random.Next(sources.Length)].ToLower(), categories[random.Next(categories.Length)].ToLower().Replace(" ", "-") },
                IsResolved = random.Next(5) == 0, // 20% resolved
                ResolvedAt = null,
                ResolvedBy = null,
                Notes = random.Next(5) == 0 ? "Error has been investigated and resolved" : null
            });
        }

        return await Task.FromResult(errors.OrderByDescending(e => e.Timestamp).ToList());
    }

    public async Task<ErrorLogVM?> GetErrorLogByIdAsync(Guid errorId)
    {
        var errors = await GetErrorLogsAsync(1, 100);
        return errors.FirstOrDefault(e => e.Id == errorId);
    }

    public async Task<Guid> LogErrorAsync(LogErrorVM request)
    {
        // In real implementation, save error log to database
        await Task.Delay(50);
        return Guid.NewGuid();
    }

    public async Task<bool> ResolveErrorAsync(Guid errorId, string resolvedBy, string? notes = null)
    {
        // In real implementation, mark error as resolved
        await Task.Delay(100);
        return true;
    }

    public async Task<bool> DeleteErrorLogAsync(Guid errorId)
    {
        // In real implementation, delete error log
        await Task.Delay(100);
        return true;
    }

    public async Task<ErrorStatsVM> GetErrorStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var random = new Random();
        var start = startDate ?? DateTime.UtcNow.AddDays(-7);
        var end = endDate ?? DateTime.UtcNow;

        return new ErrorStatsVM
        {
            TotalErrors = random.Next(100, 1000),
            CriticalErrors = random.Next(5, 50),
            ErrorsToday = random.Next(10, 100),
            ErrorsThisWeek = random.Next(50, 500),
            ErrorsThisMonth = random.Next(200, 2000),
            AverageErrorsPerDay = random.Next(20, 200),
            ErrorRate = (decimal)(random.NextDouble() * 0.1), // 0-10%
            MostCommonError = "NullReferenceException",
            MostActiveErrorSource = "Web Application",
            ErrorTrend = random.Next(2) == 0 ? "Increasing" : "Decreasing",
            ErrorTrendPercentage = (decimal)(random.NextDouble() * 20 + 5), // 5-25%
            LastErrorTime = DateTime.UtcNow.AddMinutes(-random.Next(1, 60)),
            ResolvedErrorsCount = random.Next(80, 800),
            UnresolvedErrorsCount = random.Next(20, 200),
            AverageResolutionTime = TimeSpan.FromHours(random.Next(1, 24)),
            TopErrorCategories = new Dictionary<string, int>
            {
                { "System", random.Next(50, 300) },
                { "Business Logic", random.Next(30, 200) },
                { "Validation", random.Next(20, 150) },
                { "Security", random.Next(5, 50) },
                { "Performance", random.Next(10, 100) }
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

    public async Task<List<ChartDataVM>> GetErrorTrendAsync(int days = 7)
    {
        var data = new List<ChartDataVM>();
        var random = new Random();
        var startDate = DateTime.UtcNow.AddDays(-days);

        for (int i = 0; i < days; i++)
        {
            var date = startDate.AddDays(i);
            data.Add(new ChartDataVM
            {
                Label = date.ToString("MMM dd"),
                Value = random.Next(5, 100), // Errors per day
                Date = date
            });
        }

        return await Task.FromResult(data);
    }

    public async Task<List<ErrorLogVM>> GetRecentErrorsAsync(int count = 10)
    {
        var errors = await GetErrorLogsAsync(1, count);
        return errors.Take(count).ToList();
    }

    public async Task<List<ErrorLogVM>> GetCriticalErrorsAsync(int count = 20)
    {
        var errors = await GetErrorLogsAsync(1, 100, "Critical");
        return errors.Take(count).ToList();
    }

    public async Task<List<ErrorSummaryVM>> GetErrorSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var random = new Random();
        var summaries = new List<ErrorSummaryVM>();
        var errorTypes = new[] { "NullReferenceException", "ArgumentException", "InvalidOperationException", "SqlException", "TimeoutException" };

        foreach (var errorType in errorTypes)
        {
            summaries.Add(new ErrorSummaryVM
            {
                ErrorType = errorType,
                Count = random.Next(10, 200),
                LastOccurrence = DateTime.UtcNow.AddMinutes(-random.Next(1, 1440)),
                Severity = new[] { "Low", "Medium", "High", "Critical" }[random.Next(4)],
                AffectedUsers = random.Next(1, 50),
                IsResolved = random.Next(3) == 0, // 33% resolved
                TrendDirection = random.Next(2) == 0 ? "Up" : "Down",
                TrendPercentage = (decimal)(random.NextDouble() * 50)
            });
        }

        return await Task.FromResult(summaries.OrderByDescending(s => s.Count).ToList());
    }

    public async Task<bool> BulkResolveErrorsAsync(List<Guid> errorIds, string resolvedBy, string? notes = null)
    {
        // In real implementation, resolve multiple errors
        await Task.Delay(500);
        return true;
    }

    public async Task<bool> BulkDeleteErrorsAsync(List<Guid> errorIds)
    {
        // In real implementation, delete multiple errors
        await Task.Delay(500);
        return true;
    }

    public async Task<bool> ClearOldErrorsAsync(DateTime olderThan)
    {
        // In real implementation, delete old error logs
        await Task.Delay(1000);
        return true;
    }

    public async Task<List<ErrorLogVM>> SearchErrorsAsync(string query, int page = 1, int pageSize = 20)
    {
        var allErrors = await GetErrorLogsAsync(1, 100);
        var filteredErrors = allErrors.Where(e => 
            e.Message.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            e.Source.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            (e.Exception?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return filteredErrors;
    }

    public async Task<bool> ExportErrorLogsAsync(DateTime startDate, DateTime endDate, string format = "csv")
    {
        // In real implementation, export error logs to file
        await Task.Delay(2000);
        return true;
    }
}