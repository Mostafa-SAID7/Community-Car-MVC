using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

public class ParkingLocationVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public ParkingType Type { get; set; }
    public int TotalSpaces { get; set; }
    public int AvailableSpaces { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal DailyRate { get; set; }
    public bool HasEVCharging { get; set; }
    public int EVChargingSpaces { get; set; }
    public List<string> ChargingTypes { get; set; } = new();
    public decimal EVChargingRate { get; set; }
    public bool IsOpen24Hours { get; set; }
    public string OpeningHours { get; set; } = string.Empty;
    public bool RequiresReservation { get; set; }
    public List<string> Amenities { get; set; } = new();
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public double DistanceKm { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public ParkingAvailabilityStatus AvailabilityStatus { get; set; }
}

public enum ParkingAvailabilityStatus
{
    Available = 0,
    Limited = 1,
    Full = 2,
    Unknown = 3
}