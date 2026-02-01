using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Shared.ViewModels;

public class RelatedContentVM
{
    public Guid Id { get; set; }
    public EntityType EntityType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public double SimilarityScore { get; set; }
    public List<string> CommonTags { get; set; } = new();
    public string RelationType { get; set; } = string.Empty; // "Similar", "Related", "Recommended"
}