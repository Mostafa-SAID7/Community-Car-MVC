using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;

namespace CommunityCar.Infrastructure.Services.Authentication.Registration;

/// <summary>
/// Interface for user registration operations
/// </summary>
public interface IRegistrationService
{
    Task<Result> RegisterAsync(RegisterRequest request);
    Task<Result> ConfirmEmailAsync(string userId, string token);
    Task<Result> ResendEmailConfirmationAsync(string email);
}
