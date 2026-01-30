using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Application.Common.Interfaces.Services.Authentication;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using CommunityCar.Domain.Entities.Account.Authentication;
using CommunityCar.Domain.Enums.Account;
using CommunityCar.Infrastructure.Persistence.Data;
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

    public async Task<UserToken?> GetTokenAsync(Guid userId, TokenType tokenType, string token)
    {
        return await Context.UserTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && t.TokenType == tokenType && t.Token == token);
    }

    public async Task<UserToken?> GetActiveTokenAsync(Guid userId, TokenType tokenType)
    {
        return await Context.UserTokens
            .Where(t => t.UserId == userId && t.TokenType == tokenType && t.IsActive && !t.IsExpired())
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserToken>> GetUserTokensAsync(Guid userId, TokenType? tokenType = null)
    {
        var query = Context.UserTokens.Where(t => t.UserId == userId);
        
        if (tokenType.HasValue)
        {
            query = query.Where(t => t.TokenType == tokenType.Value);
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> CreateTokenAsync(Guid userId, TokenType tokenType, string token, DateTime expiresAt, Dictionary<string, object>? metadata = null)
    {
        // Note: UserToken constructor adds validity to DateTime.UtcNow
        // For CreateTokenAsync, we'll calculate the validity span
        var validity = expiresAt - DateTime.UtcNow;
        var userToken = new UserToken(userId, tokenType, token, validity);
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

    public async Task<bool> InvalidateUserTokensAsync(Guid userId, TokenType? tokenType = null)
    {
        var query = Context.UserTokens.Where(t => t.UserId == userId && t.IsUsed == false);
        
        if (tokenType.HasValue)
        {
            query = query.Where(t => t.TokenType == tokenType.Value);
        }

        var tokens = await query.ToListAsync();

        foreach (var token in tokens)
        {
            token.MarkAsUsed();
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

        return userToken?.IsActive == true;
    }

    public async Task<bool> IsTokenExpiredAsync(string token)
    {
        var userToken = await Context.UserTokens
            .FirstOrDefaultAsync(t => t.Token == token);

        return userToken?.IsExpired() == true;
    }

    public async Task<UserToken?> ValidateAndGetTokenAsync(string token)
    {
        var userToken = await Context.UserTokens
            .FirstOrDefaultAsync(t => t.Token == token && t.IsUsed == false);

        if (userToken?.IsExpired() == true)
        {
            userToken.MarkAsUsed();
            await UpdateAsync(userToken);
            return null;
        }

        return userToken;
    }

    public async Task<bool> CleanupExpiredTokensAsync()
    {
        var expiredTokens = await Context.UserTokens
            .Where(t => t.IsUsed == false && t.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();

        foreach (var token in expiredTokens)
        {
            token.MarkAsUsed();
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
        return await GetActiveTokenAsync(userId, TokenType.PasswordReset);
    }

    public async Task<UserToken?> GetEmailConfirmationTokenAsync(Guid userId)
    {
        return await GetActiveTokenAsync(userId, TokenType.EmailVerification);
    }

    public async Task<UserToken?> GetTwoFactorTokenAsync(Guid userId)
    {
        // Try both email and SMS two factor tokens
        var emailToken = await GetActiveTokenAsync(userId, TokenType.EmailTwoFactor);
        if (emailToken != null) return emailToken;
        
        return await GetActiveTokenAsync(userId, TokenType.SmsTwoFactor);
    }

    public async Task<UserToken?> GetRefreshTokenAsync(Guid userId)
    {
        return await GetActiveTokenAsync(userId, TokenType.RefreshToken);
    }

    #endregion
}