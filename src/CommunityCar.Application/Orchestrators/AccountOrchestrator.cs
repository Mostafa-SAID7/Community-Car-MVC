using CommunityCar.Application.Common.Interfaces.Orchestrators;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Services.Authentication;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models.Security;

namespace CommunityCar.Application.Orchestrators;

public class AccountOrchestrator : IAccountOrchestrator
{
    private readonly IAccountManagementService _managementService;
    private readonly IAccountSecurityService _securityService;
    private readonly IAuthenticationService _authService;
    private readonly IOAuthService _oauthService;

    public AccountOrchestrator(
        IAccountManagementService managementService,
        IAccountSecurityService securityService,
        IAuthenticationService authService,
        IOAuthService oauthService)
    {
        _managementService = managementService;
        _securityService = securityService;
        _authService = authService;
        _oauthService = oauthService;
    }

    #region Lifecycle

    public Task<Result> DeactivateAccountAsync(DeactivateAccountRequest request) => _managementService.DeactivateAccountAsync(request);
    public Task<Result> ReactivateAccountAsync(Guid userId, string password) => _managementService.ReactivateAccountAsync(userId, password);
    public Task<Result> DeleteAccountAsync(DeleteAccountRequest request) => _managementService.DeleteAccountAsync(request);
    public Task<Result> ExportUserDataAsync(ExportUserDataRequest request) => _managementService.ExportUserDataAsync(request);

    #endregion

    #region Security

    public Task<SecurityInfoVM> GetSecurityInfoAsync(Guid userId) => _securityService.GetSecurityInfoAsync(userId);
    public Task<Result> ChangePasswordAsync(ChangePasswordRequest request) => _securityService.ChangePasswordAsync(request.UserId, request);
    public Task<TwoFactorSetupVM> SetupTwoFactorAsync(Guid userId) => _securityService.SetupTwoFactorAsync(userId);
    public Task<Result> EnableTwoFactorAsync(Guid userId, TwoFactorSetupRequest request) 
        => _securityService.EnableTwoFactorAsync(userId, request).ContinueWith(t => t.Result ? Result.Success("2FA enabled") : Result.Failure("Failed to enable 2FA"));
    public Task<Result> DisableTwoFactorAsync(Guid userId, string password)
        => _securityService.DisableTwoFactorAsync(userId, password).ContinueWith(t => t.Result ? Result.Success("2FA disabled") : Result.Failure("Failed to disable 2FA"));
    public Task<IEnumerable<string>> GenerateRecoveryCodesAsync(Guid userId) => Task.FromResult(Enumerable.Empty<string>()); // Placeholder

    #endregion

    #region External Accounts

    public Task<Result> LinkExternalAccountAsync(LinkExternalAccountRequest request) => _oauthService.LinkAccountAsync(request);
    public Task<Result> UnlinkExternalAccountAsync(Guid userId, string provider) => _oauthService.UnlinkAccountAsync(userId, provider);
    public Task<IEnumerable<ExternalLoginInfo>> GetExternalLoginsAsync(Guid userId) => _oauthService.GetExternalLoginsAsync(userId.ToString());

    #endregion

    #region Settings

    public Task<CommunityCar.Application.Common.Models.Profile.PrivacySettingsVM> GetPrivacySettingsAsync(Guid userId) => _managementService.GetPrivacySettingsAsync(userId);
    public Task<Result> UpdatePrivacySettingsAsync(CommunityCar.Application.Common.Models.Profile.UpdatePrivacySettingsRequest request) => _managementService.UpdatePrivacySettingsAsync(request);
    public Task<CommunityCar.Application.Common.Models.Profile.NotificationSettingsVM> GetNotificationSettingsAsync(Guid userId) => _managementService.GetNotificationSettingsAsync(userId);
    public Task<Result> UpdateNotificationSettingsAsync(CommunityCar.Application.Common.Models.Profile.UpdateNotificationSettingsRequest request) => _managementService.UpdateNotificationSettingsAsync(request);

    #endregion
}
