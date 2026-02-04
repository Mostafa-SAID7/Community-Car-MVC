namespace CommunityCar.Application.Features.Community.Events.ViewModels;

public class CreateEventVM
{
    public string Title { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? LocationDetails { get; set; }
    public string? LocationDetailsAr { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsPublic { get; set; } = true;
    public bool RequiresApproval { get; set; }
    public int MaxAttendees { get; set; }
    public decimal? TicketPrice { get; set; }
    public string? TicketInfo { get; set; }
    public string? ExternalUrl { get; set; }
    public string? ContactInfo { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? Category { get; set; }
}

public class EditEventVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsPublic { get; set; } = true;
    public int MaxAttendees { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? Category { get; set; }
}