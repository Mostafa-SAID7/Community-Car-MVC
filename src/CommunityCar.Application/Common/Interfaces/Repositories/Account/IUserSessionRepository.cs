using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Account.Authentication;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Account;

/// <summary>
/// Repository interface for UserSession entity operations
/// </summary>
public interface IUserSessionRepository : IBaseRepository<UserSession>
{
    #region Session Management
    Task<UserSession?> GetActiveSessionAsync(Guid userId, string sessionId);
    Task<IEnumerable<UserSession>> GetActiveSessionsAsync(Guid userId);
    Task<IEnumerable<UserSession>> GetAllSessionsAsync(Guid userId);
    Task<bool> CreateSessionAsync(Guid userId, string sessionId, string ipAddress, string userAgent);
    Task<bool> UpdateSessionActivityAsync(string sessionId);
    Task<bool> EndSessionAsync(string sessionId);
    Task<bool> EndAllUserSessionsAsync(Guid userId);
    #endregion

    #region Session Security
    Task<bool> IsSessionValidAsync(string sessionId);
    Task<IEnumerable<UserSession>> GetSuspiciousSessionsAsync(Guid userId);
    Task<int> GetActiveSessionCountAsync(Guid userId);
    Task<bool> EndExpiredSessionsAsync();
    #endregion

    #region Session Analytics
    Task<IEnumerable<UserSession>> GetRecentSessionsAsync(Guid userId, int count = 10);
    Task<Dictionary<string, int>> GetSessionsByLocationAsync(Guid userId);
    Task<TimeSpan> GetAverageSessionDurationAsync(Guid userId);
    #endregion
}