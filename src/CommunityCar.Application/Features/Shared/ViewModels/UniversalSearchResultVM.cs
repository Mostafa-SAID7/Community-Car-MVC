using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Shared.ViewModels;

public class UniversalSearchResultVM
{
    public List<SearchItemVM> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public string Query { get; set; } = string.Empty;
    public TimeSpan SearchDuration { get; set; }
    public Dictionary<EntityType, int> EntityTypeCounts { get; set; } = new();
    public List<SearchFacetVM> Facets { get; set; } = new();
    public List<SearchSuggestionVM> Suggestions { get; set; } = new();
}