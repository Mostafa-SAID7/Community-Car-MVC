namespace CommunityCar.Web.Areas.Identity.ViewModels.Authorization;

public class UserPermissionSummaryVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public DateTime GrantedAt { get; set; }
}
