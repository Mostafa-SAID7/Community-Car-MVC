using CommunityCar.Application.Common.Models;
using CommunityCar.Domain.Entities.Localization;

namespace CommunityCar.Application.Features.Dashboard.ViewModels.Localization;

public class ResourcesVM
{
    public List<LocalizationResource> Resources { get; set; } = new();
    public List<LocalizationCulture> Cultures { get; set; } = new();
    public List<string> ResourceGroups { get; set; } = new();
    
    public string? SelectedCulture { get; set; }
    public string? SelectedGroup { get; set; }
    public string? SearchTerm { get; set; }
    
    public PaginationInfo Pagination { get; set; } = new();
}