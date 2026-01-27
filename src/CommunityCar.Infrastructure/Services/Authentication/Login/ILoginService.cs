using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Domain.Entities.Auth;

namespace CommunityCar.Infrastructure.Services.Authentication.Login;

/// <summary>
/// Interface for user login operations
/// </summary>
public interface ILoginService
{
    Task<Result> LoginAsync(LoginRequest request);
    Task<User?> GetCurrentUserAsync();
    Task LogoutAsync();
}
