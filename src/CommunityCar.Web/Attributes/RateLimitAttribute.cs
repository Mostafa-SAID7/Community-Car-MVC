using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Concurrent;
using System.Net;

namespace CommunityCar.Web.Attributes;

/// <summary>
/// Rate limiting attribute for actions
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RateLimitAttribute : ActionFilterAttribute
{
    private static readonly ConcurrentDictionary<string, RateLimitInfo> _rateLimitStore = new();
    private static readonly Timer _cleanupTimer = new(CleanupExpiredEntries, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));

    /// <summary>
    /// Maximum number of requests allowed
    /// </summary>
    public int MaxVMs { get; set; } = 10;

    /// <summary>
    /// Time window in seconds
    /// </summary>
    public int WindowSeconds { get; set; } = 60;

    /// <summary>
    /// Whether to apply rate limiting per user (if authenticated) or per IP
    /// </summary>
    public bool PerUser { get; set; } = true;

    /// <summary>
    /// Custom error message
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// HTTP status code to return when rate limit is exceeded
    /// </summary>
    public int StatusCode { get; set; } = 429;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var key = GenerateKey(context);
        var now = DateTime.UtcNow;
        var windowStart = now.AddSeconds(-WindowSeconds);

        var rateLimitInfo = _rateLimitStore.AddOrUpdate(key,
            new RateLimitInfo { Count = 1, WindowStart = now },
            (k, existing) =>
            {
                if (existing.WindowStart < windowStart)
                {
                    // Reset window
                    return new RateLimitInfo { Count = 1, WindowStart = now };
                }
                else
                {
                    // Increment count
                    existing.Count++;
                    return existing;
                }
            });

        if (rateLimitInfo.Count > MaxVMs)
        {
            var retryAfter = (int)(WindowSeconds - (now - rateLimitInfo.WindowStart).TotalSeconds);
            
            context.HttpContext.Response.Headers.Append("Retry-After", retryAfter.ToString());
            context.HttpContext.Response.Headers.Append("X-RateLimit-Limit", MaxVMs.ToString());
            context.HttpContext.Response.Headers.Append("X-RateLimit-Remaining", "0");
            context.HttpContext.Response.Headers.Append("X-RateLimit-Reset", 
                ((DateTimeOffset)rateLimitInfo.WindowStart.AddSeconds(WindowSeconds)).ToUnixTimeSeconds().ToString());

            var isAjaxVM = context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpVM" ||
                               context.HttpContext.Request.Headers.Accept.Any(a => a?.Contains("application/json") == true);

            if (isAjaxVM)
            {
                context.Result = new JsonResult(new
                {
                    error = ErrorMessage ?? "Rate limit exceeded",
                    retryAfter = retryAfter
                })
                {
                    StatusCode = StatusCode
                };
            }
            else
            {
                context.Result = new ContentResult
                {
                    Content = ErrorMessage ?? "Rate limit exceeded. Please try again later.",
                    StatusCode = StatusCode,
                    ContentType = "text/plain"
                };
            }
        }
        else
        {
            // Add rate limit headers
            var remaining = Math.Max(0, MaxVMs - rateLimitInfo.Count);
            context.HttpContext.Response.Headers.Append("X-RateLimit-Limit", MaxVMs.ToString());
            context.HttpContext.Response.Headers.Append("X-RateLimit-Remaining", remaining.ToString());
            context.HttpContext.Response.Headers.Append("X-RateLimit-Reset", 
                ((DateTimeOffset)rateLimitInfo.WindowStart.AddSeconds(WindowSeconds)).ToUnixTimeSeconds().ToString());
        }

        base.OnActionExecuting(context);
    }

    private string GenerateKey(ActionExecutingContext context)
    {
        var keyBuilder = new System.Text.StringBuilder();
        
        // Add controller and action
        keyBuilder.Append(context.RouteData.Values["controller"])
                  .Append(":")
                  .Append(context.RouteData.Values["action"]);

        if (PerUser && context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            // Use user ID for authenticated users
            var userId = context.HttpContext.User.FindFirst("sub")?.Value ?? 
                        context.HttpContext.User.FindFirst("id")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                keyBuilder.Append(":user:").Append(userId);
            }
        }
        else
        {
            // Use IP address for anonymous users
            var ipAddress = GetClientIpAddress(context.HttpContext);
            keyBuilder.Append(":ip:").Append(ipAddress);
        }

        return keyBuilder.ToString();
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
        return ipAddress ?? "unknown";
    }

    private static void CleanupExpiredEntries(object? state)
    {
        var cutoff = DateTime.UtcNow.AddMinutes(-10); // Clean entries older than 10 minutes
        var expiredKeys = _rateLimitStore
            .Where(kvp => kvp.Value.WindowStart < cutoff)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            _rateLimitStore.TryRemove(key, out _);
        }
    }

    private class RateLimitInfo
    {
        public int Count { get; set; }
        public DateTime WindowStart { get; set; }
    }
}

/// <summary>
/// Strict rate limiting for sensitive operations
/// </summary>
public class StrictRateLimitAttribute : RateLimitAttribute
{
    public StrictRateLimitAttribute()
    {
        MaxVMs = 3;
        WindowSeconds = 300; // 5 minutes
        PerUser = true;
        ErrorMessage = "Too many attempts. Please wait before trying again.";
    }
}

/// <summary>
/// Lenient rate limiting for general operations
/// </summary>
public class LenientRateLimitAttribute : RateLimitAttribute
{
    public LenientRateLimitAttribute()
    {
        MaxVMs = 100;
        WindowSeconds = 60; // 1 minute
        PerUser = false;
    }
}