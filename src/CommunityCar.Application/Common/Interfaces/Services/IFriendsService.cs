using CommunityCar.Application.Features.Friends.DTOs;
using CommunityCar.Application.Features.Friends.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services;

public interface IFriendsService
{
    Task<FriendsOverviewVM> GetFriendsOverviewAsync(Guid userId);
    Task<IEnumerable<FriendVM>> GetFriendsAsync(Guid userId);
    Task<IEnumerable<FriendRequestVM>> GetPendingRequestsAsync(Guid userId);
    Task<IEnumerable<FriendRequestVM>> GetSentRequestsAsync(Guid userId);
    Task<IEnumerable<FriendSuggestionVM>> GetFriendSuggestionsAsync(Guid userId, int count = 10);
    Task<FriendshipResultDTO> SendFriendRequestAsync(Guid requesterId, Guid receiverId);
    Task<FriendshipResultDTO> AcceptFriendRequestAsync(Guid userId, Guid friendshipId);
    Task<FriendshipResultDTO> DeclineFriendRequestAsync(Guid userId, Guid friendshipId);
    Task<FriendshipResultDTO> RemoveFriendAsync(Guid userId, Guid friendId);
    Task<FriendshipResultDTO> BlockUserAsync(Guid userId, Guid userToBlockId);
    Task<FriendshipResultDTO> UnblockUserAsync(Guid userId, Guid userToUnblockId);
    Task<FriendshipStatusDTO> GetFriendshipStatusAsync(Guid userId1, Guid userId2);
    Task<IEnumerable<FriendVM>> GetMutualFriendsAsync(Guid userId1, Guid userId2);
}


