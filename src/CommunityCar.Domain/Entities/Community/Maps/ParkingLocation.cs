using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Domain.Entities.Community.Maps;

public class ParkingLocation : BaseEntity
{
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
    public List<string> ChargingTypes { get; set; } = new(); // Type2, CCS, CHAdeMO
    public decimal EVChargingRate { get; set; }
    public bool IsOpen24Hours { get; set; }
    public string OpeningHours { get; set; } = string.Empty;
    public bool RequiresReservation { get; set; }
    public string? ReservationUrl { get; set; }
    public string? ContactPhone { get; set; }
    public List<string> Amenities { get; set; } = new(); // Security, Covered, Valet, etc.
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool IsVerified { get; set; }
    public string? ImageUrl { get; set; }
    
    // Navigation properties
    public List<ParkingReservation> Reservations { get; set; } = new();
    public List<ParkingReview> Reviews { get; set; } = new();
}

public class ParkingReservation : BaseEntity
{
    public Guid ParkingLocationId { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? SpaceNumber { get; set; }
    public decimal TotalCost { get; set; }
    public ParkingReservationStatus Status { get; set; }
    public bool RequiresEVCharging { get; set; }
    public string? ChargingType { get; set; }
    public DateTime ReservedAt { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    // Navigation property
    public ParkingLocation ParkingLocation { get; set; } = null!;
}

public class ParkingReview : BaseEntity
{
    public Guid ParkingLocationId { get; set; }
    public Guid UserId { get; set; }
    public int Rating { get; set; } // 1-5
    public string? Comment { get; set; }
    public List<string> Tags { get; set; } = new(); // Clean, Safe, Easy Access, etc.
    public DateTime ReviewedAt { get; set; }
    public bool IsVerified { get; set; } // User actually parked there
    
    // Navigation property
    public ParkingLocation ParkingLocation { get; set; } = null!;
}