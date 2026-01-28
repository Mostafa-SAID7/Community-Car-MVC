using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Domain.Entities.Community.Reviews;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class ReviewsRepository : BaseRepository<Review>, IReviewsRepository
{
    public ReviewsRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Review>> GetByTargetAsync(Guid targetId, string targetType)
    {
        return await Context.Set<Review>()
            .Where(r => r.TargetId == targetId && r.TargetType == targetType)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetByReviewerAsync(Guid reviewerId)
    {
        return await Context.Set<Review>()
            .Where(r => r.ReviewerId == reviewerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetApprovedAsync()
    {
        return await Context.Set<Review>()
            .Where(r => r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetFlaggedAsync()
    {
        return await Context.Set<Review>()
            .Where(r => r.IsFlagged)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetVerifiedPurchasesAsync()
    {
        return await Context.Set<Review>()
            .Where(r => r.IsVerifiedPurchase)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetByCarMakeAsync(string carMake)
    {
        return await Context.Set<Review>()
            .Where(r => r.CarMake != null && r.CarMake.ToLower() == carMake.ToLower())
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetByRatingAsync(int rating)
    {
        return await Context.Set<Review>()
            .Where(r => r.Rating == rating)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetRecentAsync(int count)
    {
        return await Context.Set<Review>()
            .Where(r => r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetAvailableCarMakesAsync()
    {
        return await Context.Set<Review>()
            .Where(r => r.CarMake != null)
            .Select(r => r.CarMake!)
            .Distinct()
            .OrderBy(m => m)
            .ToListAsync();
    }

    public async Task<double> GetAverageRatingByTargetAsync(Guid targetId, string targetType)
    {
        var reviews = await Context.Set<Review>()
            .Where(r => r.TargetId == targetId && r.TargetType == targetType && r.IsApproved)
            .ToListAsync();

        return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
    }

    public async Task<int> GetReviewCountByTargetAsync(Guid targetId, string targetType)
    {
        return await Context.Set<Review>()
            .CountAsync(r => r.TargetId == targetId && r.TargetType == targetType && r.IsApproved);
    }

    public async Task<bool> HasUserReviewedTargetAsync(Guid userId, Guid targetId, string targetType)
    {
        return await Context.Set<Review>()
            .AnyAsync(r => r.ReviewerId == userId && r.TargetId == targetId && r.TargetType == targetType);
    }

    public async Task<int> GetCountByUserAndDateAsync(Guid userId, DateTime date)
    {
        return await Context.Set<Review>()
            .CountAsync(r => r.ReviewerId == userId && r.CreatedAt.Date == date.Date);
    }
}

