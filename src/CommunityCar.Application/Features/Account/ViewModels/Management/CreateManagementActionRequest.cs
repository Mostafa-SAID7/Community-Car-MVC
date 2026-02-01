namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class CreateManagementActionRequest
{
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public Guid PerformedBy { get; set; }
    public string? Notes { get; set; }
    public bool IsReversible { get; set; } = true;
}