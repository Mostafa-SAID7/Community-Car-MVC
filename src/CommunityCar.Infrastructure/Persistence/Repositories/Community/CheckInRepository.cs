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

    public async Task<IEnumerable<CheckIn>> GetByPointOfInterestAsync(
        Guid pointOfInterestId, 
        int page = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default)
    {
        return await Context.CheckIns
            .Where(c => c.PointOfInterestId == pointOfInterestId && !c.IsPrivate)
            .OrderByDescending(c => c.CheckInTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CheckIn>> GetByUserAsync(
        Guid userId, 
        int page = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default)
    {
        return await Context.CheckIns
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CheckInTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<CheckIn?> GetUserCheckInForPOIAsync(
        Guid userId, 
        Guid pointOfInterestId, 
        CancellationToken cancellationToken = default)
    {
        return await Context.CheckIns
            .Where(c => c.UserId == userId && c.PointOfInterestId == pointOfInterestId)
            .OrderByDescending(c => c.CheckInTime)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> GetCheckInCountForPOIAsync(
        Guid pointOfInterestId, 
        CancellationToken cancellationToken = default)
    {
        return await Context.CheckIns
            .CountAsync(c => c.PointOfInterestId == pointOfInterestId, cancellationToken);
    }

    public async Task<IEnumerable<CheckIn>> GetRecentCheckInsAsync(
        int count = 10, 
        CancellationToken cancellationToken = default)
    {
        return await Context.CheckIns
            .Where(c => !c.IsPrivate)
            .OrderByDescending(c => c.CheckInTime)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
