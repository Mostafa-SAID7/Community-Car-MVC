using CommunityCar.Application.Features.Profile.DTOs;

namespace CommunityCar.Application.Common.Interfaces.Services.Profile;

public interface IProfileEmailService
{
    // Email management
    Task<bool> UpdateEmailAsync(Guid userId, UpdateEmailRequest request);
    Task<bool> SendEmailConfirmationAsync(Guid userId);
    Task<bool> ConfirmEmailAsync(Guid userId, string token);
}