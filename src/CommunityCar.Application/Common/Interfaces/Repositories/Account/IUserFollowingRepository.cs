using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Account.Profile;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Account;

/// <summary>
/// Repository interface for UserFollowing entity operations
/// </summary>
public interface IUserFollowingRepository : IBaseRepository<UserFollowing>
{
    #region Following Management
    Task<bool> FollowUserAsync(Guid followerId, Guid followingId);
    Task<bool> UnfollowUserAsync(Guid followerId, Guid followingId);
    Task<bool> IsFollowingAsync(Guid followerId, Guid followingId);
    Task<UserFollowing?> GetFollowingRelationshipAsync(Guid followerId, Guid followingId);
    #endregion

    #region Following Lists
    Task<IEnumerable<UserFollowing>> GetFollowingAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<IEnumerable<UserFollowing>> GetFollowersAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<IEnumerable<UserFollowing>> GetMutualFollowingAsync(Guid userId1, Guid userId2);
    Task<IEnumerable<Guid>> GetFollowingIdsAsync(Guid userId);
    Task<IEnumerable<Guid>> GetFollowerIdsAsync(Guid userId);
    #endregion

    #region Following Analytics
    Task<int> GetFollowingCountAsync(Guid userId);
    Task<int> GetFollowerCountAsync(Guid userId);
    Task<IEnumerable<UserFollowing>> GetRecentFollowersAsync(Guid userId, int count = 10);
    Task<IEnumerable<UserFollowing>> GetRecentFollowingAsync(Guid userId, int count = 10);
    Task<Dictionary<Guid, int>> GetFollowingStatisticsAsync(IEnumerable<Guid> userIds);
    #endregion

    #region Following Suggestions
    Task<IEnumerable<Guid>> GetSuggestedFollowingAsync(Guid userId, int count = 10);
    Task<IEnumerable<Guid>> GetMutualConnectionsAsync(Guid userId1, Guid userId2);
    Task<bool> AreMutualFollowersAsync(Guid userId1, Guid userId2);
    #endregion
}