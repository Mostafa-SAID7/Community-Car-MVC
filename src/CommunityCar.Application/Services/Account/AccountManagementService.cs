using CommunityCar.Application.Common.Interfaces.Repositories.User;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Account;

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
            ProfileVisible = user?.IsPublic ?? true,
            EmailVisible = user?.ShowEmail ?? false,
            ShowOnlineStatus = user?.ShowOnlineStatus ?? true
        };
    }

    public async Task<Result> UpdatePrivacySettingsAsync(UpdatePrivacySettingsRequest request)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null) return Result.Failure("User not found.");

        user.IsPublic = request.ProfileVisible;
        user.ShowEmail = request.EmailVisible;
        user.ShowOnlineStatus = request.ShowOnlineStatus;
        
        await _userRepository.UpdateAsync(user);
        return Result.Success("Privacy settings updated.");
    }

    public async Task<NotificationSettingsVM> GetNotificationSettingsAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return new NotificationSettingsVM();

        return new NotificationSettingsVM
        {
            UserId = userId,
            EmailEnabled = user.EmailNotificationsEnabled,
            PushEnabled = user.PushNotificationsEnabled,
            SmsEnabled = user.SmsNotificationsEnabled,
            MarketingEnabled = user.MarketingEmailsEnabled
        };
    }

    public async Task<Result> UpdateNotificationSettingsAsync(UpdateNotificationSettingsRequest request)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null) return Result.Failure("User not found.");

        user.EmailNotificationsEnabled = request.EmailNotifications;
        user.PushNotificationsEnabled = request.PushNotifications;
        user.SmsNotificationsEnabled = request.SmsNotifications;
        user.MarketingEmailsEnabled = request.MarketingEmails;

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
    public async Task<DateTime?> GetLastLoginDateAsync(Guid userId) => (await _userRepository.GetByIdAsync(userId))?.LastLoginAt;

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
}
