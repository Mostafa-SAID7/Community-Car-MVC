using CommunityCar.Domain.Entities.Community.Friends;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IFriendsRepository
{
    Task<IEnumerable<Friendship>> GetUserFriendsAsync(Guid userId);
    Task<IEnumerable<Friendship>> GetPendingFriendRequestsAsync(Guid userId);
    Task<IEnumerable<Friendship>> GetSentFriendRequestsAsync(Guid userId);
    Task<Friendship?> GetFriendshipAsync(Guid userId1, Guid userId2);
    Task<Friendship?> GetFriendshipByIdAsync(Guid friendshipId);
    Task<Friendship> CreateFriendshipAsync(Friendship friendship);
    Task<Friendship> UpdateFriendshipAsync(Friendship friendship);
    Task DeleteFriendshipAsync(Guid friendshipId);
    Task<IEnumerable<Guid>> GetMutualFriendsAsync(Guid userId1, Guid userId2);
    Task<IEnumerable<Guid>> GetFriendSuggestionsAsync(Guid userId, int count = 10);
    Task<bool> AreFriendsAsync(Guid userId1, Guid userId2);
    Task<int> GetFriendsCountAsync(Guid userId);
}


