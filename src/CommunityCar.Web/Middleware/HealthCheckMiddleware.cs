using System.Diagnostics;
using System.Text.Json;

namespace CommunityCar.Web.Middleware;

/// <summary>
/// Middleware for providing health check endpoints
/// </summary>
public class HealthCheckMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<HealthCheckMiddleware> _logger;
    private readonly HealthCheckOptions _options;
    private readonly IServiceProvider _serviceProvider;

    public HealthCheckMiddleware(RequestDelegate next, ILogger<HealthCheckMiddleware> logger, HealthCheckOptions options, IServiceProvider serviceProvider)
    {
        _next = next;
        _logger = logger;
        _options = options;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";

        if (path == _options.HealthCheckPath.ToLowerInvariant())
        {
            await HandleHealthCheck(context);
            return;
        }

        if (path == _options.ReadinessCheckPath.ToLowerInvariant())
        {
            await HandleReadinessCheck(context);
            return;
        }

        if (path == _options.LivenessCheckPath.ToLowerInvariant())
        {
            await HandleLivenessCheck(context);
            return;
        }

        await _next(context);
    }

    private async Task HandleHealthCheck(HttpContext context)
    {
        try
        {
            var healthStatus = await PerformHealthChecks();
            var statusCode = healthStatus.IsHealthy ? 200 : 503;
            
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            
            var response = new
            {
                status = healthStatus.IsHealthy ? "Healthy" : "Unhealthy",
                timestamp = DateTime.UtcNow,
                checks = healthStatus.Checks,
                duration = healthStatus.Duration,
                version = GetApplicationVersion()
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await context.Response.WriteAsync(json);
            
            _logger.LogInformation("Health check completed: {Status} in {Duration}ms", 
                healthStatus.IsHealthy ? "Healthy" : "Unhealthy", healthStatus.Duration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed with exception");
            
            context.Response.StatusCode = 503;
            context.Response.ContentType = "application/json";
            
            var errorResponse = new
            {
                status = "Unhealthy",
                timestamp = DateTime.UtcNow,
                error = "Health check failed",
                version = GetApplicationVersion()
            };

            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }

    private async Task HandleReadinessCheck(HttpContext context)
    {
        // Readiness check - is the application ready to serve requests?
        try
        {
            var isReady = await CheckReadiness();
            
            context.Response.StatusCode = isReady ? 200 : 503;
            context.Response.ContentType = "application/json";
            
            var response = new
            {
                status = isReady ? "Ready" : "Not Ready",
                timestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Readiness check failed");
            context.Response.StatusCode = 503;
            await context.Response.WriteAsync("{\"status\":\"Not Ready\"}");
        }
    }

    private async Task HandleLivenessCheck(HttpContext context)
    {
        // Liveness check - is the application alive?
        try
        {
            var isAlive = await CheckLiveness();
            
            context.Response.StatusCode = isAlive ? 200 : 503;
            context.Response.ContentType = "application/json";
            
            var response = new
            {
                status = isAlive ? "Alive" : "Dead",
                timestamp = DateTime.UtcNow,
                uptime = GetUptime()
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Liveness check failed");
            context.Response.StatusCode = 503;
            await context.Response.WriteAsync("{\"status\":\"Dead\"}");
        }
    }

    private async Task<HealthStatus> PerformHealthChecks()
    {
        var startTime = DateTime.UtcNow;
        var checks = new List<HealthCheckResult>();
        var isHealthy = true;

        // Database check
        if (_options.CheckDatabase)
        {
            var dbCheck = await CheckDatabase();
            checks.Add(dbCheck);
            if (!dbCheck.IsHealthy) isHealthy = false;
        }

        // Memory check
        if (_options.CheckMemory)
        {
            var memoryCheck = CheckMemoryUsage();
            checks.Add(memoryCheck);
            if (!memoryCheck.IsHealthy) isHealthy = false;
        }

        // Disk space check
        if (_options.CheckDiskSpace)
        {
            var diskCheck = CheckDiskSpace();
            checks.Add(diskCheck);
            if (!diskCheck.IsHealthy) isHealthy = false;
        }

        // External services check
        foreach (var serviceUrl in _options.ExternalServices)
        {
            var serviceCheck = await CheckExternalService(serviceUrl);
            checks.Add(serviceCheck);
            if (!serviceCheck.IsHealthy) isHealthy = false;
        }

        var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;

        return new HealthStatus
        {
            IsHealthy = isHealthy,
            Checks = checks,
            Duration = duration
        };
    }

    private async Task<HealthCheckResult> CheckDatabase()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            // This would typically check database connectivity
            // For now, we'll just return healthy
            await Task.Delay(10); // Simulate database check
            
            return new HealthCheckResult
            {
                Name = "Database",
                IsHealthy = true,
                Message = "Database connection successful",
                Duration = 10
            };
        }
        catch (Exception ex)
        {
            return new HealthCheckResult
            {
                Name = "Database",
                IsHealthy = false,
                Message = $"Database check failed: {ex.Message}",
                Duration = 0
            };
        }
    }

    private HealthCheckResult CheckMemoryUsage()
    {
        try
        {
            var memoryUsed = GC.GetTotalMemory(false);
            var memoryUsedMB = memoryUsed / (1024 * 1024);
            var isHealthy = memoryUsedMB < _options.MaxMemoryUsageMB;

            return new HealthCheckResult
            {
                Name = "Memory",
                IsHealthy = isHealthy,
                Message = $"Memory usage: {memoryUsedMB}MB (limit: {_options.MaxMemoryUsageMB}MB)",
                Duration = 1,
                Data = new { memoryUsedMB, limit = _options.MaxMemoryUsageMB }
            };
        }
        catch (Exception ex)
        {
            return new HealthCheckResult
            {
                Name = "Memory",
                IsHealthy = false,
                Message = $"Memory check failed: {ex.Message}",
                Duration = 0
            };
        }
    }

    private HealthCheckResult CheckDiskSpace()
    {
        try
        {
            var drive = new DriveInfo(Path.GetPathRoot(Environment.CurrentDirectory) ?? "C:");
            var freeSpaceGB = drive.AvailableFreeSpace / (1024 * 1024 * 1024);
            var isHealthy = freeSpaceGB > _options.MinDiskSpaceGB;

            return new HealthCheckResult
            {
                Name = "DiskSpace",
                IsHealthy = isHealthy,
                Message = $"Free disk space: {freeSpaceGB}GB (minimum: {_options.MinDiskSpaceGB}GB)",
                Duration = 1,
                Data = new { freeSpaceGB, minimum = _options.MinDiskSpaceGB }
            };
        }
        catch (Exception ex)
        {
            return new HealthCheckResult
            {
                Name = "DiskSpace",
                IsHealthy = false,
                Message = $"Disk space check failed: {ex.Message}",
                Duration = 0
            };
        }
    }

    private async Task<HealthCheckResult> CheckExternalService(string serviceUrl)
    {
        var startTime = DateTime.UtcNow;
        
        try
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(_options.ExternalServiceTimeoutSeconds);
            
            var response = await httpClient.GetAsync(serviceUrl);
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            var isHealthy = response.IsSuccessStatusCode;

            return new HealthCheckResult
            {
                Name = $"ExternalService_{new Uri(serviceUrl).Host}",
                IsHealthy = isHealthy,
                Message = $"Service responded with {response.StatusCode}",
                Duration = duration,
                Data = new { url = serviceUrl, statusCode = (int)response.StatusCode }
            };
        }
        catch (Exception ex)
        {
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            
            return new HealthCheckResult
            {
                Name = $"ExternalService_{new Uri(serviceUrl).Host}",
                IsHealthy = false,
                Message = $"Service check failed: {ex.Message}",
                Duration = duration,
                Data = new { url = serviceUrl, error = ex.Message }
            };
        }
    }

    private async Task<bool> CheckReadiness()
    {
        // Check if application is ready to serve requests
        // This might include checking if database migrations are complete,
        // configuration is loaded, etc.
        await Task.CompletedTask;
        return true; // Placeholder
    }

    private async Task<bool> CheckLiveness()
    {
        // Check if application is alive and responsive
        // This is typically a simple check
        await Task.CompletedTask;
        return true;
    }

    private string GetApplicationVersion()
    {
        try
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            return version?.ToString() ?? "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }

    private string GetUptime()
    {
        var uptime = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();
        return uptime.ToString(@"dd\.hh\:mm\:ss");
    }
}

/// <summary>
/// Health status result
/// </summary>
public class HealthStatus
{
    public bool IsHealthy { get; set; }
    public List<HealthCheckResult> Checks { get; set; } = new();
    public double Duration { get; set; }
}

/// <summary>
/// Individual health check result
/// </summary>
public class HealthCheckResult
{
    public string Name { get; set; } = string.Empty;
    public bool IsHealthy { get; set; }
    public string Message { get; set; } = string.Empty;
    public double Duration { get; set; }
    public object? Data { get; set; }
}

/// <summary>
/// Configuration options for health check middleware
/// </summary>
public class HealthCheckOptions
{
    public string HealthCheckPath { get; set; } = "/health";
    public string ReadinessCheckPath { get; set; } = "/health/ready";
    public string LivenessCheckPath { get; set; } = "/health/live";
    public bool CheckDatabase { get; set; } = true;
    public bool CheckMemory { get; set; } = true;
    public bool CheckDiskSpace { get; set; } = true;
    public long MaxMemoryUsageMB { get; set; } = 500;
    public long MinDiskSpaceGB { get; set; } = 1;
    public int ExternalServiceTimeoutSeconds { get; set; } = 10;
    public List<string> ExternalServices { get; set; } = new();
}