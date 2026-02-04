using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Account.ViewModels.Core;
using CommunityCar.Application.Features.Account.ViewModels.Management;
using System.Security.Claims;

namespace CommunityCar.Application.Common.Interfaces.Services.Account.Management;

public interface IAccountManagementService
{
    // Account Lifecycle
    Task<Result> DeactivateAccountAsync(DeactivateAccountRequest request);
    Task<Result> ReactivateAccountAsync(Guid userId, string password);
    Task<Result> DeleteAccountAsync(DeleteAccountRequest request);
    Task<bool> IsAccountDeactivatedAsync(Guid userId);

    // Settings (Privacy & Notifications)
    Task<PrivacySettingsVM> GetPrivacySettingsAsync(Guid userId);
    Task<Result> UpdatePrivacySettingsAsync(UpdatePrivacySettingsRequest request);
    Task<NotificationSettingsVM> GetNotificationSettingsAsync(Guid userId);
    Task<Result> UpdateNotificationSettingsAsync(UpdateNotificationSettingsRequest request);

    // Data Export
    Task<Result> ExportUserDataAsync(ExportUserDataRequest request);
    Task<bool> RequestDataExportAsync(Guid userId);
    Task<IEnumerable<DataExportVM>> GetDataExportHistoryAsync(Guid userId);

    // Information & Recovery
    Task<AccountInfoVM> GetAccountInfoAsync(Guid userId);
    Task<DateTime> GetAccountCreationDateAsync(Guid userId);
    Task<DateTime?> GetLastLoginDateAsync(Guid userId);
    Task<bool> CanRecoverAccountAsync(string email);
    Task<Result> RecoverAccountAsync(string email, string password);

    // Compliance
    Task<bool> AcceptTermsOfServiceAsync(Guid userId, string version);
    Task<bool> AcceptPrivacyPolicyAsync(Guid userId, string version);
    Task<IEnumerable<ConsentVM>> GetUserConsentsAsync(Guid userId);

    // User Identity Management
    Task<UserIdentityVM?> GetUserIdentityAsync(Guid userId);
    Task<IEnumerable<UserIdentityVM>> GetAllUsersAsync(int page = 1, int pageSize = 20);
    Task<bool> IsUserActiveAsync(Guid userId);
    Task<bool> LockUserAsync(Guid userId, string reason);
    Task<bool> UnlockUserAsync(Guid userId);

    // Claims Management
    Task<IEnumerable<UserClaimVM>> GetUserClaimsAsync(Guid userId);
    Task<bool> AddClaimToUserAsync(Guid userId, string claimType, string claimValue);
    Task<bool> RemoveClaimFromUserAsync(Guid userId, string claimType, string claimValue);
    Task<bool> UpdateUserClaimAsync(Guid userId, string oldClaimType, string oldClaimValue, string newClaimType, string newClaimValue);
}