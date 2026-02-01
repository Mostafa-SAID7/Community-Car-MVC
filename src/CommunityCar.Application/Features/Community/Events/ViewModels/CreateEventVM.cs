namespace CommunityCar.Application.Features.Community.Events.ViewModels;

public class CreateEventVM
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? LocationDetails { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int? MaxAttendees { get; set; }
    public bool RequiresApproval { get; set; }
    public decimal? TicketPrice { get; set; }
    public string? TicketInfo { get; set; }
    public bool IsPublic { get; set; } = true;
    public List<string> Tags { get; set; } = new();
    public string? ExternalUrl { get; set; }
    public string? ContactInfo { get; set; }
    
    // Arabic content properties
    public string? TitleAr { get; set; }
    public string? DescriptionAr { get; set; }
    public string? LocationDetailsAr { get; set; }
    
    // Image properties
    public List<string> ImageUrls { get; set; } = new();
}