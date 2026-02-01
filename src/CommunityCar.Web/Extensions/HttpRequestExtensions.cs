using Microsoft.AspNetCore.Http;

namespace CommunityCar.Web.Extensions;

/// <summary>
/// Extension methods for HttpRequest to simplify request handling
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Checks if the request is an AJAX request
    /// </summary>
    public static bool IsAjaxRequest(this HttpRequest request)
    {
        return request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }

    /// <summary>
    /// Checks if the request is an API request
    /// </summary>
    public static bool IsApiRequest(this HttpRequest request)
    {
        return request.Path.StartsWithSegments("/api") ||
               request.Headers.Accept.Any(h => h?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true) ||
               request.IsAjaxRequest();
    }

    /// <summary>
    /// Checks if the request accepts JSON
    /// </summary>
    public static bool AcceptsJson(this HttpRequest request)
    {
        return request.Headers.Accept.Any(h => h?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true);
    }

    /// <summary>
    /// Checks if the request is from a mobile device
    /// </summary>
    public static bool IsMobileRequest(this HttpRequest request)
    {
        var userAgent = request.Headers["User-Agent"].ToString().ToLower();
        return userAgent.Contains("mobile") || 
               userAgent.Contains("android") || 
               userAgent.Contains("iphone") || 
               userAgent.Contains("ipad") ||
               userAgent.Contains("tablet");
    }

    /// <summary>
    /// Gets the client IP address
    /// </summary>
    public static string? GetClientIpAddress(this HttpRequest request)
    {
        // Check for forwarded IP first (for load balancers/proxies)
        var forwardedFor = request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIp = request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        return request.HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    /// <summary>
    /// Gets the user agent string
    /// </summary>
    public static string? GetUserAgent(this HttpRequest request)
    {
        return request.Headers["User-Agent"].FirstOrDefault();
    }

    /// <summary>
    /// Gets the referer URL
    /// </summary>
    public static string? GetReferer(this HttpRequest request)
    {
        return request.Headers["Referer"].FirstOrDefault();
    }

    /// <summary>
    /// Checks if the request is secure (HTTPS)
    /// </summary>
    public static bool IsSecure(this HttpRequest request)
    {
        return request.IsHttps;
    }

    /// <summary>
    /// Gets the full URL of the request
    /// </summary>
    public static string GetFullUrl(this HttpRequest request)
    {
        return $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}";
    }

    /// <summary>
    /// Gets a query parameter value
    /// </summary>
    public static string? GetQueryValue(this HttpRequest request, string key)
    {
        return request.Query[key].FirstOrDefault();
    }

    /// <summary>
    /// Checks if a query parameter exists
    /// </summary>
    public static bool HasQuery(this HttpRequest request, string key)
    {
        return request.Query.ContainsKey(key);
    }
}