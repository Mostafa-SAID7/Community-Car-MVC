using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Text.Json;

namespace CommunityCar.Web.Attributes;

/// <summary>
/// Logs action execution details
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class LogActionAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Whether to log request parameters
    /// </summary>
    public bool LogParameters { get; set; } = true;

    /// <summary>
    /// Whether to log execution time
    /// </summary>
    public bool LogExecutionTime { get; set; } = true;

    /// <summary>
    /// Whether to log user information
    /// </summary>
    public bool LogUser { get; set; } = true;

    /// <summary>
    /// Whether to log request headers
    /// </summary>
    public bool LogHeaders { get; set; } = false;

    /// <summary>
    /// Custom log message prefix
    /// </summary>
    public string? MessagePrefix { get; set; }

    /// <summary>
    /// Log level for successful actions
    /// </summary>
    public LogLevel SuccessLogLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// Log level for failed actions
    /// </summary>
    public LogLevel ErrorLogLevel { get; set; } = LogLevel.Error;

    protected const string StopwatchKey = "ActionStopwatch";

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var logger = context.HttpContext.RequestServices.GetService<ILogger<LogActionAttribute>>();
        if (logger == null) return;

        var stopwatch = Stopwatch.StartNew();
        context.HttpContext.Items[StopwatchKey] = stopwatch;

        var logData = new Dictionary<string, object?>
        {
            ["Action"] = context.ActionDescriptor.DisplayName,
            ["Controller"] = context.RouteData.Values["controller"],
            ["ActionMethod"] = context.RouteData.Values["action"],
            ["RequestId"] = context.HttpContext.TraceIdentifier,
            ["Timestamp"] = DateTime.UtcNow
        };

        if (LogUser && context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            logData["UserId"] = GetUserId(context.HttpContext);
            logData["UserName"] = context.HttpContext.User.Identity.Name;
        }

        if (LogParameters && context.ActionArguments.Count > 0)
        {
            var parameters = new Dictionary<string, object?>();
            foreach (var param in context.ActionArguments)
            {
                // Avoid logging sensitive data
                if (IsSensitiveParameter(param.Key))
                {
                    parameters[param.Key] = "[REDACTED]";
                }
                else
                {
                    parameters[param.Key] = param.Value;
                }
            }
            logData["Parameters"] = parameters;
        }

        if (LogHeaders)
        {
            logData["Headers"] = GetSafeHeaders(context.HttpContext.Request.Headers);
        }

        var message = !string.IsNullOrEmpty(MessagePrefix) 
            ? $"{MessagePrefix} - Action Executing: {context.ActionDescriptor.DisplayName}"
            : $"Action Executing: {context.ActionDescriptor.DisplayName}";

        logger.Log(SuccessLogLevel, message + " {@LogData}", logData);

        base.OnActionExecuting(context);
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        var logger = context.HttpContext.RequestServices.GetService<ILogger<LogActionAttribute>>();
        if (logger == null) return;

        var stopwatch = context.HttpContext.Items[StopwatchKey] as Stopwatch;
        stopwatch?.Stop();

        var logData = new Dictionary<string, object?>
        {
            ["Action"] = context.ActionDescriptor.DisplayName,
            ["Controller"] = context.RouteData.Values["controller"],
            ["ActionMethod"] = context.RouteData.Values["action"],
            ["RequestId"] = context.HttpContext.TraceIdentifier,
            ["Timestamp"] = DateTime.UtcNow,
            ["Success"] = context.Exception == null
        };

        if (LogExecutionTime && stopwatch != null)
        {
            logData["ExecutionTimeMs"] = stopwatch.ElapsedMilliseconds;
        }

        if (context.Exception != null)
        {
            logData["Exception"] = new
            {
                Type = context.Exception.GetType().Name,
                Message = context.Exception.Message,
                StackTrace = context.Exception.StackTrace
            };
        }

        if (context.Result != null)
        {
            logData["ResultType"] = context.Result.GetType().Name;
        }

        var logLevel = context.Exception != null ? ErrorLogLevel : SuccessLogLevel;
        var message = !string.IsNullOrEmpty(MessagePrefix) 
            ? $"{MessagePrefix} - Action Executed: {context.ActionDescriptor.DisplayName}"
            : $"Action Executed: {context.ActionDescriptor.DisplayName}";

        logger.Log(logLevel, message + " {@LogData}", logData);

        base.OnActionExecuted(context);
    }

    private static string? GetUserId(HttpContext context)
    {
        return context.User.FindFirst("sub")?.Value ?? 
               context.User.FindFirst("id")?.Value ?? 
               context.User.FindFirst("userId")?.Value;
    }

    private static bool IsSensitiveParameter(string parameterName)
    {
        var sensitiveNames = new[] { "password", "token", "secret", "key", "auth", "credential" };
        return sensitiveNames.Any(name => parameterName.ToLowerInvariant().Contains(name));
    }

    private static Dictionary<string, string> GetSafeHeaders(IHeaderDictionary headers)
    {
        var sensitiveHeaders = new[] { "authorization", "cookie", "x-api-key", "x-auth-token" };
        
        return headers
            .Where(h => !sensitiveHeaders.Contains(h.Key.ToLowerInvariant()))
            .ToDictionary(h => h.Key, h => h.Value.ToString());
    }
}

/// <summary>
/// Logs only failed actions
/// </summary>
public class LogErrorsOnlyAttribute : LogActionAttribute
{
    public LogErrorsOnlyAttribute()
    {
        SuccessLogLevel = LogLevel.None; // Don't log successful actions
        ErrorLogLevel = LogLevel.Error;
        LogParameters = true;
        LogUser = true;
        LogExecutionTime = true;
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        // Only log if there's an exception
        if (context.Exception != null)
        {
            base.OnActionExecuted(context);
        }
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Start stopwatch but don't log
        var stopwatch = Stopwatch.StartNew();
        context.HttpContext.Items[StopwatchKey] = stopwatch;
    }
}

/// <summary>
/// Logs performance metrics for slow actions
/// </summary>
public class LogSlowActionsAttribute : LogActionAttribute
{
    /// <summary>
    /// Threshold in milliseconds for considering an action slow
    /// </summary>
    public int SlowThresholdMs { get; set; } = 1000;

    public LogSlowActionsAttribute()
    {
        LogExecutionTime = true;
        SuccessLogLevel = LogLevel.Warning; // Log slow actions as warnings
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        var stopwatch = context.HttpContext.Items[StopwatchKey] as Stopwatch;
        
        // Only log if action took longer than threshold
        if (stopwatch != null && stopwatch.ElapsedMilliseconds >= SlowThresholdMs)
        {
            base.OnActionExecuted(context);
        }
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Start stopwatch but don't log
        var stopwatch = Stopwatch.StartNew();
        context.HttpContext.Items[StopwatchKey] = stopwatch;
    }
}