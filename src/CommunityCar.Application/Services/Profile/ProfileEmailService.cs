using CommunityCar.Application.Common.Interfaces.Services.Profile;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Profile.DTOs;
using Microsoft.AspNetCore.Identity;
using CommunityCar.Domain.Entities.Auth;

namespace CommunityCar.Application.Services.Profile;

public class ProfileEmailService : IProfileEmailService
{
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public ProfileEmailService(
        UserManager<User> userManager,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<bool> UpdateEmailAsync(Guid userId, UpdateEmailRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        // Validate current password
        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            return false;

        // Generate email change token
        var token = await _userManager.GenerateChangeEmailTokenAsync(user, request.NewEmail);
        var result = await _userManager.ChangeEmailAsync(user, request.NewEmail, token);

        if (result.Succeeded)
        {
            user.UserName = request.NewEmail; // Update username to match email
            user.Audit(_currentUserService.UserId);
            await _userManager.UpdateAsync(user);
        }

        return result.Succeeded;
    }

    public async Task<bool> SendEmailConfirmationAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        // In real app, send email with confirmation link
        // await _emailService.SendEmailConfirmationAsync(user.Email, token);
        
        return true;
    }

    public async Task<bool> ConfirmEmailAsync(Guid userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }
}