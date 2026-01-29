using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Domain.Entities.Account.Authentication;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Account.Authentication;

/// <summary>
/// Repository implementation for UserToken entity operations
/// </summary>
public class UserTokenRepository : BaseRepository<UserToken>, IUserTokenRepository
{
    public UserTokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    #region Token Management

    public async Task<UserToken?> GetTokenAsync(Guid userId, string tokenType, string token)
    {
        return await Context.UserTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && t.TokenType == tokenType && t.Token == token);
    }

    public async Task<UserToken?> GetActiveTokenAsync(Guid userId, string tokenType)
    {
        return await Context.UserTokens
            .Where(t => t.UserId == userId && t.TokenType == tokenType && t.IsActive && !t.IsExpired)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserToken>> GetUserTokensAsync(Guid userId, string? tokenType = null)
    {
        var query = Context.UserTokens.Where(t => t.UserId == userId);
        
        if (!string.IsNullOrEmpty(tokenType))
        {
            query = query.Where(t => t.TokenType == tokenType);
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> CreateTokenAsync(Guid userId, string tokenType, string token, DateTime expiresAt, Dictionary<string, object>? metadata = null)
    {
        var userToken = UserToken.Create(userId, tokenType, token, expiresAt, metadata);
        await AddAsync(userToken);
        return true;
    }

    public async Task<bool> InvalidateTokenAsync(string token)
    {
        var userToken = await Context.UserTokens
            .FirstOrDefaultAsync(t => t.Token == token);

        if (userToken == null) return false;

        userToken.Invalidate();
        await UpdateAsync(userToken);
        return true;
    }

    public async Task<bool> InvalidateUserTokensAsync(Guid userId, string? tokenType = null)
    {
        var query = Context.UserTokens.Where(t => t.UserId == userId && t.IsActive);
        
        if (!string.IsNullOrEmpty(tokenType))
        {
            query = query.Where(t => t.TokenType == tokenType);
        }

        var tokens = await query.ToListAsync();

        foreach (var token in tokens)
        {
            token.Invalidate();
        }

        if (tokens.Any())
        {
            Context.UserTokens.UpdateRange(tokens);
            return true;
        }

        return false;
    }

    #endregion

    #region Token Validation

    public async Task<bool> IsTokenValidAsync(string token)
    {
        var userToken = await Context.UserTokens
            .FirstOrDefaultAsync(t => t.Token == token);

        return userToken?.IsActive == true && !userToken.IsExpired;
    }

    public async Task<bool> IsTokenExpiredAsync(string token)
    {
        var userToken = await Context.UserTokens
            .FirstOrDefaultAsync(t => t.Token == token);

        return userToken?.IsExpired == true;
    }

    public async Task<UserToken?> ValidateAndGetTokenAsync(string token)
    {
        var userToken = await Context.UserTokens
            .FirstOrDefaultAsync(t => t.Token == token && t.IsActive);

        if (userToken?.IsExpired == true)
        {
            userToken.Invalidate();
            await UpdateAsync(userToken);
            return null;
        }

        return userToken;
    }

    public async Task<bool> CleanupExpiredTokensAsync()
    {
        var expiredTokens = await Context.UserTokens
            .Where(t => t.IsActive && t.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();

        foreach (var token in expiredTokens)
        {
            token.Invalidate();
        }

        if (expiredTokens.Any())
        {
            Context.UserTokens.UpdateRange(expiredTokens);
            return true;
        }

        return false;
    }

    #endregion

    #region Token Types

    public async Task<UserToken?> GetPasswordResetTokenAsync(Guid userId)
    {
        return await GetActiveTokenAsync(userId, "PasswordReset");
    }

    public async Task<UserToken?> GetEmailConfirmationTokenAsync(Guid userId)
    {
        return await GetActiveTokenAsync(userId, "EmailConfirmation");
    }

    public async Task<UserToken?> GetTwoFactorTokenAsync(Guid userId)
    {
        return await GetActiveTokenAsync(userId, "TwoFactor");
    }

    public async Task<UserToken?> GetRefreshTokenAsync(Guid userId)
    {
        return await GetActiveTokenAsync(userId, "RefreshToken");
    }

    #endregion
}