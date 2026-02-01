using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Features.Community.Maps.DTOs;
using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class RouteRepository : BaseRepository<Route>, IRouteRepository
{
    public RouteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Route> Items, int TotalCount)> SearchAsync(MapsRouteSearchRequest request, CancellationToken cancellationToken = default)
    {
        var query = Context.Routes.AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchTerm))
            query = query.Where(r => r.Name.Contains(request.SearchTerm));

        if (request.Type.HasValue)
            query = query.Where(r => r.Type == request.Type.Value);

        if (request.Difficulty.HasValue)
            query = query.Where(r => r.Difficulty == request.Difficulty.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IEnumerable<Route>> GetNearbyAsync(double latitude, double longitude, double radiusKm, CancellationToken cancellationToken = default)
    {
        // Simple bounding box logic for stub
        var latBound = radiusKm / 111.0;
        var lonBound = radiusKm / (111.0 * Math.Cos(latitude * Math.PI / 180.0));

        return await Context.Routes
            .Where(r => r.StartLatitude >= latitude - latBound && r.StartLatitude <= latitude + latBound &&
                       r.StartLongitude >= longitude - lonBound && r.StartLongitude <= longitude + lonBound)
            .ToListAsync(cancellationToken);
    }
}
