using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Account.Profile;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Profile;

public interface IUserFollowingRepository : IBaseRepository<UserFollowing>
{
    Task<IEnumerable<UserFollowing>> GetUserFollowingAsync(Guid userId, bool activeOnly = true);
    Task<IEnumerable<UserFollowing>> GetUserFollowersAsync(Guid userId, bool activeOnly = true);
    Task<UserFollowing?> GetFollowingRelationshipAsync(Guid followerId, Guid followedUserId);
    Task<UserFollowing?> GetFollowingAsync(Guid followerId, Guid followedUserId);
    Task<bool> IsFollowingAsync(Guid followerId, Guid followedUserId);
    Task<int> GetFollowingCountAsync(Guid userId);
    Task<int> GetFollowersCountAsync(Guid userId);
    Task<IEnumerable<UserFollowing>> GetRecentFollowersAsync(Guid userId, int count = 10);
    Task<IEnumerable<UserFollowing>> GetMutualFollowingAsync(Guid userId1, Guid userId2);
    
    // Methods expected by the controller
    Task<bool> FollowUserAsync(Guid followerId, Guid followedUserId);
    Task<bool> UnfollowUserAsync(Guid followerId, Guid followedUserId);
    Task<IEnumerable<UserFollowing>> GetFollowingAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<IEnumerable<UserFollowing>> GetFollowersAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<IEnumerable<UserFollowing>> GetFollowSuggestionsAsync(Guid userId, int count = 10);
    Task<IEnumerable<UserFollowing>> GetMutualFollowersAsync(Guid userId1, Guid userId2);
}


