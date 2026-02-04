namespace CommunityCar.Application.Features.Dashboard.UserManagement.ViewModels;

/// <summary>
/// ViewModel for bulk user updates
/// </summary>
public class BulkUserUpdateVM
{
    public string? Action { get; set; } // "activate", "deactivate", "delete", "assign_role", "remove_role"
    public string? Role { get; set; }
    public string? Reason { get; set; }
    public bool SendNotification { get; set; } = true;
    public DateTime? EffectiveDate { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}