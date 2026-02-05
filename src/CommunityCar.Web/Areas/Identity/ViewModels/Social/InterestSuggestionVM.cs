namespace CommunityCar.Web.Areas.Identity.ViewModels.Social;

public class InterestSuggestionVM
{
    public Guid InterestId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public int PopularityScore { get; set; }
    public string RecommendationReason { get; set; } = string.Empty;
    public bool IsAdded { get; set; }
}
