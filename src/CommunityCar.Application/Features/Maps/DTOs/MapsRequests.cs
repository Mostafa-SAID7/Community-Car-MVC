using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Maps.DTOs;

public class CreatePointOfInterestRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Range(-90, 90)]
    public double Latitude { get; set; }
    
    [Required]
    [Range(-180, 180)]
    public double Longitude { get; set; }
    
    [Required]
    public POIType Type { get; set; }
    
    [Required]
    public POICategory Category { get; set; }
    
    [StringLength(200)]
    public string? NameAr { get; set; }
    
    [StringLength(1000)]
    public string? DescriptionAr { get; set; }
    
    [StringLength(500)]
    public string? Address { get; set; }
    
    [StringLength(500)]
    public string? AddressAr { get; set; }
    
    [Phone]
    public string? PhoneNumber { get; set; }
    
    [Url]
    public string? Website { get; set; }
    
    [EmailAddress]
    public string? Email { get; set; }
    
    public string? OpeningHours { get; set; }
    public bool IsOpen24Hours { get; set; }
    public List<string>? Services { get; set; }
    public List<string>? SupportedVehicleTypes { get; set; }
    public List<string>? PaymentMethods { get; set; }
    public List<string>? Amenities { get; set; }
    public decimal? PriceRange { get; set; }
    public string? PricingInfo { get; set; }
    public string? PricingInfoAr { get; set; }
    public List<string>? ImageUrls { get; set; }
    
    // Event-specific
    public DateTime? EventStartTime { get; set; }
    public DateTime? EventEndTime { get; set; }
    public int? MaxAttendees { get; set; }
}

public class UpdatePointOfInterestRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public POIType Type { get; set; }
    
    [Required]
    public POICategory Category { get; set; }
    
    [StringLength(200)]
    public string? NameAr { get; set; }
    
    [StringLength(1000)]
    public string? DescriptionAr { get; set; }
    
    [StringLength(500)]
    public string? Address { get; set; }
    
    [StringLength(500)]
    public string? AddressAr { get; set; }
    
    [Phone]
    public string? PhoneNumber { get; set; }
    
    [Url]
    public string? Website { get; set; }
    
    [EmailAddress]
    public string? Email { get; set; }
    
    public string? OpeningHours { get; set; }
    public bool IsOpen24Hours { get; set; }
    public bool IsTemporarilyClosed { get; set; }
    public List<string>? Services { get; set; }
    public List<string>? SupportedVehicleTypes { get; set; }
    public List<string>? PaymentMethods { get; set; }
    public List<string>? Amenities { get; set; }
    public decimal? PriceRange { get; set; }
    public string? PricingInfo { get; set; }
    public string? PricingInfoAr { get; set; }
    public List<string>? ImageUrls { get; set; }
    
    // Event-specific
    public DateTime? EventStartTime { get; set; }
    public DateTime? EventEndTime { get; set; }
    public int? MaxAttendees { get; set; }
}

public class CreateCheckInRequest
{
    [StringLength(500)]
    public string? Comment { get; set; }
    
    [Range(1, 5)]
    public double? Rating { get; set; }
    
    public bool IsPrivate { get; set; }
    
    [Range(-90, 90)]
    public double? CheckInLatitude { get; set; }
    
    [Range(-180, 180)]
    public double? CheckInLongitude { get; set; }
}

public class CreateRouteRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [StringLength(200)]
    public string? NameAr { get; set; }
    
    [StringLength(1000)]
    public string? DescriptionAr { get; set; }
    
    [Required]
    public RouteType Type { get; set; }
    
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Beginner;
    
    [Range(0, double.MaxValue)]
    public double DistanceKm { get; set; }
    
    [Range(0, int.MaxValue)]
    public int EstimatedDurationMinutes { get; set; }
    
    public bool IsScenic { get; set; }
    public bool HasTolls { get; set; }
    public bool IsOffRoad { get; set; }
    public string? SurfaceType { get; set; }
    public string? SurfaceTypeAr { get; set; }
    public string? BestTimeToVisit { get; set; }
    public string? BestTimeToVisitAr { get; set; }
    public string? SafetyNotes { get; set; }
    public string? SafetyNotesAr { get; set; }
    public List<string>? Tags { get; set; }
    public List<string>? ImageUrls { get; set; }
    public List<CreateRouteWaypointRequest>? Waypoints { get; set; }
}

public class UpdateRouteRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [StringLength(200)]
    public string? NameAr { get; set; }
    
    [StringLength(1000)]
    public string? DescriptionAr { get; set; }
    
    [Required]
    public RouteType Type { get; set; }
    
    public DifficultyLevel Difficulty { get; set; }
    
    [Range(0, double.MaxValue)]
    public double DistanceKm { get; set; }
    
    [Range(0, int.MaxValue)]
    public int EstimatedDurationMinutes { get; set; }
    
    public bool IsScenic { get; set; }
    public bool HasTolls { get; set; }
    public bool IsOffRoad { get; set; }
    public string? SurfaceType { get; set; }
    public string? SurfaceTypeAr { get; set; }
    public string? BestTimeToVisit { get; set; }
    public string? BestTimeToVisitAr { get; set; }
    public string? SafetyNotes { get; set; }
    public string? SafetyNotesAr { get; set; }
    public string? CurrentConditions { get; set; }
    public string? CurrentConditionsAr { get; set; }
    public List<string>? Tags { get; set; }
    public List<string>? ImageUrls { get; set; }
    public List<CreateRouteWaypointRequest>? Waypoints { get; set; }
}

public class CreateRouteWaypointRequest
{
    [Required]
    [Range(-90, 90)]
    public double Latitude { get; set; }
    
    [Required]
    [Range(-180, 180)]
    public double Longitude { get; set; }
    
    [StringLength(200)]
    public string? Name { get; set; }
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [StringLength(200)]
    public string? NameAr { get; set; }
    
    [StringLength(500)]
    public string? DescriptionAr { get; set; }
    
    public int Order { get; set; }
}


