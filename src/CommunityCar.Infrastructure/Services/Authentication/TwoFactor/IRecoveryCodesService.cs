using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;

namespace CommunityCar.Infrastructure.Services.Authentication.TwoFactor;

/// <summary>
/// Interface for recovery codes management
/// </summary>
public interface IRecoveryCodesService
{
    Task<Result> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request);
    Task<Result> VerifyRecoveryCodeAsync(VerifyRecoveryCodeRequest request);
    Task<int> GetRecoveryCodesCountAsync(string userId);
}
