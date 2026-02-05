namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Users.Actions;

/// <summary>
/// ViewModel for user quick actions
/// </summary>
public class UserQuickActionVM
{
    public string ActionName { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string ActionUrl { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public string Permission { get; set; } = string.Empty;
}




