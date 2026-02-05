using CommunityCar.Application.Features.SEO.ViewModels;
using CommunityCar.Application.Features.Dashboard.Analytics.Users.Segments;
using CommunityCar.Application.Features.Dashboard.Management.developer.Performance.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Performance;

public interface IPerformanceRepository
{
    Task<PerformanceMetricsVM> GetPerformanceMetricsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<PerformanceTrendVM>> GetPerformanceTrendsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<PerformanceReportVM> GetPerformanceReportAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<bool> LogPerformanceMetricAsync(string metricName, double value, DateTime? timestamp = null);
    Task<List<SlowQueryVM>> GetSlowQueriesAsync(int count = 10);
}