using CommunityCar.Application.Common.Interfaces.Services.Profile;
using CommunityCar.Application.Common.Interfaces.Services.Storage;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Profile.DTOs;
using CommunityCar.Application.Features.Profile.ViewModels;
using Microsoft.AspNetCore.Identity;
using CommunityCar.Domain.Entities.Auth;

namespace CommunityCar.Application.Services.Profile;

public class ProfileService : IProfileService
{
    private readonly UserManager<User> _userManager;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserService _currentUserService;

    public ProfileService(
        UserManager<User> userManager,
        IFileStorageService fileStorageService,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _fileStorageService = fileStorageService;
        _currentUserService = currentUserService;
    }

    public async Task<ProfileVM?> GetProfileAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return null;

        // Get statistics (placeholder implementation)
        var stats = await GetProfileStatsAsync(userId);

        return new ProfileVM
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            Bio = user.Bio,
            City = user.City,
            Country = user.Country,
            ProfilePictureUrl = user.ProfilePictureUrl,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            IsEmailConfirmed = user.EmailConfirmed,
            IsPhoneNumberConfirmed = user.PhoneNumberConfirmed,
            IsTwoFactorEnabled = user.IsTwoFactorEnabled,
            IsActive = user.IsActive,
            HasGoogleAccount = !string.IsNullOrEmpty(user.GoogleId),
            HasFacebookAccount = !string.IsNullOrEmpty(user.FacebookId),
            PostsCount = stats.PostsCount,
            CommentsCount = stats.CommentsCount,
            LikesReceived = stats.LikesReceived
        };
    }

    public async Task<ProfileSettingsVM?> GetProfileSettingsAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return null;

        return new ProfileSettingsVM
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            Bio = user.Bio,
            City = user.City,
            Country = user.Country,
            ProfilePictureUrl = user.ProfilePictureUrl,
            IsEmailConfirmed = user.EmailConfirmed,
            IsPhoneNumberConfirmed = user.PhoneNumberConfirmed,
            IsTwoFactorEnabled = user.IsTwoFactorEnabled,
            HasGoogleAccount = !string.IsNullOrEmpty(user.GoogleId),
            HasFacebookAccount = !string.IsNullOrEmpty(user.FacebookId),
            // Default notification settings - in real app, these would come from user preferences
            EmailNotifications = true,
            PushNotifications = true,
            SmsNotifications = false,
            MarketingEmails = false,
            ActiveSessions = 1 // Placeholder
        };
    }

    public async Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;
        user.Bio = request.Bio;
        user.City = request.City;
        user.Country = request.Country;
        user.Audit(_currentUserService.UserId);

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UpdateProfilePictureAsync(Guid userId, Stream imageStream, string fileName)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        try
        {
            // Delete old profile picture if exists
            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                await _fileStorageService.DeleteFileAsync(user.ProfilePictureUrl);
            }

            // Upload new profile picture
            var profilePictureUrl = await _fileStorageService.UploadFileAsync(
                imageStream, 
                $"profiles/{userId}/{fileName}",
                "image/jpeg");

            user.ProfilePictureUrl = profilePictureUrl;
            user.Audit(_currentUserService.UserId);

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteProfilePictureAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
        {
            await _fileStorageService.DeleteFileAsync(user.ProfilePictureUrl);
            user.ProfilePictureUrl = null;
            user.Audit(_currentUserService.UserId);

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        return true;
    }

    public async Task<bool> UpdatePrivacySettingsAsync(Guid userId, UpdatePrivacySettingsRequest request)
    {
        // In a real implementation, this would update user privacy settings in the database
        // For now, we'll just return true as a placeholder
        await Task.Delay(1);
        return true;
    }

    public async Task<bool> UpdateNotificationSettingsAsync(Guid userId, UpdateNotificationSettingsRequest request)
    {
        // In a real implementation, this would update user notification preferences in the database
        // For now, we'll just return true as a placeholder
        await Task.Delay(1);
        return true;
    }

    public async Task<ProfileStatsVM> GetProfileStatsAsync(Guid userId)
    {
        // Placeholder implementation - in real app, query actual statistics
        return await Task.FromResult(new ProfileStatsVM
        {
            PostsCount = 0,
            CommentsCount = 0,
            LikesReceived = 0,
            SharesReceived = 0,
            FollowersCount = 0,
            FollowingCount = 0,
            JoinedDate = DateTime.UtcNow,
            DaysActive = 1
        });
    }
}