using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

namespace CommunityCar.Application.Services.Account.Compliance;

/// <summary>
/// Interface for compliance and legal consent management
/// </summary>
public interface IComplianceService
{
    Task<bool> AcceptTermsOfServiceAsync(Guid userId, string version);
    Task<bool> AcceptPrivacyPolicyAsync(Guid userId, string version);
    Task<IEnumerable<ConsentVM>> GetUserConsentsAsync(Guid userId);
}


