using CommunityCar.Application.Features.Account.ViewModels.Activity;
using CommunityCar.Application.Features.Account.ViewModels.Core;
using CommunityCar.Application.Features.Account.ViewModels.Social;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Account.Core;

public interface IUserAnalyticsService
{
    Task<UserActivityStatsVM> GetUserActivityStatsAsync(Guid userId);
    Task<List<UserInteractionVM>> GetUserInteractionsAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<UserEngagementStatsVM> GetUserEngagementStatsAsync(Guid userId);
    Task<List<StatsBoxVM>> GetUserStatsBoxesAsync(Guid userId);
    Task<bool> TrackUserActivityAsync(Guid userId, string activityType, string? details = null);
    Task<List<UserActivityVM>> GetRecentUserActivitiesAsync(Guid userId, int count = 10);
    Task<UserSocialStatsVM> GetUserSocialStatsAsync(Guid userId);
    Task<bool> UpdateUserEngagementAsync(Guid userId, string engagementType);
}