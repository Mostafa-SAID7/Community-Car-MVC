using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Shared.ViewModels;

public class UniversalSearchVM
{
    public string? Query { get; set; }
    public List<EntityType> EntityTypes { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "Relevance";
    public string SortOrder { get; set; } = "desc";
    
    // Additional search properties
    public Guid? UserId { get; set; }
    
    // Results
    public List<SearchItemVM> Results { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}