using System;
using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Community.Maps;

public class CheckIn : BaseEntity
{
    public Guid PointOfInterestId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CheckInTime { get; private set; }
    public string? Comment { get; private set; }
    public double? Rating { get; private set; }
    public bool IsPrivate { get; private set; }
    
    // Location verification
    public double? CheckInLatitude { get; private set; }
    public double? CheckInLongitude { get; private set; }
    public double? DistanceFromPOI { get; private set; }

    public CheckIn(Guid pointOfInterestId, Guid userId, string? comment = null, 
        double? rating = null, bool isPrivate = false)
    {
        PointOfInterestId = pointOfInterestId;
        UserId = userId;
        CheckInTime = DateTime.UtcNow;
        Comment = comment;
        Rating = rating;
        IsPrivate = isPrivate;
    }

    private CheckIn() { }

    public void UpdateComment(string? comment)
    {
        Comment = comment;
        Audit(UpdatedBy);
    }

    public void UpdateRating(double? rating)
    {
        Rating = rating;
        Audit(UpdatedBy);
    }

    public void SetLocation(double latitude, double longitude, double distanceFromPOI)
    {
        CheckInLatitude = latitude;
        CheckInLongitude = longitude;
        DistanceFromPOI = distanceFromPOI;
        Audit(UpdatedBy);
    }

    public void SetPrivacy(bool isPrivate)
    {
        IsPrivate = isPrivate;
        Audit(UpdatedBy);
    }
}