using CommunityCar.Application.Common.Interfaces.Services.Authentication;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Repositories.User;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Infrastructure.Services.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Services.Authentication;

public class OAuthService : IOAuthService
{
    private readonly IGoogleOAuthService _googleOAuthService;
    private readonly IFacebookOAuthService _facebookOAuthService;
    private readonly IUserRepository _userRepository;
    private readonly UserManager<Domain.Entities.Auth.User> _userManager;
    private readonly ILogger<OAuthService> _logger;

    public OAuthService(
        IGoogleOAuthService googleOAuthService,
        IFacebookOAuthService facebookOAuthService,
        IUserRepository userRepository,
        UserManager<Domain.Entities.Auth.User> userManager,
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

    public async Task<IEnumerable<Application.Common.Interfaces.Services.Authentication.ExternalLoginInfo>> GetExternalLoginsAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
        if (user == null) return Enumerable.Empty<Application.Common.Interfaces.Services.Authentication.ExternalLoginInfo>();

        var logins = new List<Application.Common.Interfaces.Services.Authentication.ExternalLoginInfo>();
        if (!string.IsNullOrEmpty(user.GoogleId))
            logins.Add(new Application.Common.Interfaces.Services.Authentication.ExternalLoginInfo { LoginProvider = "Google", ProviderDisplayName = "Google", ProviderKey = user.GoogleId });
        if (!string.IsNullOrEmpty(user.FacebookId))
            logins.Add(new Application.Common.Interfaces.Services.Authentication.ExternalLoginInfo { LoginProvider = "Facebook", ProviderDisplayName = "Facebook", ProviderKey = user.FacebookId });

        return logins;
    }

    public async Task<bool> IsAccountLinkedAsync(string userId, string provider)
        => await _userRepository.IsOAuthAccountLinkedAsync(Guid.Parse(userId), provider);

    public async Task<string?> GetLinkedAccountIdAsync(string userId, string provider)
        => await _userRepository.GetOAuthAccountIdAsync(Guid.Parse(userId), provider);

    public async Task<bool> CanLinkAccountAsync(string userId, string provider)
        => !(await IsAccountLinkedAsync(userId, provider));

    public async Task<bool> CanUnlinkAccountAsync(string userId, string provider)
    {
        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
        if (user == null) return false;
        
        var hasPassword = await _userManager.HasPasswordAsync(user);
        var isLinked = await IsAccountLinkedAsync(userId, provider);
        
        return hasPassword && isLinked;
    }

    public async Task<bool> HasPasswordSetAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
        return user != null && await _userManager.HasPasswordAsync(user);
    }

    public Task<IEnumerable<string>> GetAvailableProvidersAsync()
        => Task.FromResult<IEnumerable<string>>(new[] { "Google", "Facebook" });
}
