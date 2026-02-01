namespace CommunityCar.Application.Features.Shared.Interactions.ViewModels;

public class ReactionResultVM
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ReactionSummaryVM? Summary { get; set; }
}