using CommunityCar.Application.Common.Interfaces.Services.Account.Management;

namespace CommunityCar.Application.Services.Account.Management;

public class AccountManagementService : IAccountManagementService
{
    public async Task<bool> DeactivateAccountAsync(string userId)
    {
        // Stub implementation
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> DeleteAccountAsync(string userId)
    {
        // Stub implementation
        await Task.CompletedTask;
        return true;
    }

    public async Task<byte[]> ExportUserDataAsync(string userId)
    {
        // Stub implementation
        await Task.CompletedTask;
        return Array.Empty<byte>();
    }
}