namespace CommunityCar.Application.Features.Shared.ViewModels;

public class SearchFacetVM
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<SearchFacetValueVM> Values { get; set; } = new();
}