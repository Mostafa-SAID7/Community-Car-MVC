using CommunityCar.Application.Common.Interfaces.Services.Caching;
using CommunityCar.Application.Services.Caching;
using CommunityCar.Infrastructure.Services.Caching;
using CommunityCar.Web.Models.Dashboard.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace CommunityCar.Web.Controllers.Dashboard;

[Route("dashboard/cache")]
[Authorize(Roles = "Admin")]
public class CacheController : Controller
{
    private readonly ICacheService _cacheService;
    private readonly CacheWarmupService _cacheWarmupService;
    private readonly IConnectionMultiplexer? _connectionMultiplexer;
    private readonly ILogger<CacheController> _logger;

    public CacheController(
        ICacheService cacheService,
        CacheWarmupService cacheWarmupService,
        IConnectionMultiplexer? connectionMultiplexer,
        ILogger<CacheController> logger)
    {
        _cacheService = cacheService;
        _cacheWarmupService = cacheWarmupService;
        _connectionMultiplexer = connectionMultiplexer;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var model = new CacheManagementViewModel
            {
                IsRedisConnected = _connectionMultiplexer?.IsConnected ?? false,
                CacheType = _connectionMultiplexer != null ? "Redis" : "In-Memory",
                Statistics = await GetCacheStatisticsAsync()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading cache management page");
            TempData["ErrorMessage"] = "Failed to load cache statistics.";
            return View(new CacheManagementViewModel());
        }
    }

    [HttpPost("clear")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ClearCache()
    {
        try
        {
            await _cacheService.ClearAsync();
            TempData["SuccessMessage"] = "Cache cleared successfully.";
            _logger.LogInformation("Cache cleared by admin user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cache");
            TempData["ErrorMessage"] = "Failed to clear cache.";
        }

        return RedirectToAction("Index");
    }

    [HttpPost("warmup")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> WarmupCache()
    {
        try
        {
            await _cacheWarmupService.WarmupAllCacheAsync();
            TempData["SuccessMessage"] = "Cache warmup completed successfully.";
            _logger.LogInformation("Cache warmup initiated by admin user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error warming up cache");
            TempData["ErrorMessage"] = "Failed to warm up cache.";
        }

        return RedirectToAction("Index");
    }

    [HttpPost("clear-pattern")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ClearCachePattern(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            TempData["ErrorMessage"] = "Pattern cannot be empty.";
            return RedirectToAction("Index");
        }

        try
        {
            await _cacheService.RemoveByPatternAsync(pattern);
            TempData["SuccessMessage"] = $"Cache entries matching pattern '{pattern}' cleared successfully.";
            _logger.LogInformation("Cache pattern {Pattern} cleared by admin user", pattern);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cache pattern {Pattern}", pattern);
            TempData["ErrorMessage"] = $"Failed to clear cache pattern '{pattern}'.";
        }

        return RedirectToAction("Index");
    }

    [HttpGet("keys")]
    public async Task<IActionResult> ViewKeys(string pattern = "*")
    {
        try
        {
            var keys = new List<string>();
            
            if (_cacheService is RedisCacheService redisService)
            {
                keys = (await redisService.GetKeysAsync(pattern)).ToList();
            }

            var model = new CacheKeysViewModel
            {
                Pattern = pattern,
                Keys = keys,
                TotalCount = keys.Count
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cache keys");
            TempData["ErrorMessage"] = "Failed to retrieve cache keys.";
            return View(new CacheKeysViewModel { Pattern = pattern });
        }
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        try
        {
            var statistics = await GetCacheStatisticsAsync();
            return Json(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache statistics");
            return Json(new { error = "Failed to get statistics" });
        }
    }

    [HttpGet("key/{*key}")]
    public async Task<IActionResult> GetKeyValue(string key)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return BadRequest(new { error = "Key cannot be empty" });
            }

            // Decode the key from URL encoding
            key = Uri.UnescapeDataString(key);

            var exists = await _cacheService.ExistsAsync(key);
            if (!exists)
            {
                return NotFound(new { error = "Key not found" });
            }

            // Try to get the value as a generic object
            var value = "Key exists but value retrieval not implemented in current cache service";
            TimeSpan? ttl = null;

            if (_cacheService is RedisCacheService redisService)
            {
                ttl = await redisService.GetTtlAsync(key);
            }

            return Json(new
            {
                key = key,
                value = value,
                ttl = ttl?.TotalSeconds,
                exists = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting key value for {Key}", key);
            return Json(new { error = ex.Message });
        }
    }

    [HttpDelete("key/{*key}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteKey(string key)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return BadRequest(new { error = "Key cannot be empty" });
            }

            // Decode the key from URL encoding
            key = Uri.UnescapeDataString(key);

            await _cacheService.RemoveAsync(key);
            _logger.LogInformation("Cache key {Key} deleted by admin user", key);

            return Json(new { success = true, message = $"Key '{key}' deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting key {Key}", key);
            return Json(new { error = ex.Message });
        }
    }

    private async Task<Dictionary<string, object>> GetCacheStatisticsAsync()
    {
        var statistics = new Dictionary<string, object>();

        try
        {
            if (_cacheService is RedisCacheService redisService)
            {
                var redisStats = await redisService.GetCacheStatisticsAsync();
                foreach (var stat in redisStats)
                {
                    statistics[stat.Key] = stat.Value;
                }
            }
            else
            {
                statistics["cache_type"] = "In-Memory";
                statistics["status"] = "Active";
            }

            statistics["connection_status"] = _connectionMultiplexer?.IsConnected ?? false;
            statistics["last_updated"] = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache statistics");
            statistics["error"] = ex.Message;
        }

        return statistics;
    }
}


