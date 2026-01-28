using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Common.Interfaces.Repositories.Profile;
using CommunityCar.Application.Common.Interfaces.Repositories.User;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Domain.Entities.Account;
using CommunityCar.Domain.ValueObjects.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Account;

public class ProfileService : IProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserFollowingRepository _followingRepository;
    private readonly IPostsRepository _postsRepository;
    private readonly IGamificationService _gamificationService;
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(
        IUserRepository userRepository,
        IUserFollowingRepository followingRepository,
        IPostsRepository postsRepository,
        IGamificationService gamificationService,
        UserManager<User> userManager,
        ICurrentUserService currentUserService,
        ILogger<ProfileService> logger)
    {
        _userRepository = userRepository;
        _followingRepository = followingRepository;
        _postsRepository = postsRepository;
        _gamificationService = gamificationService;
        _userManager = userManager;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    #region Retrieval

    public async Task<ProfileVM?> GetProfileAsync(Guid userId)
    {
        var user = await _userRepository.GetUserWithProfileAsync(userId);
        if (user == null) return null;

        var profile = MapToProfileVM(user);
        
        // Fetch counts in parallel for better performance
        var followingCountTask = _followingRepository.GetFollowingCountAsync(userId);
        var followersCountTask = _followingRepository.GetFollowersCountAsync(userId);
        var postsCountTask = _postsRepository.GetUserPostsCountAsync(userId);
        var statsTask = GetProfileStatsAsync(userId);

        await Task.WhenAll(followingCountTask, followersCountTask, postsCountTask, statsTask);

        profile.FollowingCount = await followingCountTask;
        profile.FollowersCount = await followersCountTask;
        profile.PostsCount = await postsCountTask;
        profile.Stats = await statsTask;

        return profile;
    }

    public async Task<ProfileVM?> GetPublicProfileAsync(Guid userId)
    {
        var profile = await GetProfileAsync(userId);
        if (profile == null) return null;
        profile.Email = string.Empty;
        profile.PhoneNumber = null;
        return profile;
    }

    public async Task<IEnumerable<ProfileVM>> SearchProfilesAsync(string searchTerm, int page = 1, int pageSize = 20)
    {
        var users = await _userRepository.SearchUsersAsync(searchTerm, page, pageSize);
        return users.Select(u => new ProfileVM
        {
            Id = u.Id,
            FullName = u.Profile.FullName,
            Bio = u.Profile.Bio,
            City = u.Profile.City,
            Country = u.Profile.Country,
            ProfilePictureUrl = u.Profile.ProfilePictureUrl
        });
    }

    #endregion

    #region Management

    public async Task<bool> UpdateProfileAsync(UpdateProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null) return false;

        // Update profile using value object
        var updatedProfile = user.Profile
            .UpdateBasicInfo(request.FullName, null, null)
            .UpdateBio(request.Bio)
            .UpdateLocation(
                !string.IsNullOrEmpty(request.Location) ? request.Location.Split(',')[0].Trim() : user.Profile.City,
                !string.IsNullOrEmpty(request.Location) && request.Location.Split(',').Length > 1 ? request.Location.Split(',')[1].Trim() : user.Profile.Country);

        // Create new profile with website update
        var finalProfile = new UserProfile(
            updatedProfile.FullName,
            updatedProfile.FirstName,
            updatedProfile.LastName,
            updatedProfile.Bio,
            updatedProfile.City,
            updatedProfile.Country,
            updatedProfile.BioAr,
            updatedProfile.CityAr,
            updatedProfile.CountryAr,
            request.Website,
            updatedProfile.ProfilePictureUrl,
            updatedProfile.CoverImageUrl);

        user.UpdateProfile(finalProfile);
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public Task<bool> UpdateProfilePictureAsync(Guid userId, string imageUrl) => _userRepository.UpdateProfilePictureAsync(userId, imageUrl);
    public Task<bool> UpdateCoverImageAsync(Guid userId, string imageUrl) => _userRepository.UpdateCoverImageAsync(userId, imageUrl);

    public Task<bool> RemoveProfilePictureAsync(Guid userId) => _userRepository.RemoveProfilePictureAsync(userId);
    public Task<bool> RemoveCoverImageAsync(Guid userId) => _userRepository.RemoveCoverImageAsync(userId);
    public Task<bool> DeleteProfilePictureAsync(Guid userId) => RemoveProfilePictureAsync(userId);

    #endregion

    #region Statistics & Validation

    public async Task<ProfileStatsVM> GetProfileStatsAsync(Guid userId) 
        => await _gamificationService.GetUserStatsAsync(userId);
    public Task<bool> UpdateProfileStatsAsync(Guid userId) => Task.FromResult(true);

    public Task<bool> IsProfileCompleteAsync(Guid userId) => Task.FromResult(true);
    public Task<IEnumerable<string>> GetProfileCompletionSuggestionsAsync(Guid userId) => Task.FromResult(Enumerable.Empty<string>());

    #endregion

    private ProfileVM MapToProfileVM(User user)
    {
        return new ProfileVM
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            FullName = user.Profile.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            Bio = user.Profile.Bio,
            City = user.Profile.City,
            Country = user.Profile.Country,
            ProfilePictureUrl = user.Profile.ProfilePictureUrl,
            CoverImageUrl = user.Profile.CoverImageUrl,
            CreatedAt = user.CreatedAt,
            IsEmailConfirmed = user.EmailConfirmed,
            IsActive = user.IsActive,
            CommentsCount = 0, // Will be set by GetProfileAsync method
            LikesReceived = 0  // Will be set by GetProfileAsync method
        };
    }
}
