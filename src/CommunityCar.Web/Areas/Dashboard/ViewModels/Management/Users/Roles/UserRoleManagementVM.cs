namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Users.Roles;

/// <summary>
/// User role management view model
/// </summary>
public class UserRoleManagementVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public List<string> CurrentRoles { get; set; } = new();
    public List<string> AvailableRoles { get; set; } = new();
    public List<string> AssignedPermissions { get; set; } = new();
    public DateTime LastRoleChange { get; set; }
    public string LastRoleChangedBy { get; set; } = string.Empty;
    public bool CanManageRoles { get; set; }
    public List<RoleChangeHistoryVM> RoleHistory { get; set; } = new();
}




