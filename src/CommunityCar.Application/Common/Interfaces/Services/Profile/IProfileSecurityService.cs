using CommunityCar.Application.Features.Profile.DTOs;
using CommunityCar.Application.Features.Profile.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Profile;

public interface IProfileSecurityService
{
    // Password management
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<bool> ValidateCurrentPasswordAsync(Guid userId, string password);
    
    // Email management
    Task<bool> UpdateEmailAsync(Guid userId, UpdateEmailRequest request);
    Task<bool> SendEmailConfirmationAsync(Guid userId);
    Task<bool> ConfirmEmailAsync(Guid userId, string token);
    
    // Two-factor authentication
    Task<TwoFactorSetupVM> SetupTwoFactorAsync(Guid userId);
    Task<bool> EnableTwoFactorAsync(Guid userId, TwoFactorSetupRequest request);
    Task<bool> DisableTwoFactorAsync(Guid userId, string password);
    Task<List<string>> GenerateBackupCodesAsync(Guid userId);
    
    // OAuth connections
    Task<bool> LinkGoogleAccountAsync(Guid userId, string googleId, string? profilePictureUrl = null);
    Task<bool> LinkFacebookAccountAsync(Guid userId, string facebookId, string? profilePictureUrl = null);
    Task<bool> UnlinkGoogleAccountAsync(Guid userId);
    Task<bool> UnlinkFacebookAccountAsync(Guid userId);
    
    // Security logs and monitoring
    Task<List<SecurityLogVM>> GetSecurityLogAsync(Guid userId, int page = 1, int pageSize = 20);
}