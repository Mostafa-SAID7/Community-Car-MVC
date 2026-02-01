namespace CommunityCar.Application.Features.Account.ViewModels.Authorization;

public class RoleStatsVM
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int TotalPermissions { get; set; }
    public Dictionary<string, int> PermissionsByCategory { get; set; } = new();
    public List<UserRoleSummaryVM> RecentAssignments { get; set; } = new();
}