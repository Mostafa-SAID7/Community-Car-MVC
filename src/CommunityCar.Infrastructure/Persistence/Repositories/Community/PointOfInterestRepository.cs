using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Features.Maps.DTOs;
using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class PointOfInterestRepository : BaseRepository<PointOfInterest>, IPointOfInterestRepository
{
    public PointOfInterestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<PointOfInterest> Items, int TotalCount)> SearchAsync(MapsSearchRequest request, CancellationToken cancellationToken = default)
    {
        var query = Context.PointsOfInterest.AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchTerm))
            query = query.Where(p => p.Name.Contains(request.SearchTerm));

        if (request.Type.HasValue)
            query = query.Where(p => p.Type == request.Type.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IEnumerable<PointOfInterest>> GetNearbyAsync(double latitude, double longitude, double radiusKm, POIType? type, CancellationToken cancellationToken = default)
    {
        // Simple bounding box logic for stub
        var latBound = radiusKm / 111.0;
        var lonBound = radiusKm / (111.0 * Math.Cos(latitude * Math.PI / 180.0));

        var query = Context.PointsOfInterest.AsQueryable();
        
        if (type.HasValue)
            query = query.Where(p => p.Type == type.Value);

        return await query
            .Where(p => p.Latitude >= latitude - latBound && p.Latitude <= latitude + latBound &&
                       p.Longitude >= longitude - lonBound && p.Longitude <= longitude + lonBound)
            .ToListAsync(cancellationToken);
    }
}
