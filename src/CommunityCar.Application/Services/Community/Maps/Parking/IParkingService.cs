using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Services.Maps.Parking;

public interface IParkingService
{
    Task<List<ParkingLocationDto>> GetNearbyParkingAsync(double latitude, double longitude, double radiusKm = 2);
    Task<List<ParkingLocationDto>> SearchParkingAsync(ParkingSearchRequest request);
    Task<ParkingLocationDto?> GetParkingLocationAsync(Guid id);
    Task<ParkingReservationDto> ReserveParkingAsync(ReserveParkingRequest request);
    Task<bool> CancelReservationAsync(Guid reservationId, Guid userId);
    Task<List<ParkingLocationDto>> GetEVChargingStationsAsync(double latitude, double longitude, double radiusKm = 10);
    Task<ParkingCostEstimateDto> EstimateParkingCostAsync(Guid parkingLocationId, DateTime startTime, DateTime endTime);
    Task UpdateParkingAvailabilityAsync();
}

public class ParkingSearchRequest
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
}

public class ReserveParkingRequest
{
    public Guid UserId { get; set; }
    public Guid ParkingLocationId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool RequiresEVCharging { get; set; }
    public string? ChargingType { get; set; }
}

public class ParkingLocationDto
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

public class ParkingReservationDto
{
    public Guid Id { get; set; }
    public Guid ParkingLocationId { get; set; }
    public string ParkingLocationName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? SpaceNumber { get; set; }
    public decimal TotalCost { get; set; }
    public ParkingReservationStatus Status { get; set; }
    public bool RequiresEVCharging { get; set; }
    public string? ChargingType { get; set; }
    public DateTime ReservedAt { get; set; }
    public string ReservationCode { get; set; } = string.Empty;
}

public class ParkingCostEstimateDto
{
    public decimal HourlyCost { get; set; }
    public decimal DailyCost { get; set; }
    public decimal TotalCost { get; set; }
    public decimal EVChargingCost { get; set; }
    public int TotalHours { get; set; }
    public string Currency { get; set; } = "USD";
    public List<string> CostFactors { get; set; } = new();
}

public enum ParkingAvailabilityStatus
{
    Available = 0,
    Limited = 1,
    Full = 2,
    Unknown = 3
}