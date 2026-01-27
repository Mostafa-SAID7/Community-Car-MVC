using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Profile;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Profile;

public interface IUserFollowingRepository : IBaseRepository<UserFollowing>
{
    Task<IEnumerable<UserFollowing>> GetUserFollowingAsync(Guid userId, bool activeOnly = true);
    Task<IEnumerable<UserFollowing>> GetUserFollowersAsync(Guid userId, bool activeOnly = true);
    Task<UserFollowing?> GetFollowingRelationshipAsync(Guid followerId, Guid followedUserId);
    Task<bool> IsFollowingAsync(Guid followerId, Guid followedUserId);
    Task<int> GetFollowingCountAsync(Guid userId);
    Task<int> GetFollowersCountAsync(Guid userId);
    Task<IEnumerable<UserFollowing>> GetRecentFollowersAsync(Guid userId, int count = 10);
    Task<IEnumerable<UserFollowing>> GetMutualFollowingAsync(Guid userId1, Guid userId2);
}


