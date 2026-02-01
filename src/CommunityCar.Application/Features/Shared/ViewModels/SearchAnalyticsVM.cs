using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Shared.ViewModels;

public class SearchAnalyticsVM
{
    public int TotalSearches { get; set; }
    public List<PopularQueryVM> PopularQueries { get; set; } = new();
    public Dictionary<EntityType, int> SearchesByEntityType { get; set; } = new();
    public Dictionary<string, int> SearchesByTimeOfDay { get; set; } = new();
    public double AverageResultsPerSearch { get; set; }
    public double AverageSearchDuration { get; set; }
}