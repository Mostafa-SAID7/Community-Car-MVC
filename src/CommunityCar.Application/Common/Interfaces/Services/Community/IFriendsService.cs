using CommunityCar.Application.Features.Community.Friends.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Community;

public interface IFriendsService
{
    Task<FriendsOverviewVM> GetFriendsOverviewAsync(Guid userId);
    Task<IEnumerable<FriendVM>> GetFriendsAsync(Guid userId);
    Task<IEnumerable<FriendRequestVM>> GetPendingRequestsAsync(Guid userId);
    Task<IEnumerable<FriendRequestVM>> GetSentRequestsAsync(Guid userId);
    Task<IEnumerable<FriendSuggestionVM>> GetFriendSuggestionsAsync(Guid userId, int count = 10);
    Task<FriendshipResultVM> SendFriendRequestAsync(Guid requesterId, Guid receiverId);
    Task<FriendshipResultVM> AcceptFriendRequestAsync(Guid userId, Guid friendshipId);
    Task<FriendshipResultVM> DeclineFriendRequestAsync(Guid userId, Guid friendshipId);
    Task<FriendshipResultVM> RemoveFriendAsync(Guid userId, Guid friendId);
    Task<FriendshipResultVM> BlockUserAsync(Guid userId, Guid userToBlockId);
    Task<FriendshipResultVM> UnblockUserAsync(Guid userId, Guid userToUnblockId);
    Task<FriendshipStatusVM> GetFriendshipStatusAsync(Guid userId1, Guid userId2);
    Task<IEnumerable<FriendVM>> GetMutualFriendsAsync(Guid userId1, Guid userId2);
}


