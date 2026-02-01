# MVC Attributes Library

This folder contains comprehensive custom attributes for ASP.NET Core MVC to enhance functionality, security, performance, and maintainability.

## Available Attributes

### 1. **RequirePermissionAttribute**
Permission-based authorization for actions and controllers.

```csharp
[RequirePermission("Posts.Create", "Posts.Edit")]
public IActionResult CreatePost() { }

[RequireAnyPermission("Admin.View", "Moderator.View")]
public IActionResult AdminPanel() { }
```

**Features:**
- Support for multiple permissions (ALL or ANY)
- Convenience attributes: `RequireAnyPermissionAttribute`, `RequireAllPermissionsAttribute`
- Integrates with custom permission service

### 2. **ValidateModelAttribute**
Automatic model validation with AJAX support.

```csharp
[ValidateModel(ErrorMessage = "Invalid data provided")]
public IActionResult CreateUser(CreateUserVM model) { }

[ValidateProperties("Email", "Password")]
public IActionResult UpdateProfile(ProfileVM model) { }
```

**Features:**
- Automatic JSON responses for AJAX requests
- Custom error messages
- Property-specific validation
- Configurable HTTP status codes

### 3. **CacheAttribute**
Memory caching for action results.

```csharp
[Cache(Duration = 300, VaryByUser = true)]
public IActionResult UserProfile() { }

[Cache(Duration = 600, VaryByQuery = true, QueryParameters = new[] { "page", "size" })]
public IActionResult GetPosts(int page, int size) { }
```

**Features:**
- Configurable cache duration
- User-specific caching
- Query parameter variation
- Cache priority settings
- Companion `ClearCacheAttribute` for cache invalidation

### 4. **RateLimitAttribute**
Action-level rate limiting with sliding window algorithm.

```csharp
[RateLimit(MaxRequests = 10, WindowSeconds = 60, PerUser = true)]
public IActionResult SendMessage() { }

[StrictRateLimit] // 3 requests per 5 minutes
public IActionResult ResetPassword() { }

[LenientRateLimit] // 100 requests per minute
public IActionResult GetData() { }
```

**Features:**
- Per-user or per-IP limiting
- Configurable time windows
- Rate limit headers (X-RateLimit-*)
- Automatic cleanup of expired entries
- Predefined strict/lenient configurations

### 5. **AjaxOnlyAttribute**
Restricts actions to AJAX requests only.

```csharp
[AjaxOnly]
public IActionResult GetPartialView() { }

[AjaxOnly(RedirectAction = "Index", RedirectController = "Home")]
public IActionResult AjaxOnlyAction() { }

[NoAjax] // Opposite - no AJAX requests allowed
public IActionResult RegularPageOnly() { }
```

**Features:**
- AJAX request detection
- Automatic redirects for non-AJAX requests
- Companion `NoAjaxAttribute` for regular requests only
- `AjaxResponseAttribute` for different responses based on request type

### 6. **LogActionAttribute**
Comprehensive action execution logging.

```csharp
[LogAction(LogParameters = true, LogUser = true, LogExecutionTime = true)]
public class AccountController : Controller { }

[LogErrorsOnly] // Only log failed actions
public IActionResult RiskyOperation() { }

[LogSlowActions(SlowThresholdMs = 2000)] // Log actions taking > 2 seconds
public IActionResult ExpensiveOperation() { }
```

**Features:**
- Configurable logging levels
- Parameter logging (with sensitive data protection)
- Execution time tracking
- User context logging
- Request headers logging
- Specialized variants for errors and slow actions

### 7. **CompressResponseAttribute**
Response compression using Gzip/Deflate.

```csharp
[CompressResponse]
public IActionResult GetLargeData() { }

[CompressLargeResponses] // Only compress responses > 10KB
public IActionResult GetData() { }

[HighCompression] // Maximum compression for static content
public IActionResult GetStaticContent() { }

[NoCompression] // Disable compression
public IActionResult GetBinaryData() { }
```

**Features:**
- Automatic compression format detection
- Configurable minimum size thresholds
- Content-type filtering
- Compression level control
- Vary header management

### 8. **CultureAttribute**
Culture and localization management.

```csharp
[Culture("ar-EG")]
public IActionResult ArabicPage() { }

[AutoCulture] // Automatic culture detection
public IActionResult MultilingualPage() { }

[EnglishCulture] // Force English
public IActionResult EnglishOnlyPage() { }

[ArabicCulture] // Force Arabic
public IActionResult ArabicOnlyPage() { }
```

**Features:**
- Manual culture setting
- Automatic culture detection from multiple sources
- Session and cookie persistence
- Fallback culture handling
- Support for UI culture separation

## Usage Examples

### Controller-Level Application
```csharp
[LogAction(LogUser = true, LogExecutionTime = true)]
[RequirePermission("Admin.Access")]
public class AdminController : Controller
{
    [Cache(Duration = 300)]
    [CompressResponse]
    public IActionResult Dashboard() { }
    
    [RateLimit(MaxRequests = 5, WindowSeconds = 300)]
    [ValidateModel]
    public IActionResult CreateUser(CreateUserVM model) { }
}
```

### Action-Level Combinations
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
[ValidateModel]
[RateLimit(MaxRequests = 3, WindowSeconds = 60)]
[LogAction(LogParameters = false)] // Don't log sensitive data
public async Task<IActionResult> Login(LoginVM model) { }
```

### AJAX-Specific Actions
```csharp
[AjaxOnly]
[Cache(Duration = 60, VaryByUser = true)]
[CompressResponse]
public IActionResult GetUserNotifications() { }
```

## Best Practices

### 1. **Security First**
- Always use `RequirePermissionAttribute` for sensitive actions
- Apply `RateLimitAttribute` to prevent abuse
- Use `ValidateModelAttribute` to ensure data integrity

### 2. **Performance Optimization**
- Apply `CacheAttribute` to expensive operations
- Use `CompressResponseAttribute` for large responses
- Monitor with `LogSlowActionsAttribute`

### 3. **User Experience**
- Use `CultureAttribute` for internationalization
- Apply `AjaxOnlyAttribute` for partial views
- Implement proper error handling with `ValidateModelAttribute`

### 4. **Monitoring and Debugging**
- Use `LogActionAttribute` for audit trails
- Apply `LogErrorsOnlyAttribute` for error tracking
- Monitor performance with execution time logging

## Configuration

### Dependency Injection Setup
```csharp
// In Program.cs or Startup.cs
services.AddScoped<IPermissionService, PermissionService>();
services.AddMemoryCache(); // For CacheAttribute
services.AddLogging(); // For LogActionAttribute
```

### Global Filters
```csharp
// Apply attributes globally
services.AddMvc(options =>
{
    options.Filters.Add<LogActionAttribute>();
    options.Filters.Add<ValidateModelAttribute>();
});
```

## Error Handling

All attributes include comprehensive error handling:
- Graceful degradation when services are unavailable
- Proper HTTP status codes
- Informative error messages
- Logging of exceptions

## Thread Safety

All attributes are designed to be thread-safe:
- Static collections use `ConcurrentDictionary`
- Atomic operations for counters
- Proper locking where necessary

## Extensibility

The attribute library is designed for extensibility:
- Base classes for common functionality
- Virtual methods for customization
- Interface-based service dependencies
- Configuration through properties

This comprehensive attribute library transforms your MVC application into a robust, secure, and performant web application with minimal configuration and maximum flexibility.