using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Application.Features.Community.Maps.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IRouteRepository : IBaseRepository<Route>
{
    Task<(IEnumerable<Route> Items, int TotalCount)> SearchAsync(MapsRouteSearchVM request, CancellationToken cancellationToken = default);
    Task<IEnumerable<Route>> GetNearbyAsync(double latitude, double longitude, double radiusKm, CancellationToken cancellationToken = default);
}
