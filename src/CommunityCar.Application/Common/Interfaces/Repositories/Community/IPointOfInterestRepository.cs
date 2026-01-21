using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Application.Features.Maps.DTOs;
using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IPointOfInterestRepository : IBaseRepository<PointOfInterest>
{
    Task<(IEnumerable<PointOfInterest> Items, int TotalCount)> SearchAsync(
        MapsSearchRequest request, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<PointOfInterest>> GetNearbyAsync(
        double latitude, 
        double longitude, 
        double radiusKm, 
        POIType? type = null, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<PointOfInterest>> GetByTypeAsync(
        POIType type, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<PointOfInterest>> GetByCategoryAsync(
        POICategory category, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<PointOfInterest>> GetVerifiedAsync(
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<PointOfInterest>> GetReportedAsync(
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<PointOfInterest>> GetByCreatorAsync(
        Guid createdBy, 
        CancellationToken cancellationToken = default);
}