using CommunityCar.Domain.Enums.Community;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Community.Stories.ViewModels;

public class StoriesSearchVM
{
    public string? Query { get; set; }
    public string? SearchTerm { get; set; }
    public StoryType? Type { get; set; }
    public StoryVisibility? Visibility { get; set; }
    public string? Location { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool? IsExpired { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "CreatedAt";
    public string SortOrder { get; set; } = "desc";
    
    // Additional search properties needed by services
    public Guid? AuthorId { get; set; }
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public string? EventType { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsArchived { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsHighlighted { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public DateTime? ExpiresAfter { get; set; }
    public DateTime? ExpiresBefore { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? RadiusKm { get; set; }
    public int? MinViews { get; set; }
    public int? MaxViews { get; set; }
    public int? MinLikes { get; set; }
    public int? MaxLikes { get; set; }
    
    // Results - Properties expected by services
    public List<StoryVM> Results { get; set; } = new();
    public List<StoryVM> Stories { get; set; } = new();
    public PaginationVM Pagination { get; set; } = new();
    public StoriesStatsVM Stats { get; set; } = new();
    public List<string> AvailableTags { get; set; } = new();
    public List<string> AvailableCarMakes { get; set; } = new();
    
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
    public int CurrentPage => Page;
}