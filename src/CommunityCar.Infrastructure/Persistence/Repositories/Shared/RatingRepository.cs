using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Shared;

public class RatingRepository : BaseRepository<Rating>, IRatingRepository
{
    public RatingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Rating?> GetUserRatingAsync(Guid entityId, EntityType entityType, Guid userId)
    {
        return await DbSet.FirstOrDefaultAsync(r => 
            r.EntityId == entityId && 
            r.EntityType == entityType && 
            r.UserId == userId);
    }

    public async Task<List<Rating>> GetEntityRatingsAsync(Guid entityId, EntityType entityType)
    {
        return await DbSet.Where(r => 
            r.EntityId == entityId && 
            r.EntityType == entityType)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<double> GetAverageRatingAsync(Guid entityId, EntityType entityType)
    {
        var ratings = await DbSet.Where(r => 
            r.EntityId == entityId && 
            r.EntityType == entityType)
            .Select(r => r.Value)
            .ToListAsync();

        if (!ratings.Any())
            return 0;

        return ratings.Average();
    }

    public async Task<Dictionary<int, int>> GetRatingDistributionAsync(Guid entityId, EntityType entityType)
    {
        var ratings = await DbSet.Where(r => 
            r.EntityId == entityId && 
            r.EntityType == entityType)
            .GroupBy(r => r.Value)
            .Select(g => new { Value = g.Key, Count = g.Count() })
            .ToListAsync();

        return ratings.ToDictionary(r => r.Value, r => r.Count);
    }
}
