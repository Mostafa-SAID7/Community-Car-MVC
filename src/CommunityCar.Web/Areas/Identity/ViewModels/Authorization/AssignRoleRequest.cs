namespace CommunityCar.Web.Areas.Identity.ViewModels.Authorization;

public class AssignRoleRequest
{
    public Guid UserId { get; set; }
    public string RoleName { get; set; } = string.Empty;
}
