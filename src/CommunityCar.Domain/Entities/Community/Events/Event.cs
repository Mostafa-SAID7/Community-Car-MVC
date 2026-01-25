using System;
using System.Collections.Generic;
using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Community.Events;

public class Event : AggregateRoot
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    
    // Arabic Localization
    public string? TitleAr { get; private set; }
    public string? DescriptionAr { get; private set; }
    public string? LocationDetailsAr { get; private set; }
    
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public string Location { get; private set; }
    public  string? LocationDetails { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public Guid OrganizerId { get; private set; }
    
    // Attendance
    public int AttendeeCount { get; private set; }
    public int? MaxAttendees { get; private set; }
    public bool RequiresApproval { get; private set; }
    
    // Pricing
    public decimal? TicketPrice { get; private set; }
    public string? TicketInfo { get; private set; }
    
    // Visibility and access
    public bool IsPublic { get; private set; }
    public bool IsCancelled { get; private set; }
    
    // Content
    private readonly List<string> _tags = new();
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();
    
    private readonly List<string> _imageUrls = new();
    public IReadOnlyCollection<string> ImageUrls => _imageUrls.AsReadOnly();
    
    public string? ExternalUrl { get; private set; }
    public string? ContactInfo { get; private set; }

    public Event(string title, string description, DateTime startTime, DateTime endTime, 
        string location, Guid organizerId)
    {
        Title = title;
        Description = description;
        StartTime = startTime;
        EndTime = endTime;
        Location = location;
        OrganizerId = organizerId;
        AttendeeCount = 0;
        RequiresApproval = false;
        IsPublic = true;
        IsCancelled = false;
    }

    private Event() { }

    public void UpdateBasicInfo(string title, string description, DateTime startTime, DateTime endTime, string location)
    {
        Title = title;
        Description = description;
        StartTime = startTime;
        EndTime = endTime;
        Location = location;
        Audit(UpdatedBy);
    }

    public void UpdateLocationDetails(string? locationDetails, double? latitude, double? longitude)
    {
        LocationDetails = locationDetails;
        Latitude = latitude;
        Longitude = longitude;
        Audit(UpdatedBy);
    }

    public void UpdateArabicContent(string? titleAr, string? descriptionAr, string? locationDetailsAr)
    {
        TitleAr = titleAr;
        DescriptionAr = descriptionAr;
        LocationDetailsAr = locationDetailsAr;
        Audit(UpdatedBy);
    }

    public void UpdateAttendanceSettings(int? maxAttendees, bool requiresApproval)
    {
        MaxAttendees = maxAttendees;
        RequiresApproval = requiresApproval;
        Audit(UpdatedBy);
    }

    public void UpdatePricing(decimal? ticketPrice, string? ticketInfo)
    {
        TicketPrice = ticketPrice;
        TicketInfo = ticketInfo;
        Audit(UpdatedBy);
    }

    public void UpdateVisibility(bool isPublic)
    {
        IsPublic = isPublic;
        Audit(UpdatedBy);
    }

    public void UpdateContactInfo(string? externalUrl, string? contactInfo)
    {
        ExternalUrl = externalUrl;
        ContactInfo = contactInfo;
        Audit(UpdatedBy);
    }

    public void AddTag(string tag)
    {
        if (!_tags.Contains(tag.ToLowerInvariant()))
        {
            _tags.Add(tag.ToLowerInvariant());
            Audit(UpdatedBy);
        }
    }

    public void RemoveTag(string tag)
    {
        if (_tags.Remove(tag.ToLowerInvariant()))
        {
            Audit(UpdatedBy);
        }
    }

    public void AddImage(string imageUrl)
    {
        if (!_imageUrls.Contains(imageUrl))
        {
            _imageUrls.Add(imageUrl);
            Audit(UpdatedBy);
        }
    }

    public void RemoveImage(string imageUrl)
    {
        if (_imageUrls.Remove(imageUrl))
        {
            Audit(UpdatedBy);
        }
    }

    public void IncrementAttendeeCount()
    {
        if (!MaxAttendees.HasValue || AttendeeCount < MaxAttendees.Value)
        {
            AttendeeCount++;
            Audit(UpdatedBy);
        }
    }

    public void DecrementAttendeeCount()
    {
        if (AttendeeCount > 0)
        {
            AttendeeCount--;
            Audit(UpdatedBy);
        }
    }

    public void Cancel()
    {
        IsCancelled = true;
        Audit(UpdatedBy);
    }

    public void Uncancel()
    {
        IsCancelled = false;
        Audit(UpdatedBy);
    }

    // Helper methods
    public bool IsUpcoming => DateTime.UtcNow < StartTime;
    public bool IsActive => DateTime.UtcNow >= StartTime && DateTime.UtcNow <= EndTime;
    public bool IsPast => DateTime.UtcNow > EndTime;
    public bool HasAvailableSpots => !MaxAttendees.HasValue || AttendeeCount < MaxAttendees.Value;
    public bool IsFree => !TicketPrice.HasValue || TicketPrice.Value == 0;
}
