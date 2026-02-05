using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Account.ViewModels.Authentication;

namespace CommunityCar.Application.Common.Interfaces.Services.Account.Security;

public interface IAccountSecurityService
{
    // Password Management
    Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<Result> ChangePasswordAsync(ChangePasswordRequest request);
    Task<bool> ValidatePasswordAsync(Guid userId, string password);
    Task<DateTime?> GetLastPasswordChangeAsync(Guid userId);

    // Two-Factor Authentication
    Task<TwoFactorSetupVM> SetupTwoFactorAsync(Guid userId);
    Task<bool> EnableTwoFactorAsync(Guid userId, TwoFactorSetupRequest request);
    Task<bool> DisableTwoFactorAsync(Guid userId, string password);
    Task<bool> VerifyTwoFactorCodeAsync(Guid userId, string code);
}