using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Application.Features.Maps.DTOs;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IPointOfInterestRepository : IBaseRepository<PointOfInterest>
{
    Task<(IEnumerable<PointOfInterest> Items, int TotalCount)> SearchAsync(MapsSearchRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<PointOfInterest>> GetNearbyAsync(double latitude, double longitude, double radiusKm, POIType? type, CancellationToken cancellationToken = default);
}
