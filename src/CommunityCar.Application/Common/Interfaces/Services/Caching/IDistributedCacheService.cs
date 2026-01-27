using System;
using System.Threading.Tasks;

namespace CommunityCar.Application.Common.Interfaces.Services.Caching;

public interface IDistributedCacheService : ICacheService
{
    Task<T?> GetAsync<T>(string key, string region) where T : class;
    Task SetAsync<T>(string key, T value, string region, TimeSpan? expiration = null) where T : class;
    Task RemoveAsync(string key, string region);
    Task RemoveRegionAsync(string region);
    Task<T> GetOrSetAsync<T>(string key, string region, Func<Task<T>> getItem, TimeSpan? expiration = null) where T : class;
}


