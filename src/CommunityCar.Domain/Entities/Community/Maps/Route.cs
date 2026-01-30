using System;
using System.Collections.Generic;
using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Domain.Entities.Community.Maps;

public class Route : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string? NameAr { get; private set; }
    public string? DescriptionAr { get; private set; }
    public new Guid CreatedBy { get; private set; }
    public RouteType Type { get; private set; }
    public DifficultyLevel Difficulty { get; private set; }
    
    // Route metrics
    public double DistanceKm { get; private set; }
    public int EstimatedDurationMinutes { get; private set; }
    public double StartLatitude { get; private set; }
    public double StartLongitude { get; private set; }
    public double EndLatitude { get; private set; }
    public double EndLongitude { get; private set; }
    public double AverageRating { get; private set; }
    public int ReviewCount { get; private set; }
    public int TimesCompleted { get; private set; }
    
    // Route characteristics
    public bool IsScenic { get; private set; }
    public bool HasTolls { get; private set; }
    public bool IsOffRoad { get; private set; }
    public string? SurfaceType { get; private set; }
    public string? SurfaceTypeAr { get; private set; }
    public string? BestTimeToVisit { get; private set; }
    public string? BestTimeToVisitAr { get; private set; }
    
    // Safety and conditions
    public string? SafetyNotes { get; private set; }
    public string? SafetyNotesAr { get; private set; }
    public string? CurrentConditions { get; private set; }
    public string? CurrentConditionsAr { get; private set; }
    public DateTime? LastConditionUpdate { get; private set; }
    
    // Waypoints as JSON string for simplicity
    public string WaypointsJson { get; private set; } = "[]";
    
    // Tags and categories
    private readonly List<string> _tags = new();
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();
    
    // Images
    private readonly List<string> _imageUrls = new();
    public IReadOnlyCollection<string> ImageUrls => _imageUrls.AsReadOnly();

    // Computed property for waypoints
    public IReadOnlyCollection<RouteWaypoint> Waypoints
    {
        get
        {
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<RouteWaypoint>>(WaypointsJson) ?? new List<RouteWaypoint>();
            }
            catch
            {
                return new List<RouteWaypoint>();
            }
        }
    }

    public Route(string name, string description, Guid createdBy, RouteType type, 
        DifficultyLevel difficulty = DifficultyLevel.Beginner)
    {
        Name = name;
        Description = description;
        CreatedBy = createdBy;
        Type = type;
        Difficulty = difficulty;
        DistanceKm = 0;
        EstimatedDurationMinutes = 0;
        AverageRating = 0;
        ReviewCount = 0;
        TimesCompleted = 0;
        IsScenic = false;
        HasTolls = false;
        IsOffRoad = false;
    }

    private Route() { }

    public void UpdateBasicInfo(string name, string description, RouteType type, DifficultyLevel difficulty)
    {
        Name = name;
        Description = description;
        Type = type;
        Difficulty = difficulty;
        Audit(UpdatedBy);
    }

    public void UpdateArabicContent(string? nameAr, string? descriptionAr, string? surfaceTypeAr = null, string? bestTimeToVisitAr = null, string? safetyNotesAr = null, string? currentConditionsAr = null)
    {
        NameAr = nameAr;
        DescriptionAr = descriptionAr;
        SurfaceTypeAr = surfaceTypeAr;
        BestTimeToVisitAr = bestTimeToVisitAr;
        SafetyNotesAr = safetyNotesAr;
        CurrentConditionsAr = currentConditionsAr;
        Audit(UpdatedBy);
    }

    public void UpdateMetrics(double distanceKm, int estimatedDurationMinutes)
    {
        DistanceKm = distanceKm;
        EstimatedDurationMinutes = estimatedDurationMinutes;
        Audit(UpdatedBy);
    }

    public void UpdateCharacteristics(bool isScenic, bool hasTolls, bool isOffRoad, 
        string? surfaceType, string? bestTimeToVisit)
    {
        IsScenic = isScenic;
        HasTolls = hasTolls;
        IsOffRoad = isOffRoad;
        SurfaceType = surfaceType;
        BestTimeToVisit = bestTimeToVisit;
        Audit(UpdatedBy);
    }

    public void UpdateSafetyInfo(string? safetyNotes, string? currentConditions)
    {
        SafetyNotes = safetyNotes;
        CurrentConditions = currentConditions;
        LastConditionUpdate = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public void AddWaypoint(double latitude, double longitude, string? name = null, 
        string? description = null, int order = 0)
    {
        var waypoints = Waypoints.ToList();
        var waypoint = new RouteWaypoint(latitude, longitude, name, description, order);
        waypoints.Add(waypoint);
        WaypointsJson = System.Text.Json.JsonSerializer.Serialize(waypoints);
        Audit(UpdatedBy);
    }

    public void RemoveWaypoint(RouteWaypoint waypoint)
    {
        var waypoints = Waypoints.ToList();
        var toRemove = waypoints.FirstOrDefault(w => 
            w.Latitude == waypoint.Latitude && 
            w.Longitude == waypoint.Longitude && 
            w.Order == waypoint.Order);
        
        if (toRemove != null)
        {
            waypoints.Remove(toRemove);
            WaypointsJson = System.Text.Json.JsonSerializer.Serialize(waypoints);
            Audit(UpdatedBy);
        }
    }

    public void ClearWaypoints()
    {
        WaypointsJson = "[]";
        Audit(UpdatedBy);
    }

    public void SetWaypoints(IEnumerable<RouteWaypoint> waypoints)
    {
        WaypointsJson = System.Text.Json.JsonSerializer.Serialize(waypoints.ToList());
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

    public void UpdateRating(double averageRating, int reviewCount)
    {
        AverageRating = averageRating;
        ReviewCount = reviewCount;
        Audit(UpdatedBy);
    }

    public void IncrementCompletionCount()
    {
        TimesCompleted++;
        Audit(UpdatedBy);
    }
}

public class RouteWaypoint
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string? Name { get; private set; }
    public string? NameAr { get; private set; }
    public string? Description { get; private set; }
    public string? DescriptionAr { get; private set; }
    public int Order { get; private set; }

    public RouteWaypoint(double latitude, double longitude, string? name = null, 
        string? description = null, int order = 0)
    {
        Latitude = latitude;
        Longitude = longitude;
        Name = name;
        Description = description;
        Order = order;
    }

    public void UpdateInfo(string? name, string? description, int order)
    {
        Name = name;
        Description = description;
        Order = order;
    }

    public void UpdateArabicContent(string? nameAr, string? descriptionAr)
    {
        NameAr = nameAr;
        DescriptionAr = descriptionAr;
    }
}