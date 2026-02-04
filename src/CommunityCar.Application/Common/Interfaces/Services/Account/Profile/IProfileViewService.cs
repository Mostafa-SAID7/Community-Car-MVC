using CommunityCar.Application.Features.Account.ViewModels.Activity;

namespace CommunityCar.Application.Common.Interfaces.Services.Account.Profile;

public interface IProfileViewService
{
    Task<List<ProfileViewVM>> GetProfileViewsAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<List<ProfileViewVM>> GetWhoViewedMyProfileAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<bool> RecordProfileViewAsync(Guid viewerId, Guid profileUserId);
    Task<int> GetProfileViewCountAsync(Guid userId);
    Task<List<ProfileViewVM>> GetRecentProfileViewsAsync(Guid userId, int count = 10);
    Task<ProfileViewStatsVM> GetProfileViewStatsAsync(Guid userId);
}