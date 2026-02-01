namespace CommunityCar.Web.Middleware;

/// <summary>
/// Extension methods for registering middleware
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Adds security headers middleware to the pipeline
    /// </summary>
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app, Action<SecurityHeadersOptions>? configure = null)
    {
        var options = new SecurityHeadersOptions();
        configure?.Invoke(options);
        
        return app.UseMiddleware<SecurityHeadersMiddleware>(options);
    }

    /// <summary>
    /// Adds request logging middleware to the pipeline
    /// </summary>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app, Action<RequestLoggingOptions>? configure = null)
    {
        var options = new RequestLoggingOptions();
        configure?.Invoke(options);
        
        return app.UseMiddleware<RequestLoggingMiddleware>(options);
    }

    /// <summary>
    /// Adds rate limiting middleware to the pipeline
    /// </summary>
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app, Action<RateLimitingOptions>? configure = null)
    {
        var options = new RateLimitingOptions();
        configure?.Invoke(options);
        
        return app.UseMiddleware<RateLimitingMiddleware>(options);
    }

    /// <summary>
    /// Adds culture middleware to the pipeline
    /// </summary>
    public static IApplicationBuilder UseCultureMiddleware(this IApplicationBuilder app, Action<CultureOptions>? configure = null)
    {
        var options = new CultureOptions();
        configure?.Invoke(options);
        
        return app.UseMiddleware<CultureMiddleware>(options);
    }

    /// <summary>
    /// Adds performance monitoring middleware to the pipeline
    /// </summary>
    public static IApplicationBuilder UsePerformanceMonitoring(this IApplicationBuilder app, Action<PerformanceOptions>? configure = null)
    {
        var options = new PerformanceOptions();
        configure?.Invoke(options);
        
        return app.UseMiddleware<PerformanceMiddleware>(options);
    }

    /// <summary>
    /// Adds API middleware to the pipeline
    /// </summary>
    public static IApplicationBuilder UseApiMiddleware(this IApplicationBuilder app, Action<ApiMiddlewareOptions>? configure = null)
    {
        var options = new ApiMiddlewareOptions();
        configure?.Invoke(options);
        
        return app.UseMiddleware<ApiMiddleware>(options);
    }

    /// <summary>
    /// Adds health check middleware to the pipeline
    /// </summary>
    public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app, Action<HealthCheckOptions>? configure = null)
    {
        var options = new HealthCheckOptions();
        configure?.Invoke(options);
        
        return app.UseMiddleware<HealthCheckMiddleware>(options);
    }

    /// <summary>
    /// Adds all Community Car middleware to the pipeline with default configuration
    /// </summary>
    public static IApplicationBuilder UseCommunityCar(this IApplicationBuilder app, bool isDevelopment = false)
    {
        // API middleware (should be early for API requests)
        app.UseApiMiddleware(options =>
        {
            options.IncludeExceptionDetails = isDevelopment;
            options.PrettyPrintJson = isDevelopment;
            options.RequireApiKey = false; // Set to true in production if needed
        });

        // Health checks (should be early in pipeline)
        app.UseHealthChecks(options =>
        {
            options.CheckDatabase = true;
            options.CheckMemory = true;
            options.CheckDiskSpace = true;
        });

        // Security headers (should be early)
        app.UseSecurityHeaders(options =>
        {
            if (isDevelopment)
            {
                // More relaxed CSP for development
                options.ContentSecurityPolicyValue = "default-src 'self' 'unsafe-inline' 'unsafe-eval'; img-src 'self' data: https:; font-src 'self' data:;";
            }
        });

        // Performance monitoring
        app.UsePerformanceMonitoring(options =>
        {
            options.AddPerformanceHeaders = isDevelopment;
            options.SlowRequestThresholdMs = isDevelopment ? 2000 : 1000;
        });

        // Request logging (in development only by default)
        if (isDevelopment)
        {
            app.UseRequestLogging(options =>
            {
                options.LogHeaders = true;
                options.LogRequestBody = false; // Be careful with sensitive data
                options.LogResponseBody = false;
            });
        }

        // Rate limiting (more lenient in development)
        app.UseRateLimiting(options =>
        {
            options.MaxRequests = isDevelopment ? 1000 : 100;
            options.WindowSize = TimeSpan.FromMinutes(1);
            options.ApplyToAuthenticatedUsers = !isDevelopment;
        });

        // Culture handling
        app.UseCultureMiddleware(options =>
        {
            options.SupportedCultures = new List<string> { "en-US", "ar-EG" };
            options.DefaultCulture = "en-US";
        });

        return app;
    }

    /// <summary>
    /// Adds Community Car middleware with custom configuration
    /// </summary>
    public static IApplicationBuilder UseCommunityCar(this IApplicationBuilder app, Action<CommunityCarMiddlewareOptions> configure)
    {
        var options = new CommunityCarMiddlewareOptions();
        configure(options);

        if (options.UseApiMiddleware)
        {
            app.UseApiMiddleware(options.ApiMiddlewareOptions);
        }

        if (options.UseHealthChecks)
        {
            app.UseHealthChecks(options.HealthCheckOptions);
        }

        if (options.UseSecurityHeaders)
        {
            app.UseSecurityHeaders(options.SecurityHeadersOptions);
        }

        if (options.UsePerformanceMonitoring)
        {
            app.UsePerformanceMonitoring(options.PerformanceOptions);
        }

        if (options.UseRequestLogging)
        {
            app.UseRequestLogging(options.RequestLoggingOptions);
        }

        if (options.UseRateLimiting)
        {
            app.UseRateLimiting(options.RateLimitingOptions);
        }

        if (options.UseCultureMiddleware)
        {
            app.UseCultureMiddleware(options.CultureOptions);
        }

        return app;
    }
}

/// <summary>
/// Configuration options for Community Car middleware
/// </summary>
public class CommunityCarMiddlewareOptions
{
    public bool UseApiMiddleware { get; set; } = true;
    public bool UseHealthChecks { get; set; } = true;
    public bool UseSecurityHeaders { get; set; } = true;
    public bool UsePerformanceMonitoring { get; set; } = true;
    public bool UseRequestLogging { get; set; } = false;
    public bool UseRateLimiting { get; set; } = true;
    public bool UseCultureMiddleware { get; set; } = true;

    public Action<ApiMiddlewareOptions>? ApiMiddlewareOptions { get; set; }
    public Action<HealthCheckOptions>? HealthCheckOptions { get; set; }
    public Action<SecurityHeadersOptions>? SecurityHeadersOptions { get; set; }
    public Action<PerformanceOptions>? PerformanceOptions { get; set; }
    public Action<RequestLoggingOptions>? RequestLoggingOptions { get; set; }
    public Action<RateLimitingOptions>? RateLimitingOptions { get; set; }
    public Action<CultureOptions>? CultureOptions { get; set; }
}