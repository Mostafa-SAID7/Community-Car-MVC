# Community Car Middleware

This folder contains comprehensive middleware components for the Community Car MVC application. Each middleware serves a specific purpose in the request pipeline and can be configured independently.

## Available Middleware

### 1. ErrorHandlingMiddleware
**Purpose**: Global exception handling and error logging
**Features**:
- Catches unhandled exceptions
- Logs errors to database and console
- Returns appropriate error responses (JSON for API, HTML for web)
- Includes detailed error information in development
- Captures request context for debugging

**Usage**:
```csharp
app.UseMiddleware<ErrorHandlingMiddleware>();
```

### 2. MaintenanceMiddleware
**Purpose**: Maintenance mode management
**Features**:
- Enables/disables maintenance mode
- Allows admin access during maintenance
- Redirects users to maintenance page
- Excludes static assets from maintenance checks

**Usage**:
```csharp
app.UseMiddleware<MaintenanceMiddleware>();
```

### 3. SecurityHeadersMiddleware
**Purpose**: Adds security headers to HTTP responses
**Features**:
- X-Content-Type-Options: nosniff
- X-Frame-Options: DENY/SAMEORIGIN
- X-XSS-Protection: 1; mode=block
- Strict-Transport-Security (HSTS)
- Content-Security-Policy (CSP)
- Referrer-Policy
- Permissions-Policy
- Custom headers support

**Usage**:
```csharp
app.UseSecurityHeaders(options =>
{
    options.ContentSecurityPolicyValue = "default-src 'self'; script-src 'self' 'unsafe-inline';";
    options.XFrameOptionsValue = "SAMEORIGIN";
});
```

### 4. RequestLoggingMiddleware
**Purpose**: Logs HTTP requests and responses for monitoring
**Features**:
- Logs request/response details
- Configurable logging levels
- Excludes sensitive headers
- Optional body logging
- Performance metrics
- Excludes static files (configurable)

**Usage**:
```csharp
app.UseRequestLogging(options =>
{
    options.LogHeaders = true;
    options.LogRequestBody = false; // Be careful with sensitive data
    options.LogResponseBody = false;
});
```

### 5. RateLimitingMiddleware
**Purpose**: Prevents abuse by limiting request rates
**Features**:
- Configurable rate limits per client
- IP-based and user-based limiting
- Sliding window algorithm
- Custom rate limit headers
- Excludes static files and specific paths
- Different limits for authenticated users

**Usage**:
```csharp
app.UseRateLimiting(options =>
{
    options.MaxRequests = 100;
    options.WindowSize = TimeSpan.FromMinutes(1);
    options.ApplyToAuthenticatedUsers = true;
});
```

### 6. CultureMiddleware
**Purpose**: Handles culture and localization
**Features**:
- Multiple culture detection methods (URL, query, cookie, user preference, Accept-Language)
- Fallback to default culture
- Sets CurrentCulture and CurrentUICulture
- Stores culture info in HttpContext

**Usage**:
```csharp
app.UseCultureMiddleware(options =>
{
    options.SupportedCultures = new List<string> { "en-US", "ar-EG" };
    options.DefaultCulture = "en-US";
});
```

### 7. PerformanceMiddleware
**Purpose**: Monitors and logs performance metrics
**Features**:
- Request execution time tracking
- Memory usage monitoring
- Performance headers
- Slow request detection
- High memory usage alerts
- Metrics collection for monitoring services

**Usage**:
```csharp
app.UsePerformanceMonitoring(options =>
{
    options.SlowRequestThresholdMs = 1000;
    options.AddPerformanceHeaders = true;
    options.EnableMetricsCollection = true;
});
```

### 8. HealthCheckMiddleware
**Purpose**: Provides health check endpoints
**Features**:
- Multiple health check endpoints (/health, /health/ready, /health/live)
- Database connectivity checks
- Memory usage validation
- Disk space monitoring
- External service health checks
- JSON response format

**Usage**:
```csharp
app.UseHealthChecks(options =>
{
    options.CheckDatabase = true;
    options.CheckMemory = true;
    options.MaxMemoryUsageMB = 500;
});
```

### 9. ApiMiddleware
**Purpose**: Handles API-specific concerns
**Features**:
- API request detection
- API versioning support
- CORS handling
- API key validation
- Consistent JSON error responses
- Preflight request handling

**Usage**:
```csharp
app.UseApiMiddleware(options =>
{
    options.RequireApiKey = false;
    options.EnableCors = true;
    options.IncludeExceptionDetails = isDevelopment;
});
```

## Quick Setup

### Option 1: Use All Middleware with Defaults
```csharp
app.UseCommunityCar(isDevelopment: app.Environment.IsDevelopment());
```

### Option 2: Custom Configuration
```csharp
app.UseCommunityCar(options =>
{
    options.UseSecurityHeaders = true;
    options.UseRateLimiting = true;
    options.UseRequestLogging = app.Environment.IsDevelopment();
    
    options.SecurityHeadersOptions = opts =>
    {
        opts.ContentSecurityPolicyValue = "default-src 'self';";
    };
    
    options.RateLimitingOptions = opts =>
    {
        opts.MaxRequests = 200;
        opts.WindowSize = TimeSpan.FromMinutes(1);
    };
});
```

### Option 3: Individual Middleware
```csharp
// Add middleware individually for fine-grained control
app.UseHealthChecks();
app.UseSecurityHeaders();
app.UsePerformanceMonitoring();
app.UseRateLimiting();
app.UseCultureMiddleware();
app.UseApiMiddleware();
```

## Middleware Order

The middleware should be added in the following order for optimal functionality:

1. **HealthCheckMiddleware** - Early exit for health checks
2. **SecurityHeadersMiddleware** - Add security headers early
3. **ApiMiddleware** - Handle API-specific concerns
4. **PerformanceMiddleware** - Start performance monitoring
5. **RequestLoggingMiddleware** - Log requests (development)
6. **RateLimitingMiddleware** - Apply rate limiting
7. **CultureMiddleware** - Set culture context
8. **ErrorHandlingMiddleware** - Global error handling
9. **MaintenanceMiddleware** - Maintenance mode checks

## Configuration Examples

### Production Configuration
```csharp
app.UseCommunityCar(options =>
{
    options.UseRequestLogging = false; // Disable in production
    options.UseRateLimiting = true;
    options.UseSecurityHeaders = true;
    
    options.RateLimitingOptions = opts =>
    {
        opts.MaxRequests = 60; // Stricter limits
        opts.ApplyToAuthenticatedUsers = true;
    };
    
    options.SecurityHeadersOptions = opts =>
    {
        opts.ContentSecurityPolicyValue = "default-src 'self'; script-src 'self';";
    };
});
```

### Development Configuration
```csharp
app.UseCommunityCar(options =>
{
    options.UseRequestLogging = true;
    options.UsePerformanceMonitoring = true;
    
    options.RequestLoggingOptions = opts =>
    {
        opts.LogHeaders = true;
        opts.LogRequestBody = false;
    };
    
    options.PerformanceOptions = opts =>
    {
        opts.AddPerformanceHeaders = true;
        opts.SlowRequestThresholdMs = 2000;
    };
});
```

## Health Check Endpoints

- `GET /health` - Comprehensive health check with all configured checks
- `GET /health/ready` - Readiness probe (is the app ready to serve requests?)
- `GET /health/live` - Liveness probe (is the app alive and responsive?)

## Performance Considerations

1. **RequestLoggingMiddleware**: Can impact performance in high-traffic scenarios. Use sparingly in production.
2. **RateLimitingMiddleware**: Uses in-memory storage. Consider Redis for distributed scenarios.
3. **PerformanceMiddleware**: Minimal overhead, safe for production use.
4. **SecurityHeadersMiddleware**: Very low overhead, recommended for all environments.

## Security Best Practices

1. Always use **SecurityHeadersMiddleware** in production
2. Configure **RateLimitingMiddleware** with appropriate limits
3. Use **ApiMiddleware** for API key validation when needed
4. Enable **ErrorHandlingMiddleware** to prevent information disclosure
5. Configure **CultureMiddleware** to prevent culture-based attacks

## Monitoring and Observability

The middleware provides extensive logging and metrics:
- Request/response logging
- Performance metrics
- Error tracking
- Health status monitoring
- Rate limiting statistics

Consider integrating with monitoring services like Application Insights, Prometheus, or custom monitoring solutions.