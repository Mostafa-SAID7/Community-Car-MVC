using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Domain.Entities.Account;

namespace CommunityCar.Application.Common.Interfaces.Services.Authentication;

public interface IAuthenticationService
{
    Task<Result> RegisterAsync(RegisterRequest request);
    Task<Result> LoginAsync(LoginRequest request);
    Task<Result> ConfirmEmailAsync(string userId, string token);
    Task<Result> ForgotPasswordAsync(string email);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
    Task<Result> ResendEmailConfirmationAsync(string email);
    Task<User?> GetCurrentUserAsync();
    Task LogoutAsync();
}


