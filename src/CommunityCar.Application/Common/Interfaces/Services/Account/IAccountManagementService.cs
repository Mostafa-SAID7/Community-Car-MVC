using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Account.ViewModels.Core;

namespace CommunityCar.Application.Common.Interfaces.Services.Account;

/// <summary>
/// Unified interface for account management operations including identity, deactivation, deletion, data export, privacy, and claims management
/// </summary>
public interface IAccountManagementService
{
    // Account Status Management
    Task<Result> DeactivateAccountAsync(DeactivateAccountRequest request);
    Task<Result> ReactivateAccountAsync(Guid userId, string password);
    Task<Result> DeleteAccountAsync(DeleteAccountRequest request);
    Task<bool> IsAccountDeactivatedAsync(Guid userId);

    // Data Export
    Task<Result> ExportUserDataAsync(ExportUserDataRequest request);
    Task<bool> RequestDataExportAsync(Guid userId);
    Task<IEnumerable<DataExportVM>> GetDataExportHistoryAsync(Guid userId);

    // Privacy & Notification Settings
    Task<PrivacySettingsVM> GetPrivacySettingsAsync(Guid userId);
    Task<Result> UpdatePrivacySettingsAsync(UpdatePrivacySettingsRequest request);
    Task<NotificationSettingsVM> GetNotificationSettingsAsync(Guid userId);
    Task<Result> UpdateNotificationSettingsAsync(UpdateNotificationSettingsRequest request);

    // Account Recovery
    Task<bool> CanRecoverAccountAsync(string email);
    Task<Result> RecoverAccountAsync(string email, string password);

    // Account Information
    Task<AccountInfoVM> GetAccountInfoAsync(Guid userId);
    Task<DateTime> GetAccountCreationDateAsync(Guid userId);
    Task<DateTime?> GetLastLoginDateAsync(Guid userId);

    // Compliance & Legal
    Task<bool> AcceptTermsOfServiceAsync(Guid userId, string version);
    Task<bool> AcceptPrivacyPolicyAsync(Guid userId, string version);
    Task<IEnumerable<ConsentVM>> GetUserConsentsAsync(Guid userId);

    // User Identity Management (merged from IIdentityManagementService)
    Task<UserIdentityVM?> GetUserIdentityAsync(Guid userId);
    Task<IEnumerable<UserIdentityVM>> GetAllUsersAsync(int page = 1, int pageSize = 20);
    Task<bool> IsUserActiveAsync(Guid userId);
    Task<bool> LockUserAsync(Guid userId, string reason);
    Task<bool> UnlockUserAsync(Guid userId);

    // Claims Management (merged from IIdentityManagementService)
    Task<IEnumerable<UserClaimVM>> GetUserClaimsAsync(Guid userId);
    Task<bool> AddClaimToUserAsync(Guid userId, string claimType, string claimValue);
    Task<bool> RemoveClaimFromUserAsync(Guid userId, string claimType, string claimValue);
    Task<bool> UpdateUserClaimAsync(Guid userId, string oldClaimType, string oldClaimValue, string newClaimType, string newClaimValue);
}


