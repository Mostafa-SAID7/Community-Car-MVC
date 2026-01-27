using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

namespace CommunityCar.Application.Common.Interfaces.Orchestrators;

public interface IAccountSecurityOrchestrator
{
    // Password Management
    Task<Result> ChangePasswordAsync(ChangePasswordRequest request);
    
    // Two Factor Authentication
    Task<TwoFactorInfo> GetTwoFactorInfoAsync(string userId);
    Task<Result> EnableTwoFactorAsync(string userId, string code);
    Task<Result> DisableTwoFactorAsync(string userId);
    Task<IEnumerable<string>> GenerateRecoveryCodesAsync(string userId);
    Task<string> GetAuthenticatorKeyAsync(string userId);

    // External Accounts
    Task<Result> LinkGoogleAccountAsync(LinkExternalAccountRequest request);
    Task<Result> LinkFacebookAccountAsync(LinkExternalAccountRequest request);
    Task<Result> UnlinkExternalAccountAsync(UnlinkExternalAccountRequest request);
    Task<IEnumerable<ExternalLoginInfo>> GetExternalLoginsAsync(string userId);
}



