using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Features.Community.Friends.ViewModels;
using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Entities.Community.Friends;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Application.Services.Community.Friends;

public class FriendsService : IFriendsService
{
    private readonly IFriendsRepository _friendsRepository;
    private readonly UserManager<User> _userManager;
    private readonly INotificationService _notificationService;

    public FriendsService(IFriendsRepository friendsRepository, UserManager<User> userManager, INotificationService notificationService)
    {
        _friendsRepository = friendsRepository;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    public async Task<FriendsOverviewVM> GetFriendsOverviewAsync(Guid userId)
    {
        var friends = await GetFriendsAsync(userId);
        var pendingRequests = await GetPendingRequestsAsync(userId);
        var sentRequests = await GetSentRequestsAsync(userId);
        var suggestions = await GetFriendSuggestionsAsync(userId, 5);

        return new FriendsOverviewVM
        {
            TotalFriends = friends.Count(),
            PendingRequests = pendingRequests.Count(),
            SentRequests = sentRequests.Count(),
            RecentFriends = friends.Take(6),
            RecentRequests = pendingRequests.Take(3),
            Suggestions = suggestions
        };
    }

    public async Task<IEnumerable<FriendVM>> GetFriendsAsync(Guid userId)
    {
        var friendships = await _friendsRepository.GetUserFriendsAsync(userId);
        var friendVMs = new List<FriendVM>();

        foreach (var friendship in friendships)
        {
            var friendId = friendship.RequesterId == userId ? friendship.ReceiverId : friendship.RequesterId;
            var friend = await _userManager.FindByIdAsync(friendId.ToString());
            
            if (friend != null)
            {
                var mutualFriendsCount = (await _friendsRepository.GetMutualFriendsAsync(userId, friendId)).Count();
                
                friendVMs.Add(new FriendVM
                {
                    UserId = friend.Id,
                    FullName = friend.FullName,
                    UserName = friend.UserName ?? "",
                    ProfilePictureUrl = friend.ProfilePictureUrl,
                    City = friend.City,
                    Country = friend.Country,
                    Bio = friend.Bio,
                    FriendsSince = friendship.CreatedAt,
                    IsOnline = friend.OAuthInfo.LastLoginAt.HasValue && friend.OAuthInfo.LastLoginAt > DateTime.UtcNow.AddMinutes(-15),
                    LastSeen = friend.OAuthInfo.LastLoginAt,
                    MutualFriendsCount = mutualFriendsCount
                });
            }
        }

        return friendVMs.OrderBy(f => f.FullName);
    }

    public async Task<IEnumerable<FriendRequestVM>> GetPendingRequestsAsync(Guid userId)
    {
        var requests = await _friendsRepository.GetPendingFriendRequestsAsync(userId);
        var requestVMs = new List<FriendRequestVM>();

        foreach (var request in requests)
        {
            var requester = await _userManager.FindByIdAsync(request.RequesterId.ToString());
            
            if (requester != null)
            {
                var mutualFriendsCount = (await _friendsRepository.GetMutualFriendsAsync(userId, requester.Id)).Count();
                
                requestVMs.Add(new FriendRequestVM
                {
                    FriendshipId = request.Id,
                    UserId = requester.Id,
                    FullName = requester.FullName,
                    UserName = requester.UserName ?? "",
                    ProfilePictureUrl = requester.ProfilePictureUrl,
                    City = requester.City,
                    Country = requester.Country,
                    Bio = requester.Bio,
                    RequestDate = request.CreatedAt,
                    MutualFriendsCount = mutualFriendsCount,
                    IsIncoming = true
                });
            }
        }

        return requestVMs.OrderByDescending(r => r.RequestDate);
    }

    public async Task<IEnumerable<FriendRequestVM>> GetSentRequestsAsync(Guid userId)
    {
        var requests = await _friendsRepository.GetSentFriendRequestsAsync(userId);
        var requestVMs = new List<FriendRequestVM>();

        foreach (var request in requests)
        {
            var receiver = await _userManager.FindByIdAsync(request.ReceiverId.ToString());
            
            if (receiver != null)
            {
                var mutualFriendsCount = (await _friendsRepository.GetMutualFriendsAsync(userId, receiver.Id)).Count();
                
                requestVMs.Add(new FriendRequestVM
                {
                    FriendshipId = request.Id,
                    UserId = receiver.Id,
                    FullName = receiver.FullName,
                    UserName = receiver.UserName ?? "",
                    ProfilePictureUrl = receiver.ProfilePictureUrl,
                    City = receiver.City,
                    Country = receiver.Country,
                    Bio = receiver.Bio,
                    RequestDate = request.CreatedAt,
                    MutualFriendsCount = mutualFriendsCount,
                    IsIncoming = false
                });
            }
        }

        return requestVMs.OrderByDescending(r => r.RequestDate);
    }

    public async Task<IEnumerable<FriendSuggestionVM>> GetFriendSuggestionsAsync(Guid userId, int count = 10)
    {
        var suggestionIds = await _friendsRepository.GetFriendSuggestionsAsync(userId, count);
        var suggestions = new List<FriendSuggestionVM>();

        foreach (var suggestionId in suggestionIds)
        {
            var user = await _userManager.FindByIdAsync(suggestionId.ToString());
            
            if (user != null)
            {
                var mutualFriends = await _friendsRepository.GetMutualFriendsAsync(userId, suggestionId);
                var mutualFriendsNames = new List<string>();
                
                foreach (var mutualFriendId in mutualFriends.Take(3))
                {
                    var mutualFriend = await _userManager.FindByIdAsync(mutualFriendId.ToString());
                    if (mutualFriend != null)
                    {
                        mutualFriendsNames.Add(mutualFriend.FullName);
                    }
                }

                var reason = mutualFriends.Any() 
                    ? $"Mutual friends with {string.Join(", ", mutualFriendsNames.Take(2))}"
                    : "Suggested for you";

                suggestions.Add(new FriendSuggestionVM
                {
                    UserId = user.Id,
                    FullName = user.Profile.FullName,
                    UserName = user.UserName ?? "",
                    ProfilePictureUrl = user.Profile.ProfilePictureUrl,
                    City = user.Profile.City,
                    Country = user.Profile.Country,
                    Bio = user.Profile.Bio,
                    MutualFriendsCount = mutualFriends.Count(),
                    MutualFriendsNames = mutualFriendsNames,
                    SuggestionReason = reason
                });
            }
        }

        return suggestions;
    }

    public async Task<FriendshipResultVM> SendFriendRequestAsync(Guid requesterId, Guid receiverId)
    {
        if (requesterId == receiverId)
        {
            return new FriendshipResultVM { Success = false, Message = "Cannot send friend request to yourself" };
        }

        var existingFriendship = await _friendsRepository.GetFriendshipAsync(requesterId, receiverId);
        if (existingFriendship != null)
        {
            return existingFriendship.Status switch
            {
                FriendshipStatus.Accepted => new FriendshipResultVM { Success = false, Message = "You are already friends" },
                FriendshipStatus.Pending => new FriendshipResultVM { Success = false, Message = "Friend request already sent" },
                FriendshipStatus.Blocked => new FriendshipResultVM { Success = false, Message = "Cannot send friend request" },
                _ => new FriendshipResultVM { Success = false, Message = "Unknown friendship status" }
            };
        }

        var receiver = await _userManager.FindByIdAsync(receiverId.ToString());
        if (receiver == null)
        {
            return new FriendshipResultVM { Success = false, Message = "User not found" };
        }

        var friendship = new Friendship(requesterId, receiverId);
        await _friendsRepository.CreateFriendshipAsync(friendship);

        // Send notification to receiver
        var requester = await _userManager.FindByIdAsync(requesterId.ToString());
        if (requester != null)
        {
            await _notificationService.NotifyFriendRequestAsync(receiverId, requester.FullName, requesterId);
        }

        return new FriendshipResultVM 
        { 
            Success = true, 
            Message = $"Friend request sent to {receiver.FullName}",
            FriendshipId = friendship.Id
        };
    }

    public async Task<FriendshipResultVM> AcceptFriendRequestAsync(Guid userId, Guid friendshipId)
    {
        var friendship = await _friendsRepository.GetFriendshipByIdAsync(friendshipId);
        if (friendship == null)
        {
            return new FriendshipResultVM { Success = false, Message = "Friend request not found" };
        }

        if (friendship.ReceiverId != userId)
        {
            return new FriendshipResultVM { Success = false, Message = "You can only accept requests sent to you" };
        }

        if (friendship.Status != FriendshipStatus.Pending)
        {
            return new FriendshipResultVM { Success = false, Message = "Friend request is no longer pending" };
        }

        friendship.Accept();
        await _friendsRepository.UpdateFriendshipAsync(friendship);

        // Send notification to requester
        var requester = await _userManager.FindByIdAsync(friendship.RequesterId.ToString());
        var accepter = await _userManager.FindByIdAsync(userId.ToString());
        if (requester != null && accepter != null)
        {
            await _notificationService.NotifyFriendRequestAcceptedAsync(friendship.RequesterId, accepter.FullName, userId);
        }

        return new FriendshipResultVM 
        { 
            Success = true, 
            Message = $"You are now friends with {requester?.FullName}",
            FriendshipId = friendship.Id
        };
    }

    public async Task<FriendshipResultVM> DeclineFriendRequestAsync(Guid userId, Guid friendshipId)
    {
        var friendship = await _friendsRepository.GetFriendshipByIdAsync(friendshipId);
        if (friendship == null)
        {
            return new FriendshipResultVM { Success = false, Message = "Friend request not found" };
        }

        if (friendship.ReceiverId != userId)
        {
            return new FriendshipResultVM { Success = false, Message = "You can only decline requests sent to you" };
        }

        await _friendsRepository.DeleteFriendshipAsync(friendshipId);

        // Send notification to requester (optional - might be too much notification)
        var decliner = await _userManager.FindByIdAsync(userId.ToString());
        var requester = await _userManager.FindByIdAsync(friendship.RequesterId.ToString());
        if (decliner != null && requester != null)
        {
            // Only notify if it's a decline, not a cancel (when userId == RequesterId)
            if (userId != friendship.RequesterId)
            {
                await _notificationService.NotifyFriendRequestDeclinedAsync(friendship.RequesterId, decliner.FullName);
            }
        }

        return new FriendshipResultVM { Success = true, Message = "Friend request declined" };
    }

    public async Task<FriendshipResultVM> RemoveFriendAsync(Guid userId, Guid friendId)
    {
        var friendship = await _friendsRepository.GetFriendshipAsync(userId, friendId);
        if (friendship == null)
        {
            return new FriendshipResultVM { Success = false, Message = "Friendship not found" };
        }

        if (friendship.Status != FriendshipStatus.Accepted)
        {
            return new FriendshipResultVM { Success = false, Message = "You are not friends with this user" };
        }

        await _friendsRepository.DeleteFriendshipAsync(friendship.Id);

        // Send notification to the removed friend
        var remover = await _userManager.FindByIdAsync(userId.ToString());
        if (remover != null)
        {
            await _notificationService.NotifyFriendRemovedAsync(friendId, remover.FullName);
        }

        var friend = await _userManager.FindByIdAsync(friendId.ToString());
        return new FriendshipResultVM 
        { 
            Success = true, 
            Message = $"Removed {friend?.FullName} from your friends"
        };
    }

    public async Task<FriendshipResultVM> BlockUserAsync(Guid userId, Guid userToBlockId)
    {
        var friendship = await _friendsRepository.GetFriendshipAsync(userId, userToBlockId);
        
        if (friendship == null)
        {
            // Create new friendship with blocked status
            friendship = new Friendship(userId, userToBlockId);
            friendship.Block();
            await _friendsRepository.CreateFriendshipAsync(friendship);
        }
        else
        {
            friendship.Block();
            await _friendsRepository.UpdateFriendshipAsync(friendship);
        }

        var blockedUser = await _userManager.FindByIdAsync(userToBlockId.ToString());
        return new FriendshipResultVM 
        { 
            Success = true, 
            Message = $"Blocked {blockedUser?.FullName}",
            FriendshipId = friendship.Id
        };
    }

    public async Task<FriendshipResultVM> UnblockUserAsync(Guid userId, Guid userToUnblockId)
    {
        var friendship = await _friendsRepository.GetFriendshipAsync(userId, userToUnblockId);
        if (friendship == null || friendship.Status != FriendshipStatus.Blocked)
        {
            return new FriendshipResultVM { Success = false, Message = "User is not blocked" };
        }

        await _friendsRepository.DeleteFriendshipAsync(friendship.Id);

        var unblockedUser = await _userManager.FindByIdAsync(userToUnblockId.ToString());
        return new FriendshipResultVM 
        { 
            Success = true, 
            Message = $"Unblocked {unblockedUser?.FullName}"
        };
    }

    public async Task<FriendshipStatusVM> GetFriendshipStatusAsync(Guid userId1, Guid userId2)
    {
        var friendship = await _friendsRepository.GetFriendshipAsync(userId1, userId2);
        
        if (friendship == null)
        {
            return new FriendshipStatusVM
            {
                AreFriends = false,
                HasPendingRequest = false,
                HasSentRequest = false,
                IsBlocked = false
            };
        }

        return new FriendshipStatusVM
        {
            AreFriends = friendship.Status == FriendshipStatus.Accepted,
            HasPendingRequest = friendship.Status == FriendshipStatus.Pending && friendship.ReceiverId == userId1,
            HasSentRequest = friendship.Status == FriendshipStatus.Pending && friendship.RequesterId == userId1,
            IsBlocked = friendship.Status == FriendshipStatus.Blocked,
            FriendshipId = friendship.Id
        };
    }

    public async Task<IEnumerable<FriendVM>> GetMutualFriendsAsync(Guid userId1, Guid userId2)
    {
        var mutualFriendIds = await _friendsRepository.GetMutualFriendsAsync(userId1, userId2);
        var mutualFriends = new List<FriendVM>();

        foreach (var friendId in mutualFriendIds)
        {
            var friend = await _userManager.FindByIdAsync(friendId.ToString());
            if (friend != null)
            {
                mutualFriends.Add(new FriendVM
                {
                    UserId = friend.Id,
                    FullName = friend.FullName,
                    UserName = friend.UserName ?? "",
                    ProfilePictureUrl = friend.ProfilePictureUrl,
                    City = friend.Profile.City,
                    Country = friend.Profile.Country,
                    Bio = friend.Profile.Bio,
                    IsOnline = friend.OAuthInfo.LastLoginAt.HasValue && friend.OAuthInfo.LastLoginAt > DateTime.UtcNow.AddMinutes(-15),
                    LastSeen = friend.OAuthInfo.LastLoginAt
                });
            }
        }

        return mutualFriends.OrderBy(f => f.FullName);
    }
}


