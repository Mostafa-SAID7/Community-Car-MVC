using CommunityCar.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Maps.DTOs;

public class MapsSearchRequest
{
    public string? SearchTerm { get; set; }
    public POIType? Type { get; set; }
    public POICategory? Category { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double RadiusKm { get; set; } = 10;
    public bool? IsVerified { get; set; }
    public bool? IsOpen24Hours { get; set; }
    public double? MinRating { get; set; }
    public List<string>? Services { get; set; }
    public List<string>? PaymentMethods { get; set; }
    public string? SortBy { get; set; } = "distance"; // distance, rating, name, created
    public bool SortDescending { get; set; } = false;
    
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
    
    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
}

public class MapsSearchResponse
{
    public IEnumerable<PointOfInterestSummaryVM> Items { get; set; } = new List<PointOfInterestSummaryVM>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

public class PointOfInterestSummaryVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? DescriptionAr { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public POIType Type { get; set; }
    public POICategory Category { get; set; }
    public string? Address { get; set; }
    public string? AddressAr { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int ViewCount { get; set; }
    public int CheckInCount { get; set; }
    public bool IsVerified { get; set; }
    public bool IsOpen24Hours { get; set; }
    public bool IsTemporarilyClosed { get; set; }
    public List<string> Services { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
    public double? DistanceKm { get; set; }
    public DateTime CreatedAt { get; set; }
}