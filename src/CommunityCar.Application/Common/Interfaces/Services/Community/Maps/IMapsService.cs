using CommunityCar.Application.Features.Community.Maps.ViewModels;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Common.Interfaces.Services.Community.Maps;

public interface IMapsService
{
    // Points of Interest
    Task<PointOfInterestVM?> GetPointOfInterestByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MapsSearchVM> SearchPointsOfInterestAsync(MapsSearchVM request, CancellationToken cancellationToken = default);
    Task<PointOfInterestVM> CreatePointOfInterestAsync(CreatePointOfInterestVM request, CancellationToken cancellationToken = default);
    Task<PointOfInterestVM> UpdatePointOfInterestAsync(Guid id, UpdatePointOfInterestVM request, CancellationToken cancellationToken = default);
    Task<bool> DeletePointOfInterestAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> VerifyPointOfInterestAsync(Guid id, Guid verifiedBy, CancellationToken cancellationToken = default);
    Task<bool> ReportPointOfInterestAsync(Guid id, CancellationToken cancellationToken = default);

    // Check-ins
    Task<CheckInVM> CheckInAsync(Guid pointOfInterestId, CreateCheckInVM request, CancellationToken cancellationToken = default);
    Task<IEnumerable<CheckInVM>> GetCheckInsAsync(Guid pointOfInterestId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<IEnumerable<CheckInVM>> GetUserCheckInsAsync(Guid userId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    // Routes
    Task<RouteVM?> GetRouteByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MapsRouteSearchVM> SearchRoutesAsync(MapsRouteSearchVM request, CancellationToken cancellationToken = default);
    Task<RouteVM> CreateRouteAsync(CreateRouteVM request, CancellationToken cancellationToken = default);
    Task<RouteVM> UpdateRouteAsync(Guid id, UpdateRouteVM request, CancellationToken cancellationToken = default);
    Task<bool> DeleteRouteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> CompleteRouteAsync(Guid id, CancellationToken cancellationToken = default);

    // Nearby
    Task<IEnumerable<PointOfInterestVM>> GetNearbyPointsOfInterestAsync(double latitude, double longitude, double radiusKm = 10, POIType? type = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteVM>> GetNearbyRoutesAsync(double latitude, double longitude, double radiusKm = 50, CancellationToken cancellationToken = default);

    // Statistics
    Task<MapsStatsVM> GetMapsStatsAsync(CancellationToken cancellationToken = default);
}