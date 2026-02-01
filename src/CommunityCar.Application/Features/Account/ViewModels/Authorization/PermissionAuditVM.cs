namespace CommunityCar.Application.Features.Account.ViewModels.Authorization;

public class PermissionAuditVM
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Actor { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Details { get; set; }
}