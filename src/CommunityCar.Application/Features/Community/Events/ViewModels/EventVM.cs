namespace CommunityCar.Application.Features.Community.Events.ViewModels;

public class EventVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? DescriptionAr { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? LocationDetails { get; set; }
    public string? LocationDetailsAr { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Guid OrganizerId { get; set; }
    public string OrganizerName { get; set; } = string.Empty;
    
    // Attendance
    public int AttendeeCount { get; set; }
    public int? MaxAttendees { get; set; }
    public bool RequiresApproval { get; set; }
    
    // Pricing
    public decimal? TicketPrice { get; set; }
    public string? TicketInfo { get; set; }
    
    // Visibility and access
    public bool IsPublic { get; set; }
    
    // Content
    public List<string> Tags { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
    public string? ExternalUrl { get; set; }
    public string? ContactInfo { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Helper properties
    public bool IsUpcoming => DateTime.UtcNow < StartTime;
    public bool IsActive => DateTime.UtcNow >= StartTime && DateTime.UtcNow <= EndTime;
    public bool IsPast => DateTime.UtcNow > EndTime;
    public bool HasAvailableSpots => !MaxAttendees.HasValue || AttendeeCount < MaxAttendees.Value;
    public bool IsFree => !TicketPrice.HasValue || TicketPrice.Value == 0;
    public TimeSpan Duration => EndTime - StartTime;
    
    public string StatusText
    {
        get
        {
            if (IsActive) return "Active Now";
            if (IsUpcoming) return "Upcoming";
            return "Past Event";
        }
    }
    
    public string StatusColor
    {
        get
        {
            if (IsActive) return "green";
            if (IsUpcoming) return "blue";
            return "gray";
        }
    }
}