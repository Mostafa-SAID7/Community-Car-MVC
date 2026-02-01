namespace CommunityCar.Application.Features.Shared.ViewModels;

public class TagSearchVM
{
    public string? Query { get; set; }
    public int? MinUsageCount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "UsageCount";
    public string SortOrder { get; set; } = "desc";
    
    // Additional search properties
    public bool? PopularOnly { get; set; }
    public string? StartsWith { get; set; }
    public int? MaxUsageCount { get; set; }
    
    // Results
    public List<TagVM> Results { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}