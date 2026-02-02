using CommunityCar.Application.Common.Interfaces.Services.Caching;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace CommunityCar.Application.Services.Caching;

/// <summary>
/// Redis-specific cache service implementation
/// </summary>
public class RedisCacheService : ICacheService, IDistributedCacheService
{
    private readonly IDatabase _database;
    private readonly IServer _server;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(
        IConnectionMultiplexer connectionMultiplexer,
        ILogger<RedisCacheService> logger)
    {
        _database = connectionMultiplexer.GetDatabase();
        _server = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints().First());
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    #region ICacheService Implementation

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var value = await _database.StringGetAsync(key);
            if (!value.HasValue)
            {
                _logger.LogDebug("Cache miss: {Key}", key);
                return default;
            }

            var deserializedValue = JsonSerializer.Deserialize<T>(value!, _jsonOptions);
            _logger.LogDebug("Cache hit: {Key}", key);
            return deserializedValue;
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
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            var defaultExpiration = expiration ?? TimeSpan.FromMinutes(30);
            
            await _database.StringSetAsync(key, serializedValue, defaultExpiration);
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
            await _database.KeyDeleteAsync(key);
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
            var keys = _server.Keys(pattern: pattern).ToArray();
            if (keys.Length > 0)
            {
                await _database.KeyDeleteAsync(keys);
                _logger.LogDebug("Cache pattern removed: {Pattern} ({Count} keys)", pattern, keys.Length);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache values by pattern: {Pattern}", pattern);
        }
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getItem, TimeSpan? expiration = null)
    {
        var cachedValue = await GetAsync<T>(key);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        try
        {
            var value = await getItem();
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
            return await _database.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
            return false;
        }
    }

    public async Task ClearAsync()
    {
        try
        {
            await _server.FlushDatabaseAsync();
            _logger.LogInformation("Cache cleared");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cache");
        }
    }

    #endregion

    #region IDistributedCacheService Implementation

    public async Task<T?> GetAsync<T>(string key, string region)
    {
        var regionKey = $"{region}:{key}";
        return await GetAsync<T>(regionKey);
    }

    public async Task SetAsync<T>(string key, T value, string region, TimeSpan? expiration = null)
    {
        var regionKey = $"{region}:{key}";
        await SetAsync(regionKey, value, expiration);
    }

    public async Task RemoveAsync(string key, string region)
    {
        var regionKey = $"{region}:{key}";
        await RemoveAsync(regionKey);
    }

    public async Task RemoveRegionAsync(string region)
    {
        await RemoveByPatternAsync($"{region}:*");
    }

    public async Task<T> GetOrSetAsync<T>(string key, string region, Func<Task<T>> getItem, TimeSpan? expiration = null)
    {
        var regionKey = $"{region}:{key}";
        return await GetOrSetAsync(regionKey, getItem, expiration);
    }

    #endregion

    #region Additional Redis-specific Methods

    /// <summary>
    /// Set cache with sliding expiration
    /// </summary>
    public async Task SetWithSlidingExpirationAsync<T>(string key, T value, TimeSpan slidingExpiration)
    {
        try
        {
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            await _database.StringSetAsync(key, serializedValue, slidingExpiration);
            
            // Set up sliding expiration by updating TTL on access
            await _database.KeyExpireAsync(key, slidingExpiration);
            
            _logger.LogDebug("Cache set with sliding expiration: {Key} (Expiration: {Expiration})", key, slidingExpiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache with sliding expiration for key: {Key}", key);
        }
    }

    /// <summary>
    /// Increment a numeric value in cache
    /// </summary>
    public async Task<long> IncrementAsync(string key, long value = 1, TimeSpan? expiration = null)
    {
        try
        {
            var result = await _database.StringIncrementAsync(key, value);
            
            if (expiration.HasValue)
            {
                await _database.KeyExpireAsync(key, expiration.Value);
            }
            
            _logger.LogDebug("Cache incremented: {Key} by {Value} = {Result}", key, value, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing cache value for key: {Key}", key);
            return 0;
        }
    }

    /// <summary>
    /// Decrement a numeric value in cache
    /// </summary>
    public async Task<long> DecrementAsync(string key, long value = 1)
    {
        try
        {
            var result = await _database.StringDecrementAsync(key, value);
            _logger.LogDebug("Cache decremented: {Key} by {Value} = {Result}", key, value, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decrementing cache value for key: {Key}", key);
            return 0;
        }
    }

    /// <summary>
    /// Get cache statistics
    /// </summary>
    public async Task<Dictionary<string, object>> GetCacheStatisticsAsync()
    {
        try
        {
            var info = await _server.InfoAsync();
            var stats = new Dictionary<string, object>();
            
            foreach (var section in info)
            {
                foreach (var item in section)
                {
                    if (item.Key.Contains("memory") || item.Key.Contains("keyspace") || item.Key.Contains("stats"))
                    {
                        stats[item.Key] = item.Value;
                    }
                }
            }
            
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache statistics");
            return new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Get all keys matching a pattern
    /// </summary>
    public async Task<IEnumerable<string>> GetKeysAsync(string pattern = "*")
    {
        try
        {
            var keys = _server.Keys(pattern: pattern).Select(k => k.ToString()).ToList();
            _logger.LogDebug("Found {Count} keys matching pattern: {Pattern}", keys.Count, pattern);
            return keys;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting keys with pattern: {Pattern}", pattern);
            return Enumerable.Empty<string>();
        }
    }

    /// <summary>
    /// Get TTL for a key
    /// </summary>
    public async Task<TimeSpan?> GetTtlAsync(string key)
    {
        try
        {
            return await _database.KeyTimeToLiveAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting TTL for key: {Key}", key);
            return null;
        }
    }

    /// <summary>
    /// Refresh expiration for a key
    /// </summary>
    public async Task RefreshAsync(string key, TimeSpan? expiration = null)
    {
        try
        {
            var defaultExpiration = expiration ?? TimeSpan.FromMinutes(30);
            await _database.KeyExpireAsync(key, defaultExpiration);
            _logger.LogDebug("Cache refreshed: {Key} (New expiration: {Expiration})", key, defaultExpiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing cache for key: {Key}", key);
        }
    }

    #endregion
}