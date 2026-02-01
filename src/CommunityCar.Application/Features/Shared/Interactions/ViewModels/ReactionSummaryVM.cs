using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Shared.Interactions.ViewModels;

public class ReactionSummaryVM
{
    public Dictionary<ReactionType, int> ReactionCounts { get; set; } = new();
    public int TotalReactions { get; set; }
    public ReactionType? UserReaction { get; set; }
    public bool HasUserReacted { get; set; }
    public List<ReactionTypeInfoVM> AvailableReactions { get; set; } = new();
}