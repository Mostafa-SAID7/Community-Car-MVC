using System.Collections.Concurrent;
using System.Net;

namespace CommunityCar.Web.Middleware;

/// <summary>
/// Middleware for rate limiting HTTP requests
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly RateLimitingOptions _options;
    private readonly ConcurrentDictionary<string, ClientRequestInfo> _clients = new();
    private readonly Timer _cleanupTimer;

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, RateLimitingOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
        
        // Cleanup expired entries every minute
        _cleanupTimer = new Timer(CleanupExpiredEntries, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!ShouldApplyRateLimit(context))
        {
            await _next(context);
            return;
        }

        var clientId = GetClientIdentifier(context);
        var now = DateTime.UtcNow;

        var clientInfo = _clients.AddOrUpdate(clientId, 
            new ClientRequestInfo { WindowStart = now, RequestCount = 1 },
            (key, existing) => UpdateClientInfo(existing, now));

        if (clientInfo.RequestCount > _options.MaxRequests)
        {
            await HandleRateLimitExceeded(context, clientInfo);
            return;
        }

        // Add rate limit headers
        AddRateLimitHeaders(context, clientInfo);

        await _next(context);
    }

    private bool ShouldApplyRateLimit(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        
        // Skip rate limiting for excluded paths
        if (_options.ExcludedPaths.Any(excludedPath => path.Contains(excludedPath.ToLowerInvariant())))
        {
            return false;
        }

        // Skip rate limiting for static files if configured
        if (!_options.ApplyToStaticFiles && IsStaticFile(path))
        {
            return false;
        }

        // Skip rate limiting for authenticated users if configured
        if (!_options.ApplyToAuthenticatedUsers && context.User?.Identity?.IsAuthenticated == true)
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

    private string GetClientIdentifier(HttpContext context)
    {
        // Use user ID for authenticated users, IP address for anonymous users
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst("sub")?.Value ?? 
                        context.User.FindFirst("id")?.Value ?? 
                        context.User.FindFirst("userId")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                return $"user:{userId}";
            }
        }

        // Fall back to IP address
        var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        }
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = context.Connection.RemoteIpAddress?.ToString();
        }

        return $"ip:{ipAddress ?? "unknown"}";
    }

    private ClientRequestInfo UpdateClientInfo(ClientRequestInfo existing, DateTime now)
    {
        // Reset window if enough time has passed
        if (now - existing.WindowStart >= _options.WindowSize)
        {
            return new ClientRequestInfo { WindowStart = now, RequestCount = 1 };
        }

        // Increment request count within the same window
        existing.RequestCount++;
        return existing;
    }

    private async Task HandleRateLimitExceeded(HttpContext context, ClientRequestInfo clientInfo)
    {
        var retryAfter = (int)(_options.WindowSize - (DateTime.UtcNow - clientInfo.WindowStart)).TotalSeconds;
        
        context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
        context.Response.Headers.Append("Retry-After", retryAfter.ToString());
        
        _logger.LogWarning("Rate limit exceeded for client {ClientId}. Requests: {RequestCount}, Window: {WindowStart}", 
            GetClientIdentifier(context), clientInfo.RequestCount, clientInfo.WindowStart);

        var isApiRequest = context.Request.Path.StartsWithSegments("/api") || 
                          context.Request.Headers.Accept.Any(a => a?.Contains("application/json") == true);

        if (isApiRequest)
        {
            context.Response.ContentType = "application/json";
            var response = new
            {
                error = "Rate limit exceeded",
                message = $"Too many requests. Try again in {retryAfter} seconds.",
                retryAfter = retryAfter
            };
            
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
        }
        else
        {
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync($@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Rate Limit Exceeded</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; text-align: center; margin-top: 50px; }}
                        .container {{ max-width: 500px; margin: 0 auto; }}
                        .error-code {{ font-size: 72px; color: #e74c3c; margin-bottom: 20px; }}
                        .error-message {{ font-size: 24px; color: #333; margin-bottom: 20px; }}
                        .retry-info {{ font-size: 16px; color: #666; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='error-code'>429</div>
                        <div class='error-message'>Too Many Requests</div>
                        <div class='retry-info'>Please try again in {retryAfter} seconds.</div>
                    </div>
                </body>
                </html>");
        }
    }

    private void AddRateLimitHeaders(HttpContext context, ClientRequestInfo clientInfo)
    {
        var remaining = Math.Max(0, _options.MaxRequests - clientInfo.RequestCount);
        var resetTime = clientInfo.WindowStart.Add(_options.WindowSize);
        
        context.Response.Headers.Append("X-RateLimit-Limit", _options.MaxRequests.ToString());
        context.Response.Headers.Append("X-RateLimit-Remaining", remaining.ToString());
        context.Response.Headers.Append("X-RateLimit-Reset", ((DateTimeOffset)resetTime).ToUnixTimeSeconds().ToString());
    }

    private void CleanupExpiredEntries(object? state)
    {
        try
        {
            var cutoff = DateTime.UtcNow - _options.WindowSize - TimeSpan.FromMinutes(5); // Add 5 minute buffer
            var expiredKeys = _clients
                .Where(kvp => kvp.Value.WindowStart < cutoff)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _clients.TryRemove(key, out _);
            }

            if (expiredKeys.Count > 0)
            {
                _logger.LogDebug("Cleaned up {Count} expired rate limit entries", expiredKeys.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cleanup expired rate limit entries");
        }
    }

    public void Dispose()
    {
        _cleanupTimer?.Dispose();
    }
}

/// <summary>
/// Information about a client's requests within a time window
/// </summary>
public class ClientRequestInfo
{
    public DateTime WindowStart { get; set; }
    public int RequestCount { get; set; }
}

/// <summary>
/// Configuration options for rate limiting middleware
/// </summary>
public class RateLimitingOptions
{
    public int MaxRequests { get; set; } = 100;
    public TimeSpan WindowSize { get; set; } = TimeSpan.FromMinutes(1);
    public bool ApplyToStaticFiles { get; set; } = false;
    public bool ApplyToAuthenticatedUsers { get; set; } = true;
    public List<string> ExcludedPaths { get; set; } = new()
    {
        "/health",
        "/metrics"
    };
}