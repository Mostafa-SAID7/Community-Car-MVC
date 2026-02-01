using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace CommunityCar.Web.Attributes;

/// <summary>
/// Caches action results in memory
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class CacheAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Cache duration in seconds
    /// </summary>
    public int Duration { get; set; } = 300; // 5 minutes default

    /// <summary>
    /// Cache key prefix
    /// </summary>
    public string? KeyPrefix { get; set; }

    /// <summary>
    /// Whether to vary cache by user
    /// </summary>
    public bool VaryByUser { get; set; } = false;

    /// <summary>
    /// Whether to vary cache by query parameters
    /// </summary>
    public bool VaryByQuery { get; set; } = true;

    /// <summary>
    /// Specific query parameters to include in cache key
    /// </summary>
    public string[]? QueryParameters { get; set; }

    /// <summary>
    /// Cache priority
    /// </summary>
    public CacheItemPriority Priority { get; set; } = CacheItemPriority.Normal;

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cache = context.HttpContext.RequestServices.GetService<IMemoryCache>();
        if (cache == null)
        {
            await next();
            return;
        }

        var cacheKey = GenerateCacheKey(context);
        
        // Try to get cached result
        if (cache.TryGetValue(cacheKey, out var cachedResult) && cachedResult is IActionResult actionResult)
        {
            context.Result = actionResult;
            return;
        }

        // Execute action
        var executedContext = await next();

        // Cache the result if successful
        if (executedContext.Result != null && executedContext.Exception == null)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(Duration),
                Priority = Priority
            };

            cache.Set(cacheKey, executedContext.Result, cacheOptions);
        }
    }

    private string GenerateCacheKey(ActionExecutingContext context)
    {
        var keyBuilder = new StringBuilder();
        
        // Add prefix
        if (!string.IsNullOrEmpty(KeyPrefix))
        {
            keyBuilder.Append(KeyPrefix).Append(":");
        }

        // Add controller and action
        keyBuilder.Append(context.RouteData.Values["controller"])
                  .Append(":")
                  .Append(context.RouteData.Values["action"]);

        // Add user ID if varying by user
        if (VaryByUser && context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.HttpContext.User.FindFirst("sub")?.Value ?? 
                        context.HttpContext.User.FindFirst("id")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                keyBuilder.Append(":user:").Append(userId);
            }
        }

        // Add query parameters
        if (VaryByQuery)
        {
            var queryParams = QueryParameters ?? context.HttpContext.Request.Query.Keys.ToArray();
            
            foreach (var param in queryParams.OrderBy(p => p))
            {
                if (context.HttpContext.Request.Query.TryGetValue(param, out var value))
                {
                    keyBuilder.Append(":").Append(param).Append("=").Append(value);
                }
            }
        }

        // Add route parameters
        foreach (var routeValue in context.RouteData.Values.Where(rv => rv.Key != "controller" && rv.Key != "action"))
        {
            keyBuilder.Append(":").Append(routeValue.Key).Append("=").Append(routeValue.Value);
        }

        return keyBuilder.ToString();
    }
}

/// <summary>
/// Clears cache entries with specified patterns
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ClearCacheAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Cache key patterns to clear
    /// </summary>
    public string[] KeyPatterns { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Whether to clear cache before or after action execution
    /// </summary>
    public bool ClearBefore { get; set; } = false;

    public ClearCacheAttribute(params string[] keyPatterns)
    {
        KeyPatterns = keyPatterns ?? Array.Empty<string>();
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cache = context.HttpContext.RequestServices.GetService<IMemoryCache>();
        
        if (ClearBefore && cache != null)
        {
            ClearCacheEntries(cache, context);
        }

        var executedContext = await next();

        if (!ClearBefore && cache != null && executedContext.Exception == null)
        {
            ClearCacheEntries(cache, context);
        }
    }

    private void ClearCacheEntries(IMemoryCache cache, ActionExecutingContext context)
    {
        // Note: IMemoryCache doesn't provide a way to enumerate keys
        // In a real implementation, you might want to use a distributed cache
        // or maintain a separate index of cache keys
        
        foreach (var pattern in KeyPatterns)
        {
            // This is a simplified implementation
            // You would need to implement pattern matching based on your caching strategy
            cache.Remove(pattern);
        }
    }
}