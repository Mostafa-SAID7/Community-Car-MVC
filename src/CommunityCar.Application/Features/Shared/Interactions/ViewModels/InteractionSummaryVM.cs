namespace CommunityCar.Application.Features.Shared.Interactions.ViewModels;

public class InteractionSummaryVM
{
    public ReactionSummaryVM Reactions { get; set; } = new();
    public int CommentCount { get; set; }
    public ShareSummaryVM Shares { get; set; } = new();
    public bool CanComment { get; set; }
    public bool CanShare { get; set; }
    public bool CanReact { get; set; }
}