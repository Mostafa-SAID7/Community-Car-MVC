using CommunityCar.Application.Common.Interfaces.Orchestrators;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Services.Authentication;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

namespace CommunityCar.Application.Orchestrators;

public class AccountSecurityOrchestrator : IAccountSecurityOrchestrator
{
    private readonly IAccountSecurityService _securityService;
    private readonly ITwoFactorService _twoFactorService;
    private readonly IOAuthService _oauthService;

    public AccountSecurityOrchestrator(
        IAccountSecurityService securityService,
        ITwoFactorService twoFactorService,
        IOAuthService oauthService)
    {
        _securityService = securityService;
        _twoFactorService = twoFactorService;
        _oauthService = oauthService;
    }

    public async Task<Result> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var result = await _securityService.ChangePasswordAsync(Guid.Parse(request.UserId), request);
        return result.Succeeded ? Result.Success(result.Message) : Result.Failure(result.Errors);
    }

    public Task<TwoFactorInfo> GetTwoFactorInfoAsync(string userId)
    {
        return _twoFactorService.GetTwoFactorInfoAsync(userId);
    }

    public async Task<Result> EnableTwoFactorAsync(string userId, string code)
    {
        var result = await _twoFactorService.EnableTwoFactorAsync(userId, code);
        return result.Succeeded ? Result.Success(result.Message) : Result.Failure(result.Errors);
    }

    public async Task<Result> DisableTwoFactorAsync(string userId)
    {
        var result = await _twoFactorService.DisableTwoFactorAsync(userId);
        return result.Succeeded ? Result.Success(result.Message) : Result.Failure(result.Errors);
    }

    public Task<IEnumerable<string>> GenerateRecoveryCodesAsync(string userId)
    {
        return _twoFactorService.GenerateRecoveryCodesAsync(userId);
    }

    public Task<string> GetAuthenticatorKeyAsync(string userId)
    {
        return _twoFactorService.GetAuthenticatorKeyAsync(userId);
    }

    public async Task<Result> LinkGoogleAccountAsync(LinkExternalAccountRequest request)
    {
        var result = await _oauthService.LinkGoogleAccountAsync(request);
        return result.Succeeded ? Result.Success(result.Message) : Result.Failure(result.Errors);
    }

    public async Task<Result> LinkFacebookAccountAsync(LinkExternalAccountRequest request)
    {
        var result = await _oauthService.LinkFacebookAccountAsync(request);
        return result.Succeeded ? Result.Success(result.Message) : Result.Failure(result.Errors);
    }

    public async Task<Result> UnlinkExternalAccountAsync(UnlinkExternalAccountRequest request)
    {
        var result = await _oauthService.UnlinkExternalAccountAsync(request);
        return result.Succeeded ? Result.Success(result.Message) : Result.Failure(result.Errors);
    }

    public Task<IEnumerable<ExternalLoginInfo>> GetExternalLoginsAsync(string userId)
    {
        return _oauthService.GetExternalLoginsAsync(userId);
    }
}



