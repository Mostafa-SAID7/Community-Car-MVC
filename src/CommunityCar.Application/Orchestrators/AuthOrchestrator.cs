using CommunityCar.Application.Common.Interfaces.Orchestrators;
using CommunityCar.Application.Common.Interfaces.Services.Authentication;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;

namespace CommunityCar.Application.Orchestrators;

public class AuthOrchestrator : IAuthOrchestrator
{
    private readonly IAuthenticationService _authService;
    private readonly IOAuthService _oauthService;

    public AuthOrchestrator(
        IAuthenticationService authService,
        IOAuthService oauthService)
    {
        _authService = authService;
        _oauthService = oauthService;
    }

    public Task<Result> RegisterAsync(RegisterRequest request) => _authService.RegisterAsync(request);
    public Task<Result> LoginAsync(LoginRequest request) => _authService.LoginAsync(request);
    public Task LogoutAsync() => _authService.LogoutAsync();
    public Task<Result> ConfirmEmailAsync(string userId, string token) => _authService.ConfirmEmailAsync(userId, token);
    public Task<Result> ForgotPasswordAsync(string email) => _authService.ForgotPasswordAsync(email);
    public Task<Result> ResetPasswordAsync(ResetPasswordRequest request) => _authService.ResetPasswordAsync(request);
    public Task<Result> GoogleSignInAsync(GoogleSignInRequest request) => _oauthService.GoogleSignInAsync(request);
    public Task<Result> FacebookSignInAsync(FacebookSignInRequest request) => _oauthService.FacebookSignInAsync(request);
}
