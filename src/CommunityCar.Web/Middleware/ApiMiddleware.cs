using System.Text.Json;

namespace CommunityCar.Web.Middleware;

/// <summary>
/// Middleware for handling API-specific concerns
/// </summary>
public class ApiMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiMiddleware> _logger;
    private readonly ApiMiddlewareOptions _options;

    public ApiMiddleware(RequestDelegate next, ILogger<ApiMiddleware> logger, ApiMiddlewareOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only apply to API requests
        if (!IsApiRequest(context))
        {
            await _next(context);
            return;
        }

        // Set API-specific headers
        SetApiHeaders(context);

        // Handle API versioning
        HandleApiVersioning(context);

        // Validate API key if required
        if (_options.RequireApiKey && !await ValidateApiKey(context))
        {
            await WriteUnauthorizedResponse(context, "Invalid or missing API key");
            return;
        }

        // Handle CORS for API requests
        if (_options.EnableCors)
        {
            HandleCors(context);
        }

        // Handle preflight requests
        if (context.Request.Method == "OPTIONS")
        {
            context.Response.StatusCode = 200;
            return;
        }

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleApiException(context, ex);
        }
    }

    private bool IsApiRequest(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        
        // Check if path starts with API prefix
        if (_options.ApiPrefixes.Any(prefix => path.StartsWith(prefix.ToLowerInvariant())))
        {
            return true;
        }

        // Check Accept header
        var acceptHeader = context.Request.Headers.Accept.ToString();
        if (acceptHeader.Contains("application/json"))
        {
            return true;
        }

        // Check Content-Type header
        var contentType = context.Request.ContentType?.ToLowerInvariant() ?? "";
        if (contentType.Contains("application/json"))
        {
            return true;
        }

        return false;
    }

    private void SetApiHeaders(HttpContext context)
    {
        var response = context.Response;
        
        // Set default content type for API responses
        if (string.IsNullOrEmpty(response.ContentType))
        {
            response.ContentType = "application/json";
        }

        // Add API version header
        if (!string.IsNullOrEmpty(_options.DefaultApiVersion))
        {
            response.Headers.Append("X-API-Version", _options.DefaultApiVersion);
        }

        // Add custom API headers
        foreach (var header in _options.CustomHeaders)
        {
            response.Headers.Append(header.Key, header.Value);
        }
    }

    private void HandleApiVersioning(HttpContext context)
    {
        var version = GetRequestedApiVersion(context);
        if (!string.IsNullOrEmpty(version))
        {
            context.Items["ApiVersion"] = version;
        }
        else
        {
            context.Items["ApiVersion"] = _options.DefaultApiVersion;
        }
    }

    private string? GetRequestedApiVersion(HttpContext context)
    {
        // Check header
        var versionHeader = context.Request.Headers["X-API-Version"].FirstOrDefault();
        if (!string.IsNullOrEmpty(versionHeader))
        {
            return versionHeader;
        }

        // Check query parameter
        var versionQuery = context.Request.Query["version"].FirstOrDefault();
        if (!string.IsNullOrEmpty(versionQuery))
        {
            return versionQuery;
        }

        // Check path (e.g., /api/v1/users)
        var path = context.Request.Path.Value ?? "";
        var versionMatch = System.Text.RegularExpressions.Regex.Match(path, @"/v(\d+(?:\.\d+)?)/");
        if (versionMatch.Success)
        {
            return versionMatch.Groups[1].Value;
        }

        return null;
    }

    private async Task<bool> ValidateApiKey(HttpContext context)
    {
        var apiKey = GetApiKey(context);
        if (string.IsNullOrEmpty(apiKey))
        {
            return false;
        }

        // In a real implementation, you would validate against a database or service
        // For now, we'll just check against configured keys
        return _options.ValidApiKeys.Contains(apiKey);
    }

    private string? GetApiKey(HttpContext context)
    {
        // Check Authorization header (Bearer token)
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring("Bearer ".Length);
        }

        // Check X-API-Key header
        var apiKeyHeader = context.Request.Headers["X-API-Key"].FirstOrDefault();
        if (!string.IsNullOrEmpty(apiKeyHeader))
        {
            return apiKeyHeader;
        }

        // Check query parameter
        var apiKeyQuery = context.Request.Query["api_key"].FirstOrDefault();
        if (!string.IsNullOrEmpty(apiKeyQuery))
        {
            return apiKeyQuery;
        }

        return null;
    }

    private void HandleCors(HttpContext context)
    {
        var origin = context.Request.Headers.Origin.FirstOrDefault();
        
        if (!string.IsNullOrEmpty(origin))
        {
            // Check if origin is allowed
            var isAllowed = _options.AllowedOrigins.Contains("*") || 
                           _options.AllowedOrigins.Contains(origin);

            if (isAllowed)
            {
                context.Response.Headers.Append("Access-Control-Allow-Origin", origin);
                context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
            }
        }

        // Add other CORS headers
        context.Response.Headers.Append("Access-Control-Allow-Methods", string.Join(", ", _options.AllowedMethods));
        context.Response.Headers.Append("Access-Control-Allow-Headers", string.Join(", ", _options.AllowedHeaders));
        context.Response.Headers.Append("Access-Control-Max-Age", _options.PreflightMaxAge.ToString());
    }

    private async Task HandleApiException(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "API request failed: {Method} {Path}", 
            context.Request.Method, context.Request.Path);

        var (statusCode, message) = GetApiErrorResponse(exception);
        
        var errorResponse = new
        {
            error = new
            {
                code = statusCode,
                message = message,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.Value,
                method = context.Request.Method,
                traceId = context.TraceIdentifier
            }
        };

        // Add detailed error info in development
        if (_options.IncludeExceptionDetails)
        {
            var detailedErrorResponse = new
            {
                error = new
                {
                    code = statusCode,
                    message = message,
                    details = exception.Message,
                    stackTrace = exception.StackTrace,
                    timestamp = DateTime.UtcNow,
                    path = context.Request.Path.Value,
                    method = context.Request.Method,
                    traceId = context.TraceIdentifier
                }
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(detailedErrorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _options.PrettyPrintJson
            });

            await context.Response.WriteAsync(json);
        }
        else
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _options.PrettyPrintJson
            });

            await context.Response.WriteAsync(json);
        }
    }

    private static (int statusCode, string message) GetApiErrorResponse(Exception exception)
    {
        return exception switch
        {
            ArgumentException or ArgumentNullException => (400, "Bad Request"),
            UnauthorizedAccessException => (401, "Unauthorized"),
            KeyNotFoundException => (404, "Not Found"),
            TimeoutException => (408, "Request Timeout"),
            NotImplementedException => (501, "Not Implemented"),
            InvalidOperationException => (409, "Conflict"),
            _ => (500, "Internal Server Error")
        };
    }

    private async Task WriteUnauthorizedResponse(HttpContext context, string message)
    {
        context.Response.StatusCode = 401;
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = new
            {
                code = 401,
                message = message,
                timestamp = DateTime.UtcNow
            }
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

/// <summary>
/// Configuration options for API middleware
/// </summary>
public class ApiMiddlewareOptions
{
    public List<string> ApiPrefixes { get; set; } = new() { "/api" };
    public string DefaultApiVersion { get; set; } = "1.0";
    public bool RequireApiKey { get; set; } = false;
    public List<string> ValidApiKeys { get; set; } = new();
    public bool EnableCors { get; set; } = true;
    public List<string> AllowedOrigins { get; set; } = new() { "*" };
    public List<string> AllowedMethods { get; set; } = new() { "GET", "POST", "PUT", "DELETE", "OPTIONS" };
    public List<string> AllowedHeaders { get; set; } = new() { "Content-Type", "Authorization", "X-API-Key", "X-API-Version" };
    public int PreflightMaxAge { get; set; } = 86400; // 24 hours
    public bool IncludeExceptionDetails { get; set; } = false;
    public bool PrettyPrintJson { get; set; } = false;
    public Dictionary<string, string> CustomHeaders { get; set; } = new();
}