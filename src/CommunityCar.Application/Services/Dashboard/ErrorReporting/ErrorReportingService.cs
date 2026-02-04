using CommunityCar.Application.Common.Interfaces.Services.Dashboard.ErrorReporting;
using CommunityCar.Application.Features.Dashboard.ErrorReporting.ViewModels;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Services.Dashboard.ErrorReporting;

public class ErrorReportingService : IErrorReportingService
{
    public async Task<List<ErrorReportVM>> GetErrorReportsAsync(int page = 1, int pageSize = 20, string? severity = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var errors = new List<ErrorReportVM>();
        var random = new Random();
        var severities = new[] { "Low", "Medium", "High", "Critical" };
        var errorTypes = new[] { "Application Error", "Database Error", "Network Error", "Validation Error", "Security Error" };
        var sources = new[] { "Web Application", "API", "Background Service", "Database", "External Service" };

        var start = startDate ?? DateTime.UtcNow.AddDays(-30);
        var end = endDate ?? DateTime.UtcNow;

        for (int i = 0; i < pageSize; i++)
        {
            var errorSeverity = severity ?? severities[random.Next(severities.Length)];
            var timestamp = start.AddMinutes(random.Next(0, (int)(end - start).TotalMinutes));
            
            errors.Add(new ErrorReportVM
            {
                Id = Guid.NewGuid(),
                Title = $"{errorTypes[random.Next(errorTypes.Length)]} #{i + 1}",
                Message = $"Sample error message for error #{i + 1}",
                Severity = errorSeverity,
                Source = sources[random.Next(sources.Length)],
                ErrorType = errorTypes[random.Next(errorTypes.Length)],
                Timestamp = timestamp,
                UserId = random.Next(3) == 0 ? Guid.NewGuid() : null,
                UserName = random.Next(3) == 0 ? $"user{random.Next(1, 100)}" : null,
                IpAddress = $"{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                RequestPath = $"/api/endpoint{random.Next(1, 10)}",
                HttpMethod = new[] { "GET", "POST", "PUT", "DELETE" }[random.Next(4)],
                StatusCode = new[] { 400, 401, 403, 404, 500, 502, 503 }[random.Next(7)],
                StackTrace = $"   at System.Example.Method() in Example.cs:line {random.Next(10, 100)}\n   at System.Another.Method() in Another.cs:line {random.Next(10, 100)}",
                InnerException = random.Next(3) == 0 ? "Inner exception details here" : null,
                AdditionalData = new Dictionary<string, object>
                {
                    { "RequestId", Guid.NewGuid().ToString() },
                    { "CorrelationId", Guid.NewGuid().ToString() },
                    { "ResponseTime", random.Next(100, 5000) }
                },
                IsResolved = random.Next(4) == 0, // 25% resolved
                ResolvedAt = null,
                ResolvedBy = null,
                Resolution = null,
                OccurrenceCount = random.Next(1, 50),
                FirstOccurrence = timestamp.AddDays(-random.Next(0, 7)),
                LastOccurrence = timestamp,
                AffectedUsers = random.Next(1, 100),
                Environment = "Development",
                Version = "1.0.0",
                Tags = new List<string> { "error", errorSeverity.ToLower(), sources[random.Next(sources.Length)].ToLower().Replace(" ", "-") }
            });
        }

        return await Task.FromResult(errors.OrderByDescending(e => e.Timestamp).ToList());
    }

    public async Task<ErrorReportVM?> GetErrorReportByIdAsync(Guid errorId)
    {
        var errors = await GetErrorReportsAsync(1, 100);
        return errors.FirstOrDefault(e => e.Id == errorId);
    }

    public async Task<Guid> CreateErrorReportAsync(CreateErrorReportVM request)
    {
        // In real implementation, save error report to database
        await Task.Delay(100);
        return Guid.NewGuid();
    }

    public async Task<bool> ResolveErrorReportAsync(Guid errorId, string resolution, string resolvedBy)
    {
        // In real implementation, mark error as resolved
        await Task.Delay(100);
        return true;
    }

    public async Task<bool> DeleteErrorReportAsync(Guid errorId)
    {
        // In real implementation, delete error report
        await Task.Delay(100);
        return true;
    }

    public async Task<ErrorStatisticsVM> GetErrorStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var random = new Random();
        var start = startDate ?? DateTime.UtcNow.AddDays(-30);
        var end = endDate ?? DateTime.UtcNow;
        var days = (int)(end - start).TotalDays;

        return new ErrorStatisticsVM
        {
            TotalErrors = random.Next(100, 1000),
            CriticalErrors = random.Next(5, 50),
            HighSeverityErrors = random.Next(20, 100),
            MediumSeverityErrors = random.Next(30, 200),
            LowSeverityErrors = random.Next(50, 500),
            ResolvedErrors = random.Next(80, 800),
            UnresolvedErrors = random.Next(20, 200),
            ErrorRate = (double)(random.NextDouble() * 0.05), // 0-5%
            AverageResolutionTime = TimeSpan.FromHours(random.Next(1, 48)),
            MostCommonErrorType = "Application Error",
            MostAffectedEndpoint = "/api/posts",
            ErrorsByDay = await GetErrorTrendChartAsync(days),
            ErrorsBySeverity = new Dictionary<string, int>
            {
                { "Critical", random.Next(5, 50) },
                { "High", random.Next(20, 100) },
                { "Medium", random.Next(30, 200) },
                { "Low", random.Next(50, 500) }
            },
            ErrorsByType = new Dictionary<string, int>
            {
                { "Application Error", random.Next(50, 300) },
                { "Database Error", random.Next(20, 100) },
                { "Network Error", random.Next(10, 50) },
                { "Validation Error", random.Next(30, 150) },
                { "Security Error", random.Next(5, 25) }
            },
            TopErrorSources = new Dictionary<string, int>
            {
                { "Web Application", random.Next(100, 400) },
                { "API", random.Next(50, 200) },
                { "Background Service", random.Next(20, 100) },
                { "Database", random.Next(10, 50) }
            }.Select(kvp => kvp.Key).ToList()
        };
    }

    public async Task<List<ChartDataVM>> GetErrorTrendChartAsync(int days = 30)
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
                Value = random.Next(0, 50), // Errors per day
                Date = date
            });
        }

        return await Task.FromResult(data);
    }

    public async Task<List<ErrorReportVM>> GetTopErrorsAsync(int limit = 10, DateTime? startDate = null, DateTime? endDate = null)
    {
        var allErrors = await GetErrorReportsAsync(1, 100, null, startDate, endDate);
        return allErrors.OrderByDescending(e => e.OccurrenceCount)
                       .Take(limit)
                       .ToList();
    }

    public async Task<List<ErrorReportVM>> GetRecentErrorsAsync(int limit = 20)
    {
        var allErrors = await GetErrorReportsAsync(1, limit);
        return allErrors.Take(limit).ToList();
    }

    public async Task<bool> BulkResolveErrorsAsync(List<Guid> errorIds, string resolution, string resolvedBy)
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

    public async Task<List<ErrorReportVM>> SearchErrorsAsync(string query, int page = 1, int pageSize = 20)
    {
        var allErrors = await GetErrorReportsAsync(1, 100);
        var filteredErrors = allErrors.Where(e => 
            e.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            e.Message.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            e.Source.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return filteredErrors;
    }

    public async Task<bool> ExportErrorReportsAsync(DateTime startDate, DateTime endDate, string format = "csv")
    {
        // In real implementation, export error reports to file
        await Task.Delay(2000);
        return true;
    }

    public async Task<bool> ConfigureErrorAlertsAsync(ErrorAlertConfigVM config)
    {
        // In real implementation, configure error alerting
        await Task.Delay(200);
        return true;
    }

    public async Task<ErrorAlertConfigVM> GetErrorAlertConfigAsync()
    {
        return new ErrorAlertConfigVM
        {
            EnableEmailAlerts = true,
            AlertEmail = "alerts@communitycar.com",
            CriticalErrorThreshold = 1, // Alert immediately for critical errors
            HighErrorThreshold = 5, // Alert after 5 high severity errors
            MediumErrorThreshold = 20, // Alert after 20 medium severity errors
            AlertInterval = (int)TimeSpan.FromMinutes(15).TotalMinutes, // Don't spam alerts
            EnableSlackAlerts = false,
            SlackWebhookUrl = "",
            EnableSmsAlerts = false,
            SmsPhoneNumber = "",
            AlertOnNewErrorTypes = true,
            AlertOnErrorSpikes = true,
            ErrorSpikeThreshold = 50 // Alert if errors increase by 50% in an hour
        };
    }
}