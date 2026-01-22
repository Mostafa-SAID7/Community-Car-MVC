using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Stories.ViewModels;

namespace CommunityCar.Application.Features.Stories.DTOs;

public class StoriesSearchResponse
{
    public IEnumerable<StoryVM> Stories { get; set; } = new List<StoryVM>();
    public PaginationInfo Pagination { get; set; } = new PaginationInfo();
    public StoriesStatsVM Stats { get; set; } = new StoriesStatsVM();
    public IEnumerable<string> AvailableTags { get; set; } = new List<string>();
    public IEnumerable<string> AvailableCarMakes { get; set; } = new List<string>();
}