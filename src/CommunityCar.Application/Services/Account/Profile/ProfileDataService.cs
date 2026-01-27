using CommunityCar.Application.Common.Interfaces.Repositories.User;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Account.Profile;

/// <summary>
/// Service for profile CRUD operations
/// </summary>
public class ProfileDataService : IProfileDataService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<Domain.Entities.Auth.User> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ProfileDataService> _logger;

    public ProfileDataService(
        IUserRepository userRepository,
        UserManager<Domain.Entities.Auth.User> userManager,
        ICurrentUserService currentUserService,
        ILogger<ProfileDataService> logger)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<ProfileVM?> GetProfileAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.GetUserWithProfileAsync(userId);
            if (user == null) return null;

            return new ProfileVM
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
                City = user.City,
                Country = user.Country,
                BioAr = user.BioAr,
                CityAr = user.CityAr,
                CountryAr = user.CountryAr,
                ProfilePictureUrl = user.ProfilePictureUrl,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsEmailConfirmed = user.EmailConfirmed,
                IsPhoneConfirmed = user.PhoneNumberConfirmed,
                Website = user.Website,
                IsActive = user.IsActive
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile for user {UserId}", userId);
            return null;
        }
    }

    public async Task<ProfileVM?> GetPublicProfileAsync(Guid userId)
    {
        try
        {
            var profile = await GetProfileAsync(userId);
            if (profile == null) return null;

            // Remove sensitive information for public view
            profile.Email = string.Empty;
            profile.PhoneNumber = null;
            
            return profile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting public profile for user {UserId}", userId);
            return null;
        }
    }

    public async Task<IEnumerable<ProfileVM>> SearchProfilesAsync(string searchTerm, int page = 1, int pageSize = 20)
    {
        try
        {
            var users = await _userRepository.SearchUsersAsync(searchTerm, page, pageSize);
            var profiles = new List<ProfileVM>();

            foreach (var user in users)
            {
                profiles.Add(new ProfileVM
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = string.Empty, // Don't expose email in search
                    Bio = user.Bio,
                    City = user.City,
                    Country = user.Country,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive
                });
            }

            return profiles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching profiles with term {SearchTerm}", searchTerm);
            return Enumerable.Empty<ProfileVM>();
        }
    }

    public async Task<bool> UpdateProfileAsync(UpdateProfileRequest request)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null) return false;

            user.FullName = request.FullName;
            user.Bio = request.Bio;
            
            // Parse location
            if (!string.IsNullOrEmpty(request.Location))
            {
                var locationParts = request.Location.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (locationParts.Length >= 1)
                {
                    user.City = locationParts[0].Trim();
                }
                if (locationParts.Length >= 2)
                {
                    user.Country = locationParts[1].Trim();
                }
            }

            user.Website = request.Website;
            user.Audit(_currentUserService.UserId);

            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Profile updated for user {UserId}", request.UserId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user {UserId}", request.UserId);
            return false;
        }
    }

    #region Profile Image Management

    public async Task<bool> UpdateProfilePictureAsync(Guid userId, string imageUrl)
    {
        try
        {
            return await _userRepository.UpdateProfilePictureAsync(userId, imageUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile picture for user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> UpdateCoverImageAsync(Guid userId, string imageUrl)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            user.CoverImageUrl = imageUrl;
            user.Audit(_currentUserService.UserId);
            await _userRepository.UpdateAsync(user);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cover image for user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> RemoveProfilePictureAsync(Guid userId)
    {
        try
        {
            return await _userRepository.RemoveProfilePictureAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing profile picture for user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> RemoveCoverImageAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            user.CoverImageUrl = null;
            user.Audit(_currentUserService.UserId);
            await _userRepository.UpdateAsync(user);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cover image for user {UserId}", userId);
            return false;
        }
    }

    #endregion

    #region Profile Settings

    public async Task<ProfileSettingsVM?> GetProfileSettingsAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            return new ProfileSettingsVM
            {
                UserId = user.Id,
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
                Location = $"{user.City}, {user.Country}".Trim(' ', ','),
                City = user.City,
                Country = user.Country,
                Website = user.Website,
                ProfilePictureUrl = user.ProfilePictureUrl,
                IsPublic = user.IsPublic,
                IsEmailConfirmed = user.EmailConfirmed,
                IsPhoneNumberConfirmed = user.PhoneNumberConfirmed,
                IsTwoFactorEnabled = user.TwoFactorEnabled,
                HasGoogleAccount = !string.IsNullOrEmpty(user.GoogleId),
                HasFacebookAccount = !string.IsNullOrEmpty(user.FacebookId),
                EmailNotifications = true, // TODO: Get from user preferences
                PushNotifications = true,
                SmsNotifications = false,
                MarketingEmails = false,
                ActiveSessions = 1 // TODO: Get actual session count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile settings for user {UserId}", userId);
            return null;
        }
    }

    public async Task<bool> UpdatePrivacySettingsAsync(Guid userId, UpdatePrivacySettingsRequest request)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            user.IsPublic = request.ProfileVisible;
            user.ShowEmail = request.EmailVisible;
            user.ShowOnlineStatus = request.ShowOnlineStatus;
            user.AllowTagging = request.AllowTagging;
            user.ShowActivityStatus = request.ShowActivityStatus;
            user.Audit(_currentUserService.UserId);

            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Privacy settings updated for user {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating privacy settings for user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> UpdateNotificationSettingsAsync(Guid userId, UpdateNotificationSettingsRequest request)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            // TODO: Add notification settings fields to User entity
            // For now, just log the update
            user.Audit(_currentUserService.UserId);
            await _userRepository.UpdateAsync(user);
            
            _logger.LogInformation("Notification settings updated for user {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating notification settings for user {UserId}", userId);
            return false;
        }
    }

    #endregion
}


