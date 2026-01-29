using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Domain.Entities.Account.Profile;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Account.Social;

/// <summary>
/// Repository implementation for UserInterest entity operations
/// </summary>
public class UserInterestRepository : BaseRepository<UserInterest>, IUserInterestRepository
{
    public UserInterestRepository(ApplicationDbContext context) : base(context)
    {
    }

    #region Interest Management

    public async Task<IEnumerable<UserInterest>> GetUserInterestsAsync(Guid userId)
    {
        return await Context.UserInterests
            .Where(ui => ui.UserId == userId)
            .OrderByDescending(ui => ui.Priority)
            .ThenBy(ui => ui.InterestName)
            .ToListAsync();
    }

    public async Task<UserInterest?> GetUserInterestAsync(Guid userId, Guid interestId)
    {
        return await Context.UserInterests
            .FirstOrDefaultAsync(ui => ui.UserId == userId && ui.InterestId == interestId);
    }

    public async Task<bool> HasInterestAsync(Guid userId, Guid interestId)
    {
        return await Context.UserInterests
            .AnyAsync(ui => ui.UserId == userId && ui.InterestId == interestId);
    }

    public async Task<bool> AddInterestAsync(Guid userId, Guid interestId, int priority = 0)
    {
        var existingInterest = await GetUserInterestAsync(userId, interestId);
        if (existingInterest != null) return false; // Interest already exists

        var userInterest = UserInterest.Create(userId, interestId, priority);
        await AddAsync(userInterest);
        return true;
    }

    public async Task<bool> RemoveInterestAsync(Guid userId, Guid interestId)
    {
        var userInterest = await GetUserInterestAsync(userId, interestId);
        
        if (userInterest == null) return false;

        await DeleteAsync(userInterest);
        return true;
    }

    public async Task<bool> UpdateInterestPriorityAsync(Guid userId, Guid interestId, int priority)
    {
        var userInterest = await GetUserInterestAsync(userId, interestId);
        
        if (userInterest == null) return false;

        userInterest.UpdatePriority(priority);
        await UpdateAsync(userInterest);
        return true;
    }

    #endregion

    #region Interest Analytics

    public async Task<int> GetInterestCountAsync(Guid userId)
    {
        return await Context.UserInterests
            .Where(ui => ui.UserId == userId)
            .CountAsync();
    }

    public async Task<IEnumerable<UserInterest>> GetTopInterestsAsync(Guid userId, int count = 10)
    {
        return await Context.UserInterests
            .Where(ui => ui.UserId == userId)
            .OrderByDescending(ui => ui.Priority)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Dictionary<Guid, int>> GetInterestStatisticsAsync()
    {
        return await Context.UserInterests
            .GroupBy(ui => ui.InterestId)
            .Select(g => new { InterestId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.InterestId, x => x.Count);
    }

    public async Task<IEnumerable<Guid>> GetUsersWithInterestAsync(Guid interestId, int page = 1, int pageSize = 20)
    {
        return await Context.UserInterests
            .Where(ui => ui.InterestId == interestId)
            .OrderByDescending(ui => ui.Priority)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ui => ui.UserId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Guid>> GetUsersWithSimilarInterestsAsync(Guid userId, int count = 10)
    {
        var userInterests = await Context.UserInterests
            .Where(ui => ui.UserId == userId)
            .Select(ui => ui.InterestId)
            .ToListAsync();

        if (!userInterests.Any()) return new List<Guid>();

        return await Context.UserInterests
            .Where(ui => ui.UserId != userId && userInterests.Contains(ui.InterestId))
            .GroupBy(ui => ui.UserId)
            .OrderByDescending(g => g.Count()) // Users with most common interests
            .Take(count)
            .Select(g => g.Key)
            .ToListAsync();
    }

    #endregion

    #region Interest Categories

    public async Task<IEnumerable<UserInterest>> GetInterestsByCategoryAsync(Guid userId, string category)
    {
        return await Context.UserInterests
            .Where(ui => ui.UserId == userId && ui.Category == category)
            .OrderByDescending(ui => ui.Priority)
            .ToListAsync();
    }

    public async Task<bool> UpdateInterestCategoryAsync(Guid userId, Guid interestId, string category)
    {
        var userInterest = await GetUserInterestAsync(userId, interestId);
        
        if (userInterest == null) return false;

        userInterest.UpdateCategory(category);
        await UpdateAsync(userInterest);
        return true;
    }

    public async Task<IEnumerable<string>> GetUserInterestCategoriesAsync(Guid userId)
    {
        return await Context.UserInterests
            .Where(ui => ui.UserId == userId && !string.IsNullOrEmpty(ui.Category))
            .Select(ui => ui.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    #endregion

    #region Interest Recommendations

    public async Task<IEnumerable<Guid>> GetRecommendedInterestsAsync(Guid userId, int count = 10)
    {
        var userInterests = await Context.UserInterests
            .Where(ui => ui.UserId == userId)
            .Select(ui => ui.InterestId)
            .ToListAsync();

        // Get interests that similar users have but this user doesn't
        var similarUsers = await GetUsersWithSimilarInterestsAsync(userId, 20);
        
        var recommendedInterests = await Context.UserInterests
            .Where(ui => similarUsers.Contains(ui.UserId) && !userInterests.Contains(ui.InterestId))
            .GroupBy(ui => ui.InterestId)
            .OrderByDescending(g => g.Count())
            .Take(count)
            .Select(g => g.Key)
            .ToListAsync();

        return recommendedInterests;
    }

    public async Task<IEnumerable<Guid>> GetTrendingInterestsAsync(int count = 10)
    {
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        
        return await Context.UserInterests
            .Where(ui => ui.CreatedAt >= thirtyDaysAgo)
            .GroupBy(ui => ui.InterestId)
            .OrderByDescending(g => g.Count())
            .Take(count)
            .Select(g => g.Key)
            .ToListAsync();
    }

    public async Task<double> GetInterestSimilarityAsync(Guid userId1, Guid userId2)
    {
        var user1Interests = await Context.UserInterests
            .Where(ui => ui.UserId == userId1)
            .Select(ui => ui.InterestId)
            .ToListAsync();

        var user2Interests = await Context.UserInterests
            .Where(ui => ui.UserId == userId2)
            .Select(ui => ui.InterestId)
            .ToListAsync();

        if (!user1Interests.Any() || !user2Interests.Any()) return 0.0;

        var commonInterests = user1Interests.Intersect(user2Interests).Count();
        var totalInterests = user1Interests.Union(user2Interests).Count();

        return (double)commonInterests / totalInterests;
    }

    #endregion
}