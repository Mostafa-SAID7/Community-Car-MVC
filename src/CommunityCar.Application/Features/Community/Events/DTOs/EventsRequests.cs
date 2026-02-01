using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Community.Events.DTOs;

public class CreateEventRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? TitleAr { get; set; }

    [StringLength(2000)]
    public string? DescriptionAr { get; set; }
    
    [Required]
    public DateTime StartTime { get; set; }
    
    [Required]
    public DateTime EndTime { get; set; }
    
    [Required]
    [StringLength(500)]
    public string Location { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string? LocationDetails { get; set; }
    
    [StringLength(1000)]
    public string? LocationDetailsAr { get; set; }
    
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    
    public int? MaxAttendees { get; set; }
    
    public decimal? TicketPrice { get; set; }
    
    [StringLength(500)]
    public string? TicketInfo { get; set; }
    
    public bool IsPublic { get; set; } = true;
    public bool RequiresApproval { get; set; } = false;
    
    public List<string>? Tags { get; set; }
    public List<string>? ImageUrls { get; set; }
    
    [Url]
    public string? ExternalUrl { get; set; }
    
    [StringLength(500)]
    public string? ContactInfo { get; set; }
}

public class UpdateEventRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? TitleAr { get; set; }

    [StringLength(2000)]
    public string? DescriptionAr { get; set; }
    
    [Required]
    public DateTime StartTime { get; set; }
    
    [Required]
    public DateTime EndTime { get; set; }
    
    [Required]
    [StringLength(500)]
    public string Location { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string? LocationDetails { get; set; }
    
    [StringLength(1000)]
    public string? LocationDetailsAr { get; set; }
    
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    
    public int? MaxAttendees { get; set; }
    
    public decimal? TicketPrice { get; set; }
    
    [StringLength(500)]
    public string? TicketInfo { get; set; }
    
    public bool IsPublic { get; set; } = true;
    public bool RequiresApproval { get; set; } = false;
    
    public List<string>? Tags { get; set; }
    public List<string>? ImageUrls { get; set; }
    
    [Url]
    public string? ExternalUrl { get; set; }
    
    [StringLength(500)]
    public string? ContactInfo { get; set; }
}

public class EventsSearchRequest
{
    public string? SearchTerm { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Location { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double RadiusKm { get; set; } = 50;
    public bool IsUpcomingOnly { get; set; } = false;
    public bool IsFreeOnly { get; set; } = false;
    public bool? IsPublic { get; set; }
    public bool HasAvailableSpots { get; set; } = false;
    public List<string>? Tags { get; set; }
    public string? SortBy { get; set; } = "startTime"; // startTime, created, popularity, distance
    public bool SortDescending { get; set; } = false;
    
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
    
    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
}

public class EventsSearchResponse
{
    public IEnumerable<EventSummaryVM> Items { get; set; } = new List<EventSummaryVM>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

public class EventSummaryVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? DescriptionAr { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public Guid OrganizerId { get; set; }
    public string OrganizerName { get; set; } = string.Empty;
    public int AttendeeCount { get; set; }
    public int? MaxAttendees { get; set; }
    public decimal? TicketPrice { get; set; }
    public bool IsPublic { get; set; }
    public bool RequiresApproval { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public double? DistanceKm { get; set; }
    
    // Helper properties
    public bool IsUpcoming => DateTime.UtcNow < StartTime;
    public bool IsActive => DateTime.UtcNow >= StartTime && DateTime.UtcNow <= EndTime;
    public bool HasAvailableSpots => !MaxAttendees.HasValue || AttendeeCount < MaxAttendees.Value;
    public bool IsFree => !TicketPrice.HasValue || TicketPrice.Value == 0;
}


