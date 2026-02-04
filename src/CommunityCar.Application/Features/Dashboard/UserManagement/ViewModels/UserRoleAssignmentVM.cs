using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Dashboard.UserManagement.ViewModels;

public class UserRoleAssignmentVM
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public List<string> Roles { get; set; } = new();
    
    public string? Reason { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool NotifyUser { get; set; } = true;
}