using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Account.ViewModels.Security;

namespace CommunityCar.Application.Common.Interfaces.Services.Account.Security;

public interface IAccountSecurityService
{
    Task<Result<SecurityOverviewVM>> GetSecurityOverviewAsync(Guid userId);
    Task<Result<List<LoginHistoryVM>>> GetLoginHistoryAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<Result<List<SecurityEventVM>>> GetSecurityEventsAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<Result> LogSecurityEventAsync(Guid userId, string eventType, string description, string? ipAddress = null);
    Task<Result> EnableTwoFactorAsync(Guid userId);
    Task<Result> DisableTwoFactorAsync(Guid userId);
    Task<Result<List<DeviceVM>>> GetTrustedDevicesAsync(Guid userId);
    Task<Result> RemoveTrustedDeviceAsync(Guid userId, Guid deviceId);
    Task<Result> UpdatePasswordAsync(Guid userId, ChangePasswordVM request);
    Task<Result<List<SecurityQuestionVM>>> GetSecurityQuestionsAsync(Guid userId);
    Task<Result> UpdateSecurityQuestionsAsync(Guid userId, List<SecurityQuestionVM> questions);
    Task<Result<AccountLockoutVM>> GetLockoutInfoAsync(Guid userId);
    Task<Result> LockAccountAsync(Guid userId, string reason, TimeSpan? duration = null);
    Task<Result> UnlockAccountAsync(Guid userId);
}