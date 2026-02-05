using CommunityCar.Application.Common.Models;

namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Media.Localization.ViewModels;

public class ResourcesVM
{
    public List<LocalizationResourceVM> Resources { get; set; } = new();
    public List<string> Cultures { get; set; } = new();
    public List<string> ResourceGroups { get; set; } = new();
    public string? SelectedCulture { get; set; }
    public string? SelectedGroup { get; set; }
    public string? SearchTerm { get; set; }
    public PaginationInfo Pagination { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}




