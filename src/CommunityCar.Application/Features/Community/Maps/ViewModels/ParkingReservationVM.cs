using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

public class ParkingReservationVM
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