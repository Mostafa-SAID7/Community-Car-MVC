using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Services.Account.Authentication.OAuth;
using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Extensions;
using CommunityCar.Application.Features.Account.ViewModels.Authentication;
using CommunityCar.Domain.Entities.Account.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Account;

public class OAuthService : IOAuthService
{
    private readonly IGoogleOAuthService _googleOAuthService;
    private readonly IFacebookOAuthService _facebookOAuthService;
    private readonly IUserRepository _userRepository;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<OAuthService> _logger;

    public OAuthService(
        IGoogleOAuthService googleOAuthService,
        IFacebookOAuthService facebookOAuthService,
        IUserRepository userRepository,
        UserManager<User> userManager,
        ILogger<OAuthService> logger)
    {
        _googleOAuthService = googleOAuthService;
        _facebookOAuthService = facebookOAuthService;
        _userRepository = userRepository;
        _userManager = userManager;
        _logger = logger;
    }

    public Task<Result> GoogleSignInAsync(GoogleSignInRequest request)
        => _googleOAuthService.GoogleSignInAsync(request);

    public Task<Result> FacebookSignInAsync(FacebookSignInRequest request)
        => _facebookOAuthService.FacebookSignInAsync(request);

    public Task<Result> LinkGoogleAccountAsync(LinkExternalAccountRequest request)
        => _googleOAuthService.LinkGoogleAccountAsync(request);

    public Task<Result> LinkFacebookAccountAsync(LinkExternalAccountRequest request)
        => _facebookOAuthService.LinkFacebookAccountAsync(request);

    public async Task<Result> UnlinkExternalAccountAsync(UnlinkExternalAccountRequest request)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(Guid.Parse(request.UserId));
            if (user == null) return Result.Failure("User not found.");

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword) return Result.Failure("Please set a password before unlinking your only login method.");

            var result = await _userRepository.UnlinkOAuthAccountAsync(user.Id, request.Provider);
            if (result)
            {
                _logger.LogInformation("{Provider} account unlinked for user {UserId}", request.Provider, request.UserId);
                return Result.Success($"{request.Provider} account unlinked successfully.");
            }

            return Result.Failure($"Failed to unlink {request.Provider} account.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlinking {Provider} account for user {UserId}", request.Provider, request.UserId);
            return Result.Failure("An error occurred during unlinking.");
        }
    }

    public async Task<IEnumerable<ExternalLoginInfo>> GetExternalLoginsAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
        if (user == null) return Enumerable.Empty<ExternalLoginInfo>();

        var logins = new List<ExternalLoginInfo>();
        if (!string.IsNullOrEmpty(user.OAuthInfo.GoogleId))
            logins.Add(new ExternalLoginInfo { LoginProvider = "Google", ProviderDisplayName = "Google", ProviderKey = user.OAuthInfo.GoogleId });
        if (!string.IsNullOrEmpty(user.OAuthInfo.FacebookId))
            logins.Add(new ExternalLoginInfo { LoginProvider = "Facebook", ProviderDisplayName = "Facebook", ProviderKey = user.OAuthInfo.FacebookId });

        return logins;
    }

    public Task<Result> LinkAccountAsync(LinkExternalAccountRequest request)
    {
        if (request.Provider == "Google")
            return LinkGoogleAccountAsync(request);
        if (request.Provider == "Facebook")
            return LinkFacebookAccountAsync(request);
            
        return Task.FromResult(Result.Failure("Unsupported provider."));
    }

    public Task<Result> UnlinkAccountAsync(Guid userId, string provider)
    {
        return UnlinkExternalAccountAsync(new UnlinkExternalAccountRequest 
        { 
            UserId = userId.ToString(), 
            Provider = provider 
        });
    }

    public async Task<bool> IsAccountLinkedAsync(string userId, string provider)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;
        
        var logins = await _userManager.GetLoginsAsync(user);
        return logins.Any(l => l.LoginProvider == provider);
    }

    public async Task<string?> GetLinkedAccountIdAsync(string userId, string provider)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;
        
        var logins = await _userManager.GetLoginsAsync(user);
        return logins.FirstOrDefault(l => l.LoginProvider == provider)?.ProviderKey;
    }

    public async Task<bool> CanLinkAccountAsync(string userId, string provider)
        => !(await IsAccountLinkedAsync(userId, provider));

    public async Task<bool> CanUnlinkAccountAsync(string userId, string provider)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;
        
        var hasPassword = await _userManager.HasPasswordAsync(user);
        var isLinked = await IsAccountLinkedAsync(userId, provider);
        
        return hasPassword && isLinked;
    }

    public async Task<bool> HasPasswordSetAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null && await _userManager.HasPasswordAsync(user);
    }

    public Task<IEnumerable<string>> GetAvailableProvidersAsync()
        => Task.FromResult<IEnumerable<string>>(new[] { "Google", "Facebook" });

    // Repository Operations (extracted from IUserRepository) Implementation
    public async Task<Result> LinkOAuthAccountAsync(Guid userId, string provider, string providerId, string? profilePictureUrl = null)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Result.Failure("User not found");

        var info = new UserLoginInfo(provider, providerId, provider);
        var result = await _userManager.AddLoginAsync(user, info);
        return result.ToApplicationResult();
    }

    public async Task<Result> UnlinkOAuthAccountAsync(Guid userId, string provider)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Result.Failure("User not found");

        var logins = await _userManager.GetLoginsAsync(user);
        var login = logins.FirstOrDefault(l => l.LoginProvider == provider);
        if (login == null) return Result.Failure("Account not linked");

        var result = await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
        return result.ToApplicationResult();
    }

    public async Task<bool> IsOAuthAccountLinkedAsync(Guid userId, string provider)
    {
        return await IsAccountLinkedAsync(userId.ToString(), provider);
    }

    public async Task<string?> GetOAuthAccountIdAsync(Guid userId, string provider)
    {
        return await GetLinkedAccountIdAsync(userId.ToString(), provider);
    }
}
