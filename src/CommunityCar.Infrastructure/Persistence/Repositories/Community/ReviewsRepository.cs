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

    public async Task<int> GetCountByUserAndDateAsync(Guid userId, DateTime date)
    {
        return await Context.Reviews
            .CountAsync(r => r.ReviewerId == userId && r.CreatedAt.Date == date.Date);
    }

    public async Task<double> GetAverageRatingByTargetAsync(Guid targetId, string targetType)
    {
        var ratings = await Context.Reviews
            .Where(r => r.TargetId == targetId && r.TargetType == targetType)
            .Select(r => r.Rating)
            .ToListAsync();

        return ratings.Any() ? ratings.Average() : 0;
    }

    public async Task<IEnumerable<Review>> GetByTargetAsync(Guid targetId, string targetType)
    {
        return await Context.Reviews
            .Where(r => r.TargetId == targetId && r.TargetType == targetType)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetAvailableCarMakesAsync()
    {
        return await Context.Reviews
            .Select(r => r.CarMake)
            .Where(m => !string.IsNullOrEmpty(m))
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetApprovedAsync()
    {
        return await Context.Reviews
            .Where(r => r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
    public async Task<IEnumerable<Review>> GetTopApprovedAsync(int count)
    {
        return await Context.Reviews
            .Where(r => r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
}
