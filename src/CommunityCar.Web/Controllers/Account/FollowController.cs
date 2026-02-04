using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CommunityCar.Application.Common.Interfaces.Services.Account.Core;
using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Domain.Entities.Account;
using CommunityCar.Application.Common.Models;

namespace CommunityCar.Web.Controllers.Account;

[Authorize]
[Route("Profile/[controller]")]
public class FollowController : Controller
{
    private readonly IUserFollowingRepository _followingRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public FollowController(
        IUserFollowingRepository followingRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _followingRepository = followingRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    [HttpPost("toggle/{userId:guid}")]
    public async Task<IActionResult> ToggleFollow(Guid userId)
    {
        var currentUserId = Guid.Parse(_currentUserService.UserId!);
        
        if (currentUserId == userId)
        {
            return BadRequest("Cannot follow yourself");
        }

        var isFollowing = await _followingRepository.IsFollowingAsync(currentUserId, userId);
        
        if (isFollowing)
        {
            await _followingRepository.UnfollowUserAsync(currentUserId, userId);
        }
        else
        {
            await _followingRepository.FollowUserAsync(currentUserId, userId);
        }

        var followersCount = await _followingRepository.GetFollowingCountAsync(userId); // TODO: Should be GetFollowersCountAsync
        
        return Json(new
        {
            success = true,
            isFollowing = !isFollowing,
            followersCount = followersCount
        });
    }

    [HttpGet("following/{userId:guid}")]
    public async Task<IActionResult> Following(Guid userId, int page = 1, int pageSize = 20)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var currentUserId = Guid.TryParse(_currentUserService.UserId, out var id) ? id : (Guid?)null;
        var following = await _followingRepository.GetFollowingAsync(userId, page, pageSize);
        var totalCount = await _followingRepository.GetFollowingCountAsync(userId);

        var followingVMs = new List<FollowingVM>();
        foreach (var follow in following)
        {
            var followedUser = await _userRepository.GetByIdAsync(follow.FollowedUserId);
            if (followedUser != null)
            {
                var isFollowingBack = currentUserId.HasValue && 
                    await _followingRepository.IsFollowingAsync(follow.FollowedUserId, currentUserId.Value);

                followingVMs.Add(new FollowingVM
                {
                    UserId = followedUser.Id,
                    FullName = followedUser.Profile.FullName,
                    ProfilePictureUrl = followedUser.Profile.ProfilePictureUrl,
                    Bio = followedUser.Profile.Bio,
                    City = followedUser.Profile.City,
                    Country = followedUser.Profile.Country,
                    FollowedAt = follow.FollowedAt,
                    IsFollowingBack = isFollowingBack,
                    IsOnline = followedUser.OAuthInfo.LastLoginAt.HasValue && 
                              followedUser.OAuthInfo.LastLoginAt.Value > DateTime.UtcNow.AddMinutes(-15),
                    LastActiveAt = followedUser.OAuthInfo.LastLoginAt,
                    FollowersCount = await _followingRepository.GetFollowingCountAsync(followedUser.Id), // TODO: Should be GetFollowersCountAsync
                    FollowingCount = await _followingRepository.GetFollowingCountAsync(followedUser.Id)
                });
            }
        }

        var viewModel = new UserFollowListVM
        {
            ProfileUserId = userId,
            ProfileUserName = user.Profile.FullName,
            ListType = "following",
            Users = followingVMs,
            TotalCount = totalCount,
            IsOwnProfile = currentUserId == userId,
            Pagination = new PaginationInfo
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            }
        };

        return View("FollowList", viewModel);
    }

    [HttpGet("followers/{userId:guid}")]
    public async Task<IActionResult> Followers(Guid userId, int page = 1, int pageSize = 20)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var currentUserId = Guid.TryParse(_currentUserService.UserId, out var id) ? id : (Guid?)null;
        var followers = await _followingRepository.GetFollowersAsync(userId, page, pageSize);
        var totalCount = await _followingRepository.GetFollowingCountAsync(userId); // TODO: Should be GetFollowersCountAsync

        var followersVMs = new List<FollowingVM>();
        foreach (var follow in followers)
        {
            var followerUser = await _userRepository.GetByIdAsync(follow.FollowerId);
            if (followerUser != null)
            {
                var isFollowingBack = currentUserId.HasValue && 
                    await _followingRepository.IsFollowingAsync(currentUserId.Value, follow.FollowerId);

                followersVMs.Add(new FollowingVM
                {
                    UserId = followerUser.Id,
                    FullName = followerUser.Profile.FullName,
                    ProfilePictureUrl = followerUser.Profile.ProfilePictureUrl,
                    Bio = followerUser.Profile.Bio,
                    City = followerUser.Profile.City,
                    Country = followerUser.Profile.Country,
                    FollowedAt = follow.FollowedAt,
                    IsFollowingBack = isFollowingBack,
                    IsOnline = followerUser.OAuthInfo.LastLoginAt.HasValue && 
                              followerUser.OAuthInfo.LastLoginAt.Value > DateTime.UtcNow.AddMinutes(-15),
                    LastActiveAt = followerUser.OAuthInfo.LastLoginAt,
                    FollowersCount = await _followingRepository.GetFollowingCountAsync(followerUser.Id), // TODO: Should be GetFollowersCountAsync
                    FollowingCount = await _followingRepository.GetFollowingCountAsync(followerUser.Id)
                });
            }
        }

        var viewModel = new UserFollowListVM
        {
            ProfileUserId = userId,
            ProfileUserName = user.Profile.FullName,
            ListType = "followers",
            Users = followersVMs,
            TotalCount = totalCount,
            IsOwnProfile = currentUserId == userId,
            Pagination = new PaginationInfo
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            }
        };

        return View("FollowList", viewModel);
    }

    [HttpGet("suggestions")]
    public async Task<IActionResult> Suggestions()
    {
        var currentUserId = Guid.Parse(_currentUserService.UserId!);
        
        // Get users followed by people you follow (mutual connections)
        // TODO: Implement GetFollowSuggestionsAsync in repository
        var suggestions = Enumerable.Empty<object>(); // await _followingRepository.GetFollowSuggestionsAsync(currentUserId, 10);
        
        var suggestedUsers = new List<SuggestedUserVM>();
        
        // Disabled until repository methods are implemented
        /*
        foreach (var suggestion in suggestions)
        {
            var user = await _userRepository.GetByIdAsync(suggestion.FollowedUserId);
            if (user != null)
            {
                // TODO: Implement GetMutualFollowersAsync in repository
                var mutualFollowers = Enumerable.Empty<object>(); // await _followingRepository.GetMutualFollowersAsync(currentUserId, suggestion.FollowedUserId);
                var mutualNames = new List<string>();
                
                foreach (var mutual in mutualFollowers.Take(3))
                {
                    var mutualUser = await _userRepository.GetByIdAsync(mutual.FollowerId);
                    if (mutualUser != null)
                    {
                        mutualNames.Add(mutualUser.Profile.FullName);
                    }
                }

                suggestedUsers.Add(new SuggestedUserVM
                {
                    UserId = user.Id,
                    FullName = user.Profile.FullName,
                    ProfilePictureUrl = user.Profile.ProfilePictureUrl,
                    Bio = user.Profile.Bio,
                    City = user.Profile.City,
                    Country = user.Profile.Country,
                    MutualFollowersCount = mutualFollowers.Count(),
                    MutualFollowerNames = mutualNames,
                    SuggestionReason = mutualNames.Any() ? 
                        $"Followed by {string.Join(", ", mutualNames.Take(2))}" + 
                        (mutualNames.Count > 2 ? $" and {mutualNames.Count - 2} others" : "") :
                        "Suggested for you",
                    FollowersCount = await _followingRepository.GetFollowingCountAsync(user.Id) // TODO: Should be GetFollowersCountAsync
                });
            }
        }
        */
        
        var viewModel = new FollowSuggestionsVM
        {
            SuggestedUsers = suggestedUsers
        };

        return View(viewModel);
    }

    [HttpGet("stats/{userId:guid}")]
    public async Task<IActionResult> GetFollowStats(Guid userId)
    {
        var currentUserId = Guid.TryParse(_currentUserService.UserId, out var id) ? id : (Guid?)null;
        
        var followersCount = await _followingRepository.GetFollowingCountAsync(userId); // TODO: Should be GetFollowersCountAsync
        var followingCount = await _followingRepository.GetFollowingCountAsync(userId);
        
        var isFollowing = currentUserId.HasValue && 
            await _followingRepository.IsFollowingAsync(currentUserId.Value, userId);
        var isFollowedBy = currentUserId.HasValue && 
            await _followingRepository.IsFollowingAsync(userId, currentUserId.Value);

        var stats = new FollowStatsVM
        {
            FollowersCount = followersCount,
            FollowingCount = followingCount,
            IsFollowing = isFollowing,
            IsFollowedBy = isFollowedBy,
            CanFollow = currentUserId.HasValue && currentUserId != userId
        };

        return Json(stats);
    }
}