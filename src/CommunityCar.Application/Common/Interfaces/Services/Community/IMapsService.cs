using CommunityCar.Application.Features.Community.Maps.DTOs;
using CommunityCar.Application.Features.Community.Maps.ViewModels;
using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Common.Interfaces.Services.Community;

public interface IMapsService
{
    // Point of Interest operations
    Task<PointOfInterestVM?> GetPointOfInterestByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MapsSearchResponse> SearchPointsOfInterestAsync(MapsSearchRequest request, CancellationToken cancellationToken = default);
    Task<PointOfInterestVM> CreatePointOfInterestAsync(CreatePointOfInterestRequest request, CancellationToken cancellationToken = default);
    Task<PointOfInterestVM> UpdatePointOfInterestAsync(Guid id, UpdatePointOfInterestRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeletePointOfInterestAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> VerifyPointOfInterestAsync(Guid id, Guid verifiedBy, CancellationToken cancellationToken = default);
    Task<bool> ReportPointOfInterestAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Check-in operations
    Task<CheckInVM> CheckInAsync(Guid pointOfInterestId, CreateCheckInRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<CheckInVM>> GetCheckInsAsync(Guid pointOfInterestId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<IEnumerable<CheckInVM>> GetUserCheckInsAsync(Guid userId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    
    // Route operations
    Task<RouteVM?> GetRouteByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MapsRouteSearchResponse> SearchRoutesAsync(MapsRouteSearchRequest request, CancellationToken cancellationToken = default);
    Task<RouteVM> CreateRouteAsync(CreateRouteRequest request, CancellationToken cancellationToken = default);
    Task<RouteVM> UpdateRouteAsync(Guid id, UpdateRouteRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteRouteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> CompleteRouteAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Location-based operations
    Task<IEnumerable<PointOfInterestVM>> GetNearbyPointsOfInterestAsync(double latitude, double longitude, double radiusKm = 10, POIType? type = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteVM>> GetNearbyRoutesAsync(double latitude, double longitude, double radiusKm = 50, CancellationToken cancellationToken = default);
    
    // Statistics
    Task<MapsStatsVM> GetMapsStatsAsync(CancellationToken cancellationToken = default);
}


