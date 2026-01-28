using CommunityCar.Application.Common.Interfaces.Repositories.Profile;
using CommunityCar.Domain.Entities.Account.Profile;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Profile;

public class UserInterestRepository : BaseRepository<UserInterest>, IUserInterestRepository
{
    public UserInterestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserInterest>> GetUserInterestsAsync(Guid userId, bool activeOnly = true)
    {
        var query = Context.UserInterests.Where(ui => ui.UserId == userId);
        
        if (activeOnly)
        {
            query = query.Where(ui => ui.IsActive);
        }
        
        return await query.OrderByDescending(ui => ui.Score).ToListAsync();
    }

    public async Task<UserInterest?> GetUserInterestAsync(Guid userId, string category, string subCategory)
    {
        return await Context.UserInterests
            .FirstOrDefaultAsync(ui => ui.UserId == userId && ui.Category == category && ui.SubCategory == subCategory);
    }

    public async Task<IEnumerable<UserInterest>> GetInterestsByCategoryAsync(Guid userId, string category)
    {
        return await Context.UserInterests
            .Where(ui => ui.UserId == userId && ui.Category == category && ui.IsActive)
            .OrderByDescending(ui => ui.Score)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserInterest>> GetTopInterestsAsync(Guid userId, int count = 10)
    {
        return await Context.UserInterests
            .Where(ui => ui.UserId == userId && ui.IsActive)
            .OrderByDescending(ui => ui.Score)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Dictionary<string, float>> GetInterestScoresAsync(Guid userId)
    {
        return await Context.UserInterests
            .Where(ui => ui.UserId == userId && ui.IsActive)
            .ToDictionaryAsync(ui => $"{ui.Category}:{ui.SubCategory}", ui => (float)ui.Score);
    }

    public async Task<IEnumerable<UserInterest>> GetSimilarInterestsAsync(Guid userId1, Guid userId2)
    {
        var user1Interests = await Context.UserInterests
            .Where(ui => ui.UserId == userId1 && ui.IsActive)
            .Select(ui => new { ui.Category, ui.SubCategory })
            .ToListAsync();

        var user2Interests = await Context.UserInterests
            .Where(ui => ui.UserId == userId2 && ui.IsActive)
            .Select(ui => new { ui.Category, ui.SubCategory })
            .ToListAsync();

        var commonInterests = user1Interests.Intersect(user2Interests);

        return await Context.UserInterests
            .Where(ui => ui.UserId == userId1 && ui.IsActive &&
                        commonInterests.Any(ci => ci.Category == ui.Category && ci.SubCategory == ui.SubCategory))
            .ToListAsync();
    }

    public async Task<IEnumerable<UserInterest>> GetStaleInterestsAsync(Guid userId, DateTime cutoffDate)
    {
        return await Context.UserInterests
            .Where(ui => ui.UserId == userId && ui.LastInteraction < cutoffDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserInterest>> GetByUserIdAsync(Guid userId)
    {
        return await Context.UserInterests
            .Where(ui => ui.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserInterest?> GetByUserAndInterestAsync(Guid userId, string interest)
    {
        return await Context.UserInterests
            .FirstOrDefaultAsync(ui => ui.UserId == userId && ui.InterestValue == interest);
    }

    public async Task<IEnumerable<string>> GetPopularInterestsAsync(int count = 10)
    {
        return await Context.UserInterests
            .GroupBy(ui => ui.InterestValue)
            .OrderByDescending(g => g.Count())
            .Take(count)
            .Select(g => g.Key)
            .ToListAsync();
    }

    public async Task<int> GetInterestCountAsync(string interest)
    {
        return await Context.UserInterests
            .CountAsync(ui => ui.InterestValue == interest);
    }

    public async Task RemoveUserInterestAsync(Guid userId, string interest)
    {
        var userInterest = await GetByUserAndInterestAsync(userId, interest);
        if (userInterest != null)
        {
            Context.UserInterests.Remove(userInterest);
        }
    }

    public async Task RemoveAllUserInterestsAsync(Guid userId)
    {
        var userInterests = await GetByUserIdAsync(userId);
        Context.UserInterests.RemoveRange(userInterests);
    }
}
