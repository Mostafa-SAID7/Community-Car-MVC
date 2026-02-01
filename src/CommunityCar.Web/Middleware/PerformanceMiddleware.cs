using System.Diagnostics;

namespace CommunityCar.Web.Middleware;

/// <summary>
/// Middleware for monitoring and logging performance metrics
/// </summary>
public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMiddleware> _logger;
    private readonly PerformanceOptions _options;

    public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger, PerformanceOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!ShouldMonitor(context))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString("N")[..8];
        
        // Capture initial memory usage
        var initialMemory = GC.GetTotalMemory(false);
        
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            // Capture final memory usage
            var finalMemory = GC.GetTotalMemory(false);
            var memoryUsed = finalMemory - initialMemory;
            
            await LogPerformanceMetrics(context, requestId, stopwatch.ElapsedMilliseconds, memoryUsed);
            
            // Add performance headers if enabled
            if (_options.AddPerformanceHeaders)
            {
                context.Response.Headers.Append("X-Response-Time", $"{stopwatch.ElapsedMilliseconds}ms");
                context.Response.Headers.Append("X-Request-Id", requestId);
            }
        }
    }

    private bool ShouldMonitor(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        
        // Skip monitoring for excluded paths
        if (_options.ExcludedPaths.Any(excludedPath => path.Contains(excludedPath.ToLowerInvariant())))
        {
            return false;
        }

        // Skip monitoring for static files if configured
        if (!_options.MonitorStaticFiles && IsStaticFile(path))
        {
            return false;
        }

        return true;
    }

    private static bool IsStaticFile(string path)
    {
        var staticExtensions = new[] { ".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".ico", ".svg", ".woff", ".woff2", ".ttf", ".eot" };
        return staticExtensions.Any(ext => path.EndsWith(ext));
    }

    private async Task LogPerformanceMetrics(HttpContext context, string requestId, long elapsedMs, long memoryUsed)
    {
        try
        {
            var request = context.Request;
            var response = context.Response;
            
            var performanceData = new
            {
                RequestId = requestId,
                Method = request.Method,
                Path = request.Path.Value,
                StatusCode = response.StatusCode,
                ElapsedMs = elapsedMs,
                MemoryUsedBytes = memoryUsed,
                MemoryUsedKB = memoryUsed / 1024.0,
                UserId = GetUserId(context),
                UserAgent = request.Headers.UserAgent.ToString(),
                IpAddress = GetClientIpAddress(context),
                Timestamp = DateTime.UtcNow
            };

            // Determine log level based on performance thresholds
            var logLevel = DetermineLogLevel(elapsedMs, response.StatusCode);
            
            _logger.Log(logLevel, "Performance Metrics: {@PerformanceData}", performanceData);

            // Log slow requests separately
            if (elapsedMs > _options.SlowRequestThresholdMs)
            {
                _logger.LogWarning("Slow Request Detected: {Method} {Path} took {ElapsedMs}ms (RequestId: {RequestId})", 
                    request.Method, request.Path.Value, elapsedMs, requestId);
            }

            // Log high memory usage
            if (memoryUsed > _options.HighMemoryThresholdBytes)
            {
                _logger.LogWarning("High Memory Usage: {Method} {Path} used {MemoryUsedKB}KB (RequestId: {RequestId})", 
                    request.Method, request.Path.Value, memoryUsed / 1024.0, requestId);
            }

            // Store metrics for potential aggregation (could be sent to monitoring service)
            if (_options.EnableMetricsCollection)
            {
                await StoreMetricsAsync(performanceData);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to log performance metrics for RequestId: {RequestId}", requestId);
        }
    }

    private LogLevel DetermineLogLevel(long elapsedMs, int statusCode)
    {
        // Error responses
        if (statusCode >= 500)
            return LogLevel.Error;
        
        // Client errors
        if (statusCode >= 400)
            return LogLevel.Warning;
        
        // Slow requests
        if (elapsedMs > _options.SlowRequestThresholdMs)
            return LogLevel.Warning;
        
        // Normal requests
        return LogLevel.Information;
    }

    private async Task StoreMetricsAsync(object performanceData)
    {
        // This is a placeholder for storing metrics
        // In a real implementation, you might:
        // 1. Send to Application Insights
        // 2. Store in a time-series database
        // 3. Send to a monitoring service like Prometheus
        // 4. Store in a queue for batch processing
        
        await Task.CompletedTask; // Placeholder
    }

    private static string GetClientIpAddress(HttpContext context)
    {
        var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        }
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = context.Connection.RemoteIpAddress?.ToString();
        }
        return ipAddress ?? "Unknown";
    }

    private static string? GetUserId(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            return context.User.FindFirst("sub")?.Value ?? 
                   context.User.FindFirst("id")?.Value ?? 
                   context.User.FindFirst("userId")?.Value;
        }
        return null;
    }
}

/// <summary>
/// Configuration options for performance middleware
/// </summary>
public class PerformanceOptions
{
    public bool AddPerformanceHeaders { get; set; } = true;
    public bool MonitorStaticFiles { get; set; } = false;
    public bool EnableMetricsCollection { get; set; } = true;
    public long SlowRequestThresholdMs { get; set; } = 1000; // 1 second
    public long HighMemoryThresholdBytes { get; set; } = 10 * 1024 * 1024; // 10MB
    public List<string> ExcludedPaths { get; set; } = new()
    {
        "/health",
        "/metrics",
        "/favicon.ico"
    };
}