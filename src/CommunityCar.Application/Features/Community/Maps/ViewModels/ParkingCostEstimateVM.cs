using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

public class ParkingCostEstimateVM
{
    public decimal HourlyCost { get; set; }
    public decimal DailyCost { get; set; }
    public decimal TotalCost { get; set; }
    public decimal EVChargingCost { get; set; }
    public int TotalHours { get; set; }
    public string Currency { get; set; } = "USD";
    public List<string> CostFactors { get; set; } = new();
}