using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models;

namespace CommunityCar.Application.Common.Interfaces.Services.Account;

/// <summary>
/// Interface for account management operations (deactivation, deletion, data export, privacy)
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
}


