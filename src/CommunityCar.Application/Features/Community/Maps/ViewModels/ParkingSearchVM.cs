using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

// Parking ViewModels (converted from DTOs)
public class ParkingSearchVM
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusKm { get; set; } = 2;
    public ParkingType? Type { get; set; }
    public bool RequiresEVCharging { get; set; }
    public string? ChargingType { get; set; }
    public decimal? MaxHourlyRate { get; set; }
    public bool Open24Hours { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    
    // Results
    public List<ParkingLocationVM> Results { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}