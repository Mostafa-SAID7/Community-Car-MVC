using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;

namespace CommunityCar.Infrastructure.Services.Authentication.PasswordReset;

/// <summary>
/// Interface for password reset operations
/// </summary>
public interface IPasswordResetService
{
    Task<Result> ForgotPasswordAsync(string email);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
    Task<Result> ChangePasswordAsync(ChangePasswordRequest request);
}
