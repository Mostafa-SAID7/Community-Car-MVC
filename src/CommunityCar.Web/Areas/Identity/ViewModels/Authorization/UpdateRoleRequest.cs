namespace CommunityCar.Web.Areas.Identity.ViewModels.Authorization;

public class UpdateRoleRequest
{
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public int Priority { get; set; }
    public List<string> Permissions { get; set; } = new();
}
