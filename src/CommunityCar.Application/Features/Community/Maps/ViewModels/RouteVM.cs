using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

public class RouteVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? DescriptionAr { get; set; }
    public Guid CreatedBy { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public RouteType Type { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    
    // Route metrics
    public double DistanceKm { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int TimesCompleted { get; set; }
    
    // Route characteristics
    public bool IsScenic { get; set; }
    public bool HasTolls { get; set; }
    public bool IsOffRoad { get; set; }
    public string? SurfaceType { get; set; }
    public string? SurfaceTypeAr { get; set; }
    public string? BestTimeToVisit { get; set; }
    public string? BestTimeToVisitAr { get; set; }
    
    // Safety and conditions
    public string? SafetyNotes { get; set; }
    public string? SafetyNotesAr { get; set; }
    public string? CurrentConditions { get; set; }
    public string? CurrentConditionsAr { get; set; }
    public DateTime? LastConditionUpdate { get; set; }
    
    // Waypoints and path
    public List<RouteWaypointVM> Waypoints { get; set; } = new();
    
    // Tags and categories
    public List<string> Tags { get; set; } = new();
    
    // Images
    public List<string> ImageUrls { get; set; } = new();
    
    // Reviews
    public List<CommunityCar.Application.Features.Community.Reviews.ViewModels.ReviewVM> Reviews { get; set; } = new();
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}