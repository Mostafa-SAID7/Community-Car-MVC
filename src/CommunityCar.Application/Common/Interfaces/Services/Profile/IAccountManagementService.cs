using CommunityCar.Application.Features.Profile.DTOs;

namespace CommunityCar.Application.Common.Interfaces.Services.Profile;

public interface IAccountManagementService
{
    // Account management
    Task<bool> DeactivateAccountAsync(Guid userId, string password);
    Task<bool> DeleteAccountAsync(Guid userId, DeleteAccountRequest request);
}