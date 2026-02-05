using CommunityCar.Web.Areas.Identity.Interfaces.Repositories;
using CommunityCar.Domain.Entities.Account.Profile;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Web.Areas.Identity.Repositories.Social;

/// <summary>
/// Repository implementation for UserFollowing entity operations
/// </summary>
public class UserFollowingRepository : BaseRepository<UserFollowing>, IUserFollowingRepository
{
    public UserFollowingRepository(ApplicationDbContext context) : base(context)
    {
    }

    #region Following Management

    public async Task<bool> FollowUserAsync(Guid followerId, Guid followingId)
    {
        if (followerId == followingId) return false; // Can't follow yourself

        var existingRelationship = await GetFollowingRelationshipAsync(followerId, followingId);
        if (existingRelationship != null) return false; // Already following

        var following = UserFollowing.Create(followerId, followingId);
        await AddAsync(following);
        return true;
    }

    public async Task<bool> UnfollowUserAsync(Guid followerId, Guid followingId)
    {
        var relationship = await GetFollowingRelationshipAsync(followerId, followingId);
        
        if (relationship == null) return false;

        await DeleteAsync(relationship);
        return true;
    }

    public async Task<bool> IsFollowingAsync(Guid followerId, Guid followingId)
    {
        return await Context.UserFollowings
            .AnyAsync(uf => uf.FollowerId == followerId && uf.FollowedUserId == followingId);
    }

    public async Task<UserFollowing?> GetFollowingRelationshipAsync(Guid followerId, Guid followingId)
    {
        return await Context.UserFollowings
            .FirstOrDefaultAsync(uf => uf.FollowerId == followerId && uf.FollowedUserId == followingId);
    }

    #endregion

    #region Following Lists

    public async Task<IEnumerable<UserFollowing>> GetFollowingAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        return await Context.UserFollowings
            .Where(uf => uf.FollowerId == userId)
            .OrderByDescending(uf => uf.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserFollowing>> GetFollowersAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        return await Context.UserFollowings
            .Where(uf => uf.FollowedUserId == userId)
            .OrderByDescending(uf => uf.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserFollowing>> GetMutualFollowingAsync(Guid userId1, Guid userId2)
    {
        var user1Following = await Context.UserFollowings
            .Where(uf => uf.FollowerId == userId1)
            .Select(uf => uf.FollowedUserId)
            .ToListAsync();

        var user2Following = await Context.UserFollowings
            .Where(uf => uf.FollowerId == userId2)
            .Select(uf => uf.FollowedUserId)
            .ToListAsync();

        var mutualIds = user1Following.Intersect(user2Following).ToList();

        return await Context.UserFollowings
            .Where(uf => uf.FollowerId == userId1 && mutualIds.Contains(uf.FollowedUserId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Guid>> GetFollowingIdsAsync(Guid userId)
    {
        return await Context.UserFollowings
            .Where(uf => uf.FollowerId == userId)
            .Select(uf => uf.FollowedUserId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Guid>> GetFollowerIdsAsync(Guid userId)
    {
        return await Context.UserFollowings
            .Where(uf => uf.FollowedUserId == userId)
            .Select(uf => uf.FollowerId)
            .ToListAsync();
    }

    #endregion

    #region Following Analytics

    public async Task<int> GetFollowingCountAsync(Guid userId)
    {
        return await Context.UserFollowings
            .Where(uf => uf.FollowerId == userId)
            .CountAsync();
    }

    public async Task<int> GetFollowerCountAsync(Guid userId)
    {
        return await Context.UserFollowings
            .Where(uf => uf.FollowedUserId == userId)
            .CountAsync();
    }

    public async Task<IEnumerable<UserFollowing>> GetRecentFollowersAsync(Guid userId, int count = 10)
    {
        return await Context.UserFollowings
            .Where(uf => uf.FollowedUserId == userId)
            .OrderByDescending(uf => uf.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserFollowing>> GetRecentFollowingAsync(Guid userId, int count = 10)
    {
        return await Context.UserFollowings
            .Where(uf => uf.FollowerId == userId)
            .OrderByDescending(uf => uf.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Dictionary<Guid, int>> GetFollowingStatisticsAsync(IEnumerable<Guid> userIds)
    {
        var followingCounts = await Context.UserFollowings
            .Where(uf => userIds.Contains(uf.FollowerId))
            .GroupBy(uf => uf.FollowerId)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UserId, x => x.Count);

        var followerCounts = await Context.UserFollowings
            .Where(uf => userIds.Contains(uf.FollowedUserId))
            .GroupBy(uf => uf.FollowedUserId)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UserId, x => x.Count);

        var result = new Dictionary<Guid, int>();
        foreach (var userId in userIds)
        {
            var followingCount = followingCounts.GetValueOrDefault(userId, 0);
            var followerCount = followerCounts.GetValueOrDefault(userId, 0);
            result[userId] = followingCount + followerCount; // Total connections
        }

        return result;
    }

    #endregion

    #region Following Suggestions

    public async Task<IEnumerable<Guid>> GetSuggestedFollowingAsync(Guid userId, int count = 10)
    {
        // Get users that the user's following are following (friends of friends)
        var userFollowing = await GetFollowingIdsAsync(userId);
        
        var suggestions = await Context.UserFollowings
            .Where(uf => userFollowing.Contains(uf.FollowerId) && uf.FollowedUserId != userId)
            .Where(uf => !userFollowing.Contains(uf.FollowedUserId))
            .GroupBy(uf => uf.FollowedUserId)
            .OrderByDescending(g => g.Count())
            .Take(count)
            .Select(g => g.Key)
            .ToListAsync();

        return suggestions;
    }

    public async Task<IEnumerable<Guid>> GetMutualConnectionsAsync(Guid userId1, Guid userId2)
    {
        var user1Following = await GetFollowingIdsAsync(userId1);
        var user2Following = await GetFollowingIdsAsync(userId2);

        return user1Following.Intersect(user2Following).ToList();
    }

    public async Task<bool> AreMutualFollowersAsync(Guid userId1, Guid userId2)
    {
        var user1FollowsUser2 = await IsFollowingAsync(userId1, userId2);
        var user2FollowsUser1 = await IsFollowingAsync(userId2, userId1);

        return user1FollowsUser2 && user2FollowsUser1;
    }

    #endregion
}

