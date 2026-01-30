using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class CheckInRepository : BaseRepository<CheckIn>, ICheckInRepository
{
    public CheckInRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<CheckIn>> GetByPointOfInterestAsync(Guid pointOfInterestId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await Context.CheckIns
            .Where(c => c.PointOfInterestId == pointOfInterestId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CheckIn>> GetByUserAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await Context.CheckIns
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CheckIn>> GetRecentCheckInsAsync(int count, CancellationToken cancellationToken = default)
    {
        return await Context.CheckIns
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
