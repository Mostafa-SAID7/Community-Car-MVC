using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;

namespace CommunityCar.Application.Common.Interfaces.Orchestrators;

public interface IIdentityOrchestrator
{
    Task<Result> RegisterAsync(RegisterRequest request);
    Task<Result> LoginAsync(LoginRequest request);
    Task LogoutAsync();
    Task<Result> ConfirmEmailAsync(string userId, string token);
    Task<Result> ForgotPasswordAsync(string email);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
    Task<Result> GoogleSignInAsync(GoogleSignInRequest request);
    Task<Result> FacebookSignInAsync(FacebookSignInRequest request);
}



