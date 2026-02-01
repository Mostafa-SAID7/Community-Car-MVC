using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Application.Features.Community.Maps.DTOs;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IRouteRepository : IBaseRepository<Route>
{
    Task<(IEnumerable<Route> Items, int TotalCount)> SearchAsync(MapsRouteSearchRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<Route>> GetNearbyAsync(double latitude, double longitude, double radiusKm, CancellationToken cancellationToken = default);
}
