using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Community.Friends;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IFriendsRepository : IBaseRepository<Friendship>
{
    Task<IEnumerable<Friendship>> GetUserFriendsAsync(Guid userId);
    Task<IEnumerable<Guid>> GetMutualFriendsAsync(Guid userId1, Guid userId2);
    Task<IEnumerable<Friendship>> GetPendingFriendRequestsAsync(Guid userId);
    Task<IEnumerable<Friendship>> GetSentFriendRequestsAsync(Guid userId);
    Task<IEnumerable<Guid>> GetFriendSuggestionsAsync(Guid userId, int count);
    Task<Friendship?> GetFriendshipAsync(Guid userId1, Guid userId2);
    Task<Friendship> CreateFriendshipAsync(Friendship friendship);
    Task<Friendship?> GetFriendshipByIdAsync(Guid friendshipId);
    Task UpdateFriendshipAsync(Friendship friendship);
    Task DeleteFriendshipAsync(Guid friendshipId);
}
