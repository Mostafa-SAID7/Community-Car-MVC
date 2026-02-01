using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

public class UpdatePointOfInterestVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public POIType Type { get; set; }
    public POICategory Category { get; set; }
    public string? Address { get; set; }
    public string? AddressAr { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    public string? OpeningHours { get; set; }
    public bool IsOpen24Hours { get; set; }
    public bool IsTemporarilyClosed { get; set; }
    public List<string> Services { get; set; } = new();
    public List<string> SupportedVehicleTypes { get; set; } = new();
    public List<string> PaymentMethods { get; set; } = new();
    public List<string> Amenities { get; set; } = new();
    public decimal? PriceRange { get; set; }
    public string? PricingInfo { get; set; }
    public string? PricingInfoAr { get; set; }
    public bool AllowCheckIns { get; set; }
    public bool AllowReviews { get; set; }
    public bool IsPublic { get; set; }
    public DateTime? EventStartTime { get; set; }
    public DateTime? EventEndTime { get; set; }
    public int? MaxAttendees { get; set; }
}