using CommunityCar.Application.Features.Profile.DTOs;

namespace CommunityCar.Application.Common.Interfaces.Services.Profile;

public interface IProfileAccountService
{
    // Account management
    Task<bool> DeactivateAccountAsync(Guid userId, string password);
    Task<bool> DeleteAccountAsync(Guid userId, DeleteAccountRequest request);
    
    // Notification settings
    Task<bool> UpdateNotificationSettingsAsync(Guid userId, UpdateNotificationSettingsRequest request);
    
    // Privacy and security
    Task<bool> UpdatePrivacySettingsAsync(Guid userId, Dictionary<string, bool> settings);
    Task<Dictionary<string, bool>> GetPrivacySettingsAsync(Guid userId);
}