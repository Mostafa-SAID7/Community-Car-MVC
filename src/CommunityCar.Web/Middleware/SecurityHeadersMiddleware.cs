using Microsoft.Extensions.Primitives;

namespace CommunityCar.Web.Middleware;

/// <summary>
/// Middleware to add security headers to HTTP responses
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;
    private readonly SecurityHeadersOptions _options;

    public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger, SecurityHeadersOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add security headers before processing the request
        AddSecurityHeaders(context);

        await _next(context);
    }

    private void AddSecurityHeaders(HttpContext context)
    {
        var response = context.Response;
        var headers = response.Headers;

        try
        {
            // X-Content-Type-Options
            if (_options.AddXContentTypeOptions && !headers.ContainsKey("X-Content-Type-Options"))
            {
                headers.Append("X-Content-Type-Options", "nosniff");
            }

            // X-Frame-Options
            if (_options.AddXFrameOptions && !headers.ContainsKey("X-Frame-Options"))
            {
                headers.Append("X-Frame-Options", _options.XFrameOptionsValue);
            }

            // X-XSS-Protection
            if (_options.AddXXssProtection && !headers.ContainsKey("X-XSS-Protection"))
            {
                headers.Append("X-XSS-Protection", "1; mode=block");
            }

            // Strict-Transport-Security (HSTS)
            if (_options.AddStrictTransportSecurity && context.Request.IsHttps && !headers.ContainsKey("Strict-Transport-Security"))
            {
                headers.Append("Strict-Transport-Security", $"max-age={_options.HstsMaxAge}; includeSubDomains");
            }

            // Content-Security-Policy
            if (_options.AddContentSecurityPolicy && !string.IsNullOrEmpty(_options.ContentSecurityPolicyValue) && !headers.ContainsKey("Content-Security-Policy"))
            {
                headers.Append("Content-Security-Policy", _options.ContentSecurityPolicyValue);
            }

            // Referrer-Policy
            if (_options.AddReferrerPolicy && !headers.ContainsKey("Referrer-Policy"))
            {
                headers.Append("Referrer-Policy", _options.ReferrerPolicyValue);
            }

            // Permissions-Policy
            if (_options.AddPermissionsPolicy && !string.IsNullOrEmpty(_options.PermissionsPolicyValue) && !headers.ContainsKey("Permissions-Policy"))
            {
                headers.Append("Permissions-Policy", _options.PermissionsPolicyValue);
            }

            // Remove server header
            if (_options.RemoveServerHeader && headers.ContainsKey("Server"))
            {
                headers.Remove("Server");
            }

            // Add custom headers
            foreach (var customHeader in _options.CustomHeaders)
            {
                if (!headers.ContainsKey(customHeader.Key))
                {
                    headers.Append(customHeader.Key, customHeader.Value);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to add security headers");
        }
    }
}

/// <summary>
/// Configuration options for security headers middleware
/// </summary>
public class SecurityHeadersOptions
{
    public bool AddXContentTypeOptions { get; set; } = true;
    public bool AddXFrameOptions { get; set; } = true;
    public string XFrameOptionsValue { get; set; } = "DENY";
    public bool AddXXssProtection { get; set; } = true;
    public bool AddStrictTransportSecurity { get; set; } = true;
    public int HstsMaxAge { get; set; } = 31536000; // 1 year
    public bool AddContentSecurityPolicy { get; set; } = true;
    public string ContentSecurityPolicyValue { get; set; } = "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; font-src 'self' data:; connect-src 'self'; frame-ancestors 'none';";
    public bool AddReferrerPolicy { get; set; } = true;
    public string ReferrerPolicyValue { get; set; } = "strict-origin-when-cross-origin";
    public bool AddPermissionsPolicy { get; set; } = true;
    public string PermissionsPolicyValue { get; set; } = "camera=(), microphone=(), geolocation=()";
    public bool RemoveServerHeader { get; set; } = true;
    public Dictionary<string, string> CustomHeaders { get; set; } = new();
}