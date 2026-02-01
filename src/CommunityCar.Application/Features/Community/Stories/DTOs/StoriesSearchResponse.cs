using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Community.Stories.ViewModels;

namespace CommunityCar.Application.Features.Community.Stories.DTOs;

public class StoriesSearchResponse
{
    public IEnumerable<StoryVM> Stories { get; set; } = new List<StoryVM>();
    public PaginationInfo Pagination { get; set; } = new PaginationInfo();
    public StoriesStatsVM Stats { get; set; } = new StoriesStatsVM();
    public IEnumerable<string> AvailableTags { get; set; } = new List<string>();
    public IEnumerable<string> AvailableCarMakes { get; set; } = new List<string>();
    
    // Calculated properties
    public int CurrentPage => Pagination.CurrentPage;
    public int TotalPages => Pagination.TotalPages;
    public int TotalItems => Pagination.TotalItems;
    public bool HasNextPage => Pagination.HasNextPage;
    public bool HasPreviousPage => Pagination.HasPreviousPage;
}


