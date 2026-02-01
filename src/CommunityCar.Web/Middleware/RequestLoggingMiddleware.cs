using System.Diagnostics;
using System.Text;

namespace CommunityCar.Web.Middleware;

/// <summary>
/// Middleware for logging HTTP requests and responses
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly RequestLoggingOptions _options;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, RequestLoggingOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!ShouldLog(context))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString("N")[..8];
        
        // Log request
        await LogRequestAsync(context, requestId);

        // Capture response
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            // Log response
            await LogResponseAsync(context, requestId, stopwatch.ElapsedMilliseconds, responseBody);
            
            // Copy response back to original stream
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }

    private bool ShouldLog(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        
        // Skip logging for excluded paths
        if (_options.ExcludedPaths.Any(excludedPath => path.Contains(excludedPath.ToLowerInvariant())))
        {
            return false;
        }

        // Skip logging for static files if configured
        if (!_options.LogStaticFiles && IsStaticFile(path))
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

    private async Task LogRequestAsync(HttpContext context, string requestId)
    {
        try
        {
            var request = context.Request;
            var logData = new
            {
                RequestId = requestId,
                Method = request.Method,
                Path = request.Path.Value,
                QueryString = request.QueryString.Value,
                UserAgent = request.Headers.UserAgent.ToString(),
                IpAddress = GetClientIpAddress(context),
                UserId = GetUserId(context),
                ContentType = request.ContentType,
                ContentLength = request.ContentLength,
                Headers = _options.LogHeaders ? GetSafeHeaders(request.Headers) : null,
                Body = _options.LogRequestBody ? await GetRequestBodyAsync(request) : null
            };

            _logger.LogInformation("HTTP Request: {@RequestData}", logData);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to log request for RequestId: {RequestId}", requestId);
        }
    }

    private async Task LogResponseAsync(HttpContext context, string requestId, long elapsedMs, MemoryStream responseBody)
    {
        try
        {
            var response = context.Response;
            var logData = new
            {
                RequestId = requestId,
                StatusCode = response.StatusCode,
                ElapsedMs = elapsedMs,
                ContentType = response.ContentType,
                ContentLength = response.ContentLength ?? responseBody.Length,
                Headers = _options.LogHeaders ? GetSafeHeaders(response.Headers) : null,
                Body = _options.LogResponseBody ? await GetResponseBodyAsync(responseBody) : null
            };

            var logLevel = response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;
            _logger.Log(logLevel, "HTTP Response: {@ResponseData}", logData);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to log response for RequestId: {RequestId}", requestId);
        }
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

    private static Dictionary<string, string> GetSafeHeaders(IHeaderDictionary headers)
    {
        var sensitiveHeaders = new[] { "authorization", "cookie", "x-api-key", "x-auth-token" };
        
        return headers
            .Where(h => !sensitiveHeaders.Contains(h.Key.ToLowerInvariant()))
            .ToDictionary(h => h.Key, h => h.Value.ToString());
    }

    private async Task<string?> GetRequestBodyAsync(HttpRequest request)
    {
        if (!_options.LogRequestBody || request.ContentLength == 0 || request.ContentLength > _options.MaxBodyLogSize)
        {
            return null;
        }

        try
        {
            request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            request.Body.Position = 0;
            
            return Encoding.UTF8.GetString(buffer);
        }
        catch
        {
            return "[Failed to read request body]";
        }
    }

    private async Task<string?> GetResponseBodyAsync(MemoryStream responseBody)
    {
        if (!_options.LogResponseBody || responseBody.Length == 0 || responseBody.Length > _options.MaxBodyLogSize)
        {
            return null;
        }

        try
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[responseBody.Length];
            await responseBody.ReadAsync(buffer, 0, buffer.Length);
            responseBody.Seek(0, SeekOrigin.Begin);
            
            return Encoding.UTF8.GetString(buffer);
        }
        catch
        {
            return "[Failed to read response body]";
        }
    }
}

/// <summary>
/// Configuration options for request logging middleware
/// </summary>
public class RequestLoggingOptions
{
    public bool LogHeaders { get; set; } = false;
    public bool LogRequestBody { get; set; } = false;
    public bool LogResponseBody { get; set; } = false;
    public bool LogStaticFiles { get; set; } = false;
    public long MaxBodyLogSize { get; set; } = 4096; // 4KB
    public List<string> ExcludedPaths { get; set; } = new()
    {
        "/health",
        "/metrics",
        "/favicon.ico"
    };
}