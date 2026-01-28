using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using CommunityCar.Application.Common.Interfaces.Services.Caching;
using CommunityCar.Infrastructure.Services.Caching;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Caching.SqlServer;

namespace CommunityCar.Infrastructure.Configuration;

/// <summary>
/// Configuration for Redis caching
/// </summary>
public static class RedisConfiguration
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var cacheSettings = configuration.GetSection(CacheSettings.SectionName).Get<CacheSettings>();
        
        if (cacheSettings?.EnableDistributedCache == true && !string.IsNullOrEmpty(cacheSettings.RedisConnectionString))
        {
            // Add Redis connection
            services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<IConnectionMultiplexer>>();
                
                try
                {
                    var configuration = ConfigurationOptions.Parse(cacheSettings.RedisConnectionString);
                    configuration.AbortOnConnectFail = false;
                    configuration.ConnectTimeout = 10000;
                    configuration.SyncTimeout = 10000;
                    configuration.AsyncTimeout = 10000;
                    configuration.ConnectRetry = 3;
                    configuration.ReconnectRetryPolicy = new ExponentialRetry(1000);
                    
                    var connection = ConnectionMultiplexer.Connect(configuration);
                    
                    connection.ConnectionFailed += (sender, args) =>
                    {
                        logger.LogError("Redis connection failed: {Exception}", args.Exception?.Message);
                    };
                    
                    connection.ConnectionRestored += (sender, args) =>
                    {
                        logger.LogInformation("Redis connection restored");
                    };
                    
                    connection.ErrorMessage += (sender, args) =>
                    {
                        logger.LogError("Redis error: {Message}", args.Message);
                    };
                    
                    logger.LogInformation("Redis connection established successfully");
                    return connection;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to connect to Redis. Falling back to in-memory cache.");
                    throw;
                }
            });

            // Register Redis-specific cache service
            services.AddScoped<RedisCacheService>();
            
            // Use Redis cache as primary cache service
            services.AddScoped<ICacheService>(provider =>
            {
                try
                {
                    return provider.GetRequiredService<RedisCacheService>();
                }
                catch
                {
                    // Fallback to hybrid cache service if Redis is not available
                    return provider.GetRequiredService<CommunityCar.Infrastructure.Caching.CacheService>();
                }
            });
            
            services.AddScoped<IDistributedCacheService>(provider =>
            {
                try
                {
                    return provider.GetRequiredService<RedisCacheService>();
                }
                catch
                {
                    // Fallback to distributed cache service if Redis is not available
                    return provider.GetRequiredService<DistributedCacheService>();
                }
            });
        }
        else
        {
            // Use in-memory caching only
            services.AddScoped<ICacheService, CommunityCar.Infrastructure.Caching.CacheService>();
            services.AddScoped<IDistributedCacheService, DistributedCacheService>();
        }

        // Add distributed cache (SQL Server or Redis)
        if (cacheSettings?.EnableDistributedCache == true && !string.IsNullOrEmpty(cacheSettings.RedisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = cacheSettings.RedisConnectionString;
                options.InstanceName = "CommunityCar";
            });
        }
        else
        {
            // Fallback to SQL Server distributed cache
            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                options.SchemaName = "dbo";
                options.TableName = "DistributedCache";
            });
        }

        return services;
    }

    public static async Task<bool> TestRedisConnectionAsync(IConnectionMultiplexer connectionMultiplexer, ILogger logger)
    {
        try
        {
            var database = connectionMultiplexer.GetDatabase();
            var testKey = "connection-test";
            var testValue = DateTime.UtcNow.ToString();
            
            await database.StringSetAsync(testKey, testValue, TimeSpan.FromSeconds(10));
            var retrievedValue = await database.StringGetAsync(testKey);
            
            if (retrievedValue == testValue)
            {
                await database.KeyDeleteAsync(testKey);
                logger.LogInformation("Redis connection test successful");
                return true;
            }
            else
            {
                logger.LogWarning("Redis connection test failed: value mismatch");
                return false;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Redis connection test failed");
            return false;
        }
    }

    public static async Task WarmupRedisConnectionAsync(IServiceProvider serviceProvider)
    {
        try
        {
            var connectionMultiplexer = serviceProvider.GetService<IConnectionMultiplexer>();
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("RedisConfiguration");
            
            if (connectionMultiplexer != null)
            {
                logger.LogInformation("Warming up Redis connection");
                
                var isConnected = await TestRedisConnectionAsync(connectionMultiplexer, logger);
                if (isConnected)
                {
                    // Warm up cache with critical data
                    var cacheWarmupService = serviceProvider.GetRequiredService<CommunityCar.Application.Services.Caching.CacheWarmupService>();
                    await cacheWarmupService.WarmupReferenceDataAsync();
                    
                    logger.LogInformation("Redis connection warmed up successfully");
                }
                else
                {
                    logger.LogWarning("Redis connection warmup failed");
                }
            }
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("RedisConfiguration");
            logger.LogError(ex, "Failed to warm up Redis connection");
        }
    }
}
