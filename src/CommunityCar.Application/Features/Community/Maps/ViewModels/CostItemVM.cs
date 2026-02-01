using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

public class CostItemVM
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}