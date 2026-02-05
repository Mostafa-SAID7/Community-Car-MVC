namespace CommunityCar.Web.Areas.Identity.ViewModels.Authorization;

public class RolePermissionSummaryVM
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public DateTime GrantedAt { get; set; }
}
