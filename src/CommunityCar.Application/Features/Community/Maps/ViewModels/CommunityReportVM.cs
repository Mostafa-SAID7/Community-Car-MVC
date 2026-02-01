using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

// Community Reporting ViewModels (converted from DTOs)
public class CommunityReportVM
{
    public Guid Id { get; set; }
    public Guid ReporterId { get; set; }
    public string ReporterName { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ReportedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public int UpvoteCount { get; set; }
    public int DownvoteCount { get; set; }
    public bool IsVerified { get; set; }
    public string? VerificationNotes { get; set; }
}