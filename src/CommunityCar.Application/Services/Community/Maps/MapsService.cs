using AutoMapper;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Services.Community.Maps;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Core;
using CommunityCar.Application.Features.Community.Maps.ViewModels;
using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Community;

public class MapsService : IMapsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<MapsService> _logger;

    public MapsService(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        ICurrentUserService currentUserService,
        ILogger<MapsService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<PointOfInterestVM?> GetPointOfInterestByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var poi = await _unitOfWork.PointsOfInterest.GetByIdAsync(id);
            if (poi == null) 
            {
                _logger.LogWarning("Point of Interest with ID {PoiId} not found", id);
                return null;
            }

            // Increment view count
            poi.IncrementViewCount();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<PointOfInterestVM>(poi);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Point of Interest with ID {PoiId}", id);
            throw;
        }
    }

    public async Task<MapsSearchVM> SearchPointsOfInterestAsync(MapsSearchVM request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching Points of Interest with term: {SearchTerm}, Type: {Type}", 
                request.SearchTerm, request.Type);

            var (items, totalCount) = await _unitOfWork.PointsOfInterest.SearchAsync(request, cancellationToken);
            
            var summaryItems = items.Select(poi => 
            {
                var summary = _mapper.Map<PointOfInterestSummaryVM>(poi);
                
                // Calculate distance if location provided
                if (request.Latitude.HasValue && request.Longitude.HasValue)
                {
                    summary.DistanceKm = CalculateDistance(
                        request.Latitude.Value, request.Longitude.Value,
                        poi.Latitude, poi.Longitude);
                }
                
                return summary;
            });

            return new MapsSearchVM
            {
                Items = summaryItems.ToList(),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching Points of Interest");
            throw;
        }
    }

    public async Task<PointOfInterestVM> CreatePointOfInterestAsync(CreatePointOfInterestVM request, CancellationToken cancellationToken = default)
    {
        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");

        var poi = new PointOfInterest(
            request.Name,
            request.Description,
            request.Latitude,
            request.Longitude,
            request.Type,
            request.Category,
            currentUserId);

        if (!string.IsNullOrEmpty(request.NameAr) || !string.IsNullOrEmpty(request.DescriptionAr))
        {
            poi.UpdateArabicContent(request.NameAr, request.DescriptionAr, request.AddressAr, request.PricingInfoAr);
        }

        // Set additional properties
        if (!string.IsNullOrWhiteSpace(request.Address) || 
            !string.IsNullOrWhiteSpace(request.PhoneNumber) || 
            !string.IsNullOrWhiteSpace(request.Website) || 
            !string.IsNullOrWhiteSpace(request.Email))
        {
            poi.UpdateContactInfo(request.PhoneNumber, request.Website, request.Email);
        }

        if (!string.IsNullOrWhiteSpace(request.Address))
        {
            poi.UpdateLocation(request.Latitude, request.Longitude, request.Address);
        }

        if (!string.IsNullOrWhiteSpace(request.OpeningHours) || request.IsOpen24Hours)
        {
            poi.UpdateOperatingHours(request.OpeningHours, request.IsOpen24Hours);
        }

        if (request.Services?.Any() == true)
        {
            foreach (var service in request.Services)
            {
                poi.AddService(service);
            }
        }

        if (request.SupportedVehicleTypes?.Any() == true)
        {
            foreach (var vehicleType in request.SupportedVehicleTypes)
            {
                poi.AddSupportedVehicleType(vehicleType);
            }
        }

        if (request.PaymentMethods?.Any() == true)
        {
            foreach (var method in request.PaymentMethods)
            {
                poi.AddPaymentMethod(method);
            }
        }

        if (request.Amenities?.Any() == true)
        {
            foreach (var amenity in request.Amenities)
            {
                poi.AddAmenity(amenity);
            }
        }

        if (request.PriceRange.HasValue || !string.IsNullOrWhiteSpace(request.PricingInfo))
        {
            poi.UpdatePricing(request.PriceRange, request.PricingInfo);
        }

        if (request.ImageUrls?.Any() == true)
        {
            foreach (var imageUrl in request.ImageUrls)
            {
                poi.AddImage(imageUrl);
            }
        }

        if (request.EventStartTime.HasValue || request.EventEndTime.HasValue || request.MaxAttendees.HasValue)
        {
            poi.SetEventDetails(request.EventStartTime, request.EventEndTime, request.MaxAttendees);
        }

        await _unitOfWork.PointsOfInterest.AddAsync(poi);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PointOfInterestVM>(poi);
    }

    public async Task<PointOfInterestVM> UpdatePointOfInterestAsync(Guid id, UpdatePointOfInterestVM request, CancellationToken cancellationToken = default)
    {
        var poi = await _unitOfWork.PointsOfInterest.GetByIdAsync(id);
        if (poi == null) throw new ArgumentException("Point of interest not found");

        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");
        
        // Check if user can edit (owner or admin)
        if (poi.CreatedBy != currentUserId)
        {
            throw new UnauthorizedAccessException("You can only edit your own points of interest");
        }

        poi.UpdateBasicInfo(request.Name, request.Description, request.Type, request.Category);
        
        if (!string.IsNullOrEmpty(request.NameAr) || !string.IsNullOrEmpty(request.DescriptionAr))
        {
            poi.UpdateArabicContent(request.NameAr, request.DescriptionAr, request.AddressAr, request.PricingInfoAr);
        }
        poi.UpdateContactInfo(request.PhoneNumber, request.Website, request.Email);
        poi.UpdateOperatingHours(request.OpeningHours, request.IsOpen24Hours);
        poi.SetTemporarilyClosed(request.IsTemporarilyClosed);
        poi.UpdatePricing(request.PriceRange, request.PricingInfo);

        if (request.EventStartTime.HasValue || request.EventEndTime.HasValue || request.MaxAttendees.HasValue)
        {
            poi.SetEventDetails(request.EventStartTime, request.EventEndTime, request.MaxAttendees);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PointOfInterestVM>(poi);
    }

    public async Task<bool> DeletePointOfInterestAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var poi = await _unitOfWork.PointsOfInterest.GetByIdAsync(id);
        if (poi == null) return false;

        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");
        
        // Check if user can delete (owner or admin)
        if (poi.CreatedBy != currentUserId)
        {
            throw new UnauthorizedAccessException("You can only delete your own points of interest");
        }

        await _unitOfWork.PointsOfInterest.DeleteAsync(poi);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> VerifyPointOfInterestAsync(Guid id, Guid verifiedBy, CancellationToken cancellationToken = default)
    {
        var poi = await _unitOfWork.PointsOfInterest.GetByIdAsync(id);
        if (poi == null) return false;

        poi.Verify(verifiedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ReportPointOfInterestAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var poi = await _unitOfWork.PointsOfInterest.GetByIdAsync(id);
        if (poi == null) return false;

        poi.Report();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<CheckInVM> CheckInAsync(Guid pointOfInterestId, CreateCheckInVM request, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
            if (!Guid.TryParse(currentUserIdString, out var currentUserId))
                throw new UnauthorizedAccessException("Invalid user ID");

            var poi = await _unitOfWork.PointsOfInterest.GetByIdAsync(pointOfInterestId);
            if (poi == null) 
            {
                _logger.LogWarning("Attempted check-in to non-existent POI {PoiId}", pointOfInterestId);
                throw new ArgumentException("Point of interest not found");
            }

            if (!poi.AllowCheckIns)
            {
                _logger.LogWarning("Check-in attempted at POI {PoiId} where check-ins are not allowed", pointOfInterestId);
                throw new InvalidOperationException("Check-ins are not allowed for this location");
            }

            var checkIn = new CheckIn(pointOfInterestId, currentUserId, request.Comment, request.Rating, request.IsPrivate);

            if (request.CheckInLatitude.HasValue && request.CheckInLongitude.HasValue)
            {
                var distance = CalculateDistance(
                    request.CheckInLatitude.Value, request.CheckInLongitude.Value,
                    poi.Latitude, poi.Longitude);
                
                checkIn.SetLocation(request.CheckInLatitude.Value, request.CheckInLongitude.Value, distance);
            }

            await _unitOfWork.CheckIns.AddAsync(checkIn);
            
            // Update POI check-in count
            poi.IncrementCheckInCount();
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {UserId} checked in to POI {PoiId}", currentUserId, pointOfInterestId);

            return _mapper.Map<CheckInVM>(checkIn);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during check-in to POI {PoiId}", pointOfInterestId);
            throw;
        }
    }

    public async Task<IEnumerable<CheckInVM>> GetCheckInsAsync(Guid pointOfInterestId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var checkIns = await _unitOfWork.CheckIns.GetByPointOfInterestAsync(pointOfInterestId, page, pageSize, cancellationToken);
        return _mapper.Map<IEnumerable<CheckInVM>>(checkIns);
    }

    public async Task<IEnumerable<CheckInVM>> GetUserCheckInsAsync(Guid userId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var checkIns = await _unitOfWork.CheckIns.GetByUserAsync(userId, page, pageSize, cancellationToken);
        return _mapper.Map<IEnumerable<CheckInVM>>(checkIns);
    }

    public async Task<RouteVM?> GetRouteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var route = await _unitOfWork.Routes.GetByIdAsync(id);
        return route == null ? null : _mapper.Map<RouteVM>(route);
    }

    public async Task<MapsRouteSearchVM> SearchRoutesAsync(MapsRouteSearchVM request, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _unitOfWork.Routes.SearchAsync(request, cancellationToken);
        
        var summaryItems = _mapper.Map<IEnumerable<RouteSummaryVM>>(items);

        return new MapsRouteSearchVM
        {
            Items = summaryItems.ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<RouteVM> CreateRouteAsync(CreateRouteVM request, CancellationToken cancellationToken = default)
    {
        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");

        var route = new Route(request.Name, request.Description, currentUserId, request.Type, request.Difficulty);
        
        if (!string.IsNullOrEmpty(request.NameAr) || !string.IsNullOrEmpty(request.DescriptionAr))
        {
            route.UpdateArabicContent(request.NameAr, request.DescriptionAr, request.SurfaceTypeAr, request.BestTimeToVisitAr, request.SafetyNotesAr);
        }
        
        route.UpdateMetrics(request.DistanceKm, request.EstimatedDurationMinutes);
        route.UpdateCharacteristics(request.IsScenic, request.HasTolls, request.IsOffRoad, request.SurfaceType, request.BestTimeToVisit);
        
        if (!string.IsNullOrWhiteSpace(request.SafetyNotes))
        {
            route.UpdateSafetyInfo(request.SafetyNotes, null);
        }

        if (request.Tags?.Any() == true)
        {
            foreach (var tag in request.Tags)
            {
                route.AddTag(tag);
            }
        }

        if (request.ImageUrls?.Any() == true)
        {
            foreach (var imageUrl in request.ImageUrls)
            {
                route.AddImage(imageUrl);
            }
        }

        if (request.Waypoints?.Any() == true)
        {
            var waypoints = request.Waypoints.Select(w => new RouteWaypoint(w.Latitude, w.Longitude, w.Name, w.Description, w.Order));
            route.SetWaypoints(waypoints);
        }

        await _unitOfWork.Routes.AddAsync(route);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<RouteVM>(route);
    }

    public async Task<RouteVM> UpdateRouteAsync(Guid id, UpdateRouteVM request, CancellationToken cancellationToken = default)
    {
        var route = await _unitOfWork.Routes.GetByIdAsync(id);
        if (route == null) throw new ArgumentException("Route not found");

        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");
        
        if (route.CreatedBy != currentUserId)
        {
            throw new UnauthorizedAccessException("You can only edit your own routes");
        }

        route.UpdateBasicInfo(request.Name, request.Description, request.Type, request.Difficulty);
        
        if (!string.IsNullOrEmpty(request.NameAr) || !string.IsNullOrEmpty(request.DescriptionAr))
        {
            route.UpdateArabicContent(request.NameAr, request.DescriptionAr, request.SurfaceTypeAr, request.BestTimeToVisitAr, request.SafetyNotesAr, request.CurrentConditionsAr);
        }
        route.UpdateMetrics(request.DistanceKm, request.EstimatedDurationMinutes);
        route.UpdateCharacteristics(request.IsScenic, request.HasTolls, request.IsOffRoad, request.SurfaceType, request.BestTimeToVisit);
        route.UpdateSafetyInfo(request.SafetyNotes, request.CurrentConditions);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<RouteVM>(route);
    }

    public async Task<bool> DeleteRouteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var route = await _unitOfWork.Routes.GetByIdAsync(id);
        if (route == null) return false;

        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");
        
        if (route.CreatedBy != currentUserId)
        {
            throw new UnauthorizedAccessException("You can only delete your own routes");
        }

        await _unitOfWork.Routes.DeleteAsync(route);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> CompleteRouteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var route = await _unitOfWork.Routes.GetByIdAsync(id);
        if (route == null) return false;

        route.IncrementCompletionCount();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IEnumerable<PointOfInterestVM>> GetNearbyPointsOfInterestAsync(double latitude, double longitude, double radiusKm = 10, POIType? type = null, CancellationToken cancellationToken = default)
    {
        var pois = await _unitOfWork.PointsOfInterest.GetNearbyAsync(latitude, longitude, radiusKm, type, cancellationToken);
        return _mapper.Map<IEnumerable<PointOfInterestVM>>(pois);
    }

    public async Task<IEnumerable<RouteVM>> GetNearbyRoutesAsync(double latitude, double longitude, double radiusKm = 50, CancellationToken cancellationToken = default)
    {
        var routes = await _unitOfWork.Routes.GetNearbyAsync(latitude, longitude, radiusKm, cancellationToken);
        return _mapper.Map<IEnumerable<RouteVM>>(routes);
    }

    public async Task<MapsStatsVM> GetMapsStatsAsync(CancellationToken cancellationToken = default)
    {
        var allPOIs = await _unitOfWork.PointsOfInterest.GetAllAsync();
        var allRoutes = await _unitOfWork.Routes.GetAllAsync();
        var recentCheckIns = await _unitOfWork.CheckIns.GetRecentCheckInsAsync(100, cancellationToken);

        var stats = new MapsStatsVM
        {
            TotalPointsOfInterest = allPOIs.Count(),
            TotalRoutes = allRoutes.Count(),
            TotalCheckIns = recentCheckIns.Count(),
            VerifiedPOIs = allPOIs.Count(p => p.IsVerified),
            ActiveEvents = allPOIs.Count(p => p.IsEventActive()),
            POIsByType = allPOIs.GroupBy(p => p.Type).ToDictionary(g => g.Key, g => g.Count()),
            RoutesByType = allRoutes.GroupBy(r => r.Type).ToDictionary(g => g.Key, g => g.Count()),
            PopularPOIs = _mapper.Map<List<PointOfInterestSummaryVM>>(allPOIs.OrderByDescending(p => p.ViewCount).Take(5)),
            PopularRoutes = _mapper.Map<List<RouteSummaryVM>>(allRoutes.OrderByDescending(r => r.TimesCompleted).Take(5))
        };

        return stats;
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // Haversine formula for calculating distance between two points on Earth
        const double R = 6371; // Earth's radius in kilometers

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}



