using System.Threading.Tasks;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.BackgroundJobs;

public interface IJobProcessor
{
    Task ProcessProfileStatisticsUpdateAsync(string userId);
    Task ProcessGamificationPointsCalculationAsync(string userId);
    Task ProcessFeedContentAggregationAsync();
    Task ProcessNotificationBatchAsync();
    Task ProcessContentModerationAsync(string contentId, string contentType);
    Task ProcessEmailBatchAsync();
    Task ProcessDataCleanupAsync();
    Task ProcessAnalyticsAggregationAsync();
}


