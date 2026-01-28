using CommunityCar.Application.Common.Interfaces.Services.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace CommunityCar.Infrastructure.Caching;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CacheService> _logger;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30);

    public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        try
        {
            if (_memoryCache.TryGetValue(key, out var value))
            {
                if (value is T typedValue)
                {
                    _logger.LogDebug("Cache hit for key: {Key}", key);
                    return Task.FromResult<T?>(typedValue);
                }
                
                if (value is string jsonValue)
                {
                    var deserializedValue = JsonSerializer.Deserialize<T>(jsonValue);
                    _logger.LogDebug("Cache hit (deserialized) for key: {Key}", key);
                    return Task.FromResult(deserializedValue);
                }
            }

            _logger.LogDebug("Cache miss for key: {Key}", key);
            return Task.FromResult<T?>(default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache value for key: {Key}", key);
            return Task.FromResult<T?>(default);
        }
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration,
                Priority = CacheItemPriority.Normal
            };

            _memoryCache.Set(key, value, options);
            _logger.LogDebug("Cache set for key: {Key}, expiration: {Expiration}", key, expiration ?? _defaultExpiration);
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
            return Task.CompletedTask;
        }
    }

    public Task RemoveAsync(string key)
    {
        try
        {
            _memoryCache.Remove(key);
            _logger.LogDebug("Cache removed for key: {Key}", key);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache value for key: {Key}", key);
            return Task.CompletedTask;
        }
    }

    public Task RemoveByPatternAsync(string pattern)
    {
        // Memory cache doesn't support pattern removal natively
        // This would require tracking keys or using a different cache provider
        _logger.LogWarning("Pattern removal not supported in MemoryCache for pattern: {Pattern}", pattern);
        return Task.CompletedTask;
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getItem, TimeSpan? expiration = null)
    {
        var cachedValue = await GetAsync<T>(key);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        var value = await getItem();
        if (value != null)
        {
            await SetAsync(key, value, expiration);
        }

        return value;
    }

    public Task<bool> ExistsAsync(string key)
    {
        var exists = _memoryCache.TryGetValue(key, out _);
        return Task.FromResult(exists);
    }

    public Task RefreshAsync(string key, TimeSpan? expiration = null)
    {
        // Memory cache doesn't support refreshing expiration natively
        // This would require re-setting the value with new expiration
        _logger.LogWarning("Refresh expiration not supported in MemoryCache for key: {Key}", key);
        return Task.CompletedTask;
    }

    public Task ClearAsync()
    {
        // Memory cache doesn't support clearing all entries natively
        // This would require tracking keys or using a different cache provider
        _logger.LogWarning("Clear all not supported in MemoryCache");
        return Task.CompletedTask;
    }
}
