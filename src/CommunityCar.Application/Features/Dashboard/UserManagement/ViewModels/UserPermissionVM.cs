namespace CommunityCar.Application.Features.Dashboard.UserManagement.ViewModels;

public class UserPermissionVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public List<string> DirectPermissions { get; set; } = new();
    public List<string> InheritedPermissions { get; set; } = new();
    public List<string> AllPermissions { get; set; } = new();
    public DateTime LastUpdated { get; set; }
    public string LastUpdatedBy { get; set; } = string.Empty;
}