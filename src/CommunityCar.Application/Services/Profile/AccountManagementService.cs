using CommunityCar.Application.Common.Interfaces.Services.Profile;
using CommunityCar.Application.Common.Interfaces.Services.Storage;
using CommunityCar.Application.Features.Profile.DTOs;
using Microsoft.AspNetCore.Identity;
using CommunityCar.Domain.Entities.Auth;

namespace CommunityCar.Application.Services.Profile;

public class AccountManagementService : IAccountManagementService
{
    private readonly UserManager<User> _userManager;
    private readonly IFileStorageService _fileStorageService;

    public AccountManagementService(
        UserManager<User> userManager,
        IFileStorageService fileStorageService)
    {
        _userManager = userManager;
        _fileStorageService = fileStorageService;
    }

    public async Task<bool> DeactivateAccountAsync(Guid userId, string password)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        if (!await _userManager.CheckPasswordAsync(user, password))
            return false;

        user.Deactivate();
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

        // Delete profile picture
        if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
        {
            await _fileStorageService.DeleteFileAsync(user.ProfilePictureUrl);
        }

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }
}