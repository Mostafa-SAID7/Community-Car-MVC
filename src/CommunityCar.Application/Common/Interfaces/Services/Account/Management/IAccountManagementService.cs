namespace CommunityCar.Application.Common.Interfaces.Services.Account.Management;

public interface IAccountManagementService
{
    Task<bool> DeactivateAccountAsync(string userId);
    Task<bool> DeleteAccountAsync(string userId);
    Task<byte[]> ExportUserDataAsync(string userId);
}