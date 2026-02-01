namespace CommunityCar.Application.Features.Account.ViewModels.Authorization;

public class RoleVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsSystemRole { get; set; }
    public int Priority { get; set; }
    public int UserCount { get; set; }
    public List<PermissionVM> Permissions { get; set; } = new();
    public int PermissionCount { get; set; }
}