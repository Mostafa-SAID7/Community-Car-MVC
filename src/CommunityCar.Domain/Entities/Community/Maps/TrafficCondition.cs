using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Domain.Entities.Community.Maps;

public class TrafficCondition : BaseEntity
{
    public string Location { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public TrafficConditionType Type { get; set; }
    public TrafficSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsActive { get; set; } = true;
    public string Source { get; set; } = string.Empty; // API, User Report, etc.
    public Guid? ReportedByUserId { get; set; }
    public int ConfirmationCount { get; set; }
    public DateTime LastUpdated { get; set; }
    public double? SpeedKmh { get; set; }
    public int? DelayMinutes { get; set; }
}

public class CommunityReport : BaseEntity
{
    public Guid UserId { get; set; }
    public string Location { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public CommunityReportType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime ReportedAt { get; set; }
    public CommunityReportStatus Status { get; set; }
    public int ConfirmationCount { get; set; }
    public int DisputeCount { get; set; }
    public double ReporterReputationScore { get; set; }
    public bool IsVerified { get; set; }
    public Guid? VerifiedByUserId { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    
    // Navigation properties
    public List<CommunityReportConfirmation> Confirmations { get; set; } = new();
}

public class CommunityReportConfirmation : BaseEntity
{
    public Guid CommunityReportId { get; set; }
    public Guid UserId { get; set; }
    public bool IsConfirmed { get; set; } // true = confirm, false = dispute
    public string? Comment { get; set; }
    public DateTime ConfirmedAt { get; set; }
    
    // Navigation property
    public CommunityReport CommunityReport { get; set; } = null!;
}