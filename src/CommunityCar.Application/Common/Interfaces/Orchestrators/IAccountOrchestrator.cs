using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Interfaces.Services.Authentication;

namespace CommunityCar.Application.Common.Interfaces.Orchestrators;

public interface IAccountOrchestrator
{
    // Account Lifecycle
    Task<Result> DeactivateAccountAsync(DeactivateAccountRequest request);
    Task<Result> ReactivateAccountAsync(Guid userId, string password);
    Task<Result> DeleteAccountAsync(DeleteAccountRequest request);
    Task<Result> ExportUserDataAsync(ExportUserDataRequest request);

    // Security
    Task<SecurityInfoVM> GetSecurityInfoAsync(Guid userId);
    Task<Result> ChangePasswordAsync(ChangePasswordRequest request);
    Task<TwoFactorSetupVM> SetupTwoFactorAsync(Guid userId);
    Task<Result> EnableTwoFactorAsync(Guid userId, TwoFactorSetupRequest request);
    Task<Result> DisableTwoFactorAsync(Guid userId, string password);
    Task<IEnumerable<string>> GenerateRecoveryCodesAsync(Guid userId);
    
    // External Accounts
    Task<Result> LinkExternalAccountAsync(LinkExternalAccountRequest request);
    Task<Result> UnlinkExternalAccountAsync(Guid userId, string provider);
    Task<IEnumerable<ExternalLoginInfo>> GetExternalLoginsAsync(Guid userId);

    // Settings
    Task<PrivacySettingsVM> GetPrivacySettingsAsync(Guid userId);
    Task<Result> UpdatePrivacySettingsAsync(UpdatePrivacySettingsRequest request);
    Task<NotificationSettingsVM> GetNotificationSettingsAsync(Guid userId);
    Task<Result> UpdateNotificationSettingsAsync(UpdateNotificationSettingsRequest request);
}
