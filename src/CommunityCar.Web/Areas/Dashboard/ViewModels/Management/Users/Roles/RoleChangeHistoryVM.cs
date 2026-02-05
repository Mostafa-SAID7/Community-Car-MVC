namespace CommunityCar.Application.Features.Dashboard.Management.Users.Roles;

/// <summary>
/// Role change history view model
/// </summary>
public class RoleChangeHistoryVM
{
    public DateTime ChangeDate { get; set; }
    public string Action { get; set; } = string.Empty; // Added, Removed
    public string RoleName { get; set; } = string.Empty;
    public string ChangedBy { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}