using CommunityCar.Application.Features.Dashboard.ViewModels;

using CommunityCar.Application.Features.Dashboard.Performance.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Performance;

public interface IPerformanceService
{
    Task<PerformanceMetricsVM> GetPerformanceMetricsAsync(DateTime startDate, DateTime endDate);
    Task<List<ChartDataVM>> GetResponseTimeChartAsync(int days);
    Task<List<ChartDataVM>> GetThroughputChartAsync(int days);
    Task<List<ChartDataVM>> GetErrorRateChartAsync(int days);
    Task<List<ChartDataVM>> GetMemoryUsageChartAsync(int days);
    Task<List<ChartDataVM>> GetCpuUsageChartAsync(int days);
    Task<DatabasePerformanceVM> GetDatabasePerformanceAsync();
    Task<List<SlowQueryVM>> GetSlowQueriesAsync(int limit = 10);
    Task<CachePerformanceVM> GetCachePerformanceAsync();
    Task<List<EndpointPerformanceVM>> GetEndpointPerformanceAsync(int limit = 20);
    Task<SystemResourcesVM> GetSystemResourcesAsync();
    Task<bool> OptimizePerformanceAsync();
    Task ClearPerformanceCacheAsync();
}