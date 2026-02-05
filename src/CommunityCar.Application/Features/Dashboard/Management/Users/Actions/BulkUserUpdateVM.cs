namespace CommunityCar.Application.Features.Dashboard.Management.Users.Actions;

/// <summary>
/// ViewModel for bulk user updates
/// </summary>
public class BulkUserUpdateVM
{
    public List<string> Roles { get; set; } = new();
    public bool? IsActive { get; set; }
    public bool? EmailConfirmed { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public string? LockoutReason { get; set; }
    public string Action { get; set; } = string.Empty; // Activate, Deactivate, Lock, Unlock, etc.
    public string? Reason { get; set; }
}