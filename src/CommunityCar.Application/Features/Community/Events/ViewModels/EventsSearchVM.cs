namespace CommunityCar.Application.Features.Community.Events.ViewModels;

public class EventsSearchVM
{
    public string? Query { get; set; }
    public string? SearchTerm { get; set; }
    public string? Location { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool? IsFree { get; set; }
    public bool? IsFreeOnly { get; set; }
    public bool? IsPublic { get; set; }
    public bool? IsUpcomingOnly { get; set; }
    public bool? HasAvailableSpots { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "StartTime";
    public string SortOrder { get; set; } = "asc";
    
    // Location search properties
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    
    // Results
    public List<EventVM> Results { get; set; } = new();
    public List<EventSummaryVM> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}