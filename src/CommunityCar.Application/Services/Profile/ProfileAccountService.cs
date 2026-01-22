using CommunityCar.Application.Common.Interfaces.Services.Profile;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Profile.DTOs;
using Microsoft.AspNetCore.Identity;
using CommunityCar.Domain.Entities.Auth;

namespace CommunityCar.Application.Services.Profile;

public class ProfileAccountService : IProfileAccountService
{
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public ProfileAccountService(
        UserManager<User> userManager,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<bool> DeactivateAccountAsync(Guid userId, string password)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        if (!await _userManager.CheckPasswordAsync(user, password))
            return false;

        user.Deactivate();
        user.Audit(_currentUserService.UserId);
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> DeleteAccountAsync(Guid userId, DeleteAccountRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            return false;

        if (!request.ConfirmDeletion)
            return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UpdateNotificationSettingsAsync(Guid userId, UpdateNotificationSettingsRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        user.UpdateNotificationSettings(
            request.EmailNotifications,
            request.PushNotifications,
            request.SmsNotifications,
            request.MarketingEmails);
        
        user.Audit(_currentUserService.UserId);
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UpdatePrivacySettingsAsync(Guid userId, Dictionary<string, bool> settings)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        user.UpdatePrivacySettings(settings);
        user.Audit(_currentUserService.UserId);
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<Dictionary<string, bool>> GetPrivacySettingsAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return new Dictionary<string, bool>();

        return user.GetPrivacySettings();
    }
}