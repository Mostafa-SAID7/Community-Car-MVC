using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Domain.Entities.Community.Friends;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class FriendsRepository : BaseRepository<Friendship>, IFriendsRepository
{
    public FriendsRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Friendship>> GetUserFriendsAsync(Guid userId)
    {
        return await Context.Friendships
            .Where(f => (f.RequesterId == userId || f.ReceiverId == userId) && f.Status == FriendshipStatus.Accepted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Guid>> GetMutualFriendsAsync(Guid userId1, Guid userId2)
    {
        var user1Friends = await GetUserFriendIdsAsync(userId1);
        var user2Friends = await GetUserFriendIdsAsync(userId2);
        return user1Friends.Intersect(user2Friends);
    }

    public async Task<IEnumerable<Friendship>> GetPendingFriendRequestsAsync(Guid userId)
    {
        return await Context.Friendships
            .Where(f => f.ReceiverId == userId && f.Status == FriendshipStatus.Pending)
            .ToListAsync();
    }

    public async Task<IEnumerable<Friendship>> GetSentFriendRequestsAsync(Guid userId)
    {
        return await Context.Friendships
            .Where(f => f.RequesterId == userId && f.Status == FriendshipStatus.Pending)
            .ToListAsync();
    }

    public async Task<IEnumerable<Guid>> GetFriendSuggestionsAsync(Guid userId, int count)
    {
        // Simple suggestion: users who are not friends yet
        var friendIds = await GetUserFriendIdsAsync(userId);
        return await Context.Users
            .Where(u => u.Id != userId && !friendIds.Contains(u.Id))
            .Take(count)
            .Select(u => u.Id)
            .ToListAsync();
    }

    public async Task<Friendship?> GetFriendshipAsync(Guid userId1, Guid userId2)
    {
        return await Context.Friendships
            .FirstOrDefaultAsync(f => (f.RequesterId == userId1 && f.ReceiverId == userId2) ||
                                      (f.RequesterId == userId2 && f.ReceiverId == userId1));
    }

    public async Task<Friendship> CreateFriendshipAsync(Friendship friendship)
    {
        await AddAsync(friendship);
        return friendship;
    }

    public async Task<Friendship?> GetFriendshipByIdAsync(Guid friendshipId)
    {
        return await GetByIdAsync(friendshipId);
    }

    public async Task UpdateFriendshipAsync(Friendship friendship)
    {
        await UpdateAsync(friendship);
    }

    public async Task DeleteFriendshipAsync(Guid friendshipId)
    {
        var friendship = await GetByIdAsync(friendshipId);
        if (friendship != null)
        {
            await DeleteAsync(friendship);
        }
    }

    private async Task<List<Guid>> GetUserFriendIdsAsync(Guid userId)
    {
        var friendships = await GetUserFriendsAsync(userId);
        return friendships
            .Select(f => f.RequesterId == userId ? f.ReceiverId : f.RequesterId)
            .ToList();
    }
}
