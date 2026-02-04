using CommunityCar.Application.Common.Interfaces.Services;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.ErrorReporting;
using CommunityCar.Web.Extensions;
using System.Net;
using System.Text.Json;

namespace CommunityCar.Web.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IServiceProvider serviceProvider)
    {
        _next = next;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            Console.WriteLine($"[CRITICAL ERROR]: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var errorId = Guid.NewGuid().ToString();
        
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var errorService = scope.ServiceProvider.GetService<IErrorService>();
            
            if (errorService != null)
            {
                var userId = context.User?.Identity?.IsAuthenticated == true 
                    ? context.User.FindFirst("sub")?.Value ?? context.User.FindFirst("id")?.Value
                    : null;

                var additionalContext = JsonSerializer.Serialize(new
                {
                    RequestPath = context.Request.Path.Value,
                    RequestMethod = context.Request.Method,
                    QueryString = context.Request.QueryString.Value,
                    Headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                    UserAgent = context.Request.Headers.UserAgent.ToString(),
                    IpAddress = GetClientIpAddress(context),
                    Timestamp = DateTime.UtcNow
                });

                await errorService.LogErrorAsync(
                    exception, 
                    $"User: {userId}, Path: {context.Request.Path.Value}, Context: {JsonSerializer.Serialize(additionalContext)}");
            }
        }
        catch (Exception logEx)
        {
            _logger.LogError(logEx, "Failed to log error to database");
        }

        var (statusCode, message) = GetErrorResponse(exception);
        
        var errorResponse = new ErrorResponse
        {
            ErrorId = errorId,
            Message = message,
            StatusCode = (int)statusCode,
            Timestamp = DateTime.UtcNow,
            Path = context.Request.Path.Value ?? "",
            Method = context.Request.Method
        };

        // Add detailed error info in development
        if (IsDevelopment())
        {
            errorResponse.Details = exception.Message;
            errorResponse.StackTrace = exception.StackTrace;
        }

        // Check if this is an API request or browser request
        var isApiRequest = context.Request.IsApiRequest();

        if (isApiRequest)
        {
            // Return JSON response for API requests
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)statusCode;

            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(jsonResponse);
        }
        else
        {
            // Redirect to styled error page for browser requests
            var errorDataJson = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            var encodedErrorData = Uri.EscapeDataString(errorDataJson);
            var redirectUrl = $"/Error/Details/{errorId}?data={encodedErrorData}";
            
            context.Response.Redirect(redirectUrl);
        }
    }

    private static (HttpStatusCode statusCode, string message) GetErrorResponse(Exception exception)
    {
        return exception switch
        {
            ArgumentException or ArgumentNullException => (HttpStatusCode.BadRequest, "Invalid request parameters"),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Access denied"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            TimeoutException => (HttpStatusCode.RequestTimeout, "Request timeout"),
            NotImplementedException => (HttpStatusCode.NotImplemented, "Feature not implemented"),
            InvalidOperationException => (HttpStatusCode.Conflict, "Invalid operation"),
            _ => (HttpStatusCode.InternalServerError, "An internal server error occurred")
        };
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

    private static bool IsDevelopment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
    }
}

public class ErrorResponse
{
    public string ErrorId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; }
    public string Path { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? StackTrace { get; set; }
}


