using CommunityCar.Application.Common.Interfaces.Orchestrators;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

namespace CommunityCar.Application.Orchestrators;

public class AccountLifecycleOrchestrator : IAccountLifecycleOrchestrator
{
    private readonly IAccountManagementService _managementService;

    public AccountLifecycleOrchestrator(IAccountManagementService managementService)
    {
        _managementService = managementService;
    }

    public Task<Result> DeactivateAccountAsync(DeactivateAccountRequest request)
    {
        return _managementService.DeactivateAccountAsync(request);
    }

    public Task<Result> DeleteAccountAsync(DeleteAccountRequest request)
    {
        return _managementService.DeleteAccountAsync(request);
    }

    public Task<Result> ExportUserDataAsync(ExportUserDataRequest request)
    {
        return _managementService.ExportUserDataAsync(request);
    }
}



