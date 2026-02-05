using CommunityCar.Application.Features.Account.ViewModels.Activity;

namespace CommunityCar.Web.Areas.Identity.Interfaces.Services.Profile;

/// <summary>
/// Interface for profile view tracking services
/// </summary>
public interface IProfileViewService
{
    Task<bool> TrackProfileViewAsync(Guid profileUserId, Guid? viewerUserId = null, CancellationToken cancellationToken = default);
    Task<WhoViewedMyProfileVM> GetWhoViewedMyProfileAsync(Guid userId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<int> GetProfileViewCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetProfileViewCountTodayAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Dictionary<DateTime, int>> GetProfileViewsChartAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<List<ProfileViewStatsVM>> GetTopViewedProfilesAsync(int count = 10, CancellationToken cancellationToken = default);
}

