namespace CommunityCar.Application.Services.Account.Management;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Interfaces.Services.Account.Core;
using CommunityCar.Application.Common.Interfaces.Services.Account.Management;
using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Application.Features.Account.ViewModels.Core;
using CommunityCar.Application.Features.Account.ViewModels.Management;
using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.ValueObjects.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

public class AccountManagementService : IAccountManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AccountManagementService> _logger;

    public AccountManagementService(
        IUserRepository userRepository,
        UserManager<User> userManager,
        ICurrentUserService currentUserService,
        ILogger<AccountManagementService> logger)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    #region Account Lifecycle

    public async Task<Result> DeactivateAccountAsync(DeactivateAccountRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null) return Result.Failure("User not found.");
        
        user.IsActive = false;
        await _userManager.UpdateAsync(user);
        return Result.Success("Account deactivated.");
    }

    public async Task<Result> ReactivateAccountAsync(Guid userId, string password)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Result.Failure("User not found.");
        
        user.IsActive = true;
        await _userManager.UpdateAsync(user);
        return Result.Success("Account reactivated.");
    }

    public async Task<Result> DeleteAccountAsync(DeleteAccountRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null) return Result.Failure("User not found.");
        
        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded ? Result.Success("Account deleted.") : Result.Failure("Deletion failed.", result.Errors.Select(e => e.Description).ToList());
    }

    public async Task<bool> IsAccountDeactivatedAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null && !user.IsActive;
    }

    #endregion

    #region Settings (Privacy & Notifications)

    public async Task<PrivacySettingsVM> GetPrivacySettingsAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return new PrivacySettingsVM
        {
            ProfileVisible = user?.PrivacySettings.IsPublic ?? true,
            EmailVisible = user?.PrivacySettings.ShowEmail ?? false,
            ShowActivityStatus = user?.PrivacySettings.ShowActivityStatus ?? true
        };
    }

    public async Task<Result> UpdatePrivacySettingsAsync(UpdatePrivacySettingsRequest request)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null) return Result.Failure("User not found.");

        var newPrivacySettings = new PrivacySettings(
            isPublic: request.ProfileVisible,
            showEmail: request.EmailVisible,
            showLocation: user.PrivacySettings.ShowLocation,
            showOnlineStatus: request.ShowOnlineStatus,
            allowMessagesFromStrangers: user.PrivacySettings.AllowMessagesFromStrangers,
            allowTagging: user.PrivacySettings.AllowTagging,
            showActivityStatus: request.ShowActivityStatus);


        user.UpdatePrivacySettings(newPrivacySettings);
        await _userRepository.UpdateAsync(user);
        return Result.Success("Privacy settings updated.");
    }

    public async Task<NotificationSettingsVM> GetNotificationSettingsAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return new NotificationSettingsVM();

        return new NotificationSettingsVM
        {
            EmailNotifications = user.NotificationSettings.EmailNotificationsEnabled,
            PushNotifications = user.NotificationSettings.PushNotificationsEnabled,
            SmsNotifications = user.NotificationSettings.SmsNotificationsEnabled,
            MarketingEmails = user.NotificationSettings.MarketingEmailsEnabled
        };
    }

    public async Task<Result> UpdateNotificationSettingsAsync(UpdateNotificationSettingsRequest request)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null) return Result.Failure("User not found.");

        var newNotificationSettings = new NotificationSettings(
            emailNotifications: request.EmailNotifications,
            pushNotifications: request.PushNotifications,
            smsNotifications: request.SmsNotifications,
            marketingEmails: request.MarketingEmails,
            commentNotifications: user.NotificationSettings.CommentNotificationsEnabled,
            likeNotifications: user.NotificationSettings.LikeNotificationsEnabled,
            followNotifications: user.NotificationSettings.FollowNotificationsEnabled,
            messageNotifications: user.NotificationSettings.MessageNotificationsEnabled);

        user.UpdateNotificationSettings(newNotificationSettings);
        await _userRepository.UpdateAsync(user);
        return Result.Success("Notification settings updated.");
    }

    #endregion

    #region Data Export

    public Task<Result> ExportUserDataAsync(ExportUserDataRequest request) => Task.FromResult(Result.Success("Data export started."));
    public Task<bool> RequestDataExportAsync(Guid userId) => Task.FromResult(true);
    public Task<IEnumerable<DataExportVM>> GetDataExportHistoryAsync(Guid userId) => Task.FromResult(Enumerable.Empty<DataExportVM>());

    #endregion

    #region Information & Recovery

    public async Task<AccountInfoVM> GetAccountInfoAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return new AccountInfoVM { Id = userId, Email = user?.Email ?? string.Empty, CreatedAt = user?.CreatedAt ?? DateTime.UtcNow };
    }

    public async Task<DateTime> GetAccountCreationDateAsync(Guid userId) => (await _userRepository.GetByIdAsync(userId))?.CreatedAt ?? DateTime.UtcNow;
    public async Task<DateTime?> GetLastLoginDateAsync(Guid userId) => (await _userRepository.GetByIdAsync(userId))?.OAuthInfo.LastLoginAt;

    public Task<bool> CanRecoverAccountAsync(string email) => Task.FromResult(true);
    public Task<Result> RecoverAccountAsync(string email, string password) => Task.FromResult(Result.Success("Account recovered."));

    #endregion

    #region Compliance

    public async Task<bool> AcceptTermsOfServiceAsync(Guid userId, string version)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        user.AcceptTermsOfService(version);
        await _userRepository.UpdateAsync(user);
        
        _logger.LogInformation("Terms of service accepted for user {UserId}, version {Version}", userId, version);
        return true;
    }

    public async Task<bool> AcceptPrivacyPolicyAsync(Guid userId, string version)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        user.AcceptPrivacyPolicy(version);
        await _userRepository.UpdateAsync(user);

        _logger.LogInformation("Privacy policy accepted for user {UserId}, version {Version}", userId, version);
        return true;
    }

    public async Task<IEnumerable<ConsentVM>> GetUserConsentsAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return Enumerable.Empty<ConsentVM>();

        var consents = new List<ConsentVM>();

        if (!string.IsNullOrEmpty(user.AcceptedToSVersion))
        {
            consents.Add(new ConsentVM
            {
                Type = "TermsOfService",
                IsAccepted = true,
                AcceptedAt = user.ToSAcceptedAt ?? user.CreatedAt,
                Version = user.AcceptedToSVersion
            });
        }

        if (!string.IsNullOrEmpty(user.AcceptedPrivacyPolicyVersion))
        {
            consents.Add(new ConsentVM
            {
                Type = "PrivacyPolicy",
                IsAccepted = true,
                AcceptedAt = user.PrivacyPolicyAcceptedAt ?? user.CreatedAt,
                Version = user.AcceptedPrivacyPolicyVersion
            });
        }

        return consents;
    }

    #endregion

    #region User Identity Management (merged from IdentityManagementService)

    public async Task<UserIdentityVM?> GetUserIdentityAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        var isLocked = await _userManager.IsLockedOutAsync(user);

        return new UserIdentityVM
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FullName = user.Profile.FullName,
            IsActive = user.IsActive,
            IsEmailConfirmed = user.EmailConfirmed,
            IsLocked = isLocked,
            LockoutEnd = await _userManager.GetLockoutEndDateAsync(user),
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.OAuthInfo.LastLoginAt,
            Roles = roles.ToList()
        };
    }

    public async Task<IEnumerable<UserIdentityVM>> GetAllUsersAsync(int page = 1, int pageSize = 20)
    {
        var users = await _userRepository.GetActiveUsersAsync();
        var pagedUsers = users.Skip((page - 1) * pageSize).Take(pageSize);
        var userIdentities = new List<UserIdentityVM>();

        foreach (var user in pagedUsers)
        {
            var identity = await GetUserIdentityAsync(user.Id);
            if (identity != null) userIdentities.Add(identity);
        }

        return userIdentities;
    }

    public async Task<bool> IsUserActiveAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user?.IsActive ?? false;
    }

    public async Task<bool> LockUserAsync(Guid userId, string reason)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        var lockoutEnd = DateTimeOffset.UtcNow.AddHours(24);
        var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);
        return result.Succeeded;
    }

    public async Task<bool> UnlockUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.SetLockoutEndDateAsync(user, null);
        return result.Succeeded;
    }

    #endregion

    #region Claims Management (merged from IdentityManagementService)

    public async Task<IEnumerable<UserClaimVM>> GetUserClaimsAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Enumerable.Empty<UserClaimVM>();
        var claims = await _userManager.GetClaimsAsync(user);
        return claims.Select(c => new UserClaimVM { Type = c.Type, Value = c.Value });
    }

    public async Task<bool> AddClaimToUserAsync(Guid userId, string claimType, string claimValue)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;
        var result = await _userManager.AddClaimAsync(user, new Claim(claimType, claimValue));
        return result.Succeeded;
    }

    public async Task<bool> RemoveClaimFromUserAsync(Guid userId, string claimType, string claimValue)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;
        var result = await _userManager.RemoveClaimAsync(user, new Claim(claimType, claimValue));
        return result.Succeeded;
    }

    public async Task<bool> UpdateUserClaimAsync(Guid userId, string oldClaimType, string oldClaimValue, string newClaimType, string newClaimValue)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;
        var result = await _userManager.ReplaceClaimAsync(user, new Claim(oldClaimType, oldClaimValue), new Claim(newClaimType, newClaimValue));
        return result.Succeeded;
    }

    #endregion
}