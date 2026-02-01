using CommunityCar.Application.Common.Models;

namespace CommunityCar.Application.Features.Community.Guides.ViewModels;

public class GuideListVM
{
    public List<GuideVM> Guides { get; set; } = new();
    public PaginationInfo Pagination { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public List<string> PopularTags { get; set; } = new();
    public int TotalCount { get; set; }
    public string? CurrentSearch { get; set; }
    public string? CurrentCategory { get; set; }
    public string? CurrentDifficulty { get; set; }
    public string CurrentSortBy { get; set; } = "newest";
}


