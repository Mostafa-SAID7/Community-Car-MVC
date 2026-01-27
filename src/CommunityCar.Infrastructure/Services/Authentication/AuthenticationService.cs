using CommunityCar.Application.Common.Interfaces.Services.Authentication;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Infrastructure.Services.Authentication.Registration;
using CommunityCar.Infrastructure.Services.Authentication.Login;
using CommunityCar.Infrastructure.Services.Authentication.PasswordReset;
using CommunityCar.Domain.Entities.Auth;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Services.Authentication;

/// <summary>
/// Orchestrator service for authentication operations
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IRegistrationService _registrationService;
    private readonly ILoginService _loginService;
    private readonly IPasswordResetService _passwordResetService;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IRegistrationService registrationService,
        ILoginService loginService,
        IPasswordResetService passwordResetService,
        ILogger<AuthenticationService> logger)
    {
        _registrationService = registrationService;
        _loginService = loginService;
        _passwordResetService = passwordResetService;
        _logger = logger;
    }

    #region Registration - Delegate to RegistrationService

    public async Task<Result> RegisterAsync(RegisterRequest request)
        => await _registrationService.RegisterAsync(request);

    public async Task<Result> ConfirmEmailAsync(string userId, string token)
        => await _registrationService.ConfirmEmailAsync(userId, token);

    public async Task<Result> ResendEmailConfirmationAsync(string email)
        => await _registrationService.ResendEmailConfirmationAsync(email);

    #endregion

    #region Login - Delegate to LoginService

    public async Task<Result> LoginAsync(LoginRequest request)
        => await _loginService.LoginAsync(request);

    public async Task<User?> GetCurrentUserAsync()
        => await _loginService.GetCurrentUserAsync();

    public async Task LogoutAsync()
        => await _loginService.LogoutAsync();

    #endregion

    #region Password Reset - Delegate to PasswordResetService

    public async Task<Result> ForgotPasswordAsync(string email)
        => await _passwordResetService.ForgotPasswordAsync(email);

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
        => await _passwordResetService.ResetPasswordAsync(request);

    public async Task<Result> ChangePasswordAsync(ChangePasswordRequest request)
        => await _passwordResetService.ChangePasswordAsync(request);

    #endregion
}
