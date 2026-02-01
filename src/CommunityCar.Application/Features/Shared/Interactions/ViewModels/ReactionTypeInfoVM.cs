using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Shared.Interactions.ViewModels;

public class ReactionTypeInfoVM
{
    public ReactionType Type { get; set; }
    public string Display { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Count { get; set; }
}