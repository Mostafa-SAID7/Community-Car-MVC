namespace CommunityCar.Application.Features.Dashboard.Security.ViewModels;

public class SecurityThreatVM
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string AffectedEndpoint { get; set; } = string.Empty;
}