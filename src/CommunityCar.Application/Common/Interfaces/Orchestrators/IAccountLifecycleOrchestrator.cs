using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

namespace CommunityCar.Application.Common.Interfaces.Orchestrators;

public interface IAccountLifecycleOrchestrator
{
    Task<Result> DeactivateAccountAsync(DeactivateAccountRequest request);
    Task<Result> DeleteAccountAsync(DeleteAccountRequest request);
    Task<Result> ExportUserDataAsync(ExportUserDataRequest request);
}



