namespace CommunityCar.Application.Features.Shared.ViewModels;

public class CategorySearchVM
{
    public string? Query { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int? MinItemCount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "Name";
    public string SortOrder { get; set; } = "asc";
    
    // Additional search properties
    public bool? RootCategoriesOnly { get; set; }
    
    // Results
    public List<CategoryVM> Results { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}