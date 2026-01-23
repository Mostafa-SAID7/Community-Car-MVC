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
            .Where(f => (f.RequesterId == userId || f.ReceiverId == userId) && 
                       f.Status == FriendshipStatus.Accepted)
            .ToListAsync();
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

    public async Task<Friendship?> GetFriendshipAsync(Guid userId1, Guid userId2)
    {
        return await Context.Friendships
            .FirstOrDefaultAsync(f => 
                (f.RequesterId == userId1 && f.ReceiverId == userId2) ||
                (f.RequesterId == userId2 && f.ReceiverId == userId1));
    }

    public async Task<Friendship?> GetFriendshipByIdAsync(Guid friendshipId)
    {
        return await Context.Friendships
            .FirstOrDefaultAsync(f => f.Id == friendshipId);
    }

    public async Task<Friendship> CreateFriendshipAsync(Friendship friendship)
    {
        await Context.Friendships.AddAsync(friendship);
        await Context.SaveChangesAsync();
        return friendship;
    }

    public async Task<Friendship> UpdateFriendshipAsync(Friendship friendship)
    {
        Context.Friendships.Update(friendship);
        await Context.SaveChangesAsync();
        return friendship;
    }

    public async Task DeleteFriendshipAsync(Guid friendshipId)
    {
        var friendship = await GetFriendshipByIdAsync(friendshipId);
        if (friendship != null)
        {
            Context.Friendships.Remove(friendship);
            await Context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Guid>> GetMutualFriendsAsync(Guid userId1, Guid userId2)
    {
        var user1Friends = await Context.Friendships
            .Where(f => (f.RequesterId == userId1 || f.ReceiverId == userId1) && 
                       f.Status == FriendshipStatus.Accepted)
            .Select(f => f.RequesterId == userId1 ? f.ReceiverId : f.RequesterId)
            .ToListAsync();

        var user2Friends = await Context.Friendships
            .Where(f => (f.RequesterId == userId2 || f.ReceiverId == userId2) && 
                       f.Status == FriendshipStatus.Accepted)
            .Select(f => f.RequesterId == userId2 ? f.ReceiverId : f.RequesterId)
            .ToListAsync();

        return user1Friends.Intersect(user2Friends);
    }

    public async Task<IEnumerable<Guid>> GetFriendSuggestionsAsync(Guid userId, int count = 10)
    {
        // Get user's current friends
        var currentFriends = await Context.Friendships
            .Where(f => (f.RequesterId == userId || f.ReceiverId == userId) && 
                       f.Status == FriendshipStatus.Accepted)
            .Select(f => f.RequesterId == userId ? f.ReceiverId : f.RequesterId)
            .ToListAsync();

        // Get pending requests (both sent and received)
        var pendingUsers = await Context.Friendships
            .Where(f => (f.RequesterId == userId || f.ReceiverId == userId) && 
                       f.Status == FriendshipStatus.Pending)
            .Select(f => f.RequesterId == userId ? f.ReceiverId : f.RequesterId)
            .ToListAsync();

        // Exclude current user, friends, and pending requests
        var excludeUsers = new List<Guid> { userId };
        excludeUsers.AddRange(currentFriends);
        excludeUsers.AddRange(pendingUsers);

        // Get suggestions based on mutual friends
        var suggestions = await Context.Users
            .Where(u => !excludeUsers.Contains(u.Id))
            .Take(count)
            .Select(u => u.Id)
            .ToListAsync();

        return suggestions;
    }

    public async Task<bool> AreFriendsAsync(Guid userId1, Guid userId2)
    {
        return await Context.Friendships
            .AnyAsync(f => 
                ((f.RequesterId == userId1 && f.ReceiverId == userId2) ||
                 (f.RequesterId == userId2 && f.ReceiverId == userId1)) &&
                f.Status == FriendshipStatus.Accepted);
    }

    public async Task<int> GetFriendsCountAsync(Guid userId)
    {
        return await Context.Friendships
            .CountAsync(f => (f.RequesterId == userId || f.ReceiverId == userId) && 
                            f.Status == FriendshipStatus.Accepted);
    }
}