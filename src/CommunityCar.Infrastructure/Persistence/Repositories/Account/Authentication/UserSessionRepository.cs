using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Domain.Entities.Account.Authentication;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Account.Authentication;

/// <summary>
/// Repository implementation for UserSession entity operations
/// </summary>
public class UserSessionRepository : BaseRepository<UserSession>, IUserSessionRepository
{
    public UserSessionRepository(ApplicationDbContext context) : base(context)
    {
    }

    #region Session Management

    public async Task<UserSession?> GetActiveSessionAsync(Guid userId, string sessionId)
    {
        return await Context.UserSessions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.SessionId == sessionId && s.IsActive);
    }

    public async Task<IEnumerable<UserSession>> GetActiveSessionsAsync(Guid userId)
    {
        return await Context.UserSessions
            .Where(s => s.UserId == userId && s.IsActive)
            .OrderByDescending(s => s.LastActivityAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserSession>> GetAllSessionsAsync(Guid userId)
    {
        return await Context.UserSessions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> CreateSessionAsync(Guid userId, string sessionId, string ipAddress, string userAgent)
    {
        var session = UserSession.Create(userId, sessionId, ipAddress, userAgent);
        await AddAsync(session);
        return true;
    }

    public async Task<bool> UpdateSessionActivityAsync(string sessionId)
    {
        var session = await Context.UserSessions
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null) return false;

        session.UpdateActivity();
        await UpdateAsync(session);
        return true;
    }

    public async Task<bool> EndSessionAsync(string sessionId)
    {
        var session = await Context.UserSessions
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null) return false;

        session.EndSession();
        await UpdateAsync(session);
        return true;
    }

    public async Task<bool> EndAllUserSessionsAsync(Guid userId)
    {
        var sessions = await Context.UserSessions
            .Where(s => s.UserId == userId && s.IsActive)
            .ToListAsync();

        foreach (var session in sessions)
        {
            session.EndSession();
        }

        if (sessions.Any())
        {
            Context.UserSessions.UpdateRange(sessions);
            return true;
        }

        return false;
    }

    #endregion

    #region Session Security

    public async Task<bool> IsSessionValidAsync(string sessionId)
    {
        var session = await Context.UserSessions
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        return session?.IsActive == true && !session.IsExpired;
    }

    public async Task<IEnumerable<UserSession>> GetSuspiciousSessionsAsync(Guid userId)
    {
        var userSessions = await Context.UserSessions
            .Where(s => s.UserId == userId && s.IsActive)
            .ToListAsync();

        // Logic to identify suspicious sessions (different IP, unusual location, etc.)
        return userSessions.Where(s => s.IsSuspicious).ToList();
    }

    public async Task<int> GetActiveSessionCountAsync(Guid userId)
    {
        return await Context.UserSessions
            .Where(s => s.UserId == userId && s.IsActive)
            .CountAsync();
    }

    public async Task<bool> EndExpiredSessionsAsync()
    {
        var expiredSessions = await Context.UserSessions
            .Where(s => s.IsActive && s.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();

        foreach (var session in expiredSessions)
        {
            session.EndSession();
        }

        if (expiredSessions.Any())
        {
            Context.UserSessions.UpdateRange(expiredSessions);
            return true;
        }

        return false;
    }

    #endregion

    #region Session Analytics

    public async Task<IEnumerable<UserSession>> GetRecentSessionsAsync(Guid userId, int count = 10)
    {
        return await Context.UserSessions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetSessionsByLocationAsync(Guid userId)
    {
        return await Context.UserSessions
            .Where(s => s.UserId == userId)
            .GroupBy(s => s.Location ?? "Unknown")
            .Select(g => new { Location = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Location, x => x.Count);
    }

    public async Task<TimeSpan> GetAverageSessionDurationAsync(Guid userId)
    {
        var sessions = await Context.UserSessions
            .Where(s => s.UserId == userId && s.EndedAt.HasValue)
            .Select(s => s.EndedAt!.Value - s.CreatedAt)
            .ToListAsync();

        if (!sessions.Any()) return TimeSpan.Zero;

        var totalTicks = sessions.Sum(ts => ts.Ticks);
        return new TimeSpan(totalTicks / sessions.Count);
    }

    #endregion
}