using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Services.Maps.Community;

public interface ICommunityReportingService
{
    Task<CommunityReportDto> SubmitReportAsync(SubmitCommunityReportRequest request);
    Task<bool> ConfirmReportAsync(Guid reportId, Guid userId, bool isConfirmed, string? comment = null);
    Task<List<CommunityReportDto>> GetNearbyReportsAsync(double latitude, double longitude, double radiusKm = 5);
    Task<List<CommunityReportDto>> GetUserReportsAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<bool> ModerateReportAsync(Guid reportId, Guid moderatorId, CommunityReportStatus status, string? reason = null);
    Task<double> GetUserReputationScoreAsync(Guid userId);
    Task UpdateReputationScoresAsync();
}

public class SubmitCommunityReportRequest
{
    public Guid UserId { get; set; }
    public string Location { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public CommunityReportType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}

public class CommunityReportDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
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
    public DateTime? ExpiresAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public bool CanUserConfirm { get; set; }
    public bool HasUserConfirmed { get; set; }
}


