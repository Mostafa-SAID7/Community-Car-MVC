using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Domain.Entities.Auth;

namespace CommunityCar.Application.Common.Interfaces.Services.Authentication;

public interface IAuthenticationService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<AuthResult> ConfirmEmailAsync(string userId, string token);
    Task<AuthResult> ForgotPasswordAsync(string email);
    Task<AuthResult> ResetPasswordAsync(ResetPasswordRequest request);
    Task<AuthResult> ChangePasswordAsync(ChangePasswordRequest request);
    Task<AuthResult> ResendEmailConfirmationAsync(string email);
    Task<User?> GetCurrentUserAsync();
    Task LogoutAsync();
}