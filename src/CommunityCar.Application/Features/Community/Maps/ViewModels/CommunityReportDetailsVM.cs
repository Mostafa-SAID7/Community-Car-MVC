using CommunityCar.Domain.Entities.Community.Maps;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Maps.ViewModels;

// Enhanced CommunityReportVM with additional properties from the DTO
public class CommunityReportDetailsVM : CommunityReportVM
{
    public string UserName { get; set; } = string.Empty;
    public new string Status { get; set; } = string.Empty; // Use 'new' to hide inherited member
    public int ConfirmationCount { get; set; }
    public int DisputeCount { get; set; }
    public double ReporterReputationScore { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public bool CanUserConfirm { get; set; }
    public bool HasUserConfirmed { get; set; }
}