using CommunityCar.Application.Common.Interfaces.Repositories.Profile;
using CommunityCar.Domain.Entities.Profile;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Profile;

public class UserFollowingRepository : BaseRepository<UserFollowing>, IUserFollowingRepository
{
    public UserFollowingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserFollowing>> GetUserFollowingAsync(Guid userId, bool activeOnly = true)
    {
        var query = DbSet.Where(uf => uf.FollowerId == userId);
        
        if (activeOnly)
        {
            query = query.Where(uf => uf.IsActive);
        }

        return await query
            .OrderByDescending(uf => uf.FollowedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserFollowing>> GetUserFollowersAsync(Guid userId, bool activeOnly = true)
    {
        var query = DbSet.Where(uf => uf.FollowedUserId == userId);
        
        if (activeOnly)
        {
            query = query.Where(uf => uf.IsActive);
        }

        return await query
            .OrderByDescending(uf => uf.FollowedAt)
            .ToListAsync();
    }

    public async Task<UserFollowing?> GetFollowingRelationshipAsync(Guid followerId, Guid followedUserId)
    {
        return await DbSet
            .FirstOrDefaultAsync(uf => uf.FollowerId == followerId && uf.FollowedUserId == followedUserId);
    }

    public async Task<bool> IsFollowingAsync(Guid followerId, Guid followedUserId)
    {
        return await DbSet
            .AnyAsync(uf => uf.FollowerId == followerId && uf.FollowedUserId == followedUserId && uf.IsActive);
    }

    public async Task<int> GetFollowingCountAsync(Guid userId)
    {
        return await DbSet
            .CountAsync(uf => uf.FollowerId == userId && uf.IsActive);
    }

    public async Task<int> GetFollowersCountAsync(Guid userId)
    {
        return await DbSet
            .CountAsync(uf => uf.FollowedUserId == userId && uf.IsActive);
    }

    public async Task<IEnumerable<UserFollowing>> GetRecentFollowersAsync(Guid userId, int count = 10)
    {
        return await DbSet
            .Where(uf => uf.FollowedUserId == userId && uf.IsActive)
            .OrderByDescending(uf => uf.FollowedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserFollowing>> GetMutualFollowingAsync(Guid userId1, Guid userId2)
    {
        var user1Following = await DbSet
            .Where(uf => uf.FollowerId == userId1 && uf.IsActive)
            .Select(uf => uf.FollowedUserId)
            .ToListAsync();

        return await DbSet
            .Where(uf => uf.FollowerId == userId2 && uf.IsActive && user1Following.Contains(uf.FollowedUserId))
            .ToListAsync();
    }
}
