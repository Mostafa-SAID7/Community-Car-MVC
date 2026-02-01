namespace CommunityCar.Application.Features.Community.Events.ViewModels;

public class EventSummaryVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public int AttendeeCount { get; set; }
    public int? MaxAttendees { get; set; }
    public decimal? TicketPrice { get; set; }
    public bool IsFree => !TicketPrice.HasValue || TicketPrice.Value == 0;
    public bool IsActive => DateTime.UtcNow >= StartTime && DateTime.UtcNow <= EndTime;
    public string OrganizerName { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public string? ImageUrl { get; set; }
    public bool IsUpcoming => DateTime.UtcNow < StartTime;
    
    // Additional properties for search results
    public double? DistanceKm { get; set; }
}