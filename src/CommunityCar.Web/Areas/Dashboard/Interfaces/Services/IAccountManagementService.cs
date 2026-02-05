namespace CommunityCar.Web.Areas.Dashboard.Interfaces.Services;

public interface IAccountManagementService
{
    Task<bool> DeactivateAccountAsync(string userId);
    Task<bool> DeleteAccountAsync(string userId);
    Task<byte[]> ExportUserDataAsync(string userId);
}
