using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

public class PointOfInterestSummaryVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? Address { get; set; }
    public string? AddressAr { get; set; }
    public POIType Type { get; set; }
    public POICategory Category { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int ViewCount { get; set; }
    public int CheckInCount { get; set; }
    public bool IsVerified { get; set; }
    public bool IsOpen24Hours { get; set; }
    public bool IsTemporarilyClosed { get; set; }
    public List<string> Services { get; set; } = new();
    public string? ImageUrl { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public double? DistanceKm { get; set; }
}