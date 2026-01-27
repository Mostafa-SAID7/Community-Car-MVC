using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

namespace CommunityCar.Application.Services.Account.Profile;

/// <summary>
/// Interface for profile statistics calculation
/// </summary>
public interface IProfileStatisticsService
{
    Task<ProfileStatsVM> GetProfileStatsAsync(Guid userId);
    Task<bool> UpdateProfileStatsAsync(Guid userId);
}


