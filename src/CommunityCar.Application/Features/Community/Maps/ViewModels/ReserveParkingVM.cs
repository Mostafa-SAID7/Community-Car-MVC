using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

public class ReserveParkingVM
{
    public Guid UserId { get; set; }
    public Guid ParkingLocationId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool RequiresEVCharging { get; set; }
    public string? ChargingType { get; set; }
}