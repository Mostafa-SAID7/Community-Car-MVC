using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;
using CommunityCar.Application.Features.Community.Maps.DTOs;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

public class PointOfInterestVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? DescriptionAr { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public POIType Type { get; set; }
    public POICategory Category { get; set; }
    public Guid CreatedBy { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    
    // Contact and business information
    public string? Address { get; set; }
    public string? AddressAr { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    
    // Operating hours and availability
    public string? OpeningHours { get; set; }
    public bool IsOpen24Hours { get; set; }
    public bool IsTemporarilyClosed { get; set; }
    
    // Ratings and reviews
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int ViewCount { get; set; }
    public int CheckInCount { get; set; }
    
    // Automotive specific features
    public List<string> Services { get; set; } = new();
    public List<string> SupportedVehicleTypes { get; set; } = new();
    public List<string> PaymentMethods { get; set; } = new();
    public List<string> Amenities { get; set; } = new();
    
    // Pricing information
    public decimal? PriceRange { get; set; }
    public string? PricingInfo { get; set; }
    public string? PricingInfoAr { get; set; }
    
    // Verification and quality
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerifiedByName { get; set; }
    public bool IsReported { get; set; }
    public int ReportCount { get; set; }
    
    // Social features
    public bool AllowCheckIns { get; set; }
    public bool AllowReviews { get; set; }
    public bool IsPublic { get; set; }
    
    // Event-specific properties
    public DateTime? EventStartTime { get; set; }
    public DateTime? EventEndTime { get; set; }
    public int? MaxAttendees { get; set; }
    public int CurrentAttendees { get; set; }
    
    // Images and media
    public List<string> ImageUrls { get; set; } = new();
    
    // Reviews
    public List<CommunityCar.Application.Features.Community.Reviews.ViewModels.ReviewVM> Reviews { get; set; } = new();
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Helper properties
    public bool IsEventActive => IsEventActiveHelper();
    public bool IsEventUpcoming => IsEventUpcomingHelper();
    public bool HasAvailableSpots => HasAvailableSpotsHelper();
    
    private bool IsEventActiveHelper()
    {
        if (!EventStartTime.HasValue || !EventEndTime.HasValue)
            return false;

        var now = DateTime.UtcNow;
        return now >= EventStartTime.Value && now <= EventEndTime.Value;
    }
    
    private bool IsEventUpcomingHelper()
    {
        if (!EventStartTime.HasValue)
            return false;

        return DateTime.UtcNow < EventStartTime.Value;
    }
    
    private bool HasAvailableSpotsHelper()
    {
        if (!MaxAttendees.HasValue)
            return true;

        return CurrentAttendees < MaxAttendees.Value;
    }
}

public class CheckInVM
{
    public Guid Id { get; set; }
    public Guid PointOfInterestId { get; set; }
    public string PointOfInterestName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime CheckInTime { get; set; }
    public string? Comment { get; set; }
    public double? Rating { get; set; }
    public bool IsPrivate { get; set; }
    public double? CheckInLatitude { get; set; }
    public double? CheckInLongitude { get; set; }
    public double? DistanceFromPOI { get; set; }
}

public class RouteVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? DescriptionAr { get; set; }
    public Guid CreatedBy { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public RouteType Type { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    
    // Route metrics
    public double DistanceKm { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int TimesCompleted { get; set; }
    
    // Route characteristics
    public bool IsScenic { get; set; }
    public bool HasTolls { get; set; }
    public bool IsOffRoad { get; set; }
    public string? SurfaceType { get; set; }
    public string? SurfaceTypeAr { get; set; }
    public string? BestTimeToVisit { get; set; }
    public string? BestTimeToVisitAr { get; set; }
    
    // Safety and conditions
    public string? SafetyNotes { get; set; }
    public string? SafetyNotesAr { get; set; }
    public string? CurrentConditions { get; set; }
    public string? CurrentConditionsAr { get; set; }
    public DateTime? LastConditionUpdate { get; set; }
    
    // Waypoints and path
    public List<RouteWaypointVM> Waypoints { get; set; } = new();
    
    // Tags and categories
    public List<string> Tags { get; set; } = new();
    
    // Images
    public List<string> ImageUrls { get; set; } = new();
    
    // Reviews
    public List<CommunityCar.Application.Features.Community.Reviews.ViewModels.ReviewVM> Reviews { get; set; } = new();
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class RouteWaypointVM
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Name { get; set; }
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public int Order { get; set; }
}

public class MapsStatsVM
{
    public int TotalPointsOfInterest { get; set; }
    public int TotalRoutes { get; set; }
    public int TotalCheckIns { get; set; }
    public int VerifiedPOIs { get; set; }
    public int ActiveEvents { get; set; }
    public Dictionary<POIType, int> POIsByType { get; set; } = new();
    public Dictionary<RouteType, int> RoutesByType { get; set; } = new();
    public List<PointOfInterestSummaryVM> PopularPOIs { get; set; } = new();
    public List<RouteSummaryVM> PopularRoutes { get; set; } = new();
}


