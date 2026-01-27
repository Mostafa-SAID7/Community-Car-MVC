using CommunityCar.Application.Common.Interfaces.Orchestrators;
using CommunityCar.Application.Common.Interfaces.Services.Authentication;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;

namespace CommunityCar.Application.Orchestrators;

public class IdentityOrchestrator : IIdentityOrchestrator
{
    private readonly IAuthenticationService _authService;
    private readonly IOAuthService _oauthService;

    public IdentityOrchestrator(
        IAuthenticationService authService,
        IOAuthService oauthService)
    {
        _authService = authService;
        _oauthService = oauthService;
    }

    public Task<Result> RegisterAsync(RegisterRequest request)
    {
        return _authService.RegisterAsync(request);
    }

    public Task<Result> LoginAsync(LoginRequest request)
    {
        return _authService.LoginAsync(request);
    }

    public Task LogoutAsync()
    {
        return _authService.LogoutAsync();
    }

    public Task<Result> ConfirmEmailAsync(string userId, string token)
    {
        return _authService.ConfirmEmailAsync(userId, token);
    }

    public Task<Result> ForgotPasswordAsync(string email)
    {
        return _authService.ForgotPasswordAsync(email);
    }

    public Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        return _authService.ResetPasswordAsync(request);
    }

    public Task<Result> GoogleSignInAsync(GoogleSignInRequest request)
    {
        return _oauthService.GoogleSignInAsync(request);
    }

    public Task<Result> FacebookSignInAsync(FacebookSignInRequest request)
    {
        return _oauthService.FacebookSignInAsync(request);
    }
}



