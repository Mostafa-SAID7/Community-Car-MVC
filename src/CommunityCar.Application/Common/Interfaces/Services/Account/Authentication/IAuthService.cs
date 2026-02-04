using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Account.ViewModels.Authentication;
using CommunityCar.Domain.Entities.Account.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace CommunityCar.Application.Common.Interfaces.Services.Account;

public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterRequest request);
    Task<Result> LoginAsync(LoginRequest request);
    Task LogoutAsync();
    Task<Result> ConfirmEmailAsync(string userId, string token);
    Task<Result> ForgotPasswordAsync(string email);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
    Task<Result> ResendEmailConfirmationAsync(string email);
    Task<User?> GetCurrentUserAsync();
    
    // External login methods
    AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string? redirectUrl);
    Task<Microsoft.AspNetCore.Identity.SignInResult> ExternalLoginSignInAsync();
    Task<Microsoft.AspNetCore.Identity.ExternalLoginInfo?> GetExternalLoginInfoAsync();
    Task<Result> CreateUserWithExternalLoginAsync(Microsoft.AspNetCore.Identity.ExternalLoginInfo info);
}
