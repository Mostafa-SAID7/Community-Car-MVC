using CommunityCar.Domain.Entities.Account.Core;

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
}
