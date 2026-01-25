using System;
using System.Collections.Generic;
using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Domain.Entities.Community.Maps;

public class PointOfInterest : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string? NameAr { get; private set; }
    public string? DescriptionAr { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public POIType Type { get; private set; }
    public POICategory Category { get; private set; }
    public new Guid CreatedBy { get; private set; }
    
    // Contact and business information
    public string? Address { get; private set; }
    public string? AddressAr { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Website { get; private set; }
    public string? Email { get; private set; }
    
    // Operating hours and availability
    public string? OpeningHours { get; private set; }
    public bool IsOpen24Hours { get; private set; }
    public bool IsTemporarilyClosed { get; private set; }
    
    // Ratings and reviews
    public double AverageRating { get; private set; }
    public int ReviewCount { get; private set; }
    public int ViewCount { get; private set; }
    public int CheckInCount { get; private set; }
    
    // Automotive specific features
    public List<string> Services { get; private set; } = new();
    public List<string> SupportedVehicleTypes { get; private set; } = new();
    public List<string> PaymentMethods { get; private set; } = new();
    public List<string> Amenities { get; private set; } = new();
    
    // Pricing information
    public decimal? PriceRange { get; private set; }
    public string? PricingInfo { get; private set; }
    public string? PricingInfoAr { get; private set; }
    
    // Verification and quality
    public bool IsVerified { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public Guid? VerifiedBy { get; private set; }
    public bool IsReported { get; private set; }
    public int ReportCount { get; private set; }
    
    // Social features
    public bool AllowCheckIns { get; private set; }
    public bool AllowReviews { get; private set; }
    public bool IsPublic { get; private set; }
    
    // Event-specific properties
    public DateTime? EventStartTime { get; private set; }
    public DateTime? EventEndTime { get; private set; }
    public int? MaxAttendees { get; private set; }
    public int CurrentAttendees { get; private set; }
    
    // Images and media
    private readonly List<string> _imageUrls = new();
    public IReadOnlyCollection<string> ImageUrls => _imageUrls.AsReadOnly();

    public PointOfInterest(string name, string description, double latitude, double longitude, 
        POIType type, POICategory category, Guid createdBy)
    {
        Name = name;
        Description = description;
        Latitude = latitude;
        Longitude = longitude;
        Type = type;
        Category = category;
        CreatedBy = createdBy;
        AverageRating = 0;
        ReviewCount = 0;
        ViewCount = 0;
        CheckInCount = 0;
        IsVerified = false;
        IsReported = false;
        ReportCount = 0;
        AllowCheckIns = true;
        AllowReviews = true;
        IsPublic = true;
        CurrentAttendees = 0;
    }

    private PointOfInterest() { }

    public void UpdateBasicInfo(string name, string description, POIType type, POICategory category)
    {
        Name = name;
        Description = description;
        Type = type;
        Category = category;
        Audit(UpdatedBy);
    }

    public void UpdateArabicContent(string? nameAr, string? descriptionAr, string? addressAr = null, string? pricingInfoAr = null)
    {
        NameAr = nameAr;
        DescriptionAr = descriptionAr;
        AddressAr = addressAr;
        PricingInfoAr = pricingInfoAr;
        Audit(UpdatedBy);
    }

    public void UpdateLocation(double latitude, double longitude, string? address = null)
    {
        Latitude = latitude;
        Longitude = longitude;
        if (!string.IsNullOrWhiteSpace(address))
            Address = address;
        Audit(UpdatedBy);
    }

    public void UpdateContactInfo(string? phoneNumber, string? website, string? email)
    {
        PhoneNumber = phoneNumber;
        Website = website;
        Email = email;
        Audit(UpdatedBy);
    }

    public void UpdateOperatingHours(string? openingHours, bool isOpen24Hours = false)
    {
        OpeningHours = openingHours;
        IsOpen24Hours = isOpen24Hours;
        Audit(UpdatedBy);
    }

    public void SetTemporarilyClosed(bool isClosed)
    {
        IsTemporarilyClosed = isClosed;
        Audit(UpdatedBy);
    }

    public void AddService(string service)
    {
        if (!Services.Contains(service))
        {
            Services.Add(service);
            Audit(UpdatedBy);
        }
    }

    public void RemoveService(string service)
    {
        if (Services.Remove(service))
        {
            Audit(UpdatedBy);
        }
    }

    public void AddSupportedVehicleType(string vehicleType)
    {
        if (!SupportedVehicleTypes.Contains(vehicleType))
        {
            SupportedVehicleTypes.Add(vehicleType);
            Audit(UpdatedBy);
        }
    }

    public void AddPaymentMethod(string paymentMethod)
    {
        if (!PaymentMethods.Contains(paymentMethod))
        {
            PaymentMethods.Add(paymentMethod);
            Audit(UpdatedBy);
        }
    }

    public void AddAmenity(string amenity)
    {
        if (!Amenities.Contains(amenity))
        {
            Amenities.Add(amenity);
            Audit(UpdatedBy);
        }
    }

    public void UpdatePricing(decimal? priceRange, string? pricingInfo)
    {
        PriceRange = priceRange;
        PricingInfo = pricingInfo;
        Audit(UpdatedBy);
    }

    public void Verify(Guid verifiedBy)
    {
        IsVerified = true;
        VerifiedAt = DateTime.UtcNow;
        VerifiedBy = verifiedBy;
        Audit(UpdatedBy);
    }

    public void RemoveVerification()
    {
        IsVerified = false;
        VerifiedAt = null;
        VerifiedBy = null;
        Audit(UpdatedBy);
    }

    public void IncrementViewCount()
    {
        ViewCount++;
    }

    public void IncrementCheckInCount()
    {
        CheckInCount++;
    }

    public void UpdateRating(double averageRating, int reviewCount)
    {
        AverageRating = averageRating;
        ReviewCount = reviewCount;
        Audit(UpdatedBy);
    }

    public void Report()
    {
        ReportCount++;
        if (ReportCount >= 5)
        {
            IsReported = true;
        }
        Audit(UpdatedBy);
    }

    public void ClearReports()
    {
        IsReported = false;
        ReportCount = 0;
        Audit(UpdatedBy);
    }

    public void SetEventDetails(DateTime? startTime, DateTime? endTime, int? maxAttendees)
    {
        EventStartTime = startTime;
        EventEndTime = endTime;
        MaxAttendees = maxAttendees;
        Audit(UpdatedBy);
    }

    public void UpdateAttendeeCount(int count)
    {
        CurrentAttendees = Math.Max(0, count);
        if (MaxAttendees.HasValue)
        {
            CurrentAttendees = Math.Min(CurrentAttendees, MaxAttendees.Value);
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

    public void SetPrivacy(bool isPublic, bool allowCheckIns, bool allowReviews)
    {
        IsPublic = isPublic;
        AllowCheckIns = allowCheckIns;
        AllowReviews = allowReviews;
        Audit(UpdatedBy);
    }

    public bool IsEventActive()
    {
        if (!EventStartTime.HasValue || !EventEndTime.HasValue)
            return false;

        var now = DateTime.UtcNow;
        return now >= EventStartTime.Value && now <= EventEndTime.Value;
    }

    public bool IsEventUpcoming()
    {
        if (!EventStartTime.HasValue)
            return false;

        return DateTime.UtcNow < EventStartTime.Value;
    }

    public bool HasAvailableSpots()
    {
        if (!MaxAttendees.HasValue)
            return true;

        return CurrentAttendees < MaxAttendees.Value;
    }
}
