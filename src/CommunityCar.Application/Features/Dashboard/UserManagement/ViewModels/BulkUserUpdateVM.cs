using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Dashboard.UserManagement.ViewModels;

public class BulkUserUpdateVM
{
    [Required]
    public List<Guid> UserIds { get; set; } = new();
    
    [Required]
    public string Action { get; set; } = string.Empty; // Activate, Deactivate, Lock, Unlock, Delete, etc.
    
    public string? Reason { get; set; }
    public string? NewRole { get; set; }
    public bool NotifyUsers { get; set; } = false;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}