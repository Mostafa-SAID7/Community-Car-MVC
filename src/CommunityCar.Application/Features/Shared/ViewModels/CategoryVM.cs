namespace CommunityCar.Application.Features.Shared.ViewModels;

public class CategoryVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public List<CategoryVM> Children { get; set; } = new();
    public int ItemCount { get; set; }
}