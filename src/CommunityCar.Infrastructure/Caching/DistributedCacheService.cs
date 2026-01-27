using CommunityCar.Application.Common.Interfaces.Services.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CommunityCar.Infrastructure.Caching;

public class DistributedCacheService : IDistributedCacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<DistributedCacheService> _logger;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30);

    public DistributedCacheService(IDistributedCache distributedCache, ILogger<DistributedCacheService> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            var cachedValue = await _distributedCache.GetStringAsync(key);
            if (string.IsNullOrEmpty(cachedValue))
            {
                _logger.LogDebug("Cache miss for key: {Key}", key);
                return null;
            }

            var value = JsonSerializer.Deserialize<T>(cachedValue);
            _logger.LogDebug("Cache hit for key: {Key}", key);
            return value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting distributed cache value for key: {Key}", key);
            return null;
        }
    }

    public async Task<T?> GetAsync<T>(string key, string region) where T : class
    {
        var regionKey = $"{region}:{key}";
        return await GetAsync<T>(regionKey);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration
            };

            var serializedValue = JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key, serializedValue, options);
            
            _logger.LogDebug("Distributed cache set for key: {Key}, expiration: {Expiration}", key, expiration ?? _defaultExpiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting distributed cache value for key: {Key}", key);
        }
    }

    public async Task SetAsync<T>(string key, T value, string region, TimeSpan? expiration = null) where T : class
    {
        var regionKey = $"{region}:{key}";
        await SetAsync(regionKey, value, expiration);
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _distributedCache.RemoveAsync(key);
            _logger.LogDebug("Distributed cache removed for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing distributed cache value for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, string region)
    {
        var regionKey = $"{region}:{key}";
        await RemoveAsync(regionKey);
    }

    public Task RemoveByPatternAsync(string pattern)
    {
        // Redis implementation would use SCAN with pattern
        // For now, log warning as this requires Redis-specific implementation
        _logger.LogWarning("Pattern removal requires Redis implementation for pattern: {Pattern}", pattern);
        return Task.CompletedTask;
    }

    public Task RemoveRegionAsync(string region)
    {
        // Redis implementation would use SCAN with region prefix
        _logger.LogWarning("Region removal requires Redis implementation for region: {Region}", region);
        return Task.CompletedTask;
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getItem, TimeSpan? expiration = null) where T : class
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

    public async Task<T> GetOrSetAsync<T>(string key, string region, Func<Task<T>> getItem, TimeSpan? expiration = null) where T : class
    {
        var regionKey = $"{region}:{key}";
        return await GetOrSetAsync(regionKey, getItem, expiration);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            var value = await _distributedCache.GetStringAsync(key);
            return !string.IsNullOrEmpty(value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if distributed cache key exists: {Key}", key);
            return false;
        }
    }

    public Task ClearAsync()
    {
        // Redis implementation would use FLUSHDB
        _logger.LogWarning("Clear all requires Redis implementation");
        return Task.CompletedTask;
    }
}
