using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Shared.Interactions.ViewModels;

public class ShareSummaryVM
{
    public int TotalShares { get; set; }
    public Dictionary<ShareType, int> ShareTypeCounts { get; set; } = new();
    public bool HasUserShared { get; set; }
    public string ShareUrl { get; set; } = string.Empty;
}