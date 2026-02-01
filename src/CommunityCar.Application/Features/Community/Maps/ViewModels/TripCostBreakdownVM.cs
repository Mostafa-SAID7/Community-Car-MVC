using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

// Cost breakdown ViewModel
public class TripCostBreakdownVM
{
    public decimal FuelCost { get; set; }
    public decimal TollCost { get; set; }
    public decimal ParkingCost { get; set; }
    public decimal ServiceFee { get; set; }
    public decimal TotalCost { get; set; }
    public string Currency { get; set; } = "USD";
    public List<CostItemVM> CostItems { get; set; } = new();
}