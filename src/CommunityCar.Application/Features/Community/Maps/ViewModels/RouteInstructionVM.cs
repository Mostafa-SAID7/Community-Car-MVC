using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

public class RouteInstructionVM
{
    public int Step { get; set; }
    public string Instruction { get; set; } = string.Empty;
    public double DistanceKm { get; set; }
    public int DurationMinutes { get; set; }
    public int TimeMinutes { get; set; }
    public string Direction { get; set; } = string.Empty;
    public string RoadName { get; set; } = string.Empty;
}