using CommunityCar.Application.Common.Interfaces.Services.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using StackExchange.Redis;

namespace CommunityCar.Infrastructure.Services.Caching;

/// <summary>
/// Hybrid caching service that uses both memory cache and distributed cache (Redis)
/// </summary>
public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly IDatabase? _redisDatabase;
    private readonly ILogger<CacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public CacheService(
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        IConnectionMultiplexer? connectionMultiplexer,
        ILogger<CacheService> logger)
    {
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _redisDatabase = connectionMultiplexer?.GetDatabase();
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            // Try memory cache first (L1 cache)
            if (_memoryCache.TryGetValue(key, out T? memoryValue))
            {
                _logger.LogDebug("Cache hit (Memory): {Key}", key);
                return memoryValue;
            }

            // Try distributed cache (L2 cache)
            var distributedValue = await _distributedCache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(distributedValue))
            {
                var deserializedValue = JsonSerializer.Deserialize<T>(distributedValue, _jsonOptions);
                
                // Store in memory cache for faster subsequent access
                _memoryCache.Set(key, deserializedValue, TimeSpan.FromMinutes(5));
                
                _logger.LogDebug("Cache hit (Distributed): {Key}", key);
                return deserializedValue;
            }

            _logger.LogDebug("Cache miss: {Key}", key);
            return default;
        }
        catch (Exception ex)
        {
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var defaultExpiration = expiration ?? TimeSpan.FromMinutes(30);
            
            // Set in memory cache (L1)
            _memoryCache.Set(key, value, defaultExpiration);

            // Set in distributed cache (L2)
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = defaultExpiration
            };
            
            await _distributedCache.SetStringAsync(key, serializedValue, options);
            
            _logger.LogDebug("Cache set: {Key} (Expiration: {Expiration})", key, defaultExpiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            // Remove from memory cache
            _memoryCache.Remove(key);

            // Remove from distributed cache
            await _distributedCache.RemoveAsync(key);
            
            _logger.LogDebug("Cache removed: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache value for key: {Key}", key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        try
        {
            if (_redisDatabase != null)
            {
                // Use Redis SCAN to find keys matching pattern
                var server = _redisDatabase.Multiplexer.GetServer(_redisDatabase.Multiplexer.GetEndPoints().First());
                var keys = server.Keys(pattern: pattern).ToArray();
                
                if (keys.Length > 0)
                {
                    // Remove from Redis
                    await _redisDatabase.KeyDeleteAsync(keys);
                    
                    // Remove from memory cache (best effort - we can't pattern match in memory cache)
                    // This is a limitation, but memory cache entries will expire naturally
                    
                    _logger.LogDebug("Cache pattern removed: {Pattern} ({Count} keys)", pattern, keys.Length);
                }
            }
            else
            {
                _logger.LogWarning("Redis not available for pattern removal: {Pattern}", pattern);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache values by pattern: {Pattern}", pattern);
        }
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        var cachedValue = await GetAsync<T>(key);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        try
        {
            var value = await factory();
            if (value != null)
            {
                await SetAsync(key, value, expiration);
            }
            return value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetOrSet factory for key: {Key}", key);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            // Check memory cache first
            if (_memoryCache.TryGetValue(key, out _))
            {
                return true;
            }

            // Check distributed cache
            if (_redisDatabase != null)
            {
                return await _redisDatabase.KeyExistsAsync(key);
            }
            else
            {
                // Fallback to getting the value from distributed cache
                var value = await _distributedCache.GetStringAsync(key);
                return !string.IsNullOrEmpty(value);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
            return false;
        }
    }

    public async Task RefreshAsync(string key, TimeSpan? expiration = null)
    {
        try
        {
            if (_redisDatabase != null)
            {
                var defaultExpiration = expiration ?? TimeSpan.FromMinutes(30);
                await _redisDatabase.KeyExpireAsync(key, defaultExpiration);
                _logger.LogDebug("Cache refreshed: {Key} (New expiration: {Expiration})", key, defaultExpiration);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing cache for key: {Key}", key);
        }
    }

    public Task ClearAsync()
    {
        try
        {
            // Memory cache doesn't support clearing all natively without a reset/tracking
            // Distributed cache (Redis) supports flushing, but it's dangerous
            _logger.LogWarning("ClearAsync called on hybrid CacheService. This is not fully implemented for MemoryCache.");
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cache");
            return Task.CompletedTask;
        }
    }
}
