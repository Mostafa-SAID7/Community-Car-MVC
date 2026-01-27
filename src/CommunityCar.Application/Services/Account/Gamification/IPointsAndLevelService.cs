using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

namespace CommunityCar.Application.Services.Account.Gamification;

/// <summary>
/// Interface for points and level management
/// </summary>
public interface IPointsAndLevelService
{
    Task<UserGamificationStatsVM> GetUserStatsAsync(Guid userId);
    Task<int> GetUserPointsAsync(Guid userId);
    Task<bool> AwardPointsAsync(Guid userId, int points, string reason);
    Task<bool> AddPointsAsync(Guid userId, int points, string reason);
    Task<bool> DeductPointsAsync(Guid userId, int points, string reason);
    Task<IEnumerable<object>> GetPointHistoryAsync(Guid userId);
    Task<IEnumerable<object>> GetLeaderboardAsync(int count = 10);
    Task<int> GetUserRankAsync(Guid userId);
    Task<int> GetUserLevelAsync(Guid userId);
}


