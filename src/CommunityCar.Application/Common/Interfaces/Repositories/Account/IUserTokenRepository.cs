using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Account.Authentication;
using CommunityCar.Domain.Enums.Account;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Account;

/// <summary>
/// Repository interface for UserToken entity operations
/// </summary>
public interface IUserTokenRepository : IBaseRepository<UserToken>
{
    #region Token Management
    Task<UserToken?> GetTokenAsync(Guid userId, TokenType tokenType, string token);
    Task<UserToken?> GetActiveTokenAsync(Guid userId, TokenType tokenType);
    Task<IEnumerable<UserToken>> GetUserTokensAsync(Guid userId, TokenType? tokenType = null);
    Task<bool> CreateTokenAsync(Guid userId, TokenType tokenType, string token, DateTime expiresAt, Dictionary<string, object>? metadata = null);
    Task<bool> InvalidateTokenAsync(string token);
    Task<bool> InvalidateUserTokensAsync(Guid userId, TokenType? tokenType = null);
    #endregion

    #region Token Validation
    Task<bool> IsTokenValidAsync(string token);
    Task<bool> IsTokenExpiredAsync(string token);
    Task<UserToken?> ValidateAndGetTokenAsync(string token);
    Task<bool> CleanupExpiredTokensAsync();
    #endregion

    #region Token Types
    Task<UserToken?> GetPasswordResetTokenAsync(Guid userId);
    Task<UserToken?> GetEmailConfirmationTokenAsync(Guid userId);
    Task<UserToken?> GetTwoFactorTokenAsync(Guid userId);
    Task<UserToken?> GetRefreshTokenAsync(Guid userId);
    #endregion
}